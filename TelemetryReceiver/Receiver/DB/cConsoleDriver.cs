using System;

namespace OpenCosmos
{
    public class cConsoleDriver : iDBDriver
    {
        public bool WriteToDB(cTelemetryEntry telemetryEntry, string SatName)
        {
            Console.WriteLine("From: {0} - {1}", SatName, telemetryEntry.ToString());
            return true;
        }
    }
}