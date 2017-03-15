namespace model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TipoMedidaXDispositivo")]
    public partial class TipoMedidaXDispositivo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TipoMedidaXDispositivo()
        {
            Medidas = new HashSet<Medida>();
        }

        [Key]
        public int TipoMedidaDisID { get; set; }

        public int? TipoMedidaID { get; set; }

        public int? DispositivoID { get; set; }

        public virtual Dispositivo Dispositivo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Medida> Medidas { get; set; }

        public virtual TipoMedida TipoMedida { get; set; }
    }
}
