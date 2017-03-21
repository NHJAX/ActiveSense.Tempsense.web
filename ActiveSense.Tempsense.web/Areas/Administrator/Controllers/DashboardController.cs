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
            //esta variable permite que se pase a un identificador de usuario a helperchart.
            ViewBag.UsK = idUser;

            return View();
        }

        public JsonResult GetPastMeasures(int idDevice) {


            var dateActual = DateTime.Now;
            var hor = dateActual.Hour;
            var min = dateActual.Minute;

            var dateAyer = dateActual.Date.AddDays(-1).AddHours(hor).AddMinutes(min);

            var dateActualSt =  String.Format("{0:yyyy-MM-dd HH:mm:ss}", dateActual);
            var dateAyerSt =  String.Format("{0:yyyy-MM-dd HH:mm:ss}", dateAyer);

            List<Measure> listaMeasures = new List<Measure>();
            List<string> HoursList = new List<string>();
            List<decimal> temperatureList = new List<decimal>();
            
            string chainConexion = ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString;
            SqlDataReader reader;
            using (SqlConnection sqlConnection = new SqlConnection(chainConexion))
            {
                using (SqlCommand cmdTotal = new SqlCommand())
                {
                    sqlConnection.Open();
                    cmdTotal.CommandType = CommandType.Text;
                    cmdTotal.Connection = sqlConnection;
                    cmdTotal.CommandText = "Select DATEPART(dd,DateTime) as dia , DATEPART(hh,DateTime) as hora, "+
                                          " AVG(Value) as promedio FROM Measures WHERE DeviceID = " + idDevice +
                                          " AND DateTime>= '" + dateAyerSt + "' and DateTime<= '" + dateActualSt + "'" +
                                          " Group by DATEPART(hh,DateTime), DATEPART(dd,DateTime)  order by DATEPART(dd,DateTime) ";
                  
                    try
                    {
                        reader = cmdTotal.ExecuteReader();
                        while (reader.Read())
                        {
                            var hora = (int)reader["hora"];
                            string preHora = hora >= 12 ? " pm" : " am";
                            HoursList.Add(hora.ToString() + preHora);
                            temperatureList.Add((decimal)reader["promedio"]);
                        }
                    }
                    catch (Exception ex) { }
                }
            }
       
           
            List<double> ThresholdInferiorList = new List<double>();
            List<double> ThresholduperiorList = new List<double>();

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

            foreach (string MeasureTemp in HoursList)
            {

                ThresholdInferiorList.Add((double)umbraMin);
                ThresholduperiorList.Add((double)umbraMax);
                ToleranceSuperiorList.Add((double)toleranceMax);
                ToleranceInferiorList.Add((double)toleranceMin);

            }

            var resultado = new JsonResult();
            resultado.Data = new
            {
                HoursList = HoursList.ToArray(),
                temperatureList = temperatureList.ToArray(),
                ThresholduperiorList = ThresholdInferiorList.ToArray(),
                ThresholdInferiorList = ThresholduperiorList.ToArray(),
                ToleranceSuperiorList = ToleranceSuperiorList.ToArray(),
                ToleranceInferiorList = ToleranceInferiorList.ToArray(),
            };
            return resultado;

        }
    }
}