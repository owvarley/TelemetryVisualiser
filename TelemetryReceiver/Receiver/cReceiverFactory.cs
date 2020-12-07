using System;

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

        public static void OutputValidTypes()
        {
            Console.WriteLine("Valid encoding types are: ");
            Console.WriteLine("  string - String Encoding");
            Console.WriteLine("  binary - Binary Encoding");
        }
    }
}