using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ActiveSense.Tempsense.model.Modelo;
using ActiveSense.Tempsense.web.Models;
using ActiveSense.Tempsense.web.Controllers;

namespace ActiveSense.Tempsense.web.Areas.Usuario.Controllers
{
    public class LogController : GenericController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Usuario/Log
        public ActionResult Index()
        {
            var bitacoras = dbActiveContext.Bitacoras.Include(b => b.Dispositivos);
            return View(bitacoras.ToList());
        }

        // GET: Usuario/Log/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bitacoras bitacoras = dbActiveContext.Bitacoras.Find(id);
            if (bitacoras == null)
            {
                return HttpNotFound();
            }
            return View(bitacoras);
        }

        // GET: Usuario/Log/Create
        public ActionResult Create()
        {
            ViewBag.DispositivoID = new SelectList(dbActiveContext.Dispositivos, "DispositivoID", "Nombre");
            return View();
        }

        // POST: Usuario/Log/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BitacorasID,Fecha,HoraInicio,HoraFin,Problema,Solucion,DispositivoID")] Bitacoras bitacoras)
        {
            if (ModelState.IsValid)
            {
                dbActiveContext.Bitacoras.Add(bitacoras);
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DispositivoID = new SelectList(dbActiveContext.Dispositivos, "DispositivoID", "Nombre", bitacoras.DispositivoID);
            return View(bitacoras);
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
