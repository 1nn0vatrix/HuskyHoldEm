using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static HuskyHoldEm.NetworkUtils;

namespace HuskyHoldemServer
{
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
		 * Opening options for the menu
		 */
		public void StartMenu()
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
			WritePacket(clientSocketDescriptor, str);
			string option = ReadPacket(clientSocketDescriptor);
			Console.WriteLine("\nOption = [" + option + "]");

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
					Thread newConnection = new Thread(new ThreadStart(client.StartMenu));
					newConnection.Start();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error: {e.StackTrace}");
			}
		}
	}

}
