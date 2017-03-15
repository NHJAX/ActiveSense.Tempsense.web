using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using ActiveSense.Tempsense.model;
using ActiveSense.Tempsense.Receptor;
using System.Configuration;
using ActiveSense.Tempsense.model.Modelo;

namespace ActiveSense.Tempsense.Receptor
{
    public class SimpleEventProcessor : IEventProcessor
    {
        Stopwatch checkpointStopWatch;
        int messageCount = 0;
        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("Processor Shutting Down. Partition '{0}', Reason:'{1}'.", context.Lease.PartitionId, reason);
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            Console.WriteLine("SimpleEventProcessor initialized. Partition:'{0}',offset:'{1}'", context.Lease.PartitionId, context.Lease.Offset);
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            List<ActiveSense.Tempsense.model.Modelo.Medida> medidas = new List<ActiveSense.Tempsense.model.Modelo.Medida>();
            foreach (EventData eventData in messages)
            {
                string strConn = string.Format(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString, eventData.Properties["Ambiente"]);
                messageCount++;
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                JObject o = JObject.Parse(data);
                var deviceKey = int.Parse(o["deviceKey"].ToString());

                using (ActiveSenseContext db = new ActiveSenseContext(strConn))
                {
                    try
                    {
                        var disp = db.Dispositivos
                            .Where(p => p.DispositivoID == deviceKey);
                        if (disp.ToList().Count > 0)
                        {
                            ActiveSense.Tempsense.model.Modelo.Medida medida = new ActiveSense.Tempsense.model.Modelo.Medida()
                            {
                                DispositivoID = disp.FirstOrDefault().DispositivoID,
                                Valor = decimal.Parse(o["valor"].ToString()),
                                FechaHora = Convert.ToDateTime(o["fecha"].ToString()),
                            };
                            Console.WriteLine(string.Format("Message received. Partition:{0}, Data:{1}{2}", context.Lease.PartitionId, data, eventData.EnqueuedTimeUtc));
                            db.Medidas.Add(medida);
                            db.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine(string.Format("Device Key not found in database:{0}, Message:{1}", o["deviceKey"].ToString(), o));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("Mensaje tuvo error:{0},{1}", ex.Message, data));
                    }
                }
            }


            //if (messageCount > Configuracion.TamanoLoteMensajes)
            context.CheckpointAsync();
            return Task.FromResult<object>(null);
        }

    }
}