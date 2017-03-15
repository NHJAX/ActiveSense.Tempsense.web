using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Diagnostics;
using Microsoft.ServiceBus.Messaging;

namespace ActiveSense.Tempsense.ReceptorWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            try
            {
                Console.WriteLine(String.Format("Inicio webjob:  {0}", DateTime.Now.ToString()));

                JobHostConfiguration config = new JobHostConfiguration();
                config.Tracing.ConsoleLevel = TraceLevel.Verbose;
                config.UseTimers();
                JobHost host = new JobHost(config);
                host.RunAndBlock();

                Console.WriteLine(String.Format("Fin webjob:  {0}", DateTime.Now.ToString()));

            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error webjob:  {0}", ex.Message));
                Console.WriteLine(String.Format("Error webjob:  {0}", ex.StackTrace));
                //throw ex;
            }

        }
    }
}
