using Windows.UI.Xaml.Controls;
using System.Threading;
using Microsoft.Azure.Devices.Client;
using System;
using Windows.UI.Core;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using ActiveSense.Tempsense.Sensor;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using System.Collections.Generic;
using System.Xml;
using Sensors.Dht;
using System.Xml.Linq;
using Windows.Storage;
using System.IO;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ActiveSense.Tempsense.Sensor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region referencias
        static DeviceClient deviceClient;
        static string iotHubUri;
        static string outputConfig;
        static string deviceKey;
        static string deviceName;
        static int readingInterval;
        static string ambiente;
        private Bmp180Sensor _bmp180;
        private Timer _periodicTimer; //Condicional de temporizador periódico
        static RegistryManager registryManager;
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        //sensor dht11
        private DispatcherTimer _dispatchTimer;

        private GpioPin _temperaturePin;

        private IDht _dhtInterface;

        private List<int> _retryCount;

        private DateTimeOffset _startedAt;


        #endregion


        public MainPage()
        {
            this.InitializeComponent();
            InitHardware();
        //    // set interval of timer to 1 second
            _dispatchTimer.Interval = TimeSpan.FromSeconds(1);
        //    // invoke a method at each tick (as per interval of your timer)
           _dispatchTimer.Tick += _dispatchTimer_Tick;
        //    // initialize pin (GPIO pin on which you have set your temperature sensor)
           _temperaturePin = GpioController.GetDefault().OpenPin(4, GpioSharingMode.Exclusive);
        //    // create instance of a DHT11 
           _dhtInterface = new Dht11(_temperaturePin, GpioPinDriveMode.Input);
        //    // start the timer
           _dispatchTimer.Start();

        //    // set start date time
           _startedAt = DateTimeOffset.Now;

            InitializeDeviceKeyAndName();
            InitializeSensors();
            //deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceName, deviceKey));
        }

        //sensor dht11 method to initialize variables and hardware components
        private void InitHardware()
        {
            _dispatchTimer = new DispatcherTimer();

            _temperaturePin = null;

            _dhtInterface = null;

            _retryCount = new List<int>();

            _startedAt = DateTimeOffset.Parse("1/1/1");
        }
        /// <summary>
        /// Leemos el archivo de configuracion
        /// </summary>
        private async void InitializeDeviceKeyAndName()
        {
            //outputConfig = Configuration.ReadDeviceKey().Result;
            //string[] result = outputConfig.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            //deviceKey = result[0];
            //deviceName = result[1];
            //iotHubUri = result[2];
            //readingInterval = int.Parse(result[3]);
            //ambiente = result[4];

            XDocument xmlData;
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Configs/SensorConfig.xml"));
            using (var stream = await file.OpenStreamForReadAsync())
            {
                xmlData = XDocument.Load(stream);
            }
            foreach (var nodo in xmlData.Descendants("Device"))
            {
                deviceKey = nodo.Element("DeviceKey").Value.ToString();
                deviceName = nodo.Element("DeviceName").Value.ToString();
                iotHubUri = nodo.Element("IotHubUri").Value.ToString();
                readingInterval = int.Parse(nodo.Element("ReadingInterval").Value.ToString());
                ambiente = nodo.Element("Ambiente").Value.ToString();
                deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceName, deviceKey));
            }

        }

        /// <summary>
        /// Sensor initialization
        /// </summary>
        private async void InitializeSensors()
        {
            string calibrationData;
            try
            {
                _bmp180 = new Bmp180Sensor();
                await _bmp180.InitializeAsync();
                calibrationData = _bmp180.CalibrationData.ToString();   //Retorna una cadena que representa al objeto actual.
                if (_periodicTimer == null)
                {
                    _periodicTimer = new Timer(this.TimerCallback, null, 0, readingInterval);
                }
            }
            catch (Exception ex)
            {
                calibrationData = "Error de dispositivo! " + ex.Message;
            }
        }

        /// <summary>
        /// Este metodo se ejecuta segun el tiempo definido en el archivo de configuracion para leer datos del sensor
        /// </summary>
        /// <param name="state"></param>
        public async void TimerCallback(object state)
        {
            string temperatureText;
            try
            {
                var sensorData = await _bmp180.GetSensorDataAsync(Bmp180AccuracyMode.UltraHighResolution);
                temperatureText = sensorData.Temperature.ToString("");
                temperatureText += "°C";


                var temperatureDataPoint = new
                {
                    deviceKey = deviceKey,
                    deviceName = deviceName,
                    temperatura = sensorData.Temperature,
                    fecha = DateTime.Now
                };
                //// actualizaciones de la interfaz de usuario... deben ser invocados en el subproceso de interfaz de usuario
                var task = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    temperatura.Text = temperatureText;
                });
                var messageString = JsonConvert.SerializeObject(temperatureDataPoint);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties["Ambiente"] = ambiente;
                autoResetEvent.WaitOne();
                await deviceClient.SendEventAsync(message);
                autoResetEvent.Set();
            }
            catch (Exception ex)
            {
                var a = ex.StackTrace;
                temperatureText = "Sensor Error: " + ex.Message;
            }

            
        }
        private async void _dispatchTimer_Tick(object sender, object e)
        {
            try
            {
                DhtReading reading = new DhtReading();

                int val = this.TotalAttempts;

                this.TotalAttempts++;

                reading = await _dhtInterface.GetReadingAsync().AsTask();

                _retryCount.Add(reading.RetryCount);
                //this.OnPropertyChanged(nameof(AverageRetriesDisplay));
                //this.OnPropertyChanged(nameof(TotalAttempts));
                //this.OnPropertyChanged(nameof(PercentSuccess));

                if (reading.IsValid) // if we are able to capture value, display those
                {
                    this.TotalSuccess++;
                    this.Temperature = Convert.ToSingle(reading.Temperature);
                    this.Humidity = Convert.ToSingle(reading.Humidity);
                    this.LastUpdated = DateTimeOffset.Now;
                    //this.OnPropertyChanged(nameof(SuccessRate));

                    var telemetryDataPoint = new
                    {
                        deviceId = "dht11",
                        temperature = Temperature.ToString(),
                        humidity = Humidity.ToString(),
                        date = DateTime.Now.ToString("dd-MM-yyyy"),
                        hours = DateTime.Now.Hour.ToString(),
                        minutes = DateTime.Now.Minute.ToString(),
                        seconds = DateTime.Now.Second.ToString()

                    };

                    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));

                    //await deviceClient.SendEventAsync(message);


                    Humedad.Text = this.Humidity.ToString();

                    var task = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {

                        Humedad.Text = string.Format("{0:0.0}% RH", reading.Humidity); //reading.Humidity.ToString();
                        Temperatura_Ambiente.Text = reading.Temperature.ToString() + "°C";
                    });
                    //var task2 = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    //{

                    //    Temperatura_Ambiente.Text = reading.Temperature.ToString();
                    //});
                    Debug.WriteLine(message);


                }
                else // log if the reading is not in valid state
                {
                    Debug.WriteLine(string.Format("IsValid: {0}, RetryCount: {1}, TimedOut: {2}, Humidity: {3}, Temperature: {4}", reading.IsValid, reading.RetryCount, reading.TimedOut, reading.Humidity, reading.Temperature));
                }

                //this.OnPropertyChanged(nameof(LastUpdatedDisplay)); // show when the data was last updated
                // this.OnPropertyChanged(nameof(DateTimeDisplay));
            }
            catch (Exception ex) // log any exception that occurs
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #region application properties

        public string PercentSuccess
        {
            get
            {
                string returnValue = string.Empty;

                int attempts = this.TotalAttempts;

                if (attempts > 0)
                {
                    returnValue = string.Format("{0:0.0}%", 100f * (float)this.TotalSuccess / (float)attempts);
                }
                else
                {
                    returnValue = "0.0%";
                }

                return returnValue;
            }
        }

        private int _totalAttempts = 0;

        public int TotalAttempts
        {
            get
            {
                return _totalAttempts;
            }
            set
            {
                //this.SetProperty(ref _totalAttempts, value);
                // this.OnPropertyChanged(nameof(PercentSuccess));
            }
        }

        private int _totalSuccess = 0;

        public int TotalSuccess
        {
            get
            {
                return _totalSuccess;
            }
            set
            {
                //this.SetProperty(ref _totalSuccess, value);
                //this.OnPropertyChanged(nameof(PercentSuccess));
            }
        }

        private float _humidity = 0f;

        public float Humidity
        {
            get
            {
                return _humidity;
            }

            set
            {
                // this.SetProperty(ref _humidity, value);
                // this.OnPropertyChanged(nameof(HumidityDisplay));
            }
        }

        public string HumidityDisplay
        {
            get
            {
                return string.Format("{0:0.0}% RH", this.Humidity);
            }
        }

        private float _temperature = 0f;

        public float Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                //SetProperty(ref _temperature, value);
                // this.OnPropertyChanged(nameof(TemperatureDisplay));
            }
        }

        public string TemperatureDisplay
        {
            get
            {
                return string.Format("{0:0.0} °C", this.Temperature);
            }
        }

        private DateTimeOffset _lastUpdated = DateTimeOffset.MinValue;

        public DateTimeOffset LastUpdated
        {
            get
            {
                return _lastUpdated;
            }
            set
            {
                // this.SetProperty(ref _lastUpdated, value);
                // this.OnPropertyChanged(nameof(LastUpdatedDisplay));

            }
        }

        private DateTime _dateTime = DateTime.Now;

        public DateTime DateTimeUpdate
        {
            get
            {
                return _dateTime;
            }
            set
            {
                // this.SetProperty(ref _dateTime, value);
                // this.OnPropertyChanged(nameof(DateTimeDisplay));
            }
        }

        public string LastUpdatedDisplay
        {
            get
            {
                string returnValue = string.Empty;

                TimeSpan elapsed = DateTimeOffset.Now.Subtract(this.LastUpdated);

                if (this.LastUpdated == DateTimeOffset.MinValue)
                {
                    returnValue = "never";
                }
                else if (elapsed.TotalSeconds < 60d)
                {
                    int seconds = (int)elapsed.TotalSeconds;

                    if (seconds < 2)
                    {
                        returnValue = "just now";
                    }
                    else
                    {
                        returnValue = string.Format("{0:0} {1} ago", seconds, seconds == 1 ? "second" : "seconds");
                    }
                }
                else if (elapsed.TotalMinutes < 60d)
                {
                    int minutes = (int)elapsed.TotalMinutes == 0 ? 1 : (int)elapsed.TotalMinutes;
                    returnValue = string.Format("{0:0} {1} ago", minutes, minutes == 1 ? "minute" : "minutes");
                }
                else if (elapsed.TotalHours < 24d)
                {
                    int hours = (int)elapsed.TotalHours == 0 ? 1 : (int)elapsed.TotalHours;
                    returnValue = string.Format("{0:0} {1} ago", hours, hours == 1 ? "hour" : "hours");
                }
                else
                {
                    returnValue = "a long time ago";
                }

                return returnValue;
            }
        }

        public string DateTimeDisplay
        {
            get
            {
                string returnValue = string.Empty;

                returnValue = DateTime.Now.ToString();

                return returnValue;
            }
        }

        public int AverageRetries
        {
            get
            {
                int returnValue = 0;

                if (_retryCount.Count() > 0)
                {
                    returnValue = (int)_retryCount.Average();
                }

                return returnValue;
            }
        }

        public string AverageRetriesDisplay
        {
            get
            {
                return string.Format("{0:0}", this.AverageRetries);
            }
        }

        public string SuccessRate
        {
            get
            {
                string returnValue = string.Empty;

                double totalSeconds = DateTimeOffset.Now.Subtract(_startedAt).TotalSeconds;
                double rate = this.TotalSuccess / totalSeconds;

                if (rate < 1)
                {
                    returnValue = string.Format("{0:0.00} seconds/reading", 1d / rate);
                }
                else
                {
                    returnValue = string.Format("{0:0.00} readings/sec", rate);
                }

                return returnValue;
            }
        }

        #endregion
        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void temperatura_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

