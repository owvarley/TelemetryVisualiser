using System;
using System.Text;

namespace OpenCosmos
{
    public static class cReceiverFactory
    {
        public static iReceiver Create (string EncodingType, iDBDriver DBDriver, iClient Client)
        {
            switch (EncodingType.ToLower())
            {
                case "string":
                    return new cStringReceiver("string_sat", DBDriver, Client);
                case "binary":
                    return new cBinaryReceiver("binary_sat", DBDriver, Client);
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