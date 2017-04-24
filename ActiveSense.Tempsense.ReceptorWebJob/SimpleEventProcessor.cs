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
using ActiveSense.Tempsense.model.Model;

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
            List<ActiveSense.Tempsense.model.Model.Measure> Measures = new List<ActiveSense.Tempsense.model.Model.Measure>();
            foreach (EventData eventData in messages)
            {
                string strConn = string.Format(ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString, eventData.Properties["Environment"]);
                messageCount++;
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                JObject o = JObject.Parse(data);
                var deviceID = int.Parse(o["deviceID"].ToString());
                //var deviceID = 3;

                using (ActiveSenseContext db = new ActiveSenseContext(strConn))
                {
                    try
                    {
                        var disp = db.devices
                            .Where(p => p.DeviceID == deviceID);
                        if (disp.ToList().Count > 0)
                        {
                            ActiveSense.Tempsense.model.Model.Measure Measure = new ActiveSense.Tempsense.model.Model.Measure()
                            {
                                DeviceID = disp.FirstOrDefault().DeviceID,
                                Value = decimal.Parse(o["Temperature"].ToString()),
                                DateTime = Convert.ToDateTime(o["Date"].ToString()),
                            };
                            Console.WriteLine(string.Format("Message received. Partition:{0}, Data:{1}{2}", context.Lease.PartitionId, data, eventData.EnqueuedTimeUtc));
                            db.Measure.Add(Measure);
                            db.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine(string.Format("Device Key not found in database:{0}, Message:{1}", o["deviceKey"].ToString(), o));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("Message was mistake:{0},{1}", ex.Message, data));
                    }
                }
            }


            //if (messageCount > Configurationsizebatchmessages)
            context.CheckpointAsync();
            return Task.FromResult<object>(null);
        }

    }
}