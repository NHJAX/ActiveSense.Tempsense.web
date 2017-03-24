using ActiveSense.Tempsense.web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActiveSense.Tempsense.web.Areas.User.Controllers
{
    [ActiveSenseAutorize("User")]
    public class UsersController : Controller
    {
        // GET: User/Users
        public ActionResult Index()
        {
            return View();
        }
    }
}