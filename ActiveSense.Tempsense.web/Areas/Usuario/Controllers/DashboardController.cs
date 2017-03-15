using ActiveSense.Tempsense.model.Modelo;
using ActiveSense.Tempsense.web.Controllers;
using ActiveSense.Tempsense.web.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActiveSense.Tempsense.web.Areas.Usuario.Controllers
{
    [ActiveSenseAutorize("Usuario")]
    public class DashboardController : GenericController
    {
        // GET: Usuario/Dashboard
        public ActionResult Index()
        {
            string idUsuario = User.Identity.GetUserId();
            //esta variable permite que se pase a un identificador de usuario a helperchart.
            ViewBag.UsK = idUsuario;
            return View();
        }

        public JsonResult ObtenerMedidasPasadas(int idDispositivo)
        {

            var fechaActual = DateTime.Now;
            var hor = fechaActual.Hour;
            var min = fechaActual.Minute;

            var fechaAyer = fechaActual.Date.AddDays(-1).AddHours(hor).AddMinutes(min);

            var fechaActualSt = String.Format("{0:yyyy-MM-dd HH:mm:ss}", fechaActual);
            var fechaAyerSt = String.Format("{0:yyyy-MM-dd HH:mm:ss}", fechaAyer);

            List<Medida> listaMedidas = new List<Medida>();
            List<string> horasList = new List<string>();
            List<decimal> temperaturaList = new List<decimal>();

            string chainConexion = ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString;
            SqlDataReader reader;
            using (SqlConnection sqlConnection = new SqlConnection(chainConexion))
            {
                using (SqlCommand cmdTotal = new SqlCommand())
                {
                    sqlConnection.Open();
                    cmdTotal.CommandType = CommandType.Text;
                    cmdTotal.Connection = sqlConnection;
                    cmdTotal.CommandText = "Select DATEPART(dd,FechaHora) as dia , DATEPART(hh,FechaHora) as hora, " +
                                          " AVG(Valor) as promedio FROM Medidas WHERE DispositivoID = " + idDispositivo +
                                          " AND FechaHora>= '" + fechaAyerSt + "' and FechaHora<= '" + fechaActualSt + "'"+
                                          " Group by DATEPART(hh,FechaHora), DATEPART(dd,FechaHora)  order by DATEPART(dd,FechaHora) ";

                    try
                    {
                        reader = cmdTotal.ExecuteReader();
                        while (reader.Read())
                        {
                            var hora = (int)reader["hora"];
                            string preHora = hora >= 12 ? " pm" : " am";
                            horasList.Add(hora.ToString() + preHora);
                            temperaturaList.Add((decimal)reader["promedio"] );
                        }
                    }
                    catch (Exception ex) { }
                }
            }


            List<double> umbralInferiorList = new List<double>();
            List<double> umbralSuperiorList = new List<double>();

            List<double> toleranciaSuperiorList = new List<double>();
            List<double> toleranciaInferiorList = new List<double>();

            decimal umbraMax = 0;
            decimal umbraMin = 0;
            decimal toleranciaMin = 0;
            decimal toleranciaMax = 0;

            try
            {
                umbraMax = dbActiveContext.Umbrals.Where(p => p.DispositivoID == idDispositivo).FirstOrDefault().Temperatura_max;
                umbraMin = dbActiveContext.Umbrals.Where(p => p.DispositivoID == idDispositivo).FirstOrDefault().Temperatura_min;
                toleranciaMin = dbActiveContext.Umbrals.Where(p => p.DispositivoID == idDispositivo).FirstOrDefault().Tolerancia_min;
                toleranciaMax = dbActiveContext.Umbrals.Where(p => p.DispositivoID == idDispositivo).FirstOrDefault().Tolerancia_max;
            }
            catch (Exception ex) { }

            foreach (string medidaTemp in horasList)
            {

                umbralInferiorList.Add((double)umbraMin);
                umbralSuperiorList.Add((double)umbraMax);
                toleranciaSuperiorList.Add((double)toleranciaMax);
                toleranciaInferiorList.Add((double)toleranciaMin);

            }

            var resultado = new JsonResult();
            resultado.Data = new
            {
                horasList = horasList.ToArray(),
                temperaturaList = temperaturaList.ToArray(),
                umbralSuperiorList = umbralInferiorList.ToArray(),
                umbralInferiorList = umbralSuperiorList.ToArray(),
                toleranciaSuperiorList = toleranciaSuperiorList.ToArray(),
                toleranciaInferiorList = toleranciaInferiorList.ToArray(),
            };
            return resultado;

        }

    }
}