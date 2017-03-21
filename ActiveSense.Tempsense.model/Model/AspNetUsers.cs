using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Model
{
    [Table("AspNetUsers")]
    public class AspNetUsers
    {
        [Key]
        [DisplayName("Id")]
        public string Id { get; set; }
        [DisplayName("Mail")]
        public string Email { get; set; }
        [DisplayName("Celular")]
        public string PhoneNumber { get; set; }
        [DisplayName("Name")]
        public string UserName { get; set; }
        public int CompanyID { get; set; }
        public string State { set; get; }
    }
}
