using System;
using System.Text;
using System.Threading;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Run();
            //RunMultThread();
        }

        private static void Run()
        {
            var client = new Client("127.0.0.1", 13000);

            while (true)
            {
                Console.WriteLine("Write your message");
                var message = Console.ReadLine();

                if (message.Equals("Quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Bye");
                    break;
                }

                client.Send(message, onReceive: (bytes) =>
                {
                    var response = ClientConstants.DefaultEncoding.GetString(bytes);
                    Console.WriteLine($"Received: {response.Trim()}");
                });
            }                                  
            client.Close();
        }

        private static void RunMultThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var client = new Client("127.0.0.1", 13000);

                for (var i = 0; i < 5; i++)
                {
                    client.Send($"Hello, I'm Device 1 sending i = {i}", onReceive: (bytes) =>
                    {
                        var response = Encoding.ASCII.GetString(bytes);
                        Console.WriteLine($"Received: {response}");
                        Thread.Sleep(2000);
                    });
                }
                client.Close();
            }).Start();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var client = new Client("127.0.0.1", 13000);

                for (var i = 0; i < 5; i++)
                {
                    client.Send($"Hello, I'm Device 2 sending i = {i}", onReceive: (bytes) =>
                    {
                        var response = Encoding.ASCII.GetString(bytes);
                        Console.WriteLine($"Received: {response}");
                        Thread.Sleep(2000);
                    });
                }
                client.Close();
            }).Start();
            Console.ReadLine();
        }
    }
}
