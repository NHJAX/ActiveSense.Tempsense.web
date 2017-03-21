using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ActiveSense.Tempsense.model.Model;
using ActiveSense.Tempsense.web.Models;
using System.Web.Script.Serialization;
using ActiveSense.Tempsense.web.Controllers;

namespace ActiveSense.Tempsense.web.Areas.Administrator.Controllers
{
    public class LogController : GenericController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Log
        public ActionResult Index()
        {
            var Blogs = dbActiveContext.Blogs.Include(b => b.devices);
            return View(Blogs.ToList());
        }
        // GET: Administrator/Log/Create
        public ActionResult Create()
        {
            ViewBag.DeviceID = new SelectList(dbActiveContext.devices, "DeviceID", "Name");
            return View();
        }

        // POST: Administrator/Log/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlogsID,Date, StartTime, EndTime, Problem, Solution,DeviceID")] Blogs Blogs)
        {
            if (ModelState.IsValid)
            {
                dbActiveContext.Blogs.Add(Blogs);
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeviceID = new SelectList(dbActiveContext.devices, "DeviceID", "Name", Blogs.DeviceID);
            return View(Blogs);
        }

        // GET: Administrator/Log/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blogs Blogs = dbActiveContext.Blogs.Find(id);
            if (Blogs == null)
            {
                return HttpNotFound();
            }
            return View(Blogs);
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
