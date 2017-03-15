using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActiveSense.Tempsense.web.Models
{
    public class DispositivoViewModel
    {

        public int idDispositivo ;
        public string nombreDispositivo;
        public string tipoMedida;
    }

    public class DispositivoViewModel2
    {

        public int idDispositivo { get; set; }
        public string nombreDispositivo { get; set; }
        public string tipoMedida { get; set; }
    }
}