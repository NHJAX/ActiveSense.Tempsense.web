using ActiveSense.Tempsense.model.Modelo;
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
        public static string obtenerDispositivoAsociados(string idUser)
        {
            string idDispositivos = "";
            if ( idUser!= "" )
            {
                using (ActiveSenseContext context = new ActiveSenseContext(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString))
                {
                    AspNetUsers usuario = context.UsuariosASP.Find(idUser);
                    var list = context.Dispositivos.Where(u => u.EmpresaID == usuario.EmpresaID);
                    idDispositivos = string.Join(",", list.Select(item => item.DispositivoID));
                }
            }
            return idDispositivos;
        }
    }
}
