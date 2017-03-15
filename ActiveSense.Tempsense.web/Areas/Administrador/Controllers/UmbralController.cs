using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ActiveSense.Tempsense.model.Modelo;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.web.Helpers;

namespace ActiveSense.Tempsense.web.Areas.Administrador.Controllers
{
    [ActiveSenseAutorize("Administrador")]
    public class UmbralController : GenericController
    {
       

        // GET: Umbrals
        public ActionResult Index()
        {
            return View((from x in dbActiveContext.Umbrals where x.Activo == true select x).ToList());
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
            ViewBag.DispositivoID = new SelectList(dbActiveContext.Dispositivos, "DispositivoID", "Nombre");
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
                umbral.Activo =true;
                dbActiveContext.Umbrals.Add(umbral);
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }

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
            ViewBag.DispositivoID = new SelectList(dbActiveContext.Dispositivos, "DispositivoID", "Nombre" , umbral.DispositivoID);
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
                var z = dbActiveContext.Umbrals.Where(w => w.UmbralID == umbral.UmbralID).DefaultIfEmpty().FirstOrDefault();
                z.Temperatura_max = umbral.Temperatura_max;
                z.Temperatura_min = umbral.Temperatura_min;
                z.Tolerancia_min = umbral.Tolerancia_min;
                z.Tolerancia_max = umbral.Tolerancia_max;
                z.Activo = true;
                z.DispositivoID = umbral.DispositivoID;

                dbActiveContext.Entry(z).State = EntityState.Modified;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DispositivoID = new SelectList(dbActiveContext.Dispositivos, "DispositivoID", "Nombre", umbral.DispositivoID);
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
