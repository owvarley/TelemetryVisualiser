using System;

namespace OpenCosmos
{
    public class cConsoleDriver : iDBDriver
    {
        public bool WriteToDB(cTelemetryEntry TelemetryEntry, string SatName)
        {
            Console.WriteLine("From: {0} - {1}", SatName, TelemetryEntry.ToString());
            return true;
        }
    }
}