using System.Configuration;
using System.Data.Entity;

namespace ActiveSense.Tempsense.model.Modelo
{
    public class ActiveSenseContext : DbContext
    {
        //name=TempsenseConnection
        public ActiveSenseContext() 
        {
        }

        public ActiveSenseContext(string connString)
        {
            this.Database.Connection.ConnectionString = connString;
        }

        public virtual DbSet<Dispositivos> Dispositivos { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActiveSense.Tempsense.model.Modelo.Medida>().Property(x => x.Valor).HasPrecision(5, 2);
        }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Modelo.Usuario> Usuarios { get; set; }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Modelo.Medida> Medidas { get; set; }
        
        public virtual DbSet<TipoMedida> TipoMedidas { get; set; }
        
        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Modelo.Umbral> Umbrals { get; set; }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Modelo.Perfil> Perfiles { get; set; }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Modelo.AspNetUsers> UsuariosASP { get; set; }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Modelo.Bitacoras> Bitacoras { get; set; }
    }
}
