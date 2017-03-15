using ActiveSense.Tempsense.web.Helpers;
using System.Web.Mvc;
using ActiveSense.Tempsense.model.Modelo;
using System.Data.Entity;
using ActiveSense.Tempsense.web.Controllers;
using System.Net;
using ActiveSense.Tempsense.web.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.web.Areas.Administrador.Controllers
{
    [ActiveSenseAutorize("Administrador")]
    public class UsersController : GenericController
    {

        private const string PERFIL_EXCLUIDO_EN_CREACION = "Item";

        ApplicationDbContext context = null;
        public UsersController() {
            context = new ApplicationDbContext();
        }
        // GET: Administrador/Users
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

        // GET: Usuario/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspNetUsers usuario = dbActiveContext.UsuariosASP.Find(id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            var updatedUser = new UsuarioViewModel
            {
                Id = usuario.Id,
                UserName = usuario.UserName,
                Email = usuario.Email,
                PhoneNumber = usuario.PhoneNumber,
                EmpresaID = usuario.EmpresaID,
                ConfirmPhone = usuario.PhoneNumber

            };

            
            ViewBag.EmpresaID = new SelectList(dbActiveContext.Empresas, "EmpresaID", "Nombre", usuario.EmpresaID);

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var perfil = UserManager.GetRoles(usuario.Id);

            if (perfil != null) {
                ViewBag.UserRoles = new SelectList(context.Roles, "Name", "Name", perfil[0].ToString());
            }

            return View(updatedUser);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UsuarioViewModel usuario)
        {
            if (ModelState.IsValid)
            {

                
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                ApplicationUser userTemp = await UserManager.FindByIdAsync(usuario.Id);

                if (userTemp == null)
                {
                    return HttpNotFound();
                }

                userTemp.UserName = usuario.UserName;
                userTemp.PhoneNumber = usuario.PhoneNumber;
                userTemp.Email = usuario.Email;
                userTemp.EmpresaID = usuario.EmpresaID;

                var result = await UserManager.UpdateAsync(userTemp);

                if (result.Succeeded)
                {
                    //se modifica rol de usuario
                    var perfil = UserManager.GetRoles(userTemp.Id);
                    UserManager.RemoveFromRole(userTemp.Id, perfil[0].ToString());
                    UserManager.AddToRole(userTemp.Id, usuario.UserRoles);
                    UserManager.Update(userTemp);

                    //se modifica el password
                    if (usuario.ConfirmPassword != null) {
                        UserManager.RemovePassword(userTemp.Id);
                        UserManager.AddPassword(userTemp.Id, usuario.Password);
                        UserManager.Update(userTemp);
                    }
                    return RedirectToAction("Index");
                }
                
            }
            return View(usuario);
        }


        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers usuario = dbActiveContext.UsuariosASP.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }
        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUsers usuarios = dbActiveContext.UsuariosASP.Find(id);
            dbActiveContext.UsuariosASP.Remove(usuarios);
            dbActiveContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Register()
        {

            //SE:obtener lista de perfiles
            ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains(PERFIL_EXCLUIDO_EN_CREACION))
                                            .ToList(), "Name", "Name");

            ViewBag.EmpresaID = new SelectList(dbActiveContext.Empresas, "EmpresaID", "Nombre");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (ModelState.IsValid)
            {
                //SE: agregar campos personalizados para creacion de usuario.
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmpresaID = model.EmpresaID,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.UserRoles);

                    return RedirectToAction("Index", "Users", new { area = "Administrador" });
                }
                AddErrors(result, model);
            }
            //SE:agregar lista de roles
            ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains(PERFIL_EXCLUIDO_EN_CREACION))
                                         .ToList(), "Name", "Name");
            ViewBag.EmpresaID = new SelectList(dbActiveContext.Empresas, "EmpresaID", "Nombre");

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

        private string validateEspanish(string mensaje, RegisterViewModel model = null)
        {
            string sizeError = "";
            if (mensaje == ("Name " + model.UserName + " is already taken."))
            {
                sizeError += "1";
                ModelState.AddModelError("", "El Usuario " + model.UserName + " ya existe.");
            }

            if (mensaje == ("Passwords must have at least one lowercase ('a'-'z')."))
            {
                sizeError += "2";
                ModelState.AddModelError("", "La contraseña debe contener al menos un caracter en minúscula ('a'-'z').");
            }

            if (mensaje.Substring(0, mensaje.IndexOf(" ")) == "Email")
            {
                sizeError += "3";
                ModelState.AddModelError("", "El Email " + model.Email + " ya fue ingresado.");
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


