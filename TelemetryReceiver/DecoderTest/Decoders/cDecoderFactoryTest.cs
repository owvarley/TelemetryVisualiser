using System;
using Xunit;

namespace OpenCosmos.Test
{
    public class cDecoderFactoryTest
    {
        [Fact]
        public void Test_Create_String()
        {
            var decoder = cDecoderFactory.Create("string", null);

            Assert.IsType<cStringDecoder>(decoder);
        }

        [Fact]
        public void Test_Create_Binary()
        {
            var decoder = cDecoderFactory.Create("binary", null);

            Assert.IsType<cBinaryDecoder>(decoder);
        }

        [Fact]
        public void Test_Create_Invalid()
        {
            Assert.Throws<UnknownEncodingException>(() => cDecoderFactory.Create("letthewookiewin", null));
        }
    }
}