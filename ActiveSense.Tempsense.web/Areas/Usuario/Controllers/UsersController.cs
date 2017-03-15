using ActiveSense.Tempsense.web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActiveSense.Tempsense.web.Areas.Usuario.Controllers
{
    [ActiveSenseAutorize("Usuario")]
    public class UsersController : Controller
    {
        // GET: Usuario/Users
        public ActionResult Index()
        {
            return View();
        }
    }
}