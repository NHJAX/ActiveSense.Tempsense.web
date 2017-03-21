using ActiveSense.Tempsense.web.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.model.Model;

namespace ActiveSense.Tempsense.web.Areas.Administrator.Controllers
{
    [ActiveSenseAutorize("Administrator")]
    public class CompanyController : GenericController
    {
       
        // GET: Empresa
        public ActionResult Index()
        {
            //return View();
            return View(dbActiveContext.companies.ToList());
        }

        // GET: Empresa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            company company = dbActiveContext.companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Empresa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Empresa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompanyID,Name,Code,Mail,AbrCompany,Active,Notificar_Mail,Notificar_MSM")] company company)
        {
            if (ModelState.IsValid)
            {
                dbActiveContext.companies.Add(company);
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Empresa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            company company = dbActiveContext.companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyID,Name,Code,Mail,Aprcompany,Active,Notify_mail,Notify_MSM")] model.Model.company company)
        {
            if (ModelState.IsValid)
            {
                dbActiveContext.Entry(company).State = EntityState.Modified;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Empresa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            company company = dbActiveContext.companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Empresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            company company = dbActiveContext.companies.Find(id);
            dbActiveContext.companies.Remove(company);
            dbActiveContext.SaveChanges();
            return RedirectToAction("index");
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
