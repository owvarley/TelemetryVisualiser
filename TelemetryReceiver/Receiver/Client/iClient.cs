using System.Net.Sockets;

namespace OpenCosmos
{
    public interface iClient
    {
         void Close();
         void Connect();
         NetworkStream GetStream();
    }
}