using System;

namespace OpenCosmos
{
    public class cBinaryDecoder : cDecoderBase
    {
        private static int RECEIVER_PORT = 8001;
        private static int READ_BUFFER_BYTES = 18; 

        private bool CheckHeaderPresent(byte[] rawTelemetry)
        {
            // Valid Header should be 00-01-02-03
            if (rawTelemetry[0] == 0 &&
                rawTelemetry[1] == 1 &&
                rawTelemetry[2] == 2 &&
                rawTelemetry[3] == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static cTelemetryEntry Create(byte[] rawTelemetry)
        {
            // Binary Packet is LE and as follows
            // Header    |  0 - 3  | 4 Bytes |
            // TimeStamp |  4 - 11 | 8 Bytes |
            // ID        | 12 - 13 | 2 Bytes |
            // Value     | 14 - 18 | 4 Bytes |
            // ================================
            // Total               |18 Bytes |
            // ================================

            const int TIMESTAMP_INDEX = 4;
            const int ID_INDEX = 12;
            const int VALUE_INDEX = 14;

            var teleTimestamp = cTelemetryEntry.ConvertUnixToDateTime(BitConverter.ToInt64(rawTelemetry, TIMESTAMP_INDEX));
            var teleId = BitConverter.ToUInt16(rawTelemetry, ID_INDEX);
            var teleValue = BitConverter.ToSingle(rawTelemetry, VALUE_INDEX);

            return new cTelemetryEntry(teleTimestamp, teleId, teleValue);
        }

        public override cTelemetryEntry Decode(byte[] buffer)
        {
            if (!CheckHeaderPresent(buffer)) throw new TelemetryFormatException("Binary Format incorrect, 4 Byte header (00010203) missing");

            return Create(buffer);
        }

        public cBinaryDecoder(iDBDriver NewDBDriver) : base(READ_BUFFER_BYTES, RECEIVER_PORT, NewDBDriver, "binary_sat") { }
    }
}