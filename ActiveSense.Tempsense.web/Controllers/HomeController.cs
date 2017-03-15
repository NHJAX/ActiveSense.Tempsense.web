using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActiveSense.Tempsense.web.Controllers
{
    public class HomeController : GenericController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Inactive() {
            ViewBag.Message = "Usuario Inactivo.";
            return View();
        }
    }
}