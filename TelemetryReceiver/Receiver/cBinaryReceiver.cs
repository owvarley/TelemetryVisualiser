using System;
using System.Net.Sockets;

namespace OpenCosmos
{
    public class cBinaryReceiver : cReceiverBase
    {
        private static int FRAME_LENGTH_BYTES = 18; 

        private bool CheckHeaderPresent(byte[] RawTelemetry)
        {
            // Valid Header should be 00-01-02-03
            if (RawTelemetry[0] == 0 &&
                RawTelemetry[1] == 1 &&
                RawTelemetry[2] == 2 &&
                RawTelemetry[3] == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private cTelemetryEntry Decode(byte[] RawTelemetry)
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

            var tele_timestamp = cTelemetryEntry.ConvertUnixToDateTime(BitConverter.ToInt64(RawTelemetry, TIMESTAMP_INDEX));
            var tele_id = BitConverter.ToUInt16(RawTelemetry, ID_INDEX);
            var tele_value = BitConverter.ToSingle(RawTelemetry, VALUE_INDEX);

            return new cTelemetryEntry(tele_timestamp, tele_id, tele_value);
        }

        public override cTelemetryEntry DecodeFrame(byte[] Frame)
        {
            if (!CheckHeaderPresent(Frame)) throw new TelemetryFormatException("Binary Format incorrect, 4 Byte header (00010203) missing");

            return Decode(Frame);
        }

        public override byte[] ReadFrameFromStream(NetworkStream ns, ref int TotalBytesReadFromStream)
        {
            var frame = new byte[FRAME_LENGTH_BYTES];
            TotalBytesReadFromStream = ns.Read(frame, 0, frame.Length);

            return frame;
        }

        public cBinaryReceiver(string NewSatName, iDBDriver DBDriver, iClient NewClient) : base(NewSatName, DBDriver, NewClient)
        {

        }
    }
}