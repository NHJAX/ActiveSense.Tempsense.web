using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Diagnostics;
using Microsoft.ServiceBus.Messaging;
using ActiveSense.Tempsense.ReceptorWebJob;

namespace ActiveSense.Tempsense.Receptor.WebJob
{
    public class Functions
    {

        public static void CronJob([TimerTrigger("* */1 * * * *")] TimerInfo timer)
        {
            try
            {
                Console.WriteLine(String.Format("Inicio lectura mensajes : {0}", DateTime.Now.ToString()));
                string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                                       Configuracion.StorageAccountName, Configuracion.
                                       StorageAccountKey);
                string _guid = Guid.NewGuid().ToString();
                string eventProcessorHostName = _guid;
                EventProcessorHost eventProcessorHost = new EventProcessorHost(
                                                                eventProcessorHostName,
                                                                Configuracion.EventHubName,
                                                                EventHubConsumerGroup.DefaultGroupName,
                                                                Configuracion.EventHubConnectionString,
                                                                storageConnectionString);
                Console.WriteLine("Registering EventProcessor...");
                var options = new EventProcessorOptions();
                options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
                eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(options).Wait();
                //Console.WriteLine("Receiving.Press enter key to stop worker.");
                //Console.ReadLine();
                eventProcessorHost.UnregisterEventProcessorAsync().Wait();
                Console.WriteLine(String.Format("Fin lectura mensajes : {0}", DateTime.Now.ToString()));
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
