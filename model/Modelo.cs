namespace model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Modelo : DbContext
    {
        public Modelo()
            : base("name=Modelo")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Bitacora> Bitacoras { get; set; }
        public virtual DbSet<Dispositivo> Dispositivoes { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Medida> Medidas { get; set; }
        public virtual DbSet<Perfil> Perfils { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TipoMedida> TipoMedidas { get; set; }
        public virtual DbSet<TipoMedidaXDispositivo> TipoMedidaXDispositivoes { get; set; }
        public virtual DbSet<Umbral> Umbrals { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<UsuarioViewModel> UsuarioViewModels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Bitacora>()
                .Property(e => e.Problema)
                .IsUnicode(false);

            modelBuilder.Entity<Bitacora>()
                .Property(e => e.Solucion)
                .IsUnicode(false);

            modelBuilder.Entity<Dispositivo>()
                .HasMany(e => e.Umbrals)
                .WithRequired(e => e.Dispositivo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Medida>()
                .Property(e => e.Valor)
                .HasPrecision(5, 2);

            modelBuilder.Entity<TipoMedidaXDispositivo>()
                .HasMany(e => e.Medidas)
                .WithOptional(e => e.TipoMedidaXDispositivo)
                .HasForeignKey(e => e.TipoMedidaID);

            modelBuilder.Entity<Umbral>()
                .Property(e => e.Tolerancia_min)
                .HasPrecision(4, 2);

            modelBuilder.Entity<Umbral>()
                .Property(e => e.Tolerancia_max)
                .HasPrecision(4, 2);
        }
    }
}
