namespace model
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
            Bitacoras = new HashSet<Bitacora>();
            TipoMedidaXDispositivoes = new HashSet<TipoMedidaXDispositivo>();
            Umbrals = new HashSet<Umbral>();
        }

        public int DispositivoID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        public string ClaveDispositivo { get; set; }

        public bool Activo { get; set; }

        public int EmpresaID { get; set; }

        public int? Medida { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bitacora> Bitacoras { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TipoMedidaXDispositivo> TipoMedidaXDispositivoes { get; set; }

        public virtual Empresa Empresa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Umbral> Umbrals { get; set; }
    }
}
