//using Tempsense.model.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActiveSense.Tempsense.model.Model
{
    [Table("Devices")]
    public class devices
    {
        [Key]
        [Column(Order = 0)]
        public int DeviceID { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "name is required")]
        public string name { get; set; }
 
        public bool Active { get; set; }
        [Required(ErrorMessage = "the company name is required")]
        [Display(Name = "Company")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "The Measure is required")]
        [Display(Name = "Measure")]
        public int? TypeMeasureID { get; set; }

        public virtual Typemeasure TypeMeasure { get; set; }

        public virtual company company { get; set; }

        public virtual ICollection<Measure> Measures { get; set; }

        public virtual ICollection<Blogs> Blogs { get; set; }
        
    }
}
