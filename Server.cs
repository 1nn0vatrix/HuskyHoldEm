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
    public Socket clientSD;
    public int ThreadID;

    public ThreadData(Socket socket, int thread)
    {
        this.clientSD = socket;
        this.ThreadID = thread;
    }
    public string readFromClient(Socket tc)
    {
        byte[] readBuffer = new byte[1024];
        int bytesRead = clientSD.Receive(readBuffer);
        string response = ASCIIEncoding.ASCII.GetString(readBuffer);
        Console.WriteLine("Server received: ");
        for (int i = 0; i < bytesRead; i++)
        {
            Console.Write(Convert.ToChar(readBuffer[i]));
        }
        return response;
    }
    /**
     * Writing to the Client
     */
    public void writeToClient(string input)
    {
        ASCIIEncoding asen = new ASCIIEncoding();
        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(input);
        clientSD.Send(bytesToSend);
    }
    public void startMenu()
    {
        //---get the incoming data through a network stream---
        Console.WriteLine("Connection accepted from Client(" + Convert.ToString(ThreadID) + "): " + clientSD.RemoteEndPoint);
        string str = "Welcome to Husky Hold'Em!\n"+
            "  _____\n | A .  | _____\n |  /.\\ || A ^  | _____\n" +
            " | (_._)||  / \\ || A _  | _____\n |   |  ||  \\ / ||  ( ) || A_ _ |\n" +
            " | ____V||   .  || (_'_)|| ( v )|\n         | ____V||   |  ||  \\ / |\n" +
            "                 | ____V||   .  |\n                         | ____V|\n" +
            "\nPlease Pick from the following options:\n1. Register \n2. Join A Game "+
            "\n3. Create a Game \n4. Unregister\n5. Exit";
        writeToClient(str);
        string option = readFromClient(clientSD);
        Console.WriteLine("Option = " + option);
        // case statement is being a pain in the butt.
        switch (option)
        {
            case "1":
                Console.WriteLine("Register User Starts Here...");
                Thread.Sleep(5000);
                clientSD.Close();
                break;
            case "2":
                Console.WriteLine("Join Game Starts Here...");
                Thread.Sleep(5000);
                clientSD.Close();
                break;
            case "3":
                Console.WriteLine("Create Game Starts Here...");
                Thread.Sleep(5000);
                clientSD.Close();
                break;
            case "4":
                Console.WriteLine("Unregister User Starts Here...");
                Thread.Sleep(5000);
                clientSD.Close();
                break;
            case "5":
                Console.WriteLine("Goodbye!");
                Thread.Sleep(5000);
                clientSD.Close();
                break;
            default:
                Console.WriteLine("Invalid Option, try again.");
                Thread.Sleep(5000);
                break;
        }
    }
}

/**
 * Server Endpoint
 */
public class Server
{
    // List<Client> RegisteredUsers = new List<Client>();
    public static void Main(string[] args)
    {
        const int PORT = 8070;
        try
        {
            // initialize socket listener
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), PORT);
            tcpListener.Start();

            Console.WriteLine($"The server is listening to port {PORT}");
            Console.WriteLine($"Local Endpoint is: {tcpListener.LocalEndpoint}");
            int threadCount = 0;
            while (true)
            {
                Console.WriteLine("Waiting for a connection....");
                threadCount++;
                // accepting
                Socket clientSD = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSD = tcpListener.AcceptSocket();
                // registeredUsers.add(clientSD.RemoteEndPoint);
                ThreadData client = new ThreadData(clientSD, threadCount);
                client.startMenu();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.StackTrace}");
        }
    }
}
