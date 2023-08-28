using System.Net.Sockets;
using System.Net;
using NetworkLibrary.Shared;
using NetworkLibrary.Shared.Models;
using NetworkLibrary.Shared.Utils;
using NetworkLibrary.Shared.Parsers;
using NetworkLibrary.Shared.Constants;

namespace NetworkLibrary.Server
{
    internal class TcpServerWrapper : IDisposable
    {
        public event EventHandler<SocketMessageEvent>? MessageReceived;
        public event EventHandler<TcpServerWrapper>? TransportClosed;
        public ManualResetEventSlim IsReady = new ManualResetEventSlim(false);

        private bool canExit = false;
        private TcpListener? listener = null;
        private string ip;
        private int port;
        private int timeoutMilliseconds;
        private int maxSize;
        private List<TcpClient> clients = new List<TcpClient>();
        private Task transport;

        public TcpServerWrapper(string ip = "0.0.0.0", int port = 5858, int timeoutMilliseconds = 5000, int maxSize = 1024)
        {
            this.ip = ip;
            this.port = port;
            this.timeoutMilliseconds = timeoutMilliseconds;
            this.maxSize = maxSize;
            transport = TcpListener();
        }

        async void ReadMessagesAsync(TcpClient tcpClient)
        {
            try
            {
                string remoteEndPoint = Util.GetTcpClientEndpoint(tcpClient);
                NetworkStream? stream = tcpClient.GetStream();
                SocketMessageParser parser = new SocketMessageParser(stream);
                while (!canExit && stream != null)
                {
                    byte[] message = await parser.ReadMessage();
                    if (parser.LastOpCode == SystemOpCodes.PING)
                    {
                        Console.WriteLine("[" + remoteEndPoint + "] <Ping");
                        byte[] responseData = CreateMessage.Pong();
                        await stream.WriteAsync(responseData, 0, responseData.Length);
                        await stream.FlushAsync().ConfigureAwait(false);
                        Console.WriteLine("[" + remoteEndPoint + "] >Pong");
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                        clients.Remove(tcpClient);
                    }
                    else
                    {
                        MessageReceived?.Invoke(this, new SocketMessageEvent { tcpClient = tcpClient, message = message });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task TcpListener()
        {
            while (!canExit)
            {
                try
                {
                    listener = new TcpListener(IPAddress.Parse(ip), port);
                    listener.Start();
                    Console.WriteLine("Server listening TCP...");
                    IsReady.Set();

                    while (!canExit)
                    {
                        TcpClient tcpClient = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                        clients.Add(tcpClient);
                        NetworkStream? stream = tcpClient.GetStream();
                        stream.Socket.SendBufferSize = maxSize;
                        stream.Socket.ReceiveBufferSize = maxSize;
                        stream.ReadTimeout = timeoutMilliseconds;
                        stream.WriteTimeout = timeoutMilliseconds;
                        string remoteEndPoint = Util.GetTcpClientEndpoint(tcpClient);
                        Console.WriteLine("[" + remoteEndPoint + "] Connected");
                        var cancellationTokenSource = new CancellationTokenSource();
                        var cancellationToken = cancellationTokenSource.Token;

                        Task task = Task.Run(() => ReadMessagesAsync(tcpClient), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    TransportClosed?.Invoke(this, this);
                    Console.WriteLine("Error: " + ex.Message);
                    foreach (var client in clients)
                    {
                        client.Dispose();
                    }
                    clients.Clear();
                    if (listener != null)
                    {
                        listener.Stop();
                        listener = null;
                    }
                    Thread.Sleep(3000);
                }
            }
        }

        public void BroascastAction(string action, TcpClient? omitTcpClient = null)
        {
            var actionMessage = CreateMessage.Action(action);
            for (int i = clients.Count - 1; i >= 0; i--)
            {
                if (clients[i] == omitTcpClient) continue;
                var client = clients[i];
                try
                {
                    client.GetStream().Socket.SendAsync(actionMessage);
                }
                catch (Exception ex)
                {
                    if (ex.HResult != 2146233079)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    clients.Remove(client);
                }
            }
        }

        public void Dispose()
        {
            canExit = true;
            transport.Dispose();
        }
    }
}
