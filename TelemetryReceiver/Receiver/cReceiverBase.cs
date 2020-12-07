using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace OpenCosmos
{
    public abstract class cReceiverBase : iReceiver
    {
        private cTelemetryClient _Client;
        private readonly string _SatName;
        private readonly string _Host;
        private readonly int _Port;
        private readonly iDBDriver _DBDriver;
        private bool _IsRunning;

        private void Decoding_Worker (BlockingCollection<byte[]> bc)
        {
            while (_IsRunning)
            {
                try
                {
                    var tele = DecodeFrame(bc.Take());
                    
                    // The validity of the telemetry entry is checked here to ensure that only valid values are
                    // passed down to the relevant database drivers. A non-valid entry can only occur if an error 
                    // occurred when reading from the stream which will be picked up and handled by the above try 
                    // catch. This check ensure protection of the database.
                    if (tele.IsValid)
                    {
                        _DBDriver.WriteToDB(tele, _SatName);
                    }
                    else
                    {
                        // If we have somehow managed to get to this state then an implementation of DecodeFrame has been written
                        // in such a way that a null value for telemetryEntry was returned. This shouldn't happen, a correctly written
                        // DecodeFrame will return a valid object or throw an Exception.
                        throw new UnreachableCodeException();
                    }
                }
                catch (InvalidOperationException e) { ReportError(e); }
                catch (TelemetryFormatException e) { ReportError(e); }
                catch (ArgumentException e) { ReportError(e); }
            }   
        }

        public abstract cTelemetryEntry DecodeFrame(byte[] frame);

        private void Receiver_Worker (BlockingCollection<byte[]> bc)
        {
            var ns = _Client.GetStream();
            int bytes_read = 0;

            while (_IsRunning)
            {
                var frame = ReadFrameFromStream(ns, ref bytes_read);

                // If our system architecture is not LE then we need to swap the array to match
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(frame);

                // Guard against receiving nothing from the socket despite it saying there is data available
                if (bytes_read <= 0) throw new InvalidOperationException("Data is available, however, zero bytes were read from the Socket.");

                bc.Add(frame);
            }
        }

        public abstract byte[] ReadFrameFromStream(NetworkStream ns, ref int TotalBytesReadFromStream);

        private void ReportError(Exception e)
        {
            Console.WriteLine(DateTime.UtcNow.ToString("dd/MM/yy HH:mm:ss") + " " + e.Message);
        }

        public void Start()
        {
            _IsRunning = true;


            try
            {
                _Client = new cTelemetryClient(_Host, _Port);
                _Client.CheckConnection();

                using (BlockingCollection<byte[]> bc = new BlockingCollection<byte[]>())
                {
                    var Receive_Task = Task.Run( () => Receiver_Worker(bc) );
                    var Decode_Task = Task.Run( () => Decoding_Worker(bc) );

                    Receive_Task.Wait();
                    Decode_Task.Wait();
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
                throw;
            }
            finally
            {
                _Client.Close();
            }
        }

        public void Stop()
        {
            _IsRunning = false;
        }

        public cReceiverBase(string NewSatName, string NewHost, int NewPort, iDBDriver NewDriver)
        {
            _SatName = NewSatName;
            _Host = NewHost;
            _Port = NewPort;
            _DBDriver = NewDriver;
        }
    }
}