using System;
using System.Text;
using System.Threading;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
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
            }).Start();
            Console.ReadLine();
        }
    }
}
