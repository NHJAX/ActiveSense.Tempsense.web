using ActiveSense.Tempsense.web.Helpers;
using System.Web.Mvc;
using ActiveSense.Tempsense.model.Model;
using System.Data.Entity;
using ActiveSense.Tempsense.web.Controllers;
using System.Net;
using ActiveSense.Tempsense.web.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.web.Areas.Administrator.Controllers
{
    [ActiveSenseAutorize("Administrator")]
    public class UsersController : GenericController
    {

        private const string PROFILE_EXCLUDED_IN_CREATION = "Item";

        ApplicationDbContext context = null;
        public UsersController() {
            context = new ApplicationDbContext();
        }
        // GET: Administrator/Users
        public ActionResult Index()
        {

            var Db = new ApplicationDbContext();
            var users = Db.Users;
            var model = new List<AspNetUsers>();

            foreach (var userTemp in users)
            {
                AspNetUsers user = new AspNetUsers();
                user.UserName = userTemp.UserName;
                user.Email = userTemp.Email;
                user.PhoneNumber = userTemp.PhoneNumber;
                user.Id = userTemp.Id;
                model.Add(user);
            }


            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        // GET: user/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspNetUsers user = dbActiveContext.AspNetUsers.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var updatedUser = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CompanyID = user.CompanyID,
                ConfirmPhone = user.PhoneNumber

            };

            
            ViewBag.CompanyID = new SelectList(dbActiveContext.companies, "CompanyID", "Name", user.CompanyID);

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var perfil = UserManager.GetRoles(user.Id);

            if (perfil != null) {
                ViewBag.UserRoles = new SelectList(context.Roles, "Name", "Name", perfil[0].ToString());
            }

            return View(updatedUser);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel user)
        {
            if (ModelState.IsValid)
            {

                
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                ApplicationUser userTemp = await UserManager.FindByIdAsync(user.Id);

                if (userTemp == null)
                {
                    return HttpNotFound();
                }

                userTemp.UserName = user.UserName;
                userTemp.PhoneNumber = user.PhoneNumber;
                userTemp.Email = user.Email;
                userTemp.CompanyID = user.CompanyID;

                var result = await UserManager.UpdateAsync(userTemp);

                if (result.Succeeded)
                {
                    //modifies user role
                    var perfil = UserManager.GetRoles(userTemp.Id);
                    UserManager.RemoveFromRole(userTemp.Id, perfil[0].ToString());
                    UserManager.AddToRole(userTemp.Id, user.UserRoles);
                    UserManager.Update(userTemp);

                    //the password is changed
                    if (user.ConfirmPassword != null) {
                        UserManager.RemovePassword(userTemp.Id);
                        UserManager.AddPassword(userTemp.Id, user.Password);
                        UserManager.Update(userTemp);
                    }
                    return RedirectToAction("Index");
                }
                
            }
            return View(user);
        }


        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers User = dbActiveContext.AspNetUsers.Find(id);
            if (User == null)
            {
                return HttpNotFound();
            }
            return View(User);
        }
        // POST: user/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUsers Users = dbActiveContext.AspNetUsers.Find(id);
            dbActiveContext.AspNetUsers.Remove(Users);
            dbActiveContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Register()
        {

            //SE:obtener lista de perfiles
            ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains(PROFILE_EXCLUDED_IN_CREATION))
                                            .ToList(), "Name", "Name");

            ViewBag.CompanyID = new SelectList(dbActiveContext.companies, "CompanyID", "Name");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (ModelState.IsValid)
            {
                //SE: Add custom for creation of user fields.
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    CompanyID = model.CompanyID,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.UserRoles);

                    return RedirectToAction("Index", "Users", new { area = "Administrator" });
                }
                AddErrors(result, model);
            }
            //SE:Add list of roles
            ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains(PROFILE_EXCLUDED_IN_CREATION))
                                         .ToList(), "Name", "Name");
            ViewBag.CompanyID = new SelectList(dbActiveContext.companies, "CompanyID", "Name");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void AddErrors(IdentityResult result, RegisterViewModel model = null)
        {
            foreach (var error in result.Errors)
            {
                string sizeError = "";

                if (model != null)
                {
                    sizeError = validateEspanish(error, model);
                }

                if (sizeError.Length <= 0)
                {
                    ModelState.AddModelError("", error);
                }
            }
        }

        private string validateEspanish(string message, RegisterViewModel model = null)
        {
            string sizeError = "";
            if (message == ("Name " + model.UserName + " is already taken."))
            {
                sizeError += "1";
                ModelState.AddModelError("", "El User " + model.UserName + " ya existe.");
            }

            if (message == ("Passwords must have at least one lowercase ('a'-'z')."))
            {
                sizeError += "2";
                ModelState.AddModelError("", "The password must contain at least one character lowercase ('a'-'z').");
            }

            if (message.Substring(0, message.IndexOf(" ")) == "Email")
            {
                sizeError += "3";
                ModelState.AddModelError("", "Email " + model.Email + "  was already entered.");
            }
            return sizeError;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbActiveContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}


