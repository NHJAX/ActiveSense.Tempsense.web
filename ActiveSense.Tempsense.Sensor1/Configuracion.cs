using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using System.Xml;

namespace ActiveSense.Tempsense.Sensor
{
    public static class Configuration
    {
        public static async Task<string> ReadDeviceKey()
        {
            var uri = new System.Uri("ms-appx:///config.txt");
            var sampleFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            return await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
        }
    }
}