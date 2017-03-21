using ActiveSense.Tempsense.model.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Helpers
{
    class UserHelper
    {
        public static string GetAssociatedDevice(string idUser)
        {
            string idDevice = "";
            if ( idUser!= "" )
            {
                using (ActiveSenseContext context = new ActiveSenseContext(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString))
                {
                    AspNetUsers user = context.AspNetUsers.Find(idUser);
                    var list = context.devices.Where(u => u.CompanyID == user.CompanyID);
                    idDevice = string.Join(",", list.Select(item => item.DeviceID));
                }
            }
            return idDevice;
        }
    }
}
