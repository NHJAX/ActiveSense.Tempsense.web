using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Model
{
    public  class ContactUs
    {
        [Key]
        public int ContactUsID { get; set; }

        [Required, Display(Name = "User Name")]
        public string user { get; set; }

        [Required, Display(Name = "Company Name")]
        public string Company { get; set; }

        [Required, Display(Name = "Phone")]
        public int Phone { get; set; }

        [Required, Display(Name = "Mail"), EmailAddress]
        public string Mail { get; set; }

        [Required, Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


    }
}
