using System;
using System.Runtime.Serialization;

namespace OpenCosmos
{
    [Serializable()]
    public class ClientDisconnectedException : Exception
    {
        public ClientDisconnectedException(Exception ex) : base("Network Client disconnected unexpectedly", ex) { }
    }
}