using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Model
{
    public class Blogs
    {
        [Key]
        public int BlogsID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date")]
        public DateTime date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "time start")]
        public DateTime timeStart { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:T}")]
        [DataType(DataType.Time)]
        [Display(Name = "time end")]
        public DateTime timeend { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Problem")]
        public string Problem { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Solution")]
        public string Solution { get; set; }

        [Display(Name = "Device name")]
        public int DeviceID { get; set; }

        public virtual devices devices { get; set; }
    }
}
