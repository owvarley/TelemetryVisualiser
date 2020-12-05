using System;

namespace OpenCosmos
{
    class Program
    {
        static void Main(string[] args)
        {
            var FIRST_ARG = 0;

            try
            {
                var receiver = cDecoderFactory.Create(args[FIRST_ARG], new cConsoleDriver());
                Console.WriteLine(receiver.GetType());
                receiver.Start();
            }
            catch (System.IndexOutOfRangeException)
            {
                Console.WriteLine("No Encoding was supplied.");
                cDecoderFactory.OutputValidTypes();
            }
            catch (UnknownEncodingException)
            {
                Console.WriteLine("The supplied Encoding is not valid.");
                cDecoderFactory.OutputValidTypes();
            }
        }
    }
}
