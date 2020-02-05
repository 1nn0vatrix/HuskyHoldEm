using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
    public struct threadData
    {
    Socket clientID;
    Thread threadNo;
    String username;
        public threadData(Socket sd, Thread td, String un)
    {
        this.clientID = sd;
        this.threadNo = td;
        this.username = un;
    }
    }  

    public class server
    {
        static void Main(string[] args)
        {
        try
        {
            IPAddress addInfo = IPAddress.Parse("127.0.0.1");

            // initialize the listener
            TcpListener myList = new TcpListener(addInfo, 8070);

            // start listening at the specified port
            myList.Start();

            Console.WriteLine("The server is running at port 8070");
            Console.WriteLine("Local Endpoint is: " + myList.LocalEndpoint);
            Console.WriteLine("Waiting for a connection....");

            // accepting
            Socket clientSocket = myList.AcceptSocket();
            Console.WriteLine("Connection accepted from: " + clientSocket.RemoteEndPoint);
            byte[] buffer = new byte[100];
            int k = clientSocket.Receive(buffer);
            Console.WriteLine("Received...");

            for (int i = 0; i < k; i++)
            {
                Console.WriteLine(Convert.ToChar(buffer[i]));
            }
            ASCIIEncoding asen = new ASCIIEncoding();
            clientSocket.Send(asen.GetBytes("The string was recieved by the server."));
            Console.WriteLine("\nSent Acknowledgement");
            /* clean up */
            clientSocket.Close();
            myList.Stop();

        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }
}
