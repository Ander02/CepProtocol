using System;
using System.Net;
using System.Threading;
using Server.HttpClient;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new ViaCepClient();

            var cep1 = await client.GetAddressByCep("04849503");
            var cep2 = await client.GetAddressByCep("01001000");
            var cep3 = await client.GetAddressByCep("01001001");

            Console.WriteLine(cep1);
            Console.WriteLine(cep2);
            Console.WriteLine(cep3);


            new Thread(() =>
            {
                var server = new Server(IPAddress.Parse("127.0.0.1"), 13000);
                server.Start();
            }).Start();

            Console.WriteLine("Server Started...!");
        }
    }
}
