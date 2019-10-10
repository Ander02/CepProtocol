using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Server
    {
        private readonly TcpListener server;

        public Server(IPAddress ip, int port)
        {
            server = new TcpListener(ip, port);
            server.Start();
        }

        public void Start()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    var thread = new Thread(new ParameterizedThreadStart(Handle));
                    thread.Start(client);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}");
                server.Stop();
            }
        }

        private void Handle(object obj)
        {
            var client = obj as TcpClient;
            var stream = client.GetStream();

            string receivedBytes;
            byte[] bytes = new byte[2048];
            int bit;
            try
            {
                while ((bit = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    receivedBytes = Encoding.ASCII.GetString(bytes, 0, bit);
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Received: {receivedBytes}");

                    string responseText = $"Hey Device!, I receive {receivedBytes}";

                    Byte[] responseBytes = Encoding.ASCII.GetBytes(responseText);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Sent: {responseText}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                client.Close();
            }
        }
    }
}
