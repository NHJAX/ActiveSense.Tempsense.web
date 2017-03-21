
namespace ActiveSense.Tempsense.model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class company
    {
        [Key] //primary key in the table
        [Display(Name = "Company")]
        public int CompanyID { get; set; }//read / write access to the property methods

        [StringLength(100)] // should not exceed 100 characters
        [Required(ErrorMessage = "The company name is required")]
        [DisplayName("Company Name")]
        public string Name { get; set; }

        [StringLength(11, ErrorMessage = "the maximum number of characters is 11.")]
        [Required(ErrorMessage = "NIT required.")]
        [RegularExpression("^[0-9]{1,9}-[0-9]{1}$", ErrorMessage = "format the NIT is ddddddddd-d")]
        [DisplayName("Nit")]
        public string Code { get; set; }

        [Display(Name = "Mail")]
        [Required(ErrorMessage = "mail is required")]
        [EmailAddress(ErrorMessage = "invalid email address")]
        public string Mail { get; set; }

        [StringLength(5, ErrorMessage = "the maximum number of characters is 5.")]
        [DisplayName("Abr. Company")]
        [Required(ErrorMessage = "Abbreviation is required")]

        public string Abrcompany { get; set; }

        public bool Active { get; set; }
        [DisplayName("Notification by mail")]
        public bool Notify_Mail { get; set; }
        [DisplayName("SMS notification")]
        public bool Notify_SMS { get; set; }

        public virtual ICollection<devices> device { get; set; }
        public virtual ICollection<Users> users { get; set; }
    }
}
