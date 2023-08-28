using NetworkLibrary.Shared;
using NetworkLibrary.Shared.Models;
using NetworkLibrary.Shared.Utils;

namespace NetworkLibrary.Client
{
    public class GameClient
    {
        public event EventHandler<SocketMessageEvent>? MessageReceived;
        private TcpClientWrapper? client = null;

        public bool SendAction(string action)
        {
            if (client == null) return false;
            client.SendAction(action);
            return true;
        }

        public bool IsConnected()
        {
            return client == null ? false : client.IsConnected();
        }

        public async Task<string> GetServerList()
        {
            Console.WriteLine(">[GetAdvertiseServerList]");
            var response = await SockerHelper.TcpRequest(CreateMessage.GetAdvertiseServerList(), "127.0.0.1", 5959);
            if (response == null)
            {
                Console.WriteLine("<[AdvertiseServer] Fetch List Failed");
            }
            else
            {
                Console.WriteLine("<[AdvertiseServer] List: []");
            }
            return "";
        }

        public void Connect(string ip, int port)
        {
            TcpClientWrapper transport = new TcpClientWrapper(ip, port);
            if (transport.IsConnected())
            {
                client = transport;
                transport.MessageReceived += MessageReceived;
            }
        }
    }
}