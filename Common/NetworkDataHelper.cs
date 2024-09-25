using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class NetworkDataHelper
    {
        private readonly Socket _socket;

        public NetworkDataHelper(Socket socket)
        {
            _socket = socket;
        }

        public void Send(string message)
        {
            byte[] responseData = Encoding.UTF8.GetBytes(message);
            byte[] responseDataLength = BitConverter.GetBytes(responseData.Length);
            Send(responseDataLength);
            Send(responseData);
        }

        private void Send(byte[] data)
        {
            int offset = 0;
            int size = data.Length;
            while (offset < size)
            {
                int sent = _socket.Send(data, offset, size - offset, SocketFlags.None);
                if (sent == 0)
                {
                    throw new SocketException();
                }
                offset += sent;
            }
        }

        public string Receive()
        {
            byte[] dataLengthBytes = Receive(4);
            int dataLength = BitConverter.ToInt32(dataLengthBytes);
            byte[] dataBytes = Receive(dataLength);
            return Encoding.UTF8.GetString(dataBytes);
        }

        public byte[] Receive(int length)
        {
            byte[] response = new byte[length];
            int offset = 0;
            while (offset < length)
            {
                int received = _socket.Receive(response, offset, length - offset, SocketFlags.None);
                if (received == 0)
                {
                    throw new SocketException();
                }
                offset += received;
            }
            return response;
        }
    }
}