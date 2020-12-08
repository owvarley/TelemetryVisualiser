using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using Logger;

namespace OpenCosmos
{
    public class cTelemetryClient : iClient
    {
        private TcpClient _Client;
        private readonly string _Host;
        private readonly int _Port;

        private static readonly Logger.iLogger Log = new Logger.cLog4net(typeof(cTelemetryClient));

        private void CheckConnection()
        {
            try
            {
                _Client.GetStream().WriteByte(1);
            }
            catch (System.NullReferenceException)
            {
                // Not fatal to call this without calling Connect first as this checks the status of the connection
                Log.Log(enLogLevel.Error, "Client connection has not been started. Call Connect first.");
            }
            catch (IOException e)
            {
                // Attempted to write to the stream but IO exceptioned indicating that the client has disconnected
                throw new ClientDisconnectedException(e);
            }
        }

        public void Close()
        {
            try
            {
                _Client.Close();
                Log.Log(enLogLevel.Info, "Connection to Client closed.");
            }
            catch (NullReferenceException)
            {
                // Again this isn't fatal, it indicates either that Connect hasn't been called or was called and was unsuccessful and this is 
                // being called to clean up
                Log.Log(enLogLevel.Error, "Client connection has not been started. Call Connect first.");
            }
        }

        public void Connect()
        {
            const int MAX_CONNECTION_ATTEMPTS = 3;
            const int RETRY_PAUSE_S = 60;
            var connection_attempts = 0;
            var connection_successful = false;

            while (connection_attempts <= MAX_CONNECTION_ATTEMPTS && !connection_successful)
            {
                try
                {
                    Log.Log(enLogLevel.Info, "Starting up TCP Client on {0}:{1}", _Host, _Port);

                    connection_attempts++;
                    _Client = new TcpClient(_Host, _Port);
                    Log.Log(enLogLevel.Info, "TCP Client started");

                    this.CheckConnection();
                    Log.Log(enLogLevel.Info, "TCP Client connection Ok.");

                    // Successfully connected
                    connection_successful = true;
                }
                catch (Exception e)
                {
                    if (e is SocketException || e is System.IO.IOException)
                    {
                        if (connection_attempts <= MAX_CONNECTION_ATTEMPTS)
                        {
                            Log.Log(enLogLevel.Warn, "TCP Client failed to start: {0}", e.Message);
                            Log.Log(enLogLevel.Info, "Retrying connection. Attempt {0} of {1}...", connection_attempts, MAX_CONNECTION_ATTEMPTS);
                            Thread.Sleep(RETRY_PAUSE_S);
                        }
                        else
                        {
                            Log.Log(enLogLevel.Error, "TCP Client failed to connect after {0} retry attempts. Exiting..", MAX_CONNECTION_ATTEMPTS);
                            connection_successful = false;
                        }
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }

        public NetworkStream GetStream()
        {
            try
            {
                return _Client.GetStream();
            }
            catch (NullReferenceException)
            {
                // This is a fatal error, we've got to a state where we expect the Client to be connected and valid, however, it isn't
                Log.Log(enLogLevel.Fatal, "Client is not connected, no stream available");
                throw;
            }
        }

        public cTelemetryClient(string NewHost, int NewPort)
        {
            _Host = NewHost;
            _Port = NewPort;

            _Client = null;
        }
    }
}