using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using static HuskyHoldEm.NetworkUtils;
using Newtonsoft.Json;
using HuskyHoldEm;

namespace HuskyHoldemServer
{
	/**
	 * Additional thread related data
	 */
	public class RequestHandler
	{
		public Socket socket;
		public Server server;

		public RequestHandler(Socket socket, Server server)
		{
			this.socket = socket;
			this.server = server;
		}

		/**
		 * Opening options for the menu
		 */
		public void ProcessRequest()
		{
			//---get the incoming data through a network stream---
			Console.WriteLine("Connection accepted from Client: " + socket.RemoteEndPoint);
			
			bool isActive = true;
			while (isActive)
			{
				Packet packet = ReadPacket(socket);

				string jsonResponse;
				switch (packet.Command)
				{
					case Command.REGISTER_USER:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { "Register User" }));
						WritePacket(socket, jsonResponse);
						break;
					case Command.JOIN_GAME:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, true, new List<object>() { "Join Game" }));
						WritePacket(socket, jsonResponse);
						break;
					case Command.CREATE_GAME:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CREATE_GAME, true, new List<object>() { "Crate Game" }));
						WritePacket(socket, jsonResponse);
						break;
					case Command.UNREGISTER_USER:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, true, new List<object>() { "Deactivate User" }));
						WritePacket(socket, jsonResponse);
						break;
					case Command.CLOSE_SOCKET:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CLOSE_SOCKET, true, new List<object>() { "Goodbye!" }));
						WritePacket(socket, jsonResponse);
						this.server.activeSockets.Remove(this.socket);
						socket.Close();
						isActive = false;
						break;
					default:
						jsonResponse = JsonConvert.SerializeObject(new Packet(packet.Command, false, new List<object>() { "Invalid option, try again." }));
						WritePacket(socket, jsonResponse);
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
		private const int PORT = 8070;

		public List<Socket> activeSockets = new List<Socket>();

		public void Run()
		{
			try
			{
				// initialize socket listener
				TcpListener tcpListener = new TcpListener(IPAddress.Parse(LOCAL_HOST_IP), PORT);
				tcpListener.Start();

				Console.WriteLine($"The server is listening to port {PORT}");
				Console.WriteLine($"Local Endpoint is: {tcpListener.LocalEndpoint}");

				while (true)
				{
					Console.WriteLine("Waiting for a connection....");

					// accept a new socket connection
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket = tcpListener.AcceptSocket();
					activeSockets.Add(socket);
					
					// delegate thread to handle socket connection
					RequestHandler requestHandler = new RequestHandler(socket, this);
					Thread newConnection = new Thread(new ThreadStart(requestHandler.ProcessRequest));
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
