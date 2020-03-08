using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace HuskyHoldemClient
{
	/**
	 * Client Endpoint
	 */
	public class Client
	{
		const string LOCAL_HOST_IP = "127.0.0.1";
		const int PORT = 8070;
		const int STD_BUFFER_LENGTH = 100;

		/**
		 *  Reading from the Server
		 */
		public static void readFromServer(TcpClient tc)
		{
			NetworkStream networkStream = tc.GetStream();
			byte[] sizeBuffer = new byte[2];
			int bytes = networkStream.Read(sizeBuffer, 0, 2);
			ushort size = BitConverter.ToUInt16(sizeBuffer, 0);
			Console.WriteLine("size = " + size);
			byte[] readBuffer = new byte[size];
			int bytesRead = networkStream.Read(readBuffer, 0, readBuffer.Length);
			for (int i = 0; i < bytesRead; i++)
			{
				Console.Write(Convert.ToChar(readBuffer[i]));
			}
			networkStream.Flush();
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
				tcpclient.Connect(LOCAL_HOST_IP, PORT);
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
}
