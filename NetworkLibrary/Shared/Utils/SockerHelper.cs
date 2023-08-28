using System.Net.Sockets;
using NetworkLibrary.Shared.Parsers;

namespace NetworkLibrary.Shared.Utils
{
    public static class SockerHelper
    {
        public static async Task<byte[]?> TcpRequest(byte[] message, string serverIP, int serverPort = 5858, int timeoutMilliseconds = 5000, int maxSize = 1024)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(serverIP, serverPort);
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Socket.SendBufferSize = maxSize;
                        stream.Socket.ReceiveBufferSize = maxSize;
                        stream.ReadTimeout = timeoutMilliseconds;
                        stream.WriteTimeout = timeoutMilliseconds;
                        stream.Write(message, 0, message.Length);
                        stream.Flush();

                        try
                        {
                            SocketMessageParser parser = new SocketMessageParser(stream);
                            return await parser.ReadMessage();
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }
    }
}
