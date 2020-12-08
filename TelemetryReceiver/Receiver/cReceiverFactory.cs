using System;
using System.Text;

namespace OpenCosmos
{
    public static class cReceiverFactory
    {
        public static iReceiver Create (string host, int port, string encodingType, iDBDriver dBDriver)
        {
            switch (encodingType.ToLower())
            {
                case "string":
                    return new cStringReceiver("string_sat", host, port, dBDriver);
                case "binary":
                    return new cBinaryReceiver("binary_sat", host, port, dBDriver);
                default:
                    throw new UnknownEncodingException();
            }
        }

        public static string OutputValidTypes()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Valid encoding types are: ");
            sb.AppendLine("  string - String Encoding");
            sb.AppendLine("  binary - Binary Encoding");

            return sb.ToString();
        }
    }
}