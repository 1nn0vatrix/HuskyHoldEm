using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static HuskyHoldEm.NetworkUtils;
using Newtonsoft.Json;
using HuskyHoldEm;

namespace HuskyHoldemServer
{
	/**
	 * Additional thread related data
	 */
	public class ClientThread
	{
		public Socket clientSocketDescriptor;
		public int threadId;
		public Server server;

		public ClientThread(Socket socket, int thread, Server s)
		{
			this.clientSocketDescriptor = socket;
			this.threadId = thread;
			server = s;
		}

		/**
		 * Opening options for the menu
		 */
		public void StartMenu()
		{
			//---get the incoming data through a network stream---
			Console.WriteLine("Connection accepted from Client(" + Convert.ToString(threadId) + "): " + clientSocketDescriptor.RemoteEndPoint);
			string jsonResponse = string.Empty;
			bool isActive = true;
			while (isActive)
			{
				Packet packet = ReadPacket(clientSocketDescriptor);

				switch (packet.Command)
				{
					case Command.REGISTER_USER:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { "Register User" }));
						WritePacket(clientSocketDescriptor, jsonResponse);
						break;
					case Command.JOIN_GAME:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, true, new List<object>() { "Join Game" }));
						WritePacket(clientSocketDescriptor, jsonResponse);
						break;
					case Command.CREATE_GAME:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CREATE_GAME, true, new List<object>() { "Crate Game" }));
						WritePacket(clientSocketDescriptor, jsonResponse);
						break;
					case Command.UNREGISTER_USER:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, true, new List<object>() { "Deactivate User" }));
						WritePacket(clientSocketDescriptor, jsonResponse);
						break;
					case Command.CLOSE_SOCKET:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CLOSE_SOCKET, true, new List<object>() { "Goodbye!" }));
						WritePacket(clientSocketDescriptor, jsonResponse);
						clientSocketDescriptor.Close();
						isActive = false;
						break;
					default:
						jsonResponse = JsonConvert.SerializeObject(new Packet(packet.Command, false, new List<object>() { "Invalid option, try again." }));
						WritePacket(clientSocketDescriptor, jsonResponse);
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
					ClientThread client = new ClientThread(clientSD, threadCount++, this);
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
