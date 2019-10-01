using Shared;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class AsynchronousSocketListener
{
    private readonly ManualResetEvent allDone = new ManualResetEvent(false);

    private Socket SocketListener { get; set; }

    public void StartListening()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP socket.  
        SocketListener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.  
        try
        {
            SocketListener.Bind(localEndPoint);
            SocketListener.Listen(500);

            while (true)
            {
                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for a connection...");
                SocketListener.BeginAccept(new AsyncCallback(AcceptCallback), SocketListener);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();
    }

    private void AcceptCallback(IAsyncResult ar)
    {

        var socketListener = (Socket)ar.AsyncState;
        var socketHandler = socketListener.EndAccept(ar);

        var state = new StateObject
        {
            workSocket = socketHandler
        };
        socketHandler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

        allDone.Set();
    }

    public void ReadCallback(IAsyncResult ar)
    {
        string content = String.Empty;

        var state = (StateObject)ar.AsyncState;
        var socketHandler = state.workSocket;

        int bytesRead = socketHandler.EndReceive(ar);

        if (bytesRead > 0)
        {
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

            content = state.sb.ToString();
            if (content.IndexOf(Constants.EndOfMessage) > -1)
            {
                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                Send(socketHandler, content.ToUpper());
            }
            else
            {
                // Not all data received. Get more
                socketHandler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }
    }

    private void Send(Socket handler, string data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
        handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            var bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}