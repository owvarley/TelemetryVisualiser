using System.IO;
using System.Net.Sockets;

namespace OpenCosmos
{
    public class cTelemetryClient : TcpClient
    {
        public void CheckConnection()
        {
            try
            {
                this.GetStream().WriteByte(1);
            }
            catch (IOException e)
            {
                throw new ClientDisconnectedException(e);
            }
        }

        public cTelemetryClient(string Host, int NewPort) : base (Host, NewPort) {}
    }
}