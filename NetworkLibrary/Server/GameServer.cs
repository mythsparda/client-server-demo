using NetworkLibrary.Shared;
using NetworkLibrary.Shared.Models;
using NetworkLibrary.Shared.Utils;
using System.Net.Sockets;

namespace NetworkLibrary.Server
{
    public class GameServer
    {
        public event EventHandler<SocketMessageEvent>? MessageReceived;
        private TcpServerWrapper? _transport = null;

        public void Start()
        {
            TcpServerWrapper transport = new TcpServerWrapper();
            _transport = transport;
            transport.MessageReceived += MessageReceived;
            transport.TransportClosed += TransportClosed;
            _transport.IsReady.Wait();
        }

        public async Task AdvertiseServer(string ip = "", int port = 0)
        {
            Console.WriteLine(">[AdvertiseServer]");
            var response = await SockerHelper.TcpRequest(CreateMessage.AdvertiseServer(), ip, port);
            if (response == null || response[0] != 1)
            {
                Console.WriteLine("<[AdvertiseServer] Failed");
            }
            else
            {
                Console.WriteLine("<[AdvertiseServer] Success");
            }
        }

        public void BroadcastAction(string action, TcpClient? omitTcpClient = null)
        {
            if (_transport == null) return;
            _transport.BroascastAction(action, omitTcpClient);
        }

        private void TransportClosed(object? sender, TcpServerWrapper e)
        {
            Console.WriteLine("TransportClosed");
        }
    }
}