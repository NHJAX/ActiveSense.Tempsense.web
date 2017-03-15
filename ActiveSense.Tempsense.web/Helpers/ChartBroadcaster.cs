using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using static ActiveSense.Tempsense.web.Helpers.Chart_Broadcaster;
using ActiveSense.Tempsense.web.Hubs;
using ActiveSense.Tempsense.model.Modelo;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;
using System.Configuration;
using Microsoft.AspNet.Identity;
using ActiveSense.Tempsense.web.Controllers;

namespace ActiveSense.Tempsense.web.Helpers
{
    public class Chart_Broadcaster
    {

        public class LineChart
        {


            [JsonProperty("lineChartData")]
            private int[] lineChartData;
            [JsonProperty("colorString")]
            private string colorString;

            [JsonProperty("hora")]
            private string[] hora = new string[60];

            public void SetLineChartData()
            {
                //Suppose we have a list of 60 items.

                using (ActiveSenseContext dbActiveContext = new ActiveSenseContext(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString))
                {

                    var lista = (from p in dbActiveContext.Medidas
                                 orderby p.FechaHora descending
                                 select p
                                 ).ToList();

                    lineChartData = dbActiveContext.Medidas.Select(p => p.Valor).Cast<int>().ToArray();
                    hora = dbActiveContext.Medidas.Select(p => p.FechaHora).Cast<string>().ToArray();

                }

            }
        }

        public class TemperatureUpdate
        {
            [JsonProperty("DashboardTemperatureResult")]
            private List<DashboardTemperatureResult> temp;
      
            private const string PERFIL_ADMINISTRADOR = "Administrador";
            private UserHelper userHelper = null;

            public TemperatureUpdate() {
                userHelper = new UserHelper();
            }

            public void TakeLastTemp(string idUsuario)
            {

                List<DashboardTemperatureResult> lis = new List<DashboardTemperatureResult>();

                try
                {
                    using (ActiveSenseContext context = new ActiveSenseContext(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString))
                    {
                        
                        // metodo que permite validar si un usuario es de perfil administrador  o no y buscar los datos segun esto.
                        string perfil = userHelper.obtenerPerfil(idUsuario);

                        int IdEmpresa = userHelper.obtenerEmpresasAsociadas(idUsuario, context);
                        var result = context.Empresas.Where(u => u.EmpresaID == IdEmpresa).Include("Dispositivos.Medidas").ToList();
                        if (PERFIL_ADMINISTRADOR == perfil)
                        {
                             result = context.Empresas.Include("Dispositivos.Medidas").ToList();
                        }

                        foreach (var item in result.SelectMany(x => x.Dispositivos))
                        {
                            if (item.Medidas.OrderBy(y => y.FechaHora).Any())
                            {
                                decimal MaxTemp = 0;
                                decimal MinTemp = 0;
                                decimal MaxTol  = 0;
                                decimal MinTol  = 0;
                                try
                                {
                                    MaxTemp = context.Umbrals.ToList().Where(p => p.DispositivoID == item.DispositivoID).FirstOrDefault().Temperatura_max;
                                    MinTemp = context.Umbrals.ToList().Where(p => p.DispositivoID == item.DispositivoID).FirstOrDefault().Temperatura_min;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("ERROR chartBroadcaster.cs umbrales temperatura.");
                                    Debug.WriteLine(ex.GetBaseException().ToString());
                                }
                                try {
                                    MaxTol = context.Umbrals.ToList().Where(p => p.DispositivoID == item.DispositivoID).FirstOrDefault().Tolerancia_max;
                                    MinTol = context.Umbrals.ToList().Where(p => p.DispositivoID == item.DispositivoID).FirstOrDefault().Tolerancia_min;
                                }
                                catch (Exception ex) {
                                    Debug.WriteLine("ERROR chartBroadcaster.cs umbrales tolerancia.");
                                    Debug.WriteLine(ex.GetBaseException().ToString());
                                }
                                finally
                                {
                                    lis.Add(new DashboardTemperatureResult
                                    {
                                        DispositivoId = item.Medidas.OrderBy(y => y.FechaHora).LastOrDefault().DispositivoID,
                                        Empresa = item.Medidas.OrderBy(y => y.FechaHora).LastOrDefault().Dispositivo.Empresa.AbrEmpresa,
                                        Temperature = item.Medidas.OrderBy(y => y.FechaHora).LastOrDefault().Valor,
                                        Max = MaxTemp,
                                        Min = MinTemp,
                                        NombreDispositivo = context.Dispositivos.Where(p => p.DispositivoID == item.DispositivoID).FirstOrDefault().Nombre,
                                        MaxTolerancia = MaxTol,
                                        MinTolerancia = MinTol,
                                        TipoMedida = item.TipoMedida.Nombre
                                    });
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR chartBroadcaster.cs");
                    Debug.WriteLine(ex.GetBaseException().ToString());
                }
                temp = lis;
            }

        }

        public class DashboardTemperatureResult
        {
            public string Empresa { get; set; }
            public int DispositivoId { get; set; }
            public decimal? Temperature { get; set; }
            public decimal? Max { get; set; }
            public decimal? Min { get; set; }
            public string NombreDispositivo { get; set; }
            public decimal? MaxTolerancia { get; set; }
            public decimal? MinTolerancia { get; set; }
            public string TipoMedida { get; set; }

        }
    }
}


public class LastTemperatureUpdate
{

    // Singleton instance    
    private readonly static Lazy<LastTemperatureUpdate> _instance = new Lazy<LastTemperatureUpdate>(() => new LastTemperatureUpdate());
    // Send Data every 5 seconds    
    private int _updateInterval = int.Parse(ConfigurationManager.AppSettings["UpdateDashboardTime"].ToString());
    //Timer Class    
    private Timer _timer;
    private volatile bool _sendingLastTemperature = false;
    private readonly object _tempUpdateLock = new object();
    LineChart lineChart = new LineChart();
    TemperatureUpdate tempUpdate = new TemperatureUpdate();
    public string idUsuario = "";

    private LastTemperatureUpdate()
    {

    }

    public static LastTemperatureUpdate Instance
    {
        get
        {
            return _instance.Value;
        }
    }

    // Calling this method starts the Timer    
    public void GetTempData()
    {
        _timer = new Timer(TempTimerCallback, null, _updateInterval, _updateInterval);

    }
    private void TempTimerCallback(object state)
    {
        if (_sendingLastTemperature)
        {
            return;
        }
        lock (_tempUpdateLock)
        {
            if (!_sendingLastTemperature)
            {
                _sendingLastTemperature = true;
                SendLastTemperature();
                _sendingLastTemperature = false;
            }
        }
    }

    private void SendLastTemperature()
    {
        tempUpdate.TakeLastTemp(this.idUsuario);
        GetAllClients().All.UpdateTemperature(tempUpdate);
    }

    private static dynamic GetAllClients()
    {
        return GlobalHost.ConnectionManager.GetHubContext<TemperatureHub>().Clients;
    }

}