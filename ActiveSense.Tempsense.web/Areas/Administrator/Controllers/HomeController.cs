using ActiveSense.Tempsense.web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ActiveSense.Tempsense.model.Model;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ActiveSense.Tempsense.web;
using System.Configuration;

namespace ActiveSense.Tempsense.web.Areas.Administrator.Controllers
{
    [ActiveSenseAutorize("Administrator")]
    public class HomeController : Controller
    {
        // GET: Administrator/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactUs(ContactUs model)
        {
            if (ModelState.IsValid)
            {
                var body = "< p > name of user: {0} < /p > < p > name of company: {1} < /p > < p > phone: {2} < /p > < p > mail: {3} < /p > < p > Description: {4} < /p >";
                MailMessage message = new MailMessage();
                message.To.Add(ConfigurationManager.AppSettings["smtpTo"].ToString()); 
                message.From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"].ToString());
                message.Subject = ConfigurationManager.AppSettings["smtpSubject"].ToString();
                message.Body = string.Format(body, model.user, model.Company, model.Phone, model.Mail, model.Description);
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
    }
}