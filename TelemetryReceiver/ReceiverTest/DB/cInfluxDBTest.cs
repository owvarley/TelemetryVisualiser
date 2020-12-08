using System;
using Xunit;

namespace OpenCosmos.Test
{
    public class cInfluxDBTest
    {
        private static cInfluxDriver.tInfluxConfig GetConfig(string NewToken, string NewPassword)
        {
            return new cInfluxDriver.tInfluxConfig
            {
                Host = "host",
                Port = "0",
                Token = NewToken,
                Bucket = "bucket",
                Organisation = "org",
                Password = NewPassword
            };
        }

        [Fact]
        public void Test_InvalidToken()
        {
            var ex = Assert.Throws<ArgumentException>(() => new cInfluxDriver(GetConfig("", "")));
            Assert.StartsWith("No token provided", ex.Message);
        }

        [Fact]
        public void Test_InvalidPassword()
        {
            var ex = Assert.Throws<ArgumentException>(() => new cInfluxDriver(GetConfig("token", "")));
            Assert.StartsWith("No password defined", ex.Message);
        }

    }
}
