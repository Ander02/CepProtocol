namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var listener = new AsynchronousSocketListener();

            listener.StartListening();
        }
    }
}
