using ActiveSense.Tempsense.model.Model;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.web.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActiveSense.Tempsense.web.Areas.Administrator.Controllers
{
    [ActiveSenseAutorize("Administrator")]
    public class DashboardController : GenericController
    {
        // GET: Administrator/Dashboard
        public ActionResult Index()
        {
            string idUser = User.Identity.GetUserId();
            //This variable allows that it passes to a userid to helperchart.
            ViewBag.UsK = idUser;

            return View();
        }

        public JsonResult GetPastMeasures(int idDevice) {


            var dateActual = DateTime.Now;
            var hor = dateActual.Hour;
            var min = dateActual.Minute;

            var dateYesterday = dateActual.Date.AddDays(-1).AddHours(hor).AddMinutes(min);

            var dateActualSt =  String.Format("{0:yyyy-MM-dd HH:mm:ss}", dateActual);
            var dateYesterdaySt =  String.Format("{0:yyyy-MM-dd HH:mm:ss}", dateYesterday);

            List<Measure> ListMeasures = new List<Measure>();
            List<string> HoursList = new List<string>();
            List<decimal> temperatureList = new List<decimal>();
            
            string ChainConnection = ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString;
            SqlDataReader reader;
            using (SqlConnection sqlConnection = new SqlConnection(ChainConnection))
            {
                using (SqlCommand cmdTotal = new SqlCommand())
                {
                    sqlConnection.Open();
                    cmdTotal.CommandType = CommandType.Text;
                    cmdTotal.Connection = sqlConnection;
                    cmdTotal.CommandText = "Select DATEPART(dd,DateTime) as day , DATEPART(hh,DateTime) as time, "+
                                          " AVG(Value) as average FROM Measures WHERE DeviceID = " + idDevice +
                                          " AND DateTime>= '" + dateYesterdaySt + "' and DateTime<= '" + dateActualSt + "'" +
                                          " Group by DATEPART(hh,DateTime), DATEPART(dd,DateTime)  order by DATEPART(dd,DateTime) ";
                  
                    try
                    {
                        reader = cmdTotal.ExecuteReader();
                        while (reader.Read())
                        {
                            var time = (int)reader["time"];
                            string preTime = time >= 12 ? " pm" : " am";
                            HoursList.Add(time.ToString() + preTime);
                            temperatureList.Add((decimal)reader["average"]);
                        }
                    }
                    catch (Exception ex) { }
                }
            }
       
           
            List<double> ThresholdInferiorList = new List<double>();
            List<double> ThresholduperiorList = new List<double>();

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

            foreach (string MeasureTemp in HoursList)
            {

                ThresholdInferiorList.Add((double)TempatureMin);
                ThresholduperiorList.Add((double)TempatureMax);
                UpperToleranceList.Add((double)toleranceMax);
                LowerToleranceList.Add((double)toleranceMin);

            }

            var result = new JsonResult();
            result.Data = new
            {
                HoursList = HoursList.ToArray(),
                temperatureList = temperatureList.ToArray(),
                ThresholduperiorList = ThresholdInferiorList.ToArray(),
                ThresholdInferiorList = ThresholduperiorList.ToArray(),
                UpperToleranceList = UpperToleranceList.ToArray(),
                LowerToleranceList = LowerToleranceList.ToArray(),
            };
            return result;

        }
    }
}