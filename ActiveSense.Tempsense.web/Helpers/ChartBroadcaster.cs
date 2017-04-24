using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using static ActiveSense.Tempsense.web.Helpers.Chart_Broadcaster;
using ActiveSense.Tempsense.web.Hubs;
using ActiveSense.Tempsense.model.Model;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;
using System.Configuration;
using Microsoft.AspNet.Identity;
using ActiveSense.Tempsense.web.Controllers;

namespace ActiveSense.Tempsense.web.Helpers
{
    public class Chart_Broadcaster
    {

        public class LineChart
        {


            [JsonProperty("lineChartData")]
            private int[] lineChartData;
            [JsonProperty("colorString")]
            private string colorString;

            [JsonProperty("time")]
            private string[] time = new string[60];

            public void SetLineChartData()
            {
                //Suppose we have a list of 60 items.

                using (ActiveSenseContext dbActiveContext = new ActiveSenseContext(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString))
                {

                    var list = (from p in dbActiveContext.Measure
                                 orderby p.DateTime descending
                                 select p
                                 ).ToList();

                    lineChartData = dbActiveContext.Measure.Select(p => p.Value).Cast<int>().ToArray();
                    time = dbActiveContext.Measure.Select(p => p.DateTime).Cast<string>().ToArray();

                }

            }
        }

        public class TemperatureUpdate
        {
            [JsonProperty("DashboardTemperatureResult")]
            private List<DashboardTemperatureResult> temp;
      
            private const string PROFILE_Administrator= "Administrator";
            private UserHelper userHelper = null;

            public TemperatureUpdate() {
                userHelper = new UserHelper();
            }

            public void TakeLastTemp(string idUser)
            {

                List<DashboardTemperatureResult> list = new List<DashboardTemperatureResult>();

                try
                {
                    using (ActiveSenseContext context = new ActiveSenseContext(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString))
                    {

                        // a method that allows you to validate whether a user is Administrator profile and search the data according to this.
                        string perfil = userHelper.GetProfile(idUser);

                        int idCompany = userHelper.GetAssociatedCompanies(idUser, context);
                        var result = context.companies.Where(u => u.CompanyID == idCompany).Include("device.Measures").ToList();
                        if (PROFILE_Administrator== perfil)
                        {
                             result = context.companies.Include("device.Measures").ToList();
                        }

                        foreach (var item in result.SelectMany(x => x.device))
                        {
                            if (item.Measures.OrderBy(y => y.DateTime).Any())
                            {
                                decimal MaxTemp = 0;
                                decimal MinTemp = 0;
                                decimal MaxTol  = 0;
                                decimal MinTol  = 0;
                                try
                                {
                                    MaxTemp = context.Threshold.ToList().Where(p => p.DeviceID == item.DeviceID).FirstOrDefault().Temperature_max;
                                    MinTemp = context.Threshold.ToList().Where(p => p.DeviceID == item.DeviceID).FirstOrDefault().Temperature_min;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("ERROR chartBroadcaster.cs temperature Threshold.");
                                    Debug.WriteLine(ex.GetBaseException().ToString());
                                }
                                try {
                                    MaxTol = context.Threshold.ToList().Where(p => p.DeviceID == item.DeviceID).FirstOrDefault().Tolerance_max;
                                    MinTol = context.Threshold.ToList().Where(p => p.DeviceID == item.DeviceID).FirstOrDefault().Tolerance_min;
                                }
                                catch (Exception ex) {
                                    Debug.WriteLine("ERROR chartBroadcaster.cs threshold tolerance.");
                                    Debug.WriteLine(ex.GetBaseException().ToString());
                                }
                                finally
                                {
                                    list.Add(new DashboardTemperatureResult
                                    {
                                        DeviceID = item.Measures.OrderBy(y => y.DateTime).LastOrDefault().DeviceID,
                                        Company = item.Measures.OrderBy(y => y.DateTime).LastOrDefault().Device.company.Abrcompany,
                                        Temperature = item.Measures.OrderBy(y => y.DateTime).LastOrDefault().Value,
                                        Max = MaxTemp,
                                        Min = MinTemp,
                                        NameDevice = context.devices.Where(p => p.DeviceID == item.DeviceID).FirstOrDefault().name,
                                        MaxTolerance = MaxTol,
                                        MinTolerance = MinTol,
                                        TypeMeasure = item.TypeMeasure.Name
                                    });
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR chartBroadcaster.cs");
                    Debug.WriteLine(ex.GetBaseException().ToString());
                }
                temp = list;
            }

        }

        public class DashboardTemperatureResult
        {
            public string Company { get; set; }
            public int DeviceID { get; set; }
            public decimal? Temperature { get; set; }
            public decimal? Max { get; set; }
            public decimal? Min { get; set; }
            public string NameDevice { get; set; }
            public decimal? MaxTolerance { get; set; }
            public decimal? MinTolerance { get; set; }
            public string TypeMeasure { get; set; }

        }
    }
}


public class LastTemperatureUpdate
{

    // Singleton instance    
    private readonly static Lazy<LastTemperatureUpdate> _instance = new Lazy<LastTemperatureUpdate>(() => new LastTemperatureUpdate());
    // Send Data every 5 seconds    
    private int _updateInterval = int.Parse(ConfigurationManager.AppSettings["UpdateDashboardTime"].ToString());
    //Timer Class    
    private Timer _timer;
    private volatile bool _sendingLastTemperature = false;
    private readonly object _tempUpdateLock = new object();
    LineChart lineChart = new LineChart();
    TemperatureUpdate tempUpdate = new TemperatureUpdate();
    public string idUser = "";

    private LastTemperatureUpdate()
    {

    }

    public static LastTemperatureUpdate Instance
    {
        get
        {
            return _instance.Value;
        }
    }

    // Calling this method starts the Timer    
    public void GetTempData()
    {
        _timer = new Timer(TempTimerCallback, null, _updateInterval, _updateInterval);

    }
    private void TempTimerCallback(object state)
    {
        if (_sendingLastTemperature)
        {
            return;
        }
        lock (_tempUpdateLock)
        {
            if (!_sendingLastTemperature)
            {
                _sendingLastTemperature = true;
                SendLastTemperature();
                _sendingLastTemperature = false;
            }
        }
    }

    private void SendLastTemperature()
    {
        tempUpdate.TakeLastTemp(this.idUser);
        GetAllClients().All.UpdateTemperature(tempUpdate);
    }

    private static dynamic GetAllClients()
    {
        return GlobalHost.ConnectionManager.GetHubContext<TemperatureHub>().Clients;
    }

}