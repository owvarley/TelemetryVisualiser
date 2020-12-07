using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenCosmos
{
    class Program
    {
        static string GetHost()
        {
            var host = Environment.GetEnvironmentVariable("TELEMETRY_HOST");

            if (host is null)
            {
                Console.WriteLine("No host supplied via TELEMETRY_HOST environment variable, defaulting to localhost");
                host = "localhost";
            }

            return host;
        }

        static int GetPort()
        {
            var port = 0;
            
            try
            {
                port = int.Parse(Environment.GetEnvironmentVariable("TELEMETRY_PORT"));
            }
            catch (System.Exception)
            {
                Console.WriteLine("No or invalid port supplied via TELEMETRY_PORT environment variable, defaulting to 8000");
                port = 8000;
            }

            return port;
        }

        static void Main(string[] args)
        {
            var FIRST_ARG = 0;

            try
            {
                var receiver = cReceiverFactory.Create(GetHost(), GetPort(), args[FIRST_ARG], new cConsoleDriver());
                Console.WriteLine("Using Receive Type: {0}", receiver.GetType());

                receiver.Start();
            }
            catch (System.IndexOutOfRangeException)
            {
                Console.WriteLine("No Encoding was supplied.");
                cReceiverFactory.OutputValidTypes();
            }
            catch (UnknownEncodingException)
            {
                Console.WriteLine("The supplied Encoding is not valid.");
                cReceiverFactory.OutputValidTypes();
            }
        }
    }
}
