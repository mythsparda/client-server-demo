using System.Net.Sockets;

namespace NetworkLibrary.Shared.Models
{
    public class SocketMessageEvent
    {
        public TcpClient? tcpClient = null;
        public byte[]? message = null;
    }
}
