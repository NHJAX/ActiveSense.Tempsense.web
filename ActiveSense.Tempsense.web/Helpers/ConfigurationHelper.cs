using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ActiveSense.Tempsense.web.Helpers
{
    public class ConfigurationHelper
    {
        public static string IotHubConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["IotHubConnectionString"].ToString();
            }
        }

    }
}