using Xunit;

namespace OpenCosmos.Test
{
    public class cReceiverFactoryTest
    {
        [Fact]
        public void Test_Create_String()
        {
            var receiver = cReceiverFactory.Create("string", null, null);

            Assert.IsType<cStringReceiver>(receiver);
        }

        [Fact]
        public void Test_Create_Binary()
        {
            var receiver = cReceiverFactory.Create("binary", null, null);

            Assert.IsType<cBinaryReceiver>(receiver);
        }

        [Fact]
        public void Test_Create_Invalid()
        {
            Assert.Throws<UnknownEncodingException>(() => cReceiverFactory.Create("thisistheway", null, null));
        }
    }
}