using System;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Logger;

namespace OpenCosmos
{
    public abstract class cReceiverBase : iReceiver
    {
        private iClient _Client;
        private readonly string _SatName;
        private readonly iDBDriver _DBDriver;
        private bool _IsRunning;

        private static readonly Logger.iLogger Log = new Logger.cLog4net(typeof(cReceiverBase));

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
                catch (InvalidOperationException e) { Log.Log(enLogLevel.Error, e.Message); }
                catch (TelemetryFormatException e) { Log.Log(enLogLevel.Error, e.Message); }
                catch (ArgumentException e) { Log.Log(enLogLevel.Error, e.Message); }
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

        public void Start()
        {
            _IsRunning = true;

            try
            {
                _Client.Connect();

                using (BlockingCollection<byte[]> bc = new BlockingCollection<byte[]>())
                {
                    Log.Log(enLogLevel.Info, "Starting task to Receive frames from Network Stream");
                    var Receive_Task = Task.Run( () => Receiver_Worker(bc) );
                    Log.Log(enLogLevel.Info, "Receiver task started");

                    Log.Log(enLogLevel.Info, "Starting task to Decode frames from receiver");
                    var Decode_Task = Task.Run( () => Decoding_Worker(bc) );
                    Log.Log(enLogLevel.Info, "Decoding task started");

                    Receive_Task.Wait();
                    Decode_Task.Wait();
                }

                Log.Log(enLogLevel.Info, "Receiver and Decoding tasks completed");
            }
            catch (System.AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    if (x is System.IO.IOException || x is SocketException) 
                    {
                        Log.Log(enLogLevel.Error, x.Message);
                        return true;
                    }
                    return false; // Unhandled - exit program via finally
                });
            }
            catch (TelemetryFormatException e)
            {
                Log.Log(enLogLevel.Error, e.Message);
            }
            catch (ClientDisconnectedException)
            {
                Log.Log(enLogLevel.Error, "Client Disconnected unexpectedly. Exiting.");
            }
            catch (System.IO.IOException e)
            {
                Log.Log(enLogLevel.Error, e.Message);
            }
            catch (SocketException e)
            {
                Log.Log(enLogLevel.Error, e.Message);
            }
            finally
            {
                _IsRunning = false;
                _Client.Close();
            }
        }

        public void Stop()
        {
            Log.Log(enLogLevel.Info, "Stopping Receiver and Decoding tasks");
            _IsRunning = false;
        }

        public cReceiverBase(string NewSatName, iDBDriver NewDriver, iClient NewClient)
        {
            _SatName = NewSatName;
            _DBDriver = NewDriver;
            _Client = NewClient;
        }
    }
}