﻿using Server.Messages;
using Shared;
using Shared.Messages;
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
                    Console.WriteLine("Receiveid a connection!");
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

            string receivedMessage;
            byte[] bytes = new byte[1 << 16];
            int bit;
            try
            {
                while ((bit = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    receivedMessage = Encoding.UTF8.GetString(bytes, 0, bit);
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Server Received: {receivedMessage}");

                    string responseText = default;
                    try
                    {
                        var messageReader = new MessageReader(Constants.DefaultSeparator);
                        responseText = MessageHandler.Handle(messageReader.Read(receivedMessage)).GetAwaiter().GetResult();
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E);
                        responseText = "Não foi possível ler a mensagem, tente novamente";
                    }

                    Byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Server Sent: {responseText}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Falha na conexão com o cliente");
                Console.WriteLine(ex);
                client.Close();
            }
        }
    }
}
