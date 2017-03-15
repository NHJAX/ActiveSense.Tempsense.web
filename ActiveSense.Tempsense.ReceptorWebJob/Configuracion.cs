//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Configuration;
//using Microsoft.AspNet.SignalR.Configuration;

using System.Configuration;

namespace ActiveSense.Tempsense.ReceptorWebJob
{
    public static class Configuracion
    {
        public static string EventHubConnectionString
        {
            get ///ConfigurationManager
            {
                return ConfigurationManager.AppSettings["EventHubConnectionString"].ToString();
            }
        }

        public static string EventHubName
        {
            get
            {
                return ConfigurationManager.AppSettings["EventHubName"].ToString();
            }
        }

        public static string StorageAccountName
        {
            get
            {
                return ConfigurationManager.AppSettings["StorageAccountName"].ToString();
            }
        }

        public static string StorageAccountKey
        {
            get
            {
                return ConfigurationManager.AppSettings["StorageAccountKey"].ToString();
            }
        }

        public static string BDConnectionString
        {
            get
            {
                return (ConfigurationManager.AppSettings["BDConnectionString"].ToString());
            }
        }
        public static int TamanoLoteMensajes
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["TamanoLoteMensajes"].ToString());
            }
        }
    }
}
