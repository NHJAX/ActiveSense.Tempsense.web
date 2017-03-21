using ActiveSense.Tempsense.model.Model;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.web.Helpers;
using ActiveSense.Tempsense.web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ActiveSense.Tempsense.web.Areas.User.Controllers
{
    [ActiveSenseAutorize("User")]
    public class ReportController : GenericController
    {

        enum enumFilterTime
        {
            Dia = 1440,
            Sesenta_min = 60,
            Treinta_min = 30,
            Veinte_min = 20,
            Diez_min = 10,
            Cinco_min = 5,
            Seleccione_Tiempo = 0
        };

        // GET: Administrator/Report
        public const int QUANTITY_devices = 10;
        private UserHelper userHelper = null;
        public ReportController()
        {
            userHelper = new UserHelper();
        }

        // [MeterAuthorize]
        //GET:/Measure/
        public ActionResult Index(int id = 1, int idDevice = 0)
        {
            return View(ToFind(id, idDevice));
        }

        [HttpPost]
        public ActionResult GetDeviceAssociated(string idCompany)
        {

            int idCompanyT = Convert.ToInt32(idCompany);
            var lists = (dbActiveContext.devices.Where(x => x.CompanyID == idCompanyT)).ToList<devices>();

            List<DeviceViewModel> data = new List<DeviceViewModel>();
            foreach (devices dist in lists)
            {
                DeviceViewModel device = new DeviceViewModel();
                device.idDevice = dist.DeviceID;
                var typeMeasure = dbActiveContext.Typemeasure.Where(x => x.TypeMeasureID == dist.TypeMeasureID).FirstOrDefault().Name;
                device.typeMeasure = typeMeasure;
                device.NameDevice = dist.name + " ( " + device.typeMeasure + " )";
                data.Add(device);
            }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListaMeasures(int id = 1, int idDevice = 0)
        {

            return PartialView(ToFind(id, idDevice));
        }

        [HttpPost]
        public List<Measure> ToFind(int pageIndex, int idDevice)
        {

            Measure Measure = new Measure();

            string idUser = User.Identity.GetUserId();
        

            int idCompany = userHelper.GetAssociatedCompanies(idUser);
            var listEmp = new SelectList(dbActiveContext.companies.Where(disp => disp.CompanyID == idCompany), "CompanyID", "Name").ToList();
            listEmp.Insert(0, (new SelectListItem { Text = "Select a company", Value = "0" }));
            ViewBag.Companies = listEmp;


            var lists = (dbActiveContext.devices).ToList<devices>();
            List<DeviceViewModel2> data = new List<DeviceViewModel2>();
            foreach (devices disp in lists)
            {
                var typeMeasure = String.Empty;
                DeviceViewModel2 device = new DeviceViewModel2();
                device.idDevice = disp.DeviceID;
                typeMeasure = dbActiveContext.Typemeasure.Where(x => x.TypeMeasureID == disp.TypeMeasureID).FirstOrDefault().Name;
                device.typeMeasure = typeMeasure;
                device.NameDevice = disp.name + " ( " + device.typeMeasure + " )";
                data.Add(device);
            }
            var list = new SelectList(data, "idDevice", "NameDevice").ToList();
            list.Insert(0, (new SelectListItem { Text = "Select the device", Value = "0" }));
            //Add list of device
            ViewBag.DeviceID = list;

            //var listDisp = new SelectList(dbActiveContext.devices.Where(disp => disp.CompanyID == idCompany), "DeviceID", "Name").ToList();
            //listDisp.Insert(0, (new SelectListItem { Text = "Seleccione el device", Value = "0" }));
            //ViewBag.DeviceID = listDisp;

            //add filter of time
            ViewBag.FilterTime = new SelectList(Enum.GetValues(typeof(enumFilterTime)).Cast<enumFilterTime>().Select(v => new SelectListItem
            {
                Value = ((int)v).ToString(),
                Text = v.ToString()
            }).ToList(), "Value", "Text");

            return null;
        }



        public JsonResult ObtainDataGraphic(int pageIndex, int idDevice, string dateInicio, string dateFin)
        {

            int start = Request["start"] != null ? Int16.Parse(Request["start"]) : 0;
            int lenght = Request["length"] != null ? Int16.Parse(Request["length"]) : 10;

            string dateInicial = Request["dateInicio"] != null ? Request["dateInicio"] : "";
            string dateFi = Request["dateFin"] != null ? Request["dateFin"] : "";
            int filtroMeasureTiempo = Request["FilterTime"] != null ? Int16.Parse(Request["FilterTime"]) : 0;


            Measure Measure = new Measure();
            int pageCount = 0;
            List<Measure> Measures = null;

            if (filtroMeasureTiempo <= 0)
            {
                Measures = Measure.List(start, lenght, out pageCount, idDevice, dateInicial, dateFi);
            }
            else {
                Measures = Measure.ListAverages(start, lenght, out pageCount, idDevice, dateInicial, dateFi, "", "", filtroMeasureTiempo);
            }


            List<double> temperatureList = new List<double>();
            List<string> dates = new List<string>();

            List<double> ThresholdInferior = new List<double>();
            List<double> Thresholduperior = new List<double>();

            List<double> ToleranceSuperiorList = new List<double>();
            List<double> ToleranceInferiorList = new List<double>();

            decimal umbraMax = 0;
            decimal umbraMin = 0;
            decimal toleranceMin = 0;
            decimal toleranceMax = 0;

            try
            {
                umbraMax = dbActiveContext.Threshold.Where(p => p.DeviceID == idDevice).FirstOrDefault().Temperature_max;
                umbraMin = dbActiveContext.Threshold.Where(p => p.DeviceID == idDevice).FirstOrDefault().Temperature_min;
                toleranceMin = dbActiveContext.Threshold.Where(p => p.DeviceID == idDevice).FirstOrDefault().Tolerance_min;
                toleranceMax = dbActiveContext.Threshold.Where(p => p.DeviceID == idDevice).FirstOrDefault().Tolerance_max;
            }
            catch (Exception ex) { }


            foreach (Measure MeasureTemp in Measures)
            {

                temperatureList.Add((double)MeasureTemp.Value);
                dates.Add(MeasureTemp.DateTime.ToString());
                ThresholdInferior.Add((double)umbraMin);
                Thresholduperior.Add((double)umbraMax);
                ToleranceSuperiorList.Add((double)toleranceMax);
                ToleranceInferiorList.Add((double)toleranceMin);
            }


            var resultado = new JsonResult();
            resultado.Data = new
            {
                dates = dates.ToArray(),
                temperatures = temperatureList.ToArray(),
                Thresholduperior = Thresholduperior.ToArray(),
                ThresholdInferior = ThresholdInferior.ToArray(),
                ToleranceSuperiorList = ToleranceSuperiorList.ToArray(),
                ToleranceInferiorList = ToleranceInferiorList.ToArray(),
            };
            return resultado;

        }

        [HttpPost]
        public JsonResult ObtenerDatosTabla()
        {

            string search = Request["search[value]"];
            string draw = Request["draw"];

            int start = Request["start"] != null ? Int16.Parse(Request["start"]) : 0;
            int lenght = Request["length"] != null ? Int16.Parse(Request["length"]) : 10;

            int device = Request["idDevice"] != null ? Int16.Parse(Request["idDevice"]) : 0;

            string dateInicial = Request["dateInicio"] != null ? Request["dateInicio"] : "";
            string dateFinal = Request["dateFin"] != null ? Request["dateFin"] : "";

            int filtroMeasureTiempo = Request["FilterTime"] != null ? Int16.Parse(Request["FilterTime"]) : 0;

        
            Measure Measure = new Measure();
            int pageCount = 0;

            string idUser = User.Identity.GetUserId();
            string perfil = userHelper.GetProfile(idUser);

            List<Measure> Measures = null;
            if (filtroMeasureTiempo <= 0)
            {
                Measures = Measure.List(start, lenght, out pageCount,
                device, dateInicial, dateFinal, idUser, perfil);
            }
            else {
                Measures = Measure.ListAverages(start, lenght, out pageCount, device,
                  dateInicial, dateFinal, idUser, perfil, filtroMeasureTiempo);
            }

            List<double> temperatureList = new List<double>();
            List<string> dates = new List<string>();

            List<ReportModel> MeasuresModelList = new List<ReportModel>();
            ReportModel Measuresmodel = null;

            Dictionary<int, string> NamesDic = new Dictionary<int, string>();

            foreach (Measure MeasureTemp in Measures)
            {
                Measuresmodel = new ReportModel();
                Measuresmodel.date = MeasureTemp.DateTime.ToString();
                Measuresmodel.idDevice = MeasureTemp.DeviceID;
                Measuresmodel.temperature = MeasureTemp.Value.ToString();

                if (!NamesDic.ContainsKey(Measuresmodel.idDevice))
                {
                    var Name = dbActiveContext.devices.Where(dis => dis.DeviceID == Measuresmodel.idDevice).FirstOrDefault().name;
                    NamesDic.Add(Measuresmodel.idDevice, Name);
                }
                Measuresmodel.NameDevice = NamesDic[Measuresmodel.idDevice];

                MeasuresModelList.Add(Measuresmodel);
            }

            var resultado = new JsonResult();
            resultado.Data = new { draw = draw, recordsTotal = pageCount, recordsFiltered = pageCount, data = MeasuresModelList.ToArray() };
            return resultado;
        }

    }
}