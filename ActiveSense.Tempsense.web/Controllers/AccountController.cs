using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ActiveSense.Tempsense.web.Models;
using ActiveSense.Tempsense.web.Helpers;
using ActiveSense.Tempsense.model.Model;
using CaptchaMvc.HtmlHelpers;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.Text;
using System.Net.Mime;

namespace ActiveSense.Tempsense.web.Controllers
{
    [Authorize]
    public class AccountController : GenericController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        //Comment: roles of the system
        private const string USER_PROFILE = "User";
        private const string PROFILE_Administrator= "Administrator";
        private const string PROFILE_EXCLUDED_IN_CREATION = "Item";
        private const string STATE_USER = "NOT_ENABLED";

        //Comment:obtain the context to perform searches and operations EF
        ApplicationDbContext context;
        public AccountController()
        {
            context = new ApplicationDbContext();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Inactive()
        {
            return View();
        }



        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    //Comment: redirect according to profile
                    ApplicationUser user = await UserManager.FindAsync(model.UserName, model.Password);

                    //This unauthenticated user redirection
                    company company = dbActiveContext.companies.Where(em => em.CompanyID == user.CompanyID).SingleOrDefault();
                    if (company != null && company.Active == false)
                    {
                        return this.RedirectToAction("Inactive", "Account");
                    }

                    //redirect according to profile
                    if ((UserManager.IsInRole(user.Id, PROFILE_Administrator)))
                    {
                        return this.RedirectToAction("Index", "Home", new { area = PROFILE_Administrator});
                    }
                    if ((UserManager.IsInRole(user.Id, USER_PROFILE)))
                    {
                        return this.RedirectToAction("Index", "Home", new { area = USER_PROFILE });
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Attempt invalid.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterAnonymousViewModel model)
        {

             if (ModelState.IsValid && this.IsCaptchaValid("Captcha not valid."))
            {

                company company = new company();
                company.Name = model.CompanyName;
                company.Code = model.Nit;
                company.Mail = model.Email;
                company.Active = true;


                var idCompany = 0;
                try
                {
                    dbActiveContext.companies.Add(company);
                    dbActiveContext.SaveChanges();
                    idCompany = company.CompanyID;
                }
                //Captcha --Completely Automated Public Turing test to tell Computers and Humans Apart
                catch (Exception ex) {
                    ModelState.AddModelError("Error create Captcha", new Exception("Error in create Captcha "));
                }


                //Comment: Add custom for creation of user fields.
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    CompanyID = idCompany,
                    PhoneNumber = model.PhoneNumber,
                    State = STATE_USER,
                };
           
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    //Comment: excluir roles
                    user.CompanyID = idCompany;
                    await this.UserManager.AddToRoleAsync(user.Id, USER_PROFILE);
                    await this.UserManager.UpdateAsync(user);

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
                else {
                    if (idCompany != 0) {
                        dbActiveContext.companies.Remove(company);
                        dbActiveContext.SaveChanges();
                    } 

                }
                AddErrors(result, model);

            }
            //Comment:Add list of roles
            ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains(PROFILE_EXCLUDED_IN_CREATION))
                                         .ToList(), "Name", "Name");
            ViewBag.CompanyID = new SelectList(dbActiveContext.companies, "CompanyID", "Name");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                    {
                        ModelState.AddModelError("", "The Email does not exist.");
                    return View();
                }
                
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                string body = WriteBodyEmail(callbackUrl);
                MailMessage message = new MailMessage();
                message.To.Add(user.Email);
                message.From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"].ToString());
                message.Subject = ConfigurationManager.AppSettings["smtpSubject"].ToString();
                message.IsBodyHtml = true;      
                string html = "<img src='cid:imagen'/>"+ string.Format(body, callbackUrl);
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);
                LinkedResource img = new LinkedResource(Server.MapPath("~/Content/images/Tempsense-logo.png"), MediaTypeNames.Image.Jpeg);
                img.ContentId = "imagen";
                htmlView.LinkedResources.Add(img);
                message.AlternateViews.Add(htmlView);
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
                    await smtp.SendMailAsync(message);
                }
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private string WriteBodyEmail(string Url)
        {
            string Body = "<HR><h1><span style='font-family:arial,helvetica,sans-serif; '><span style='font - size:20px; '><span style='color:#ed4035;'>Requested Password Change </span></span></span></h1>";
            Body += "<p><span style='font-family:arial,helvetica,sans-serif; '><span style='font-size:14px;'><span style='color:#808080;'>To start the process of resetting passwords for your account by clicking the following URL: </span></span></span></p>";
            Body += "<p><a href='{0}'>"+Url.Split('?')[0] +"</a></p>";
            Body += "<p><span style='font-family:arial,helvetica,sans-serif; '><span style='color:#808080;'><span style='font-size:14px;'><font style='background-color:white;'>Remember that </font><font style='background-color:white;'>your</font><font style='background-color:white;'> password is personal and untransferable</font><font style='background-color:white;'> information </font><font style='background-color:white;'>.</font></span></span></span></p>";
            Body += "<p><span style='background-color:white;'>Please do not reply to this request to this e-mail.</span>>&nbsp;</span></span></span></span></p>";
            return Body;
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "The Email does not exist.");
                return View();
                // Don't reveal that the user does not exist
                //return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result, RegisterAnonymousViewModel model = null)
        {
            foreach (var error in result.Errors)
            {
                string sizeError = "";

                if (model != null) {
                    sizeError = validateEspanish(error, model);
                }
                
                if (sizeError.Length <= 0)
                {
                    ModelState.AddModelError("", error);
                }
            }
              
        }

        private string validateEspanish(string message, RegisterAnonymousViewModel model = null) {
            string sizeError = "";
            if (message == ("Name " + model.UserName + " is already taken."))
            {
                sizeError += "1";
                ModelState.AddModelError("", "The user "  + model. UserName + "already exists ");
            }
            if (message.Substring(0, message.IndexOf(" ")) == "Email")
            {
                sizeError += "2";
                ModelState.AddModelError("", "The Email  " + model.Email + " was already admitted.");
            }

            if (message == ("Passwords must have at least one lowercase ('a'-'z')."))
            {
                sizeError += "3";
                ModelState.AddModelError("", "The password must contain at least one character lowercase ('a'-'z').");
            }

            return sizeError;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}