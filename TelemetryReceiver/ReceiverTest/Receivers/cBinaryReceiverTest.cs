using System;
using Xunit;

namespace OpenCosmos.Test
{
    public class cBinaryReceiverTest
    {
        private cTelemetryEntry GetExpected(Int64 timestamp, UInt16 teleType, float teleValue)
        {
            return new cTelemetryEntry(cTelemetryEntry.ConvertUnixToDateTime(timestamp), teleType, teleValue);
        }

        private byte[] BuildTeleByteArray(Int64 timestamp, UInt16 teleType, float teleValue)
        {
            var rawTele = new byte[18];

            rawTele[0] = 00;
            rawTele[1] = 01;
            rawTele[2] = 02;
            rawTele[3] = 03;

            var timeStampArray = BitConverter.GetBytes(timestamp);
            var teleTypeArray = BitConverter.GetBytes(teleType);
            var teleValueArray = BitConverter.GetBytes(teleValue);

            Array.Copy(timeStampArray, 0, rawTele, 4, 8);
            Array.Copy(teleTypeArray, 0, rawTele, 12, 2);
            Array.Copy(teleValueArray, 0, rawTele, 14, 4);

            return rawTele;
        }

        [Fact]
        public void Test_CheckHeaderPresent()
        {
            var receiver = new cBinaryReceiver("name", "host", 0, null);
            var rawTele = new byte[18];

            rawTele[0] = 00;
            rawTele[1] = 01;
            rawTele[2] = 02;
            rawTele[4] = 04;
            
            Assert.Throws<TelemetryFormatException>(() => receiver.DecodeFrame(rawTele));
        }

        [Fact]
        public void Test_Create()
        {
            var receiver = new cBinaryReceiver("name", "host", 0, null);
            Int64 unixTimeStamp = 1604614491;
            UInt16 teleType = 02;
            float teleValue = 2.0f;

            var rawTele = BuildTeleByteArray(unixTimeStamp, teleType, teleValue);
            var actualTele = receiver.DecodeFrame(rawTele);
            var expectedTele = GetExpected(unixTimeStamp, teleType, teleValue);

            Assert.Equal(expectedTele.ToString(), actualTele.ToString());
        }

        [Fact]
        public void Test_Create_Negative()
        {
            var receiver = new cBinaryReceiver("name", "host", 0, null);
            Int64 unixTimeStamp = 1604614491;
            UInt16 teleType = 02;
            float teleValue = -500.0f;

            var rawTele = BuildTeleByteArray(unixTimeStamp, teleType, teleValue);
            var actualTele = receiver.DecodeFrame(rawTele);
            var expectedTele = GetExpected(unixTimeStamp, teleType, teleValue);

            Assert.Equal(expectedTele.ToString(), actualTele.ToString());
        }

        [Fact]
        public void Test_Create_Max()
        {
            var receiver = new cBinaryReceiver("name", "host", 0, null);
            Int64 unixTimeStamp = 253402300799; // Max value for FromUnixTimeSeconds
            UInt16 teleType = UInt16.MaxValue;
            float teleValue = float.MaxValue;

            var rawTele = BuildTeleByteArray(unixTimeStamp, teleType, teleValue);
            var actualTele = receiver.DecodeFrame(rawTele);
            var expectedTele = GetExpected(unixTimeStamp, teleType, teleValue);

            Assert.Equal(expectedTele.ToString(), actualTele.ToString());
        }
    }
}