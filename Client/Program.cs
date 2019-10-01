using System;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var client = new AsynchronousClient();

            client.StartClient();

            client.Send("Message 1");

            var response = client.Receive();

            Console.WriteLine(response);

            client.Send("Message 2");

            var response2 = client.Receive();

            Console.WriteLine(response2);

            client.Close();

            Console.ReadLine();
        }
    }
}
