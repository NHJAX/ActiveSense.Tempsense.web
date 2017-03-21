using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Model
{
    public class Threshold
    {
        public ActiveSenseContext dbActiveContext = new ActiveSenseContext(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString);
        [Key]
        [DisplayName("threshold")]
        public int thresholdID { get; set; }

        [DisplayName("Minimum Value")]
        public decimal Temperature_min { get; set; }

        [DisplayName("Maximum Value")]
        public decimal Temperature_max { get; set; }
        [Required]
        [DisplayName("Tolerance minimum")]
        public decimal Tolerance_min { get; set; }
        [Required]
        [DisplayName("Tolerance maximum")]
        public decimal Tolerance_max { get; set; }

        public bool Active { get; set; }
        public DateTime DateHome { get; set; }

        [DisplayName("Device")]
        public int DeviceID { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                return dbActiveContext.devices.Where(w => w.DeviceID == this.DeviceID).Select(s => s.name).FirstOrDefault();
            }
        }
    }
}
