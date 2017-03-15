using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Modelo
{
    public class Umbral
    {
        public ActiveSenseContext dbActiveContext = new ActiveSenseContext(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString);
        [Key]
        [DisplayName("umbral")]
        public int UmbralID { get; set; }

        [DisplayName("Valor Minimo")]
        public decimal Temperatura_min { get; set; }

        [DisplayName("Valor Maximo")]
        public decimal Temperatura_max { get; set; }
        [Required]
        [DisplayName("Tolerancia Minima")]
        public decimal Tolerancia_min { get; set; }
        [Required]
        [DisplayName("Tolerancia Maxima")]
        public decimal Tolerancia_max { get; set; }

        public bool Activo { get; set; }
        public DateTime Fecha_inicio { get; set; }

        [DisplayName("Dispositivo")]
        public int DispositivoID { get; set; }

        [NotMapped]
        public string Nombre
        {
            get
            {
                return dbActiveContext.Dispositivos.Where(w => w.DispositivoID == this.DispositivoID).Select(s => s.Nombre).FirstOrDefault();
            }
        }
    }
}
