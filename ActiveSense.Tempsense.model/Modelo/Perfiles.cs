using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Modelo
{
   
    public class Perfil
    {
        [Key]
        public int PerfilesID { get; set; }
        public string Nombre { get; set; }

    }
}
