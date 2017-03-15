namespace model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Umbral
    {
        public int UmbralID { get; set; }

        public decimal Temperatura_min { get; set; }

        public decimal Temperatura_max { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha_inicio { get; set; }

        public int DispositivoID { get; set; }

        public decimal? Tolerancia_min { get; set; }

        public decimal? Tolerancia_max { get; set; }

        public virtual Dispositivo Dispositivo { get; set; }
    }
}
