namespace ActiveSense.Tempsense.model.Modelo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TipoMedida
    {
        public int TipoMedidaID { get; set; }

        [Required(ErrorMessage = "El Nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; }

        public virtual ICollection<Dispositivos> Dispositivo { get; set; }
    }
}
