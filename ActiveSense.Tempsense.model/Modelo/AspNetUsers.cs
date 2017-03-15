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
    [Table("AspNetUsers")]
    public class AspNetUsers
    {
        [Key]
        [DisplayName("Id")]
        public string Id { get; set; }
        [DisplayName("Correo")]
        public string Email { get; set; }
        [DisplayName("Celular")]
        public string PhoneNumber { get; set; }
        [DisplayName("Nombre")]
        public string UserName { get; set; }
        public int EmpresaID { get; set; }
        public string State { set; get; }
    }
}
