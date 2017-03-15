using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Modelo
{
    public class Bitacoras
    {
        [Key]
        public int BitacorasID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Hora Inicio")]
        public DateTime HoraInicio { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:T}")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora Fin")]
        public DateTime HoraFin { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Problema")]
        public string Problema { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Solución")]
        public string Solucion { get; set; }

        [Display(Name = "Nombre Dispositivo")]
        public int DispositivoID { get; set; }

        public virtual Dispositivos Dispositivos { get; set; }
    }
}
