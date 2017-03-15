using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Modelo
{
    public partial class Usuario
    {
        [Key]
        public int UsuariosID { get; set; }
        [Required(ErrorMessage = "El Nombre es requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El Usuario es requerido")]
        [RegularExpression(@"([a-zA-Z\d]+[\w\d]*|)[a-zA-Z]+[\w\d.]*", ErrorMessage = "usuario invalido")]

        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "El Password es requerido")]
        public string Password { get; set; }
        [Required(ErrorMessage = "El Password es requerido")]
        public string Validar_Password { get; set; }
        //[Display(Name = "Empresa")]

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "dirección de correo invalido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El numero es requerido")]
        public string Celular { get; set; }

        public string ConfirmarCelular { get; set; }
        public int EmpresaID { get; set; }
        public virtual Empresa Empresa { get; set; }
        [Display(Name = "Perfil")]
        public int PerfilesID { get; set; }
        public virtual Perfil Perfiles { get; set; }
       
    }
}
