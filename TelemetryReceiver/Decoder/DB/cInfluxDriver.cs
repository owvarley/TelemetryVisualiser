using System;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;

namespace OpenCosmos
{
    public class cInfluxDriver : iDBDriver
    {
        private InfluxDBClient _Client;
        private readonly string _Token;
        private readonly string _Bucket;
        private readonly string _Organisation;

        public bool WriteToDB(cTelemetryEntry telemetryEntry)
        {
            var point = PointData
                .Measurement("mem")
                .Tag("host", "host1-" + telemetryEntry.Id)
                .Field("value", telemetryEntry.Value)
                .Timestamp(telemetryEntry.Timestamp, WritePrecision.Ns);

            using (var writeApi = _Client.GetWriteApi())
            {
                writeApi.WritePoint(_Bucket, _Organisation, point);
            }

            return true;
        }

        public cInfluxDriver()
        {
            var host = Environment.GetEnvironmentVariable("INFLUXDB_HOST");
            var port = Environment.GetEnvironmentVariable("INFLUXDB_PORT");
            _Token = Environment.GetEnvironmentVariable("INFLUXDB_TOKEN");
            _Bucket = Environment.GetEnvironmentVariable("INFLUXDB_BUCKET");
            _Organisation = Environment.GetEnvironmentVariable("INFLUXDB_ORG");

            if (host is null) host = "http://localhost:";
            if (port is null) port = "8086";
            if (_Token is null) throw new ArgumentException("No token provided. INFLUXDB_TOKEN must be set to the correct token for access to the Influx DB");
            if (_Bucket is null) _Bucket = "telemetry";
            if (_Organisation is null) throw new ArgumentException("No organisation provided. INFLUXDB_ORG must be set to the organisation to use");

            Console.WriteLine("InfluxDB Driver initialised with:");
            Console.WriteLine("Host: " + host);
            Console.WriteLine("Port: " + port);
            Console.WriteLine("Token: " + _Token);
            Console.WriteLine("Bucket: " + _Bucket);
            Console.WriteLine("Organisation: " + _Organisation);

            _Client = InfluxDBClientFactory.Create(host + ":" + port, _Token.ToCharArray());            
        }
    }
}