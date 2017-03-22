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

    //SE:user registration fields
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter the Username.")]
        [Display(Name = "User")]
        public string UserName { get; set; }

        //[Required(ErrorMessage = "Please enter Email address.")]
        //[Display(Name = "Email")]
        //[EmailAddress(ErrorMessage = "Mail with invalid format.")]
        //public string Email { get; set; }


        [Required(ErrorMessage = "Please enter the Password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember?")]
        public bool RememberMe { get; set; }
    }

    //SE:user registration fields
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please select role or profile.")]
        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }

        [Required(ErrorMessage = "Please select a company.")]
        [Display(Name = "Companies")]
        public int CompanyID { get; set; }


        [Required(ErrorMessage = "Please enter an email.")]
        [EmailAddress(ErrorMessage = "Mail with invalid format.")]
        [Display(Name = "EMail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the password.")]
        [StringLength(100, ErrorMessage = "the {0} must be at least {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "the password and confirmation are not equal.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter the Name.")]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter the phone.")]
        [Display(Name = "Phone")]
        [StringLength(10, MinimumLength = 7, ErrorMessage = "the phone number length is 7 to 10 digit")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the phone confirmation.")]
        [Display(Name = "Confirm phone")]
        [Compare("PhoneNumber", ErrorMessage = " the phone and confirmation are not equal.")]
        public string ConfirmPhone{ get; set; }
    }

    public class RegisterAnonymousViewModel
    {
       

        [Required(ErrorMessage = "Please enter an email.")]
        [EmailAddress(ErrorMessage = "Mail with invalid format.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the password.")]
        [StringLength(100, ErrorMessage = "the {0} must be at least {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "the password and confirmation are not equal.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter the Name.")]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter the phone.")]
        [Display(Name = "Phone")]
        [StringLength(10, MinimumLength = 7, ErrorMessage = "the phone number length is 7 to 10 digit")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the phone confirmation.")]
        [Display(Name = "Confirm Phone")]
        [Compare("PhoneNumber", ErrorMessage = "the phone and confirmation are not equal.")]
        public string ConfirmPhone { get; set; }


        [StringLength(100)] // not must pass 100 characters
        [Required(ErrorMessage = "the Company Nme is required")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [StringLength(11, ErrorMessage = "the maximum number of characters is 11.")]
        [Required(ErrorMessage = "NIT required.")]
        [RegularExpression("^[0-9]{1,9}-[0-9]{1}$", ErrorMessage = "the format of the NIT is ddddddddd-d")]
        [Display(Name = "Nit")]
        public string Nit { get; set; }

        [Display(Name = "Mail")]
        [Required(ErrorMessage = "company email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string CompanyEmail { get; set; }

        [Required(ErrorMessage = "Please enter the company phone.")]
        [Display(Name = "Phone")]
        [StringLength(10, MinimumLength = 7, ErrorMessage = "Lthe phone number length is 7 to 10 digit")]
        public string PhoneNumberCompany { get; set; }

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

        [Required(ErrorMessage = "Please select role or profile.")]
        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }

          [Required(ErrorMessage = "Please enter an email.")]
        [EmailAddress(ErrorMessage = "Mail with invalid format.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the password.")]
        [StringLength(100, ErrorMessage = "the {0} must be at least {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "the password and confirmation are not equal.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter the Name.")]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter the phone.")]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the phone confirmation.")]
        [Display(Name = "Confirm Phone")]
        [Compare("PhoneNumber", ErrorMessage = "the phone and confirmation are not equal.")]
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
