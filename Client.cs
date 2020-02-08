using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

/**
 * Client Endpoint
 */
public class Client
{
    public static void Main()
    {
        const int PORT = 8070;
        const int MAX_BUFFER_LENGTH = 100;

        try
        {
            Console.WriteLine("Connecting.....");
            
            TcpClient tcpclient = new TcpClient();
            tcpclient.Connect("127.0.0.1", PORT);

            Console.WriteLine("Connected");
            Console.Write("Enter the string to be transmitted: ");

            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] writeBuffer = asciiEncoding.GetBytes(Console.ReadLine());
            
            Console.WriteLine("Transmitting.....");

            Stream stream = tcpclient.GetStream();
            stream.Write(writeBuffer, 0, writeBuffer.Length);

            byte[] readBuffer = new byte[MAX_BUFFER_LENGTH];
            int bytesRead = stream.Read(readBuffer, 0, MAX_BUFFER_LENGTH);

            for (int i = 0; i < bytesRead; i++)
            {
                Console.Write(Convert.ToChar(readBuffer[i]));
            }

            tcpclient.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.StackTrace}");
        }
    }
}