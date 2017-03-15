namespace model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TipoMedida
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TipoMedida()
        {
            TipoMedidaXDispositivoes = new HashSet<TipoMedidaXDispositivo>();
        }

        public int TipoMedidaID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TipoMedidaXDispositivo> TipoMedidaXDispositivoes { get; set; }
    }
}
