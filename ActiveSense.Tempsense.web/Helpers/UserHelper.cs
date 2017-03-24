using ActiveSense.Tempsense.model.Model;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActiveSense.Tempsense.web.Helpers
{
    public class UserHelper : GenericController
    {
        public string GetDevicesPartners(string id, ActiveSenseContext context = null)
        {
            string iddevices = "";
            if (id !="")
            {
                ActiveSenseContext contextDB = context != null ? context : dbActiveContext;
                AspNetUsers User = contextDB.AspNetUsers.Find(id);

                if (User.CompanyID != 0) {
                    var list = contextDB.devices.Where(u => u.CompanyID == User.CompanyID);
                    iddevices = string.Join(",", list.Select(item => item.DeviceID));
                }
    

            }
            return iddevices;
        }
        public int GetAssociatedCompanies(string id, ActiveSenseContext contextT = null)
        {
            int idCompany = 0;
            if (id != "")
            {
                ActiveSenseContext contextDB = contextT != null ? contextT : dbActiveContext;
                AspNetUsers user = contextDB.AspNetUsers.Find(id);
                if (user.CompanyID  != 0) {
                    idCompany = user.CompanyID;
                }
            }
            return idCompany;

        }

        public string GetProfile(string id)
        {
            string perfil = "";
            if (id != "")
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var perfilTemp = UserManager.GetRoles(id);
                if (perfilTemp != null) {
                    perfil = perfilTemp[0].ToString();
                }
                
            }
            return perfil;
        }

 
    }
}