using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ActiveSense.Tempsense.web.Models
{
    public class UserViewModel
    {


        public String Id { get; set; }

        [Required(ErrorMessage = "Please select role or profile.")]
        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }

        [Required(ErrorMessage = "Please select a company.")]
        [Display(Name = "Companies")]
        public int CompanyID { get; set; }


        [Required(ErrorMessage = "Please enter an email.")]
        [EmailAddress(ErrorMessage = "Mail with invalid format.")]
        [Display(Name = "Mail")]
        public string Email { get; set; }

     
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
        [StringLength(10, MinimumLength = 7, ErrorMessage = "the length of number phone is from 7 to 10 digit.")]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the phone confirmation.")]
        [Display(Name = "Confirm phone")]
        [Compare("PhoneNumber", ErrorMessage = "the phone and confirmation are not equal.")]
        public string ConfirmPhone { get; set; }
    }
}