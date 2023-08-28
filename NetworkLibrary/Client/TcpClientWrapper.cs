using System.Net.Sockets;
using NetworkLibrary.Shared;
using NetworkLibrary.Shared.Models;
using NetworkLibrary.Shared.Parsers;

namespace NetworkLibrary.Client
{
    internal class TcpClientWrapper : IDisposable
    {
        public event EventHandler<SocketMessageEvent>? MessageReceived;

        private bool canExit = false;
        private TcpClient? client = null;
        private string ip;
        private int port;
        private int timeoutMilliseconds;
        private int maxSize;
        private Task transport;

        public TcpClientWrapper(string ip = "0.0.0.0", int port = 5858, int timeoutMilliseconds = 5000, int maxSize = 1024)
        {
            this.ip = ip;
            this.port = port;
            this.timeoutMilliseconds = timeoutMilliseconds;
            this.maxSize = maxSize;
            transport = TcpListener();
        }

        private async Task TcpListener()
        {
            Exception exitMessage = new Exception();
            try
            {
                client = new TcpClient();
                client.Connect(ip, port);
                NetworkStream stream = client.GetStream();
                stream.Socket.SendBufferSize = maxSize;
                stream.Socket.ReceiveBufferSize = maxSize;
                stream.ReadTimeout = timeoutMilliseconds;
                stream.WriteTimeout = timeoutMilliseconds;
                try
                {
                    SocketMessageParser parser = new SocketMessageParser(stream);
                    while (!canExit)
                    {
                        byte[] message = await parser.ReadMessage();
                        MessageReceived?.Invoke(this, new SocketMessageEvent { tcpClient = client, message = message });
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        Console.WriteLine("Receive operation timed out.");
                    }
                    else
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw new Exception("Failed to connect", ex);
            }
        }

        public bool IsConnected()
        {
            return client == null ? false : client.Connected;
        }

        public bool SendAction(string action)
        {
            if (client == null) return false;
            NetworkStream stream = client.GetStream();
            var actionMessage = CreateMessage.Action(action);
            stream.Socket.Send(actionMessage);
            return true;
        }

        public void Dispose()
        {
            canExit = true;
            transport.Dispose();
        }
    }
}
