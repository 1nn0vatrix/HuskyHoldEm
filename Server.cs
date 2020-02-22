using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/**
 * Additional thread related data
 */
public struct ThreadData
{
    public Socket clientSocketDescriptor;
    public int threadId;

    public ThreadData(Socket socket, int thread)
    {
        this.clientSocketDescriptor = socket;
        this.threadId = thread;
    }

    /**
    * Read client data from given socket
    */
    public string ReadFromClient()
    {
        // TODO Replace with dynamic reading 
        byte[] readBuffer = new byte[100];
        int bytesRead = clientSocketDescriptor.Receive(readBuffer);
        return Encoding.ASCII.GetString(readBuffer, 0, bytesRead);

        /*
        byte[] readBuffer = new byte[1024];
        int bytesRead = clientSD.Receive(readBuffer);
        string response = ASCIIEncoding.ASCII.GetString(readBuffer);
        Console.Write("Server received: ");
        for (int i = 0; i < bytesRead; i++)
        {
            Console.Write(Convert.ToChar(readBuffer[i]));
        }
        return response;*/
    }

    /**
     * Writes the given string to the client
     */
    public void WriteToClient(string input)
    {
        byte[] messageBuffer = ASCIIEncoding.ASCII.GetBytes(input);
        Console.WriteLine(input.Length);
        ushort size = (ushort)input.Length;
        byte[] sizeBuffer = new byte[2];
        sizeBuffer[0] = (byte)size;
        sizeBuffer[1] = (byte)(size >> 8);
        clientSocketDescriptor.Send(sizeBuffer);
        clientSocketDescriptor.Send(messageBuffer);
    }

    /**
     * Opening options for the menu
     */
    public void startMenu()
    {
        //---get the incoming data through a network stream---
        Console.WriteLine("Connection accepted from Client(" + Convert.ToString(threadId) + "): " + clientSocketDescriptor.RemoteEndPoint);
        string str = "Welcome to Husky Hold'Em!\n" +
            "  _____\n | A .  | _____\n |  /.\\ || A ^  | _____\n" +
            " | (_._)||  / \\ || A _  | _____\n |   |  ||  \\ / ||  ( ) || A_ _ |\n" +
            " | ____V||   .  || (_'_)|| ( v )|\n         | ____V||   |  ||  \\ / |\n" +
            "                 | ____V||   .  |\n                         | ____V|\n" +
            "\nPlease Pick from the following options:\n1. Register \n2. Join A Game " +
            "\n3. Create a Game \n4. Unregister\n5. Exit";
        WriteToClient(str);
        string option = ReadFromClient();

        // C# Logic breaks if you use the string value. This works. Don't change it.
        /// int option = Convert.ToInt32(opt);
        Console.WriteLine("\nOption = [" + option + "]");

        // case statement is being a pain in the butt
        switch (option)
        {
            case "1":
                Console.WriteLine("Register User Starts Here...");
                // just closing the socket until the function is implemented
                clientSocketDescriptor.Close();
                break;
            case "2":
                Console.WriteLine("Join Game Starts Here...");
                // just closing the socket until the function is implemented
                clientSocketDescriptor.Close();
                break;
            case "3":
                Console.WriteLine("Create Game Starts Here...");
                // just closing the socket until the function is implemented
                clientSocketDescriptor.Close();
                break;
            case "4":
                Console.WriteLine("Unregister User Starts Here...");
                clientSocketDescriptor.Close();
                // just closing the socket until the function is implemented
                break;
            case "5":
                Console.WriteLine("Goodbye!");
                clientSocketDescriptor.Close();
                break;
            default:
                Console.WriteLine("Invalid Option, try again.");
                clientSocketDescriptor.Close();
                break;
        }
    }
}

/**
 * Server Endpoint
 */
public class Server
{
    private const string LOCAL_HOST_IP = "127.0.0.1";

    private List<Socket> RegisteredUsers = new List<Socket>();

    public static void Main(string[] args)
    {
        const int PORT = 8070;

        try
        {
            // initialize socket listener
            TcpListener tcpListener = new TcpListener(IPAddress.Parse(LOCAL_HOST_IP), PORT);
            tcpListener.Start();

            Console.WriteLine($"The server is listening to port {PORT}");
            Console.WriteLine($"Local Endpoint is: {tcpListener.LocalEndpoint}");

            int threadCount = 0;
            while (true)
            {
                Console.WriteLine("Waiting for a connection....");

                // accepting
                Socket clientSD = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSD = tcpListener.AcceptSocket();

                // registeredUsers.add(clientSD.RemoteEndPoint);
                ThreadData client = new ThreadData(clientSD, threadCount++);
                client.startMenu();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.StackTrace}");
        }
    }
}
