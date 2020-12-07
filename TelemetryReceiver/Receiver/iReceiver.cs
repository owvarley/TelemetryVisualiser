using System.Net.Sockets;

namespace OpenCosmos
{
    public interface iReceiver
    {
        byte[] ReadFrameFromStream(NetworkStream ns, ref int bytes);
        cTelemetryEntry DecodeFrame(byte[] frame);
        void Start();
        void Stop();
    }
}