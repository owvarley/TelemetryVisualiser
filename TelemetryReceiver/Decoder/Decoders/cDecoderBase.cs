using System;
using System.Net.Sockets;

namespace OpenCosmos
{
    public abstract class cDecoderBase : iDecoder
    {
        protected readonly int _BufferSize;
        private readonly int _Port;
        protected readonly iDBDriver _DBDriver;
        protected cTelemetryClient _Client;

        public abstract cTelemetryEntry Decode(byte[] buffer);

        private string GetHost()
        {
            var host = Environment.GetEnvironmentVariable("TELEMETRY_HOST");

            if (host is null)
            {
                Console.WriteLine("No host supplied via TELEMETRY_HOST environment variable, defaulting to localhost");
                host = "localhost";
            }

            return host;
        }

        public byte[] ReadFromStream(NetworkStream ns)
        {
            var buffer = new byte[_BufferSize];
            Int32 bytes = ns.Read(buffer, 0, buffer.Length);

            // If our system architecture is not LE then we need to swap the array to match
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            // Guard against receing nothing from the socket despite it saying there is data available
            if (bytes <= 0) throw new InvalidOperationException("Data is available, however, zero bytes were read from the Socket.");

            // Resize the buffer to just what was read from the stream
            Array.Resize(ref buffer, bytes);

            return buffer;
        }

        private void ReportError(Exception e)
        {
            Console.WriteLine(DateTime.UtcNow.ToString("dd/MM/yy HH:mm:ss") + " " + e.Message);
        }

        public void Start()
        {
            const int WAIT_TIMER_S = 10;

            try
            {
                var host = GetHost();

                _Client = new cTelemetryClient(host, _Port);
                NetworkStream ns = _Client.GetStream();

                Console.WriteLine("Telemetry Client started for {0} on port {1}", host, _Port);

                while (true)
                {
                    _Client.CheckConnection();

                    if (ns.DataAvailable)
                    {
                        try
                        {
                            byte[] buffer = this.ReadFromStream(ns);
                            var telemetryEntry = Decode(buffer);

                            // The validity of the telemetry entry is checked here to ensure that only valid values are
                            // passed down to the relevant database drivers. A non-valid entry can only occur if an error 
                            // occurred when reading from the stream which will be picked up and handled by the above try 
                            // catch. This check ensure protection of the database.
                            if (telemetryEntry.IsValid)
                            {
                                _DBDriver.WriteToDB(telemetryEntry);
                            }
                            else
                            {
                                // If we have somehow managed to get to this state then an implementation of ReadFromStream has been written
                                // in such a way that a null value for telemetryEntry was returned. This shouldn't happen, a correctly written
                                // ReadFromStream will return a valid object or throw an Exception.
                                throw new UnreachableCodeException();
                            }
                        }
                        catch (InvalidOperationException e) { ReportError(e); }
                        catch (TelemetryFormatException e) { ReportError(e); }
                        catch (ArgumentException e) { ReportError(e); }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(WAIT_TIMER_S);
                    }
                }
            }
            catch (TelemetryFormatException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ClientDisconnectedException)
            {
                Console.WriteLine("Client Disconnected unexpectedly. Exiting.");
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                _Client.Close();
            }
        }

        public cDecoderBase(int BufferSize, int NewPort, iDBDriver NewDBDriver)
        {
            _BufferSize = BufferSize;
            _Port = NewPort;
            _DBDriver = NewDBDriver;
        }
    }
}