namespace model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UsuarioViewModel
    {
        public string Id { get; set; }

        [Required]
        public string UserRoles { get; set; }

        public int EmpresaID { get; set; }

        [Required]
        public string Email { get; set; }

        [StringLength(100)]
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [Required]
        public string ConfirmPhone { get; set; }
    }
}
