using System;
using System.Runtime.Serialization;

namespace OpenCosmos
{
    [Serializable()]
    public class TelemetryFormatException : Exception
    {
        public TelemetryFormatException(string Msg) : base(Msg) { }
    }
}