using ActiveSense.Tempsense.model.Modelo;
using ActiveSense.Tempsense.web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ActiveSense.Tempsense.web.Areas.Usuario.Controllers
{
    public class HomeController : Controller
    {
        // GET: Usuario/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Contactenos()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contactenos(Contactenos model)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Nombre de Usuario: {0}</p> <p>Nombre de Empresa: {1}</p> <p>Telefono: {2}</p> <p>Correo: {3}</p> <p>Descripcion: {4}</p>";
                MailMessage message = new MailMessage();
                message.To.Add(ConfigurationManager.AppSettings["smtpTo"].ToString());
                message.From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"].ToString());
                message.Subject = ConfigurationManager.AppSettings["smtpSubject"].ToString();
                message.Body = string.Format(body, model.Usuario, model.Empresa, model.Telefono, model.Correo, model.Descripcion);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = ConfigurationManager.AppSettings["smtpUserName"].ToString(),
                        Password = ConfigurationManager.AppSettings["smtpPassword"].ToString()
                    };
                    smtp.Credentials = credential;
                    smtp.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"].ToString());
                    // smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["smtpEnableSsl"].ToString());
                    await smtp.SendMailAsync(message);
                    return RedirectToAction("Contact");
                }
            }
            return View(model);
        }
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult AcquireService() {
            return View();
        }
    }
}