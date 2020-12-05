using System;

namespace OpenCosmos
{
    public class cConsoleDriver : iDBDriver
    {
        public bool WriteToDB(cTelemetryEntry telemetryEntry)
        {
            Console.WriteLine(telemetryEntry.ToString());
            return true;
        }
    }
}