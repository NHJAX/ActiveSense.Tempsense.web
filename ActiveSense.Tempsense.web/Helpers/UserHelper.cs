using ActiveSense.Tempsense.model.Modelo;
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
        public string obtenerDispositivosAsociados(string id, ActiveSenseContext context = null)
        {
            string idDispositivos = "";
            if (id !="")
            {
                ActiveSenseContext contextDB = context != null ? context : dbActiveContext;
                AspNetUsers usuario = contextDB.UsuariosASP.Find(id);

                if (usuario.EmpresaID != 0) {
                    var list = contextDB.Dispositivos.Where(u => u.EmpresaID == usuario.EmpresaID);
                    idDispositivos = string.Join(",", list.Select(item => item.DispositivoID));
                }
    

            }
            return idDispositivos;
        }
        public int obtenerEmpresasAsociadas(string id, ActiveSenseContext contextT = null)
        {
            int idEmpresa = 0;
            if (id != "")
            {
                ActiveSenseContext contextDB = contextT != null ? contextT : dbActiveContext;
                AspNetUsers usuario = contextDB.UsuariosASP.Find(id);
                if (usuario.EmpresaID  != 0) {
                    idEmpresa = usuario.EmpresaID;
                }
            }
            return idEmpresa;

        }

        public string obtenerPerfil(string id)
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