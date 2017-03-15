namespace model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Bitacora
    {
        [Key]
        public int BitacorasID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Fecha { get; set; }

        public DateTime? HoraInicio { get; set; }

        public DateTime? HoraFin { get; set; }

        [StringLength(2000)]
        public string Problema { get; set; }

        [StringLength(2000)]
        public string Solucion { get; set; }

        public int? DispositivoID { get; set; }

        public virtual Dispositivo Dispositivo { get; set; }
    }
}
