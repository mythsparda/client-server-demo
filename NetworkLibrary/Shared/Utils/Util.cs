using System.Net;
using System.Net.Sockets;

namespace NetworkLibrary.Shared.Utils
{
    static class Util
    {
        public static T[] ConcatArrays<T>(params T[][] list)
        {
            var result = new T[list.Sum(a => a.Length)];
            int offset = 0;
            for (int x = 0; x < list.Length; x++)
            {
                list[x].CopyTo(result, offset);
                offset += list[x].Length;
            }
            return result;
        }

        public static string GetTcpClientEndpoint(TcpClient tcpClient)
        {
            const string UNKNOWN_ENDPOINT = "?";
            try
            {
                IPEndPoint? endpoint = (IPEndPoint?)tcpClient.Client.RemoteEndPoint;
                return endpoint == null ? UNKNOWN_ENDPOINT : endpoint.ToString();
            }
            catch
            {
                return UNKNOWN_ENDPOINT;
            }
        }
    }
}
