using ActiveSense.Tempsense.model.Model;
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

namespace ActiveSense.Tempsense.web.Areas.Users.Controllers
{
    [ActiveSenseAutorize("User")]
    public class ThresholdController : GenericController
    {

        private UserHelper userHelper = null;
        public ThresholdController()
        { 
            userHelper = new UserHelper();
        }

        // GET: Threshold
        public ActionResult Index()
        {
            string idUser  = User.Identity.GetUserId();
            string devicesTemp = userHelper.GetDevicesPartners(idUser);
            List<Threshold> Thresholdes = dbActiveContext.Threshold.Where(x => x.Active == true && devicesTemp.Contains(x.DeviceID.ToString())).ToList();
            return View(Thresholdes);
        }

        // GET: Threshold/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Threshold Threshold = dbActiveContext.Threshold.Find(id);
            if (Threshold == null)
            {
                return HttpNotFound();
            }
            return View(Threshold);
        }

        // GET: Threshold/Create
        public ActionResult Create()
        {
            string idUser = User.Identity.GetUserId();
            int idCompany = userHelper.GetAssociatedCompanies(idUser);
            ViewBag.DeviceID = new SelectList(dbActiveContext.devices.Where(dis=>dis.CompanyID == idCompany), "DeviceID", "Name");
            return View();
        }

        // POST: Threshold/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Add active in the include and the current date
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ThresholdID,Active,Temperature_min,Temperature_max,DeviceID,Tolerance_min,Tolerance_max")] Threshold Threshold)
        {
            if (ModelState.IsValid)
            {
                Threshold.DateHome = DateTime.Now;
                Threshold.Active = true;
                dbActiveContext.Threshold.Add(Threshold);
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            string idUser = User.Identity.GetUserId();
            int idCompany = userHelper.GetAssociatedCompanies(idUser);
            ViewBag.DeviceID = new SelectList(dbActiveContext.devices.Where(dis => dis.CompanyID == idCompany), "DeviceID", "Name");
            return View(Threshold);
        }

        // GET: Threshold/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Threshold Threshold = dbActiveContext.Threshold.Find(id);
            if (Threshold == null)
            {
                return HttpNotFound();
            }
            string idUser = User.Identity.GetUserId();
            int icompany = userHelper.GetAssociatedCompanies(idUser);
            ViewBag.DeviceID = new SelectList(dbActiveContext.devices.Where(dis => dis.CompanyID == icompany), "DeviceID", "Name", Threshold.DeviceID);
            return View(Threshold);
        }

        // POST: Threshold/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ThresholdID,Active,Temperature_min,Temperature_max,DeviceID,Tolerance_min,Tolerance_max")] Threshold Threshold)
        {
            if (ModelState.IsValid)
            {
                Threshold.DateHome = DateTime.Now;
                Threshold.Active = true;
                dbActiveContext.Entry(Threshold).State = EntityState.Modified;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            string idUser = User.Identity.GetUserId();
            int idCompany = userHelper.GetAssociatedCompanies(idUser);
            ViewBag.DeviceID = new SelectList(dbActiveContext.devices.Where(dis => dis.CompanyID == idCompany), "DeviceID", "Name", Threshold.DeviceID);
            return View(Threshold);
        }

        // GET: Threshold/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Threshold Threshold = dbActiveContext.Threshold.Find(id);
            if (Threshold == null)
            {
                return HttpNotFound();
            }
            return View(Threshold);
        }

        // POST: Threshold/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Threshold Threshold = dbActiveContext.Threshold.Find(id);
            dbActiveContext.Threshold.Remove(Threshold);
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