using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Model
{
   
    public class Profiles
    {
        [Key]
        public int ProfilesID { get; set; }
        public string Name { get; set; }

    }
}
