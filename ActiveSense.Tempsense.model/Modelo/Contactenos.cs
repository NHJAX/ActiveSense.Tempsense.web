using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Modelo
{
    public  class Contactenos
    {
        [Key]
        public int ContactenosID { get; set; }

        [Required, Display(Name ="Nombre Usuario")]
        public string Usuario { get; set; }

        [Required, Display(Name ="Nombre Empresa")]
        public string Empresa { get; set; }

        [Required, Display(Name = "Télefono")]
        public int Telefono { get; set; }

        [Required, Display(Name = "Correo"), EmailAddress]
        public string Correo { get; set; }

        [Required, Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }


    }
}
