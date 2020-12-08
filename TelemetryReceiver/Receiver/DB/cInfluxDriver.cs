using System;
using System.Text;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using Logger;

namespace OpenCosmos
{
    public class cInfluxDriver : iDBDriver
    {
        private InfluxDBClient _Client;
        private readonly string _Token;
        private readonly string _Bucket;
        private readonly string _Organisation;

        private static readonly Logger.iLogger Log = new Logger.cLog4net(typeof(cInfluxDriver));

        public struct tInfluxConfig
        {
            public string Host;
            public string Port;
            public string Token;
            public string Bucket;
            public string Organisation;
            public string Password;

            public static tInfluxConfig Default => (
                new tInfluxConfig
                {
                    Host = Environment.GetEnvironmentVariable("INFLUXDB_HOST"),
                    Port = Environment.GetEnvironmentVariable("INFLUXDB_PORT"),
                    Token = Environment.GetEnvironmentVariable("INFLUXDB_TOKEN"),
                    Bucket = Environment.GetEnvironmentVariable("INFLUXDB_BUCKET"),
                    Organisation = Environment.GetEnvironmentVariable("INFLUXDB_ORG"),
                    Password = Environment.GetEnvironmentVariable("INFLUXDB_PASSWORD")
                }
            );
        }

        public bool WriteToDB(cTelemetryEntry TelemetryEntry, string SatName)
        {
            var point = PointData
                .Measurement("mem")
                .Tag("host", SatName + "-ID" + TelemetryEntry.Id.ToString("00"))
                .Field("value", TelemetryEntry.Value)
                .Timestamp(TelemetryEntry.Timestamp, WritePrecision.Ns);

            using (var writeApi = _Client.GetWriteApi())
            {
                writeApi.WritePoint(_Bucket, _Organisation, point);
            }

            return true;
        }

        public cInfluxDriver(tInfluxConfig NewConfig)
        {
            var host = NewConfig.Host;
            var port = NewConfig.Port;
            _Token = NewConfig.Token;
            _Bucket = NewConfig.Bucket;
            _Organisation = NewConfig.Organisation;
            var pass = NewConfig.Password;

            if (host is null) host = "http://localhost:";
            if (port is null) port = "8086";
            if (_Token is null || _Token == "") throw new ArgumentException("No token provided. INFLUXDB_TOKEN must be set to the correct token for access to the Influx DB");
            if (_Bucket is null) _Bucket = "telemetry";
            if (_Organisation is null) throw new ArgumentException("No organisation provided. INFLUXDB_ORG must be set to the organisation to use");
            if (pass is null || pass == "") throw new ArgumentException("No password defined. INFLUXDB_PASSWORD must be set to a valid password to configure the Influx DB");

            // Check for default password
            if (pass == "defaultpassword_changeme")
            {
                Log.Log(enLogLevel.Warn, "** SECURITY WARNING ** : The default user password is being used, this should be changed to prevent compromise.");
            }

            // Check for default token
            if (_Token == "defaulttoken_changeme")
            {
                Log.Log(enLogLevel.Warn, "** SECURITY WARNING ** : The default access token is being used, this should be changed to prevent compromise.");
            }

            var sb = new StringBuilder();
            sb.AppendLine("InfluxDB Driver initialised with:");
            sb.AppendLine("Host: " + host);
            sb.AppendLine("Port: " + port);
            sb.AppendLine("Token: " + _Token);
            sb.AppendLine("Bucket: " + _Bucket);
            sb.AppendLine("Organisation: " + _Organisation);

            Log.Log(enLogLevel.Info, sb.ToString());

            _Client = InfluxDBClientFactory.Create(host + ":" + port, _Token.ToCharArray());            
        }
    }
}