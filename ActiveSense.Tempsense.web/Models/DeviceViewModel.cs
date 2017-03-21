using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActiveSense.Tempsense.web.Models
{
    public class DeviceViewModel
    {

        public int idDevice ;
        public string NameDevice;
        public string typeMeasure;
    }

    public class DeviceViewModel2
    {

        public int idDevice { get; set; }
        public string NameDevice { get; set; }
        public string typeMeasure { get; set; }
    }
}