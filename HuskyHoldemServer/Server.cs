using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HuskyHoldEm;
using static HuskyHoldEm.NetworkUtils;
using Newtonsoft.Json;

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
			Console.WriteLine("[SERVER] Connection Accepted: " + socket.RemoteEndPoint);
			
			bool isActive = true;
			while (isActive)
			{
				Packet packet = ReadPacket(socket);

				string jsonResponse;
				switch (packet.Command)
				{
					case Command.REGISTER_USER:
						RegisterUser(packet.DataList);
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
						UnregisterUser(packet.DataList);
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

		private void RegisterUser(List<object> dataList)
		{
			string registerUserResponse;
			string username = dataList[0].ToString().Trim();
			if (string.IsNullOrEmpty(username))
			{
				registerUserResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, false, new List<object>() { "Invalid username." }));
			} 
			else if (server.playerMap.TryGetValue(socket, out Player temp))
			{
				registerUserResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, false, new List<object>() { "Player already registered." }));
			}
			else
			{
				Player player = new Player(username);
				server.playerMap.Add(socket, player);
				registerUserResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { player }));
			}

			WritePacket(socket, registerUserResponse);
		}

		private void UnregisterUser(List<object> dataList)
		{
			string unregisterUserResponse;
			if (!server.playerMap.TryGetValue(socket, out Player player))
			{
				unregisterUserResponse = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, false, new List<object>() { "Player not registered." }));
			}
			else
			{
				unregisterUserResponse = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, server.playerMap.Remove(socket)));
			}

			WritePacket(socket, unregisterUserResponse);
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
		public Dictionary<Socket, Player> playerMap = new Dictionary<Socket, Player>();

		public void Run()
		{
			try
			{
				// initialize socket listener
				TcpListener tcpListener = new TcpListener(IPAddress.Parse(LOCAL_HOST_IP), PORT);
				tcpListener.Start();

				Console.WriteLine($"[SERVER] Listening to port: {PORT}");
				Console.WriteLine($"[SERVER] Local Endpoint: {tcpListener.LocalEndpoint}");

				while (true)
				{
					Console.WriteLine("[SERVER] Waiting for a connection...");

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
