using System;
using Xunit;

namespace OpenCosmos.Test
{
    public class cTelemetryEntryTest
    {
        [Fact]
        public void Test_ConvertUnixToDateTime_OutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => cTelemetryEntry.ConvertUnixToDateTime(long.MaxValue));
        }
        
        [Fact]
        public void Test_ConvertUnixToDateTime_TestEpoch()
        {
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0), DateTime.UnixEpoch);
        }

        [Fact]
        public void Test_ConvertUnixToDateTime_ValidConversion()
        {
            var date = new DateTime(1970, 1, 11, 0, 0, 0);
            var ten_days_in_seconds = 60 * 60 * 24 * 10;

            date.AddSeconds(ten_days_in_seconds);

            Assert.Equal(date, cTelemetryEntry.ConvertUnixToDateTime(ten_days_in_seconds));
        }

        [Fact]
        public void Test_ToString_Valid()
        {
            var date = new DateTime(2020, 12, 25, 10, 15, 0);
            var tele = new cTelemetryEntry(date, 1, 15.0f);

            Assert.Equal("Timestamp: 25/12/2020 10:15:00 ID: 01 Value: 15.000000", tele.ToString());
        }

        [Fact]
        public void Test_ToString_Invalid()
        {
            var tele = new cTelemetryEntry();

            Assert.Equal("*Timestamp: 01/01/0001 00:00:00 ID: 00 Value: 0.000000", tele.ToString());
        }
    }
}
