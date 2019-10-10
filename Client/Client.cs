using System;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Client
    {
        private readonly TcpClient client;

        public Client(string server, int serverPort)
        {
            this.client = new TcpClient(server, serverPort);
        }

        public void Send(string message, Action<byte[]> onReceive)
        {
            var stream = this.client.GetStream();

            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
            Console.WriteLine($"Sent: {message}");
            messageBytes = new Byte[2048];

            stream.Read(messageBytes, 0, messageBytes.Length);
            onReceive(messageBytes);
        }

        public void Close()
        {
            client.Close();
            client.GetStream().Close();
        }
    }
}
