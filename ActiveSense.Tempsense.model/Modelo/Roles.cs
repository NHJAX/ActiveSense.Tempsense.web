using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Modelo
{
    [Table("AspNetUserRoles")]
    public  class Roles
    {
        [Key]
        [DisplayName("Id")]
        public string Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }

    }
}
