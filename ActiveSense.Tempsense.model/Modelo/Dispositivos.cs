using ActiveSense.Tempsense.model.Modelo;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActiveSense.Tempsense.model.Modelo
{
    [Table("Dispositivo")]
    public class Dispositivos
    {
        [Key]
        [Column(Order = 0)]
        public int DispositivoID { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "El nombre  es requerido")]
        public string Nombre { get; set; }
        //[Display(Name = "Clave dispositivo")]
        //public string ClaveDispositivo { get; set; }

        public bool Activo { get; set; }
        [Required(ErrorMessage = "El nombre Empresa es requerido")]
        [Display(Name = "Empresa")]
        public int EmpresaID { get; set; }

        [Required(ErrorMessage = "La medida es requerida")]
        [Display(Name = "Medida")]
        public int? TipoMedidaID { get; set; }

        public virtual TipoMedida TipoMedida { get; set; }

        public virtual Empresa Empresa { get; set; }

        public virtual ICollection<Medida> Medidas { get; set; }

        public virtual ICollection<Bitacoras> Bitacora { get; set; }
        
    }
}
