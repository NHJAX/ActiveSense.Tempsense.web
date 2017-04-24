using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ActiveSense.Tempsense.model.Model;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.web.Helpers;

namespace ActiveSense.Tempsense.web.Areas.Administrator.Controllers
{
    [ActiveSenseAutorize("Administrator")]
    public class ThresholdController : GenericController
    {
       

        // GET: Threshold
        public ActionResult Index()
        {
            return View((from x in dbActiveContext.Threshold where x.Active == true select x).ToList());
        }

        // GET: Threshold/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Threshold threshold = dbActiveContext.Threshold.Find(id);
            if (threshold == null)
            {
                return HttpNotFound();
            }
            return View(threshold);
        }

        // GET: Threshold/Create
        public ActionResult Create()
        {
            ViewBag.DeviceID = new SelectList(dbActiveContext.devices, "DeviceID", "Name");
            return View();
        }

        // POST:Threshold/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Add active in the include and the date current
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "thresholdID,Active,Temperature_min,Temperature_max,DeviceID,Tolerance_min,Tolerance_max")] Threshold threshold)
        {
            if (ModelState.IsValid)
            {
                threshold.DateHome = DateTime.Now;
                threshold.Active =true;
                dbActiveContext.Threshold.Add(threshold);
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(threshold);
        }

        // GET: threshold/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Threshold threshold = dbActiveContext.Threshold.Find(id);
            if (threshold == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceID = new SelectList(dbActiveContext.devices, "DeviceID", "Name" , threshold.DeviceID);
            return View(threshold);
        }

        // POST: Threshold/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "thresholdID,Active,Temperature_min,Temperature_max,DeviceID,Tolerance_min,Tolerance_max")] Threshold threshold)
        {
            if (ModelState.IsValid)
            {
                var z = dbActiveContext.Threshold.Where(w => w.thresholdID == threshold.thresholdID).DefaultIfEmpty().FirstOrDefault();
                z.Temperature_max = threshold.Temperature_max;
                z.Temperature_min = threshold.Temperature_min;
                z.Tolerance_min = threshold.Tolerance_min;
                z.Tolerance_max = threshold.Tolerance_max;
                z.Active = true;
                z.DeviceID = threshold.DeviceID;

                dbActiveContext.Entry(z).State = EntityState.Modified;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeviceID = new SelectList(dbActiveContext.devices, "DeviceID", "Name", threshold.DeviceID);
            return View(threshold);
        }

        // GET: Threshold/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Threshold threshold = dbActiveContext.Threshold.Find(id);
            if (threshold  == null)
            {
                return HttpNotFound();
            }
            return View(threshold );
        }

        // POST: Threshold/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Threshold threshold = dbActiveContext.Threshold.Find(id);
            dbActiveContext.Threshold.Remove(threshold);
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
