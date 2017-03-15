namespace model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Usuario
    {
        [Key]
        public int UsuariosID { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string NombreUsuario { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Validar_Password { get; set; }

        [Required]
        public string Correo { get; set; }

        public int EmpresaID { get; set; }

        public int PerfilesID { get; set; }

        [Required]
        public string Celular { get; set; }

        public string ConfirmarCelular { get; set; }

        public virtual Empresa Empresa { get; set; }

        public virtual Perfil Perfil { get; set; }
    }
}
