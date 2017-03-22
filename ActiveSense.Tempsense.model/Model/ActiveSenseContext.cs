using System.Configuration;
using System.Data.Entity;

namespace ActiveSense.Tempsense.model.Model
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

        public virtual DbSet<devices> devices { get; set; }
        public virtual DbSet<company> companies { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActiveSense.Tempsense.model.Model.Measure>().Property(x => x.Value).HasPrecision(5, 2);
        }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Model.Users> Users { get; set; }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Model.Measure> Measure { get; set; }
        
        public virtual DbSet<TypeMeasure> TypeMeasure { get; set; }
        
        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Model.Threshold> Threshold { get; set; }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Model.Profiles> Profiles { get; set; }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Model.AspNetUsers> AspNetUsers { get; set; }

        public System.Data.Entity.DbSet<ActiveSense.Tempsense.model.Model.Blogs> Blogs { get; set; }
    }
}
