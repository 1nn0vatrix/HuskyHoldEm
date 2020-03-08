using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Xml.Serialization;
using static HuskyHoldEm.NetworkUtils;

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

		public static void Main()
		{
			try
			{
				Console.WriteLine("Connecting.....");
				TcpClient tcpClient = new TcpClient();
				tcpClient.Connect(LOCAL_HOST_IP, PORT);
				Console.WriteLine("Connected");
				ReadPacket(tcpClient.Client);  // Read the intro message
				while (true)
				{
					Console.Write("\nEnter an option: ");
					string input = Console.ReadLine();
					Console.WriteLine("Transmitting.....");
					WritePacket(tcpClient.Client, input);

					// Get response back from server
					ReadPacket(tcpClient.Client);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error: {e.StackTrace}");
			}
		}
	}
}
