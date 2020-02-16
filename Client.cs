using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Timers;
/**
 * Client Endpoint
 */
public class Client
{
    const int PORT = 8070;
    const int MAX_BUFFER_LENGTH = 1024;
    /**
     *  Reading from the Server
     */
    public static void readFromServer(TcpClient tc)
    {
        NetworkStream netStream = tc.GetStream();
        byte[] readBuffer = new byte[MAX_BUFFER_LENGTH];
        int bytesRead = netStream.Read(readBuffer, 0, readBuffer.Length);
        for(int i = 0; i < bytesRead; i++)
        {
            Console.Write(Convert.ToChar(readBuffer[i]));
        }
        netStream.Flush();
    }
    /**
     * Writing to the server
     */
    public static void writeToServer(TcpClient tc, string input)
    {
        NetworkStream netStream = tc.GetStream();
        netStream.Write(Encoding.ASCII.GetBytes(input), 0, input.Length);
        netStream.Flush();
    }

    public static void Main()
    {
        try
        {
            Console.WriteLine("Connecting.....");
            TcpClient tcpclient = new TcpClient();
            tcpclient.Connect("127.0.0.1", PORT);
            Console.WriteLine("Connected");
            readFromServer(tcpclient);

            Console.Write("\nEnter an option: ");
            string input = Console.ReadLine();
            Console.WriteLine("Transmitting.....");
            writeToServer(tcpclient, input);

            readFromServer(tcpclient);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.StackTrace}");
        }
    }
}