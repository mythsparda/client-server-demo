using NetworkLibrary.Shared;
using NetworkLibrary.Shared.Utils;

namespace NetworkLibrary.Directory
{
    internal static class Monitoring
    {
        public async static Task<bool> TcpPing(string serverIP, int serverPort = 5858, int timeoutMilliseconds = 5000, int maxSize = 1024)
        {
            return await SockerHelper.TcpRequest(CreateMessage.Ping(), serverIP, serverPort, timeoutMilliseconds, maxSize) != null;
        }
    }
}
