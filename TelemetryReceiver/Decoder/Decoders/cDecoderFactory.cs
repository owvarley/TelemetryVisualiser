using System;

namespace OpenCosmos
{
    public static class cDecoderFactory
    {
        public static iDecoder Create (string encodingType, iDBDriver dBDriver)
        {
            switch (encodingType.ToLower())
            {
                case "string":
                    return new cStringDecoder(dBDriver);
                case "binary":
                    return new cBinaryDecoder(dBDriver);
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