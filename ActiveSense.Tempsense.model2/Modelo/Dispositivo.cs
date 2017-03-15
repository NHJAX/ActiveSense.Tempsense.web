namespace ActiveSense.Tempsense.model2.Modelo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Dispositivo")]
    public partial class Dispositivo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Dispositivo()
        {
            Medidas = new HashSet<Medida>();
        }

        public int DispositivoID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public int EmpresaID { get; set; }

        public int? TipoMedidaID { get; set; }

        public virtual TipoMedida TipoMedida { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Medida> Medidas { get; set; }
    }
}
