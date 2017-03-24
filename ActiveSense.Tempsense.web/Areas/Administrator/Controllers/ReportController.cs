using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI.WebControls;
using ActiveSense.Tempsense.model.Model;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.web.Models;
using ActiveSense.Tempsense.web.Helpers;
using System.Configuration;
using System.Web.Script.Serialization;

namespace ActiveSense.Tempsense.web.Areas.Administrator.Controllers
{
    [ActiveSenseAutorize("Administrator")]
    public class ReportController : GenericController
    {
        // GET: Administrator/Report
        public const int CANTIDAD_Devices = 10;
        enum enumFilterTime
        {
            Day = 1440,
            Sixty_min = 60,
            Thirty_min = 30,
            Twenty_min = 20,
            Ten_min = 10,
            Five_min = 5,
            Select_time = 0
        };

        // [MeterAuthorize]
        //GET:/Measure/
        public ActionResult Index(int id = 1, int iddevice = 0)
        {

            return View(ToFind(id, iddevice));
        }


        public ActionResult ListMeasures(int id = 1, int iddevice = 0)
        {

            return PartialView(ToFind(id, iddevice));
        }

        [HttpPost]
        public ActionResult GetDeviceAssociated(string idcompany)
        {

            int idCompany = Convert.ToInt32(idcompany);
            var lists = (dbActiveContext.devices.Where(x => x.CompanyID == idCompany)).ToList<devices>();

            List<DeviceViewModel> data = new List<DeviceViewModel>();
            foreach (devices dist in lists)
            {
                var TypeMeasure = String.Empty;
                DeviceViewModel device = new DeviceViewModel();
                device.idDevice = dist.DeviceID;
                TypeMeasure = dbActiveContext.TypeMeasure.Where(x => x.TypeMeasureID == dist.TypeMeasureID).FirstOrDefault().Name;
                device.TypeMeasure = TypeMeasure;
                device.NameDevice = dist.name + " ( "+ device.TypeMeasure +" )";
                data.Add(device);
            }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public List<Measure> ToFind(int pageIndex, int iddevice)
        {
            Measure Measure = new Measure();

            var listEmp = new SelectList(dbActiveContext.companies, "CompanyID", "Name").ToList();
            listEmp.Insert(0, (new SelectListItem { Text = "Select a company", Value = "0" }));
            ViewBag.Companies = listEmp;

            var lists = (dbActiveContext.devices).ToList<devices>();
            List<DeviceViewModel2> data = new List<DeviceViewModel2>();
            foreach (devices disp in lists)
            {
                var TypeMeasure = String.Empty;
                DeviceViewModel2 device = new DeviceViewModel2();
                device.idDevice = disp.DeviceID;
                TypeMeasure = dbActiveContext.TypeMeasure.Where(x => x.TypeMeasureID == disp.TypeMeasureID).FirstOrDefault().Name;
                device.TypeMeasure = TypeMeasure;
                device.NameDevice = disp.name + " ( " + device.TypeMeasure + " )";
                data.Add(device);
            }
            var list = new SelectList(data, "iddevice", "Namedevice").ToList();
            list.Insert(0, (new SelectListItem { Text = "Select the device", Value = "0" }));
            //Add device list
            ViewBag.DeviceID = list;

            //Add time filter
            ViewBag.FilterTime = new SelectList(Enum.GetValues(typeof(enumFilterTime)).Cast<enumFilterTime>().Select(v => new SelectListItem
            {
                Value = ((int)v).ToString(),
                Text = v.ToString()
            }).ToList(), "Value", "Text" );
            
            return null;
        }


        public JsonResult ObtainDataChart(int pageIndex, int iddevice, string DateHome, string EndDate)
        {

            int start = Request["start"] != null ? Int16.Parse(Request["start"]) : 0;
            int lenght = Request["length"] != null ? Int16.Parse(Request["length"]) : 10;
            string dateInicial = Request["DateHome"] != null ? Request["DateHome"] : "";
            string dateEndal = Request["EndDate"] != null ? Request["EndDate"] : "";
            int MeasureTimeFilter = Request["FilterTime"] != null ? Int16.Parse(Request["FilterTime"]) : 0;

            Measure Measure = new Measure();
            int pageCount = 0;
            List<Measure> MeasuresList = null;

            if ( MeasureTimeFilter <= 0)
            {
                MeasuresList = Measure.List(start, lenght, out pageCount, iddevice, DateHome, EndDate, "", "", MeasureTimeFilter);
            }
            else {
                MeasuresList = Measure.ListAverages(start, lenght, out pageCount, iddevice, DateHome, EndDate, "", "", MeasureTimeFilter);
            }
            

            List<double> temperatureList = new List<double>();
            List<string> dates = new List<string>();

            List<double> thresholdInferior = new List<double>();
            List<double> thresholdSuperior = new List<double>();

            List<double> UpperToleranceList = new List<double>();
            List<double> LowerToleranceList = new List<double>();

            decimal thresholdMax = 0;
            decimal thresholdMin = 0;
            decimal toleranceMin = 0;
            decimal toleranceMax = 0;

            try
            {
                thresholdMax = dbActiveContext.Threshold.Where(p => p.DeviceID == iddevice).FirstOrDefault().Temperature_max;
                thresholdMin = dbActiveContext.Threshold.Where(p => p.DeviceID == iddevice).FirstOrDefault().Temperature_min;
                 toleranceMin = dbActiveContext.Threshold.Where(p => p.DeviceID == iddevice).FirstOrDefault().Tolerance_min;
                 toleranceMax = dbActiveContext.Threshold.Where(p => p.DeviceID == iddevice).FirstOrDefault().Tolerance_max;
            }
            catch (Exception ex){ }

            foreach (Measure MeasureTemp in MeasuresList)
            {
                temperatureList.Add((double)MeasureTemp.Value);
                dates.Add(MeasureTemp.DateTime.ToString());
                thresholdInferior.Add((double)thresholdMin);
                thresholdSuperior.Add((double)thresholdMax);
                UpperToleranceList.Add((double)toleranceMax);
                LowerToleranceList.Add((double)toleranceMin);
            }

            var resultado = new JsonResult();
            resultado.Data = new
            {
                dates = dates.ToArray(),
                temperatures = temperatureList.ToArray(),
                thresholdSuperior = thresholdSuperior.ToArray(),
                thresholdInferior = thresholdInferior.ToArray(),
                UpperToleranceList = UpperToleranceList.ToArray(),
                LowerToleranceList = LowerToleranceList.ToArray(),
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

            int device = Request["iddevice"] != null ? Int16.Parse(Request["iddevice"]) : 0;

            string Startdate = Request["startdate"] != null ? Request["startdate"] : "";
            string EndDate = Request["endDate"] != null ? Request["endDate"] : "";

            int MeasureTimeFilter = Request["FilterTime"] != null ? Int16.Parse(Request["FilterTime"]) : 0;

            Measure Measure = new Measure();
            int pageCount = 0;

            List<Measure> MeasuresList = null;

            if (MeasureTimeFilter <= 0)
            {
                MeasuresList = Measure.List(start, lenght, out pageCount, device, Startdate, EndDate, "", "", MeasureTimeFilter);
            }
            else
            {
                MeasuresList = Measure.ListAverages(start, lenght, out pageCount, device, Startdate, EndDate, "", "", MeasureTimeFilter);
            }


            List<double> temperatureList = new List<double>();
            List<string> dates = new List<string>();

            List<ReportModel> MeasuresModelList = new List<ReportModel>();
            ReportModel Measuresmodel = null;

            Dictionary<int, string> NamesDic = new Dictionary<int, string>();

            foreach (Measure MeasureTemp in MeasuresList)
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
