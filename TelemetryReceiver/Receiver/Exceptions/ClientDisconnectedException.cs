using System;
using System.Runtime.Serialization;

namespace OpenCosmos
{
    [Serializable()]
    public class ClientDisconnectedException : Exception
    {
        public ClientDisconnectedException(Exception exception) : base("Network Client disconnected unexpectedly", exception) { }
    }
}