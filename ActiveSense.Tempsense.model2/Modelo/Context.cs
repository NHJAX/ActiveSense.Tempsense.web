namespace ActiveSense.Tempsense.model2.Modelo
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Context : DbContext
    {
        public Context()
            : base("name=Context")
        {
        }

        public virtual DbSet<Dispositivo> Dispositivoes { get; set; }
        public virtual DbSet<Medida> Medidas { get; set; }
        public virtual DbSet<TipoMedida> TipoMedidas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medida>()
                .Property(e => e.Valor)
                .HasPrecision(5, 2);
        }
    }
}
