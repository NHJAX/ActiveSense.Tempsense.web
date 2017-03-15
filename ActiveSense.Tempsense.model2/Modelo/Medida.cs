namespace ActiveSense.Tempsense.model2.Modelo
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

        public int? DispositivoID { get; set; }

        public virtual Dispositivo Dispositivo { get; set; }
    }
}
