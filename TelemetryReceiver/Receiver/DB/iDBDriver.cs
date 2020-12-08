namespace OpenCosmos
{
    public interface iDBDriver
    {
        bool WriteToDB(cTelemetryEntry TelemetryEntry, string SatName);
    }
}