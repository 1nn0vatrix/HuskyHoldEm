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
			string message = "Welcome to Husky Hold'Em!\n" +
				"  _____\n | A .  | _____\n |  /.\\ || A ^  | _____\n" +
				" | (_._)||  / \\ || A _  | _____\n |   |  ||  \\ / ||  ( ) || A_ _ |\n" +
				" | ____V||   .  || (_'_)|| ( v )|\n         | ____V||   |  ||  \\ / |\n" +
				"                 | ____V||   .  |\n                         | ____V|\n" +
				"\nPlease Pick from the following options:\n1. Register \n2. Join A Game " +
				"\n3. Create a Game \n4. Unregister\n5. Exit";
			WritePacket(clientSocketDescriptor, message);
			bool isActive = true;
			while (isActive)
			{
				string option = ReadPacket(clientSocketDescriptor);
				Console.WriteLine("\nOption = [" + option + "]");

				switch (option)
				{
					case "1":
						message = "Register User Starts Here...";
						message += "\n[Not implemented yet!]";
						Console.WriteLine(message);
						WritePacket(clientSocketDescriptor, message);
						break;
					case "2":
						message = "Join Game Starts Here...";
						message += "\n[Not implemented yet!]";
						Console.WriteLine(message);
						WritePacket(clientSocketDescriptor, message);
						break;
					case "3":
						message = "Create Game Starts Here...";
						message += "\n[Not implemented yet!]";
						Console.WriteLine(message);
						WritePacket(clientSocketDescriptor, message);
						break;
					case "4":
						message = "Unregister User Starts Here...";
						message += "\n[Not implemented yet!]";
						Console.WriteLine(message);
						WritePacket(clientSocketDescriptor, message);
						break;
					case "5":
						message = "Goodbye!";
						Console.WriteLine(message);
						WritePacket(clientSocketDescriptor, message);
						clientSocketDescriptor.Close();
						isActive = false;
						break;
					case "6":
						message = "Chat message:";
						Console.WriteLine(message);
						WritePacket(clientSocketDescriptor, message);
						break;
					default:
						Console.WriteLine("Invalid Option, try again.");
						break;
				}
			}
		}
	}

	/**
	 * Server Endpoint
	 */
	public class Server
	{
		private const string LOCAL_HOST_IP = "127.0.0.1";

		private List<Socket> registeredUsers = new List<Socket>();

		public void Run()
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
					registeredUsers.Add(clientSD);
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
