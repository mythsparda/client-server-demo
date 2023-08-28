using NetworkLibrary.Server;
using NetworkLibrary.Shared.Models;
using System.Text;

internal class Program
{
    static void Main(string[] args)
    {
        StartServer();
        (new ManualResetEventSlim(false)).Wait();
    }

    private async static void StartServer()
    {
        GameServer server = new GameServer();
        server.MessageReceived += Server_MessageReceived;

        void Server_MessageReceived(object? sender, SocketMessageEvent e)
        {
            if (e.message == null) return;
            Console.WriteLine("Server_MessageReceived: " + Encoding.ASCII.GetString(e.message));
            server.BroadcastAction("BroadcastAction! " + Encoding.ASCII.GetString(e.message), e.tcpClient);
        }

        server.Start();
        await server.AdvertiseServer("127.0.0.1", 5959);
    }
}
