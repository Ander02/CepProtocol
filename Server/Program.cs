using System;
using System.Net;
using System.Threading;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Thread(() =>
            {
                var server = new Server(IPAddress.Parse("127.0.0.1"), 13000);
                server.Start();
            }).Start();

            Console.WriteLine("Server Started...!");
        }
    }
}
