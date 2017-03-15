
namespace ActiveSense.Tempsense.model.Modelo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Empresa
    {
        [Key] //clave primaria en la tabla
        [Display(Name = "Empresa")]
        public int EmpresaID { get; set; }//metodos de acceso de lectura y escritura a la propiedad

        [StringLength(100)] // no debe pasarse de 100 caracteres
        [Required(ErrorMessage = "El nombre empresa es requerido")]
        [DisplayName("Nombre Empresa")]
        public string Nombre { get; set; }

        [StringLength(11, ErrorMessage = "La máxima cantidad de caracteres es 11.")]
        [Required(ErrorMessage = "El NIT es requerido.")]
        [RegularExpression("^[0-9]{1,9}-[0-9]{1}$", ErrorMessage = "El formato del NIT ES ddddddddd-d")]
        [DisplayName("Nit")]
        public string Codigo { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "dirección de correo invalido")]
        public string Correo { get; set; }

        [StringLength(5, ErrorMessage = "La máxima cantidad de caracteres es 5.")]
        [DisplayName("Abr. Empresa")]
        //[StringLength(5, ErrorMessage = "El campo {0} debe contener entre {1} y (2) caracteres", MinimumLength = 5)]
        [Required(ErrorMessage = "La Abreviatura es requerida")]

        public string AbrEmpresa { get; set; }

        public bool Activo { get; set; }
        [DisplayName("Notificación por correo")]
        public bool Notificar_Correo { get; set; }
        [DisplayName("Notificación por SMS")]
        public bool Notificar_SMS { get; set; }

        public virtual ICollection<Dispositivos> Dispositivos { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
