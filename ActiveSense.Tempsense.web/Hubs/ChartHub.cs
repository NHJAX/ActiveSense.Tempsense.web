using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ActiveSense.Tempsense.web.Hubs
{
    public class TemperatureHub : Hub
    {
        // Create the instance of ChartDataUpdate    
        private readonly LastTemperatureUpdate _instance;
        public TemperatureHub() : this(LastTemperatureUpdate.Instance) { }

        public TemperatureHub(LastTemperatureUpdate instance)
        {
            _instance = instance;
        }

        public void InitTempData(string data = "" )
        {
            _instance.idUsuario = data;
            _instance.GetTempData();

        }

    }
}