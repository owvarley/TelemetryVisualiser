using System;
using System.Runtime.Serialization;

namespace OpenCosmos
{
    [Serializable()]
    public class UnreachableCodeException : Exception
    {
        public UnreachableCodeException() : base("This code should not be reached, if it has this is due to configuration or developer error.") { }
    }
}