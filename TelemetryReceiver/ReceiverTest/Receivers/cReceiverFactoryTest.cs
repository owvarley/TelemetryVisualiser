using System;
using Xunit;

namespace OpenCosmos.Test
{
    public class cReceiverFactoryTest
    {
        [Fact]
        public void Test_Create_String()
        {
            var receiver = cReceiverFactory.Create("host", 0, "string", null);

            Assert.IsType<cStringReceiver>(receiver);
        }

        [Fact]
        public void Test_Create_Binary()
        {
            var receiver = cReceiverFactory.Create("host", 0, "binary", null);

            Assert.IsType<cBinaryReceiver>(receiver);
        }

        [Fact]
        public void Test_Create_Invalid()
        {
            Assert.Throws<UnknownEncodingException>(() => cReceiverFactory.Create("host", 0, "letthewookiewin", null));
        }
    }
}