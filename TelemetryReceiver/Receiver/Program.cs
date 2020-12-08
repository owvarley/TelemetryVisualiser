using System;
using Logger;

namespace OpenCosmos
{
    class Program
    {
        static string GetHost()
        {
            var host = Environment.GetEnvironmentVariable("TELEMETRY_HOST");

            if (host is null)
            {
                Log.Log(enLogLevel.Info, "No host supplied via TELEMETRY_HOST environment variable, defaulting to localhost");
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
                Log.Log(enLogLevel.Info, "No or invalid port supplied via TELEMETRY_PORT environment variable, defaulting to 8000");
                port = 8000;
            }

            return port;
        }

        private static readonly Logger.iLogger Log = new Logger.cLog4net(typeof(Program));

        static void Main(string[] args)
        {
            var FIRST_ARG = 0;

            Log.Init();

            try
            {
                var receiver = cReceiverFactory.Create(GetHost(), GetPort(), args[FIRST_ARG], new cInfluxDriver(cInfluxDriver.tInfluxConfig.Default));
                Log.Log(enLogLevel.Info, "Using Receiver Type: {0}", receiver.GetType());

                receiver.Start();
            }
            catch (System.IndexOutOfRangeException)
            {
                Log.Log(enLogLevel.Error, "No Encoding was supplied.");
                Log.Log(enLogLevel.Info, cReceiverFactory.OutputValidTypes());
            }
            catch (UnknownEncodingException)
            {
                Log.Log(enLogLevel.Error, "The supplied Encoding is not valid.");
                Log.Log(enLogLevel.Info, cReceiverFactory.OutputValidTypes());
            }
            catch (Exception e)
            {
                Log.Log(enLogLevel.Fatal, e.Message);
                throw;
            }
        }
    }
}
