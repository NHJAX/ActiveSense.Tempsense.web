using ActiveSense.Tempsense.web.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.model.Modelo;

namespace ActiveSense.Tempsense.web.Areas.Administrador.Controllers
{
    [ActiveSenseAutorize("Administrador")]
    public class CompanyController : GenericController
    {
       
        // GET: Empresa
        public ActionResult Index()
        {
            //return View();
            return View(dbActiveContext.Empresas.ToList());
        }

        // GET: Empresa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresas = dbActiveContext.Empresas.Find(id);
            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
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
        public ActionResult Create([Bind(Include = "EmpresaID,Nombre,Codigo,Correo,AbrEmpresa,Activo,Notificar_Correo,Notificar_MSM")] Empresa empresas)
        {
            if (ModelState.IsValid)
            {
                dbActiveContext.Empresas.Add(empresas);
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(empresas);
        }

        // GET: Empresa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresas = dbActiveContext.Empresas.Find(id);
            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
        }

        // POST: Empresa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmpresaID,Nombre,Codigo,Correo,AbrEmpresa,Activo,Notificar_Correo,Notificar_MSM")] model.Modelo.Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                dbActiveContext.Entry(empresa).State = EntityState.Modified;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empresa);
        }

        // GET: Empresa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresas = dbActiveContext.Empresas.Find(id);
            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
        }

        // POST: Empresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empresa empresas = dbActiveContext.Empresas.Find(id);
            dbActiveContext.Empresas.Remove(empresas);
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
