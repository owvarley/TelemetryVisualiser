using System;
using System.Net.Sockets;

namespace OpenCosmos
{
    public class cStringReceiver : cReceiverBase
    {
        private const byte FRAME_START_MARKER = (byte) '[';
        private const byte FRAME_END_MARKER = (byte) ']';
        private const int FRAME_BUFFER = 256;

        public override cTelemetryEntry DecodeFrame(byte[] frame)
        {
            var rawTelemetry = System.Text.Encoding.UTF8.GetString(frame, 0, frame.Length);

            // Expecting a UTF-8 String
            // Wrapped by square brackets []
            // 3 Telemetry values separated by :
            // e.g. [1604614491:1:2.000000]

            const int OPEN_BRACKET_POS = 0;
            int CLOSE_BRACKET_POS = rawTelemetry.Length - 1;
            const int EXPECTED_ELEMENT_LENGTH = 3;
            const int TIMESTAMP_POS = 0;
            const int ID_POS = 1;
            const int VALUE_POS = 2;
            const char FIELD_SEPARATOR = ':';

            Int64 teleUnixTimestamp = 0;
            DateTime teleTimestamp = DateTime.MinValue;
            UInt16 teleId = 0;
            float teleValue = 0.0f;

            if (rawTelemetry[OPEN_BRACKET_POS] != FRAME_START_MARKER || rawTelemetry[CLOSE_BRACKET_POS] != FRAME_END_MARKER)
            {
                throw new TelemetryFormatException("String Encoding missing opening or closing brackets: " + rawTelemetry);
            }

            // Strip the opening and closing brackets
            rawTelemetry = rawTelemetry.Substring(OPEN_BRACKET_POS + 1, rawTelemetry.Length - 1 - 1); // -1 for each bracket

            // Split the string up to get the component parts
            var teleElements = rawTelemetry.Split(FIELD_SEPARATOR);

            // Check the string encoding matches what we are expecting. It should be three items split by a ':' e.g. [1604614491:1:2.000000]
            if (teleElements.Length != EXPECTED_ELEMENT_LENGTH)
            {
                throw new TelemetryFormatException("String Encoding incorrect, expected " + EXPECTED_ELEMENT_LENGTH.ToString() + " elements, received " + teleElements.Length);
            }

            // Parse the Timestamp
            if (Int64.TryParse(teleElements[TIMESTAMP_POS], out teleUnixTimestamp))
            {
                teleTimestamp = cTelemetryEntry.ConvertUnixToDateTime(teleUnixTimestamp);
            }
            else
            {
                throw new TelemetryFormatException("Timestamp Encoding incorrect, expected 8 byte integer with unix timestamp in ms: " + teleElements[TIMESTAMP_POS]);
            }

            // Parse the ID
            if (!UInt16.TryParse(teleElements[ID_POS], out teleId))
            {
                throw new TelemetryFormatException("Telemetry ID Encoding incorrect, expected 2 byte integer: " + teleElements[ID_POS]);
            }

            // Parse the Value
            if (!float.TryParse(teleElements[VALUE_POS], out teleValue))
            {
                throw new TelemetryFormatException("Telemetry Value Encoding incorrect, expected 8 byte float: " + teleElements[VALUE_POS]);
            }

            return new cTelemetryEntry(teleTimestamp, teleId, teleValue);
        }

        private byte ReadByte(NetworkStream ns, ref int TotalBytesReadFromStream)
        {
            TotalBytesReadFromStream++;
            return (Byte) ns.ReadByte();
        }

        public override byte[] ReadFrameFromStream(NetworkStream ns, ref int TotalBytesReadFromStream)
        {
            var FrameBuffer = new byte[FRAME_BUFFER];
            var FrameIndex = 0;

            // Read from Start Marker until End Marker
            var b = ReadByte(ns, ref TotalBytesReadFromStream);

            // If we don't have a valid start marker then read and discard bytes until we do
            if (b != FRAME_START_MARKER)
            {
                SeekToByteAfterStartMarker(ns, ref TotalBytesReadFromStream);
                Console.WriteLine("Truncated Frame encountered, discarded {0} bytes.", TotalBytesReadFromStream);
            }

            // At this point we've either encountered a valid frame that starts with the correct marker
            // or an invalid frame and have fast forwarded until the start of a valid frame
            FrameBuffer[FrameIndex] = FRAME_START_MARKER;
            FrameIndex ++;

            do
            {
                b = ReadByte(ns, ref TotalBytesReadFromStream);

                // Basic check for malformed frames - if we detect one then throw out everything we've read and discount this frame then continue
                if (b == FRAME_START_MARKER)
                {
                    SeekToByteAfterStartMarker(ns, ref TotalBytesReadFromStream);
                    Console.WriteLine("Malformed Frame encountered, detected second start marker before end marker.");
                    ResetFrame(ref FrameBuffer, ref FrameIndex);
                }

                FrameBuffer[FrameIndex] = b;
                FrameIndex++;
            } while (b != FRAME_END_MARKER);

            FrameBuffer[FrameIndex] = FRAME_END_MARKER;

            // Resize the buffer to just what was read from the stream
            Array.Resize(ref FrameBuffer, FrameIndex);

            return FrameBuffer;
        }

        private void ResetFrame (ref byte[] Frame, ref int FrameIndex)
        {
            for (var i = 0; i < FRAME_BUFFER; i++)
                Frame[i] = 0;

            FrameIndex = 0;

            Frame[FrameIndex] = FRAME_START_MARKER;
            FrameIndex = 1;
        }

        private void SeekToByteAfterStartMarker(NetworkStream ns, ref int TotalBytesReadFromStream)
        {
            while (ns.ReadByte() != FRAME_START_MARKER)
            {
                TotalBytesReadFromStream ++;
            }
        }

        public cStringReceiver(string NewSatName, string NewHost, int NewPort, iDBDriver DBDriver) : base(NewSatName, NewHost, NewPort, DBDriver)
        {

        }
    }
    
}