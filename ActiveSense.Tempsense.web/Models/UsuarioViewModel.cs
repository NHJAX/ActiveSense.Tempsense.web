using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ActiveSense.Tempsense.web.Models
{
    public class UsuarioViewModel
    {


        public String Id { get; set; }

        [Required(ErrorMessage = "Por favor seleccione el perfil o rol.")]
        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }

        [Required(ErrorMessage = "Por favor seleccione una empresa.")]
        [Display(Name = "Empresas")]
        public int EmpresaID { get; set; }


        [Required(ErrorMessage = "Por favor ingrese un correo.")]
        [EmailAddress(ErrorMessage = "Correo con formato no válido.")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

     
        [StringLength(100, ErrorMessage = "El {0} debe ser al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y su confirmación no son iguales.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el nombre.")]
        [Display(Name = "Nombre")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el teléfono.")]
        [StringLength(10, MinimumLength = 7, ErrorMessage = "La logitud de número telefónico es de 7 a 10 digitos.")]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Por favor ingrese la confirmación de teléfono.")]
        [Display(Name = "Confirmar Teléfono")]
        [Compare("PhoneNumber", ErrorMessage = "El teléfono y su confirmación no son iguales.")]
        public string ConfirmPhone { get; set; }
    }
}