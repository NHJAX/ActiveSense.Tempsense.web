namespace model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Medida
    {
        public int MedidaID { get; set; }

        public decimal? Valor { get; set; }

        public DateTime? FechaHora { get; set; }

        public int? TipoMedidaID { get; set; }

        public virtual TipoMedidaXDispositivo TipoMedidaXDispositivo { get; set; }
    }
}
