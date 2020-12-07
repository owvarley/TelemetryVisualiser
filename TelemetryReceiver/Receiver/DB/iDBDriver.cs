namespace OpenCosmos
{
    public interface iDBDriver
    {
        bool WriteToDB(cTelemetryEntry telemetryEntry, string SatName);
    }
}