using Shared;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class AsynchronousClient
{
    private const int port = 11000;

    private readonly ManualResetEvent connectDone = new ManualResetEvent(false);
    private readonly ManualResetEvent sendDone = new ManualResetEvent(false);
    private readonly ManualResetEvent receiveDone = new ManualResetEvent(false);

    private Socket Socket { get; set; }

    private string Response { get; set; }

    private void Connect(string hostname)
    {
        connectDone.Reset();
        if (Socket == null || !Socket.Connected)
        {
            var ipAddress = Dns.GetHostEntry(hostname).AddressList.FirstOrDefault();
            var remoteEndPoint = new IPEndPoint(ipAddress, port);

            Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Start a Connection and execute callback when Success
            Socket.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectCallback), Socket);
            connectDone.WaitOne();
        }
    }

    public void StartClient()
    {
        try
        {
            Connect(Dns.GetHostName());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void ConnectCallback(IAsyncResult asyncResult)
    {
        try
        {
            // Retrieve the socket from the state object.  
            var socketclient = (Socket)asyncResult.AsyncState;
            socketclient.EndConnect(asyncResult);

            Console.WriteLine("Socket connected to {0}", socketclient.RemoteEndPoint.ToString());

            connectDone.Set();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }


    public void Send(string message)
    {
        if (!message.Contains(Constants.EndOfMessage))
            message = $"{message}{Constants.EndOfMessage}";

        byte[] messageBytes = Encoding.ASCII.GetBytes(message);

        Socket.BeginSend(messageBytes, 0, messageBytes.Length, 0, new AsyncCallback(SendCallback), Socket);
        sendDone.Set();
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            var client = (Socket)ar.AsyncState;

            int bytesSent = client.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            sendDone.Set();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public string Receive()
    {
        try
        {
            sendDone.WaitOne();
            StateObject state = new StateObject
            {
                workSocket = Socket
            };

            Socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

            receiveDone.WaitOne();

            receiveDone.Reset();
            return Response;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return $"{ex.Message}";
        }
    }

    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            StateObject state = (StateObject)asyncResult.AsyncState;
            Socket client = state.workSocket;

            int bytesRead = client.EndReceive(asyncResult);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            else
            {
                if (state.sb.Length > 1) Response = state.sb.ToString();

                else Response = String.Empty;

                receiveDone.Set();
                sendDone.Reset();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void Close()
    {
        Response = string.Empty;

        sendDone.Reset();
        receiveDone.Reset();
        connectDone.Reset();

        Socket.Shutdown(SocketShutdown.Both);
        Socket.Close();
    }
}