using ActiveSense.Tempsense.model.Modelo;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.web.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ActiveSense.Tempsense.web.Areas.Usuario.Controllers
{
    [ActiveSenseAutorize("Usuario")]
    public class UmbralController : GenericController
    {

        private UserHelper userHelper = null;
        public UmbralController()
        { 
            userHelper = new UserHelper();
        }

        // GET: Umbrals
        public ActionResult Index()
        {
            string idUsuario  = User.Identity.GetUserId();
            string dispositivosTemp = userHelper.obtenerDispositivosAsociados(idUsuario);
            List<Umbral> umbrales = dbActiveContext.Umbrals.Where(umb => umb.Activo == true && dispositivosTemp.Contains(umb.DispositivoID.ToString())).ToList();
            return View(umbrales);
        }

        // GET: Umbrals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Umbral umbral = dbActiveContext.Umbrals.Find(id);
            if (umbral == null)
            {
                return HttpNotFound();
            }
            return View(umbral);
        }

        // GET: Umbrals/Create
        public ActionResult Create()
        {
            string idUsuario = User.Identity.GetUserId();
            int idEmpresa = userHelper.obtenerEmpresasAsociadas(idUsuario);
            ViewBag.DispositivoID = new SelectList(dbActiveContext.Dispositivos.Where(dis=>dis.EmpresaID == idEmpresa), "DispositivoID", "Nombre");
            return View();
        }

        // POST: Umbrals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Agrego Activo en el include y la fecha actual
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UmbralID,Activo,Temperatura_min,Temperatura_max,DispositivoID,Tolerancia_min,Tolerancia_max")] Umbral umbral)
        {
            if (ModelState.IsValid)
            {
                umbral.Fecha_inicio = DateTime.Now;
                umbral.Activo = true;
                dbActiveContext.Umbrals.Add(umbral);
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            string idUsuario = User.Identity.GetUserId();
            int idEmpresa = userHelper.obtenerEmpresasAsociadas(idUsuario);
            ViewBag.DispositivoID = new SelectList(dbActiveContext.Dispositivos.Where(dis => dis.EmpresaID == idEmpresa), "DispositivoID", "Nombre");
            return View(umbral);
        }

        // GET: Umbrals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Umbral umbral = dbActiveContext.Umbrals.Find(id);
            if (umbral == null)
            {
                return HttpNotFound();
            }
            string idUsuario = User.Identity.GetUserId();
            int idEmpresa = userHelper.obtenerEmpresasAsociadas(idUsuario);
            ViewBag.DispositivoID = new SelectList(dbActiveContext.Dispositivos.Where(dis => dis.EmpresaID == idEmpresa), "DispositivoID", "Nombre", umbral.DispositivoID);
            return View(umbral);
        }

        // POST: Umbrals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UmbralID,Activo,Temperatura_min,Temperatura_max,DispositivoID,Tolerancia_min,Tolerancia_max")] Umbral umbral)
        {
            if (ModelState.IsValid)
            {
                umbral.Fecha_inicio = DateTime.Now;
                umbral.Activo = true;
                dbActiveContext.Entry(umbral).State = EntityState.Modified;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            string idUsuario = User.Identity.GetUserId();
            int idEmpresa = userHelper.obtenerEmpresasAsociadas(idUsuario);
            ViewBag.DispositivoID = new SelectList(dbActiveContext.Dispositivos.Where(dis => dis.EmpresaID == idEmpresa), "DispositivoID", "Nombre", umbral.DispositivoID);
            return View(umbral);
        }

        // GET: Umbrals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Umbral umbral = dbActiveContext.Umbrals.Find(id);
            if (umbral == null)
            {
                return HttpNotFound();
            }
            return View(umbral);
        }

        // POST: Umbrals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Umbral umbral = dbActiveContext.Umbrals.Find(id);
            dbActiveContext.Umbrals.Remove(umbral);
            dbActiveContext.SaveChanges();
            return RedirectToAction("Index");
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