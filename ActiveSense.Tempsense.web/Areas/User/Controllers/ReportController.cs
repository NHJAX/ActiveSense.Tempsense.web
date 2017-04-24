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
            Day = 1440,
            Sixty_Min = 60,
            Thirty_Min = 30,
            Twenty_Min = 20,
            Ten_Min = 10,
            Five_Min = 5,
            Select_Time = 0
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
            var controller = RouteData.Values["controller"];
            var action = RouteData.Values["action"];
            var passid = RouteData.Values["id"];
            return View(ToFind(id, idDevice));
        }

        [HttpPost]
        public ActionResult GetAssociatedDevice(string idcompany)
        {

            int idCompany = Convert.ToInt32(idcompany);
            var lists = (dbActiveContext.devices.Where(x => x.CompanyID == idCompany)).ToList<devices>();

            List<DeviceViewModel> data = new List<DeviceViewModel>();
            foreach (devices dist in lists)
            {
                DeviceViewModel device = new DeviceViewModel();
                device.idDevice = dist.DeviceID;
                var TypeMeasure = dbActiveContext.TypeMeasure.Where(x => x.TypeMeasureID == dist.TypeMeasureID).FirstOrDefault().Name;
                device.TypeMeasure = TypeMeasure;
                device.NameDevice = dist.name + " ( " + device.TypeMeasure + " )";
                data.Add(device);
            }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListMeasures(int id = 1, int idDevice = 0)
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
            listEmp.Insert(0, (new SelectListItem { Text = "Select a Department", Value = "0" }));
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



        public JsonResult GetDataChart(int pageIndex, int idDevice, string dateStart, string dateEnd)
        {

            int start = Request["start"] != null ? Int16.Parse(Request["start"]) : 0;
            int lenght = Request["length"] != null ? Int16.Parse(Request["length"]) : 15;

            string Initialdate = Request["dateStart"] != null ? Request["dateStart"] : "";
            string dateFinish = Request["dateEnd"] != null ? Request["dateEnd"] : "";
            int filterMeasureTime = Request["FilterTime"] != null ? Int16.Parse(Request["FilterTime"]) : 0;
            int offset = Request["Offset"] != null ? Int16.Parse(Request["Offset"]) : 0;


            Measure Measure = new Measure();
            int pageCount = 0;
            List<Measure> Measures = null;

            if (filterMeasureTime <= 0)
            {
                Measures = Measure.List(start, lenght, out pageCount, idDevice, Initialdate, dateFinish);
            }
            else {
                Measures = Measure.ListAverages(start, lenght, out pageCount, idDevice, Initialdate, dateFinish, "", "", filterMeasureTime);
            }

            DateTime ConvertFromUtc = new DateTime();

            List<double> temperatureList = new List<double>();
            List<string> dates = new List<string>();

            List<double> ThresholdInferior = new List<double>();
            List<double> Thresholduperior = new List<double>();

            List<double> UpperToleranceList = new List<double>();
            List<double> LowerToleranceList = new List<double>();

            decimal TempatureMax = 0;
            decimal TempatureMin = 0;
            decimal toleranceMin = 0;
            decimal toleranceMax = 0;

            try
            {
                TempatureMax = dbActiveContext.Threshold.Where(p => p.DeviceID == idDevice).FirstOrDefault().Temperature_max;
                TempatureMin = dbActiveContext.Threshold.Where(p => p.DeviceID == idDevice).FirstOrDefault().Temperature_min;
                toleranceMin = dbActiveContext.Threshold.Where(p => p.DeviceID == idDevice).FirstOrDefault().Tolerance_min;
                toleranceMax = dbActiveContext.Threshold.Where(p => p.DeviceID == idDevice).FirstOrDefault().Tolerance_max;
            }
            catch (Exception ex) { }


            foreach (Measure MeasureTemp in Measures)
            {

                temperatureList.Add((double)MeasureTemp.Value);

                ConvertFromUtc = MeasureTemp.DateTime.AddMinutes(offset);
                dates.Add(ConvertFromUtc.ToString("ddd, dd MMMM HH: mm tt"));

                ThresholdInferior.Add((double)TempatureMin);
                Thresholduperior.Add((double)TempatureMax);
                UpperToleranceList.Add((double)toleranceMax);
                LowerToleranceList.Add((double)toleranceMin);
            }


            var result = new JsonResult();
            result.Data = new
            {
                dates = dates.ToArray(),
                temperatures = temperatureList.ToArray(),
                Thresholduperior = Thresholduperior.ToArray(),
                ThresholdInferior = ThresholdInferior.ToArray(),
                UpperToleranceList = UpperToleranceList.ToArray(),
                LowerToleranceList = LowerToleranceList.ToArray(),
            };
            return result;

        }

        [HttpPost]
        public JsonResult GetDataTable()
        {

            string search = Request["search[value]"];
            string draw = Request["draw"];

            int start = Request["start"] != null ? Int16.Parse(Request["start"]) : 0;
            int lenght = Request["length"] != null ? Int16.Parse(Request["length"]) : 15;

            int device = Request["idDevice"] != null ? Int16.Parse(Request["idDevice"]) : 0;

            string dateStart = Request["dateStart"] != null ? Request["dateStart"] : "";
            string dateEnd = Request["dateEnd"] != null ? Request["dateEnd"] : "";

            int filterMeasureTime = Request["FilterTime"] != null ? Int16.Parse(Request["FilterTime"]) : 0;

            int offset = Request["Offset"] != null ? Int16.Parse(Request["Offset"]) : 0;

            Measure Measure = new Measure();
            int pageCount = 0;

            string idUser = User.Identity.GetUserId();
            string perfil = userHelper.GetProfile(idUser);

            List<Measure> Measures = null;
            if (filterMeasureTime <= 0)
            {
                Measures = Measure.List(start, lenght, out pageCount,
                device, dateStart, dateEnd, idUser, perfil);
            }
            else {
                Measures = Measure.ListAverages(start, lenght, out pageCount, device,
                  dateStart, dateEnd, idUser, perfil, filterMeasureTime);
            }
            DateTime ConvertFromUtc = new DateTime();
            List<double> temperatureList = new List<double>();
            List<string> dates = new List<string>();

            List<ReportModel> MeasuresModelList = new List<ReportModel>();
            ReportModel Measuresmodel = null;

            Dictionary<int, string> NamesDic = new Dictionary<int, string>();

            foreach (Measure MeasureTemp in Measures)
            {

                Measuresmodel = new ReportModel();
                ConvertFromUtc = MeasureTemp.DateTime.AddMinutes(offset);
                Measuresmodel.date = ConvertFromUtc.ToString("ddd, dd MMMM HH:mm tt");
                //Measuresmodel.date = MeasureTemp.DateTime.ToString();
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

            var result = new JsonResult();
            result.Data = new { draw = draw, recordsTotal = pageCount, recordsFiltered = pageCount, data = MeasuresModelList.ToArray() };
            return result;
        }

    }
}