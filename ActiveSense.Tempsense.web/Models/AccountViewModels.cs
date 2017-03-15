using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ActiveSense.Tempsense.web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    //SE:campos de registro de usuario
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Por favor ingrese el nombre usuario.")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        //[Required(ErrorMessage = "Por favor ingrese un correo.")]
        //[Display(Name = "Correo")]
        //[EmailAddress(ErrorMessage = "Correo con formato no válido.")]
        //public string Email { get; set; }


        [Required(ErrorMessage = "Por favor ingrese una contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordar?")]
        public bool RememberMe { get; set; }
    }

    //SE:campos de registro de usuario
    public class RegisterViewModel
    {
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

        [Required(ErrorMessage = "Por favor ingrese la contraseña.")]
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
        [Display(Name = "Teléfono")]
        [StringLength(10, MinimumLength = 7, ErrorMessage = "La logitud de número telefónico es de 7 a 10 digitos")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Por favor ingrese la confirmación de teléfono.")]
        [Display(Name = "Confirmar Teléfono")]
        [Compare("PhoneNumber", ErrorMessage = "El teléfono y su confirmación no son iguales.")]
        public string ConfirmPhone{ get; set; }
    }

    public class RegisterAnonymousViewModel
    {
       

        [Required(ErrorMessage = "Por favor ingrese un correo.")]
        [EmailAddress(ErrorMessage = "Correo con formato no válido.")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Por favor ingrese la contraseña.")]
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
        [Display(Name = "Teléfono")]
        [StringLength(10, MinimumLength = 7, ErrorMessage = "La logitud de número telefónico es de 7 a 10 digitos")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Por favor ingrese la confirmación de teléfono.")]
        [Display(Name = "Confirmar Teléfono")]
        [Compare("PhoneNumber", ErrorMessage = "El teléfono y su confirmación no son iguales.")]
        public string ConfirmPhone { get; set; }


        [StringLength(100)] // no debe pasarse de 100 caracteres
        [Required(ErrorMessage = "El nombre empresa es requerido")]
        [Display(Name = "Nombre Empresa")]
        public string NombreEmpresa { get; set; }

        [StringLength(11, ErrorMessage = "La máxima cantidad de caracteres es 11.")]
        [Required(ErrorMessage = "El NIT es requerido.")]
        [RegularExpression("^[0-9]{1,9}-[0-9]{1}$", ErrorMessage = "El formato del NIT ES ddddddddd-d")]
        [Display(Name = "Nit")]
        public string Nit { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El correo de empresa es requerido")]
        [EmailAddress(ErrorMessage = "Dirección de correo invalido")]
        public string CorreoEmpresa { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el teléfono de la empresa.")]
        [Display(Name = "Teléfono")]
        [StringLength(10, MinimumLength = 7, ErrorMessage = "La logitud de número telefónico es de 7 a 10 digitos")]
        public string PhoneNumberEmpresa { get; set; }

    }

    public class EditUserViewModel
    {
        public EditUserViewModel() { }
  
        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(ApplicationUser user)
        {
            this.UserName = user.Email;
            this.Password = user.PasswordHash;
            this.UserRoles = user.Roles.ToString();
            this.PhoneNumber = user.PhoneNumber;
            this.Email = user.Email;
        }

        [Required(ErrorMessage = "Por favor seleccione el perfil o rol.")]
        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }

        //[Required(ErrorMessage = "Por favor seleccione la empresa.")]
        //[Display(Name = "Empresa")]
        //public string Empresa { get; set; }

        [Required(ErrorMessage = "Por favor ingrese un correo.")]
        [EmailAddress(ErrorMessage = "Correo con formato no válido.")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Por favor ingrese la contraseña.")]
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
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Por favor ingrese la confirmación de teléfono.")]
        [Display(Name = "Confirmar Teléfono")]
        [Compare("PhoneNumber", ErrorMessage = "El teléfono y su confirmación no son iguales.")]
        public string ConfirmPhone { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
