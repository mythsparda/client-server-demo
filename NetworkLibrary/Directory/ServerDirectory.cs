using System.Net.Sockets;
using System.Net;
using NetworkLibrary.Shared;
using NetworkLibrary.Shared.Parsers;
using NetworkLibrary.Shared.Constants;

namespace NetworkLibrary.Directory
{
    public class ServerDirectory
    {
        private static bool canExit = false;
        public static ManualResetEventSlim waiter = new ManualResetEventSlim(false);
        public void Listen()
        {
            TcpRequestListener("0.0.0.0", 5959).GetAwaiter();
        }

        public void StopListener()
        {
            canExit = true;
        }

        public static async Task TcpRequestListener(string ip = "0.0.0.0", int port = 5858, int timeoutMilliseconds = 5000, int maxSize = 1024)
        {
            while (!canExit)
            {
                TcpListener? listener = null;
                try
                {
                    listener = new TcpListener(IPAddress.Parse(ip), port);
                    listener.Start();
                    Console.WriteLine("Directory listening TCP...");

                    while (!canExit)
                    {
                        try
                        {
                            using (TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false))
                            {
                                using (NetworkStream stream = client.GetStream())
                                {
                                    stream.Socket.SendBufferSize = maxSize;
                                    stream.Socket.ReceiveBufferSize = maxSize;
                                    stream.ReadTimeout = timeoutMilliseconds;
                                    stream.WriteTimeout = timeoutMilliseconds;
                                    SocketMessageParser parser = new SocketMessageParser(stream);
                                    byte[] message = await parser.ReadMessage();
                                    int remotePort = ((IPEndPoint?)client.Client.RemoteEndPoint)?.Port ?? 0;
                                    string remoteIp = ((IPEndPoint?)client.Client.RemoteEndPoint)?.Address.ToString() ?? "";
                                    if (parser.LastOpCode == SystemOpCodes.ADVERTISE_SERVER)
                                    {
                                        var didPing = await Monitoring.TcpPing(remoteIp);
                                        byte[] responseData = CreateMessage.AdvertiseServerResponse(didPing);
                                        await stream.WriteAsync(responseData, 0, responseData.Length);
                                        Console.WriteLine("[" + remoteIp + ":" + remotePort + "] " + (didPing ? "Advertise Server" : "Advertise Server Failed!"));
                                        await stream.FlushAsync();
                                    }
                                    else if (parser.LastOpCode == SystemOpCodes.ADVERTISE_SERVER_LIST)
                                    {
                                        byte[] responseData = CreateMessage.GetAdvertiseServerList();
                                        await stream.WriteAsync(responseData, 0, responseData.Length);
                                        Console.WriteLine("[" + remoteIp + ":" + remotePort + "] GetAdvertiseServerList");
                                        await stream.FlushAsync();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (listener != null) listener.Stop();
                    Console.WriteLine("Error: " + ex.Message);
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
