using System;
using System.Text;
using Xunit;

namespace OpenCosmos.Test
{
    public class cStringReceiverTest
    {
        private cTelemetryEntry GetExpected(Int64 timestamp, UInt16 teleType, float teleValue)
        {
            return new cTelemetryEntry(cTelemetryEntry.ConvertUnixToDateTime(timestamp), teleType, teleValue);
        }

        private string BuildTeleString(Int64 timestamp, UInt16 teleType, float teleValue)
        {
            var ret = new StringBuilder();

            ret.Append("[");
            ret.Append(timestamp.ToString());
            ret.Append(":");

            ret.Append(teleType.ToString("00"));
            ret.Append(":");

            ret.Append(teleValue.ToString("0.000000"));
            ret.Append("]");

            return ret.ToString();
        }


        [Fact]
        public void Test_Create()
        {
            var receiver = new cStringReceiver("name", null, null);
            Int64 unixTimeStamp = 1604614491;
            UInt16 teleType = 01;
            float teleValue = 2.000000f;

            var rawTele = BuildTeleString(unixTimeStamp, teleType, teleValue);
            var actualTele = receiver.DecodeFrame(Encoding.UTF8.GetBytes(rawTele));
            var expectedTele = GetExpected(unixTimeStamp, teleType, teleValue);

            Assert.Equal(expectedTele.ToString(), actualTele.ToString());
        }

        [Fact]
        public void Test_Create_Max()
        {
            var receiver = new cStringReceiver("name", null, null);
            Int64 unixTimeStamp = 253402300799; // Max value for FromUnixTimeSeconds
            UInt16 teleType = UInt16.MaxValue;
            float teleValue = float.MaxValue;

            var rawTele = BuildTeleString(unixTimeStamp, teleType, teleValue);
            var actualTele = receiver.DecodeFrame(Encoding.UTF8.GetBytes(rawTele));
            var expectedTele = GetExpected(unixTimeStamp, teleType, teleValue);

            Assert.Equal(expectedTele.ToString(), actualTele.ToString());
        }

        [Fact]
        public void Test_Create_Negative()
        {
            var receiver = new cStringReceiver("name", null, null);
            Int64 unixTimeStamp = 1604614491;
            UInt16 teleType = 01;
            float teleValue = -152.000000f;

            var rawTele = BuildTeleString(unixTimeStamp, teleType, teleValue);
            var actualTele = receiver.DecodeFrame(Encoding.UTF8.GetBytes(rawTele));
            var expectedTele = GetExpected(unixTimeStamp, teleType, teleValue);

            Assert.Equal(expectedTele.ToString(), actualTele.ToString());
        }

        [Fact]
        public void Test_Create_TelemetryFormatException_Brackets()
        {
            var receiver = new cStringReceiver("name", null, null);
            Int64 unixTimeStamp = 1604614491;
            UInt16 teleType = 01;
            float teleValue = -152.000000f;

            var rawTele = BuildTeleString(unixTimeStamp, teleType, teleValue);
            var rawTeleWithoutFirstBracket = rawTele.Substring(1, rawTele.Length - 1);

            var ex = Assert.Throws<TelemetryFormatException>(() => receiver.DecodeFrame(Encoding.UTF8.GetBytes(rawTeleWithoutFirstBracket)));

            Assert.StartsWith("String Encoding missing opening or closing brackets:", ex.Message);
        }

        [Fact]
        public void Test_Create_TelemetryFormatException_NumParts()
        {
            var receiver = new cStringReceiver("name", null, null);
            Int64 unixTimeStamp = 1604614491;
            UInt16 teleType = 01;
            float teleValue = -152.000000f;

            var rawTele = BuildTeleString(unixTimeStamp, teleType, teleValue);
            var rawTeleMissingPart = rawTele.Remove(1, 11);

            var ex = Assert.Throws<TelemetryFormatException>(() => receiver.DecodeFrame(Encoding.UTF8.GetBytes(rawTeleMissingPart)));

            Assert.StartsWith("String Encoding incorrect, expected", ex.Message);
        }

        [Fact]
        public void Test_Create_TelemetryFormatException_IncorrectTimestamp()
        {
            var receiver = new cStringReceiver("name", null, null);
            var rawTele = "[notatimestamp:01:-152.000000]";

            var ex = Assert.Throws<TelemetryFormatException>(() => receiver.DecodeFrame(Encoding.UTF8.GetBytes(rawTele)));

            Assert.StartsWith("Timestamp Encoding incorrect", ex.Message);
        }

        [Fact]
        public void Test_Create_TelemetryFormatException_IncorrectType()
        {
            var receiver = new cStringReceiver("name", null, null);
            var rawTele = "[1604614491:65536:-152.000000]";

            var ex = Assert.Throws<TelemetryFormatException>(() => receiver.DecodeFrame(Encoding.UTF8.GetBytes(rawTele)));

            Assert.StartsWith("Telemetry ID Encoding incorrect,", ex.Message);
        }

        [Fact]
        public void Test_Create_TelemetryFormatException_IncorrectValue()
        {
            var receiver = new cStringReceiver("name", null, null);
            var rawTele = "[1604614491:015:cheese]";

            var ex = Assert.Throws<TelemetryFormatException>(() => receiver.DecodeFrame(Encoding.UTF8.GetBytes(rawTele)));

            Assert.StartsWith("Telemetry Value Encoding incorrect,", ex.Message);
        }
    }
}