using ActiveSense.Tempsense.web.Controllers;
using Microsoft.Azure.Devices;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Common.Exceptions;
using ActiveSense.Tempsense.model.Model;
using ActiveSense.Tempsense.web.Helpers;
using System;

namespace ActiveSense.Tempsense.web.Areas.Administrator.Controllers
{
    [ActiveSenseAutorize("Administrator")]
    public class DeviceController : GenericController
    {

        enum enumMeasures
        {
            Temperature = 1,
            Humitity = 2
        };

        static RegistryManager registryManager;
        static string connectionString = "HostName=tempSense.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=muBcEp+7bjBM2SwwJ+0YD7PuxXkUUQoU3aC/EzmrrNU=";

        public DeviceController()
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        // GET: Administrator/Device
        public ActionResult Index()
        {
            var Device = dbActiveContext.devices.Include(d => d.company);
            return View(Device.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            devices device = dbActiveContext.devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        public ActionResult Create()
        {
            ViewBag.CompanyID = new SelectList(dbActiveContext.companies, "CompanyID", "Name");
            ViewBag.TypeMeasureID = new SelectList(dbActiveContext.TypeMeasure, "TypeMeasuresID", "NAME");
            ViewBag.Measure = new SelectList(Enum.GetValues(typeof(enumMeasures)).Cast<enumMeasures>().Select(v => new SelectListItem
            {
                Value = ((int)v).ToString(),
                Text = v.ToString()
            }).ToList(), "Value", "Text"

           );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DeviceID,Name,KeyDevice,Active,CompanyID,TypeMeasuresID")] devices device)
        {
            if (ModelState.IsValid)
            {
                //string deviceId = "NHJAX-PI3-01";
                Device AzureDevice;
                try
                {
                    AzureDevice = await registryManager.AddDeviceAsync(new Device(device.name));

                }
                catch (DeviceAlreadyExistsException)
                {
                    AzureDevice = await registryManager.GetDeviceAsync(device.name);
                }

                dbActiveContext.devices.Add(device);
                device.Active =true;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeMeasureID = new SelectList(dbActiveContext.TypeMeasure, "TypeMeasuresID", "Name", device.TypeMeasureID);
            //ViewBag.Measure = new SelectList(Enum.GetValues(typeof(enumMeasures)).Cast<enumMeasures>().Select(v => new SelectListItem
            //{
            //    Value = ((int)v).ToString(),
            //    Text = v.ToString()
            //}).ToList(), "Value", "Text"
            //);

            ViewBag.CompanyID = new SelectList(dbActiveContext.companies, "companyID", "Name", device.CompanyID);
            return View(device);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            devices device = dbActiveContext.devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(dbActiveContext.companies, "CompanyID", "Name", device.CompanyID);

            //ViewBag.Measure = new SelectList(Enum.GetValues(typeof(enumMeasures)).Cast<enumMeasures>().Select(v => new SelectListItem
            //{
            //    Value = ((int)v).ToString(),
            //    Text = v.ToString()
            //}).ToList(), "Value", "Text", Device.Measure

          //);
            return View(device);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeviceID,Name,KeyDevice,Active,CompanyID,Measure")] devices device)
        {
            if (ModelState.IsValid)
            {
                dbActiveContext.Entry(device).State = EntityState.Modified;
                dbActiveContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = new SelectList(dbActiveContext.companies, "CompanyID", "Name", device.CompanyID);

            //ViewBag.Measure = new SelectList(Enum.GetValues(typeof(enumMeasures)).Cast<enumMeasures>().Select(v => new SelectListItem
            //{
            //    Value = ((int)v).ToString(),
            //    Text = v.ToString()
            //}).ToList(), "Value", "Text", Device.Measure
            //);
            return View(device);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            devices device = dbActiveContext.devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            devices device = dbActiveContext.devices.Find(id);
            dbActiveContext.devices.Remove(device);
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