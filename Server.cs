using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/**
 * Additional thread related data
 */
public struct ThreadData
{
    public Socket socket;
    public Thread thread;
    public String username;
    
    public threadData(Socket socket, Thread thread, String username)
    {
        this.socket = socket;
        this.thread = thread;
        this.username = username;
    }
}

/**
 * Server Endpoint
 */
public class Server
{
    public static void Main(string[] args)
    {
        const int PORT = 8070;
        const int MAX_BUFFER_LENGTH = 100;

        try
        {
            // initialize socket listener
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), PORT);
            tcpListener.Start();

            Console.WriteLine($"The server is listening to port {PORT}");
            Console.WriteLine($"Local Endpoint is: {tcpListener.LocalEndpoint}");
            Console.WriteLine("Waiting for a connection.");

            // accept socket connection
            Socket socket = tcpListener.AcceptSocket();
            Console.WriteLine($"Connection accepted from: {socket.RemoteEndPoint}");
            
            byte[] buffer = new byte[MAX_BUFFER_LENGTH];
            
            // store read bytes from socket connection into buffer
            int bytesRead = socket.Receive(buffer);

            // print received data
            Console.WriteLine($"Received {bytesRead} bytes");
            for (int i = 0; i < bytesRead; i++)
            {
                Console.WriteLine(Convert.ToChar(buffer[i]));
            }

            // send acknowledgement response
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            socket.Send(asciiEncoding.GetBytes("The string was received by the server."));

            Console.WriteLine();
            Console.WriteLine("Sent Acknowledgement");
            
            // clean up
            socket.Close();
            tcpListener.Stop();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.StackTrace}");
        }
    }
}
