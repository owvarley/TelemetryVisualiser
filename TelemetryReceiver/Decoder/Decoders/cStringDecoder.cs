using System;

namespace OpenCosmos
{
    public class cStringDecoder : cDecoderBase
    {
        private static int RECEIVER_PORT = 8000;
        private static int READ_BUFFER_BYTES = 256; 

        private static cTelemetryEntry Create(string rawTelemetry)
        {
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

            if (rawTelemetry[OPEN_BRACKET_POS] != '[' || rawTelemetry[CLOSE_BRACKET_POS] != ']')
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

        public override cTelemetryEntry Decode(byte[] buffer)
        {
            var rawTelemetry = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            return Create(rawTelemetry);
        }

        public cStringDecoder(iDBDriver NewDBDriver) : base(READ_BUFFER_BYTES, RECEIVER_PORT, NewDBDriver, "string_sat") { }
    }
}