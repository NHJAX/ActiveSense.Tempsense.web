using ActiveSense.Tempsense.web.Controllers;
using Microsoft.Azure.Devices;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Common.Exceptions;
using ActiveSense.Tempsense.model.Modelo;
using ActiveSense.Tempsense.web.Helpers;
using System;

namespace ActiveSense.Tempsense.web.Areas.Administrador.Controllers
{
    [ActiveSenseAutorize("Administrador")]
    public class DeviceController : GenericController
    {

        enum enumMedidas
        {
            Temperatura = 1,
            Humedad = 2
        };

        static RegistryManager registryManager;
        static string connectionString = "HostName=tempSense.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=muBcEp+7bjBM2SwwJ+0YD7PuxXkUUQoU3aC/EzmrrNU=";

        public DeviceController()
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        // GET: Administrador/Device
        public ActionResult Index()
        {
            var dispositivos = dbActiveContext.Dispositivos.Include(d => d.Empresa);
            return View(dispositivos.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispositivos dispositivo = dbActiveContext.Dispositivos.Find(id);
            if (dispositivo == null)
            {
                return HttpNotFound();
            }
            return View(dispositivo);
        }

        public ActionResult Create()
        {
            ViewBag.EmpresaID = new SelectList(dbActiveContext.Empresas, "EmpresaID", "Nombre");
            ViewBag.TipoMedidaID = new SelectList(dbActiveContext.TipoMedidas, "TipoMedidaID", "Nombre");
            ViewBag.Medida = new SelectList(Enum.GetValues(typeof(enumMedidas)).Cast<enumMedidas>().Select(v => new SelectListItem
            {
                Value = ((int)v).ToString(),
                Text = v.ToString()
            }).ToList(), "Value", "Text"

           );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DispositivoID,Nombre,ClaveDispositivo,Activo,EmpresaID,TipoMedidaID")] Dispositivos dispositivo)
        {
            if (ModelState.IsValid)
            {
                //string deviceId = "minwinpc";
                Device device;
                try
                {
                    device = await registryManager.AddDeviceAsync(new Device(dispositivo.Nombre));

                }
                catch (DeviceAlreadyExistsException)
                {
                    device = await registryManager.GetDeviceAsync(dispositivo.Nombre);
                }

                //dispositivo.ClaveDispositivo = device.Authentication.SymmetricKey.PrimaryKey;
                dbActiveContext.Dispositivos.Add(dispositivo);
                dispositivo.Activo =true;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TipoMedidaID = new SelectList(dbActiveContext.TipoMedidas, "TipoMedidaID", "Nombre", dispositivo.TipoMedidaID);
            //ViewBag.Medida = new SelectList(Enum.GetValues(typeof(enumMedidas)).Cast<enumMedidas>().Select(v => new SelectListItem
            //{
            //    Value = ((int)v).ToString(),
            //    Text = v.ToString()
            //}).ToList(), "Value", "Text"
            //);

            ViewBag.EmpresaID = new SelectList(dbActiveContext.Empresas, "EmpresaID", "Nombre", dispositivo.EmpresaID);
            return View(dispositivo);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispositivos dispositivos = dbActiveContext.Dispositivos.Find(id);
            if (dispositivos == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpresaID = new SelectList(dbActiveContext.Empresas, "EmpresaID", "Nombre", dispositivos.EmpresaID);

            //ViewBag.Medida = new SelectList(Enum.GetValues(typeof(enumMedidas)).Cast<enumMedidas>().Select(v => new SelectListItem
            //{
            //    Value = ((int)v).ToString(),
            //    Text = v.ToString()
            //}).ToList(), "Value", "Text", dispositivos.Medida

          //);
            return View(dispositivos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DispositivoID,Nombre,ClaveDispositivo,Activo,EmpresaID,Medida")] Dispositivos dispositivos)
        {
            if (ModelState.IsValid)
            {
                dbActiveContext.Entry(dispositivos).State = EntityState.Modified;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpresaID = new SelectList(dbActiveContext.Empresas, "EmpresaID", "Nombre", dispositivos.EmpresaID);

            //ViewBag.Medida = new SelectList(Enum.GetValues(typeof(enumMedidas)).Cast<enumMedidas>().Select(v => new SelectListItem
            //{
            //    Value = ((int)v).ToString(),
            //    Text = v.ToString()
            //}).ToList(), "Value", "Text", dispositivos.Medida
            //);
            return View(dispositivos);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispositivos dispositivos = dbActiveContext.Dispositivos.Find(id);
            if (dispositivos == null)
            {
                return HttpNotFound();
            }
            return View(dispositivos);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dispositivos dispositivos = dbActiveContext.Dispositivos.Find(id);
            dbActiveContext.Dispositivos.Remove(dispositivos);
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