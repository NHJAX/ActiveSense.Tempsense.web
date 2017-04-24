using ActiveSense.Tempsense.model.Model;
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
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ActiveSense.Tempsense.web.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        // GET: User/Home
        public ActionResult Index()
        {
            var controller = RouteData.Values["controller"];
            var action = RouteData.Values["action"];
            var id = RouteData.Values["id"];

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
                //var body = "< p > name of user: {0} < /p > < p > name of company: {1} < /p > < p > phone: {2} < /p > < p > mail: {3} < /p > < p > Description: {4} < /p >";
                //MailMessage message = new MailMessage();
                //message.To.Add(ConfigurationManager.AppSettings["smtpTo"].ToString()); 
                //message.From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"].ToString());
                //message.Subject = ConfigurationManager.AppSettings["smtpSubject"].ToString();
                //message.Body = string.Format(body, model.user, model.Company, model.Phone, model.Mail, model.Description);
                //message.IsBodyHtml = true;

                //using (var smtp = new SmtpClient())
                //{
                //    var credential = new NetworkCredential
                //    {
                //        UserName = ConfigurationManager.AppSettings["smtpUserName"].ToString(),  
                //        Password = ConfigurationManager.AppSettings["smtpPassword"].ToString() 
                //    };
                //    smtp.UseDefaultCredentials = false;
                //    smtp.Credentials = credential;
                //    smtp.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
                //    smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"].ToString());
                //    smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["smtpEnableSsl"].ToString());
                //    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //    await smtp.SendMailAsync(message);
                //}

                var body = "<p> name of user: {0} </p> <p> name of company: {1} </p  <p> phone: {2} </p> <p> mail: {3} </p  <p> Description: {4} </p> <p> <strong>-------------TempSence   NHJAX-DEV-----------------</strong> </p>";
                var apiKey = ConfigurationManager.AppSettings["NHJAX_SendGrid"].ToString();
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(model.Mail, model.user);
                var to = new EmailAddress(ConfigurationManager.AppSettings["smtpTo"].ToString(), "Contact Us dialog");
                var subject = ConfigurationManager.AppSettings["smtpSubject"].ToString();
                var plainTextContent = " ";
                var htmlContent = string.Format(body, model.user, model.Company, model.Phone, model.Mail, model.Description);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                var response = await client.SendEmailAsync(msg);

                return RedirectToAction("Contact");
            
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