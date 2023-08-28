using NetworkLibrary.Client;
using NetworkLibrary.Shared.Models;
using System.Text;

internal class Program
{
    static void Main(string[] args)
    {
        StartClient();
        (new ManualResetEventSlim(false)).Wait();
    }

    private async static void StartClient()
    {
        GameClient client = new GameClient();
        client.MessageReceived += Server_MessageReceived;

        void Server_MessageReceived(object? sender, SocketMessageEvent e)
        {
            if (e.message == null) return;
            Console.WriteLine("Server_MessageReceived: " + Encoding.ASCII.GetString(e.message));
        }

        Random rnd = new Random();
        int random_number = rnd.Next(1, 100);

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        async void SendMessageOnInterval()
        {
            while (true)
            {
                if (client.IsConnected())
                {
                    var msg = "test" + random_number + ":" + rnd.Next(1, 100);
                    Console.WriteLine(">" + msg);
                    client.SendAction(msg);
                }
                await Task.Delay(2000);
            }
        }

        await client.GetServerList();
        Console.WriteLine("Attempt to connect to the server");
        client.Connect("127.0.0.1", 5858);
        SendMessageOnInterval();
    }
}
