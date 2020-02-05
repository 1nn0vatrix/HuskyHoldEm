using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;


public class client
{

    public static void Main()
    {

        try
        {
            TcpClient tcpclient = new TcpClient();
            Console.WriteLine("Connecting.....");

            tcpclient.Connect("127.0.0.1", 8070);
            // use the ipaddress as in the server program

            Console.WriteLine("Connected");
            Console.Write("Enter the string to be transmitted : ");

            String str = Console.ReadLine();
            Stream stm = tcpclient.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] sendStream = asen.GetBytes(str);
            Console.WriteLine("Transmitting.....");

            stm.Write(sendStream, 0, sendStream.Length);

            byte[] byteBuffer = new byte[100];
            int k = stm.Read(byteBuffer, 0, 100);

            for (int i = 0; i < k; i++)
                Console.Write(Convert.ToChar(byteBuffer[i]));

            tcpclient.Close();
        }

        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }

}