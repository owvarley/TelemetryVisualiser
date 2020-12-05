namespace OpenCosmos
{
    public interface iDecoder
    {
        cTelemetryEntry Decode(byte[] buffer);
        void Start();
    }
}