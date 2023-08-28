using System.Text;
using NetworkLibrary.Shared.Constants;
using NetworkLibrary.Shared.Utils;

namespace NetworkLibrary.Shared
{
    public static class CreateMessage
    {
        public static byte[] Ping() { return CreateMessageWithHeader(SystemOpCodes.PING); }
        public static byte[] Pong() { return CreateMessageWithHeader(SystemOpCodes.PONG); }
        public static byte[] GetAdvertiseServerList() { return CreateMessageWithHeader(SystemOpCodes.ADVERTISE_SERVER_LIST); }
        public static byte[] AdvertiseServerListResponse() { return CreateMessageWithHeader(SystemOpCodes.ADVERTISE_SERVER_LIST_RESPONSE, true); }
        public static byte[] AdvertiseServer(string title = "") { return CreateMessageWithHeader(SystemOpCodes.ADVERTISE_SERVER, title); }
        public static byte[] AdvertiseServerResponse(bool didPing) { return CreateMessageWithHeader(SystemOpCodes.ADVERTISE_SERVER_RESPONSE, didPing); }
        public static byte[] Action(string action = "") { return CreateMessageWithHeader(SystemOpCodes.ACTION, action); }
        public static byte[] Action(byte[] data) { return CreateMessageWithHeader(SystemOpCodes.ACTION, data); }
        public static byte[] ActionResponse() { return CreateMessageWithHeader(SystemOpCodes.ACTION_RESPONSE, true); }

        private static byte[] CreateMessageWithHeader(SystemOpCodes opcode, string message = "") {
            var data = Encoding.ASCII.GetBytes(message);
            return Util.ConcatArrays(BitConverter.GetBytes(data.Length), new byte[] { (byte)opcode }, data);
        }
        private static byte[] CreateMessageWithHeader(SystemOpCodes opcode, bool value)
        {
            return Util.ConcatArrays(BitConverter.GetBytes(1), new byte[] { (byte)opcode, (byte)(value ? 1 : 0) });
        }
        private static byte[] CreateMessageWithHeader(SystemOpCodes opcode, byte[] values)
        {
            return Util.ConcatArrays(BitConverter.GetBytes(1), new byte[] { (byte)opcode }, values);
        }
    }
}
