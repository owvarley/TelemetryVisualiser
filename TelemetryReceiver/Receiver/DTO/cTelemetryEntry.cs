using System;
using System.Text;

namespace OpenCosmos
{
    public sealed class cTelemetryEntry
    {
        private readonly DateTime _TimeStamp;
        private readonly UInt16 _Id;
        private readonly float _Value;
        private readonly bool _ParsedCorrectly;

        public bool IsValid => _ParsedCorrectly;
        public DateTime Timestamp => _TimeStamp;
        public UInt16 Id => _Id;
        public float Value => _Value;

        public static DateTime ConvertUnixToDateTime(Int64 unixTimeStamp)
        {
            var dto = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
            return dto.UtcDateTime;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (!_ParsedCorrectly) sb.Append("*");

            sb.Append("Timestamp: ");
            sb.Append(_TimeStamp.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss"));

            sb.Append(" ID: ");
            sb.Append(_Id.ToString("00"));

            sb.Append(" Value: ");
            sb.Append(_Value.ToString("0.000000"));

            return sb.ToString();
        }

        public cTelemetryEntry()
        {
            _TimeStamp = DateTime.MinValue;
            _Id = 0;
            _Value = 0.0f;
            _ParsedCorrectly = false; 
        }

        public cTelemetryEntry(DateTime NewTimeStamp, UInt16 NewId, float NewValue)
        {
            _TimeStamp = NewTimeStamp;
            _Id = NewId;
            _Value = NewValue;
            _ParsedCorrectly = true;
        }
    }
}