using System.Net.Sockets;
using NetworkLibrary.Shared.Constants;
using NetworkLibrary.Shared.Utils;

namespace NetworkLibrary.Shared.Parsers
{
    public class SocketMessageParser
    {
        private const int INT_BYTE_SIZE = 4;
        private NetworkStream _stream;
        private int _maxSize;
        private int? _lenght = null;
        private SystemOpCodes _lastOpCodes = SystemOpCodes.PING;
        private int _bufferSize = 0;
        private List<byte[]> _buffer = new List<byte[]>();

        public SocketMessageParser(NetworkStream stream, int maxSize = -1)
        {
            _stream = stream;
            _maxSize = maxSize;
        }

        public SystemOpCodes LastOpCode { get { return _lastOpCodes; } }

        public async Task<byte[]> ReadMessage()
        {
            byte[] buffer = new byte[_lenght.HasValue ? _lenght.Value : INT_BYTE_SIZE + 1];
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            if (_lenght.HasValue)
            {
                if (bytesRead == _lenght.Value + _bufferSize)
                {
                    _buffer.Add(buffer);
                    var data = Util.ConcatArrays(_buffer.ToArray());
                    _bufferSize = 0;
                    _buffer.Clear();
                    _lenght = null;
                    return data;
                }
                else
                {
                    _buffer.Add(buffer);
                    _bufferSize += buffer.Length;
                }
            }
            else
            {
                if (bytesRead != INT_BYTE_SIZE + 1) throw new Exception("Failed to read 5 bytes. " + bytesRead);
                _lenght = BitConverter.ToInt32(buffer);
                _lastOpCodes = (SystemOpCodes)buffer[4];
                if (_lenght > _maxSize && _maxSize != -1) throw new Exception("Message size bigger than maxSize. " + _lenght + " > " + _maxSize);
                if (_lenght.Value == 0) return new byte[0];
                return await ReadMessage();
            }
            return await ReadMessage();
        }
    }
}
