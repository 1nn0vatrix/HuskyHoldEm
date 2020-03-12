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
				Console.WriteLine(packet.PacketToString());

				string jsonResponse;
				switch (packet.Command)
				{
					case Command.REGISTER_USER:
						RegisterUser((string)packet.DataList[0]);
						break;
					case Command.CHANGE_NAME:
						ChangeName((string)packet.DataList[0]);
						break;
					case Command.UNREGISTER_USER:
						UnregisterUser();
						break;
					case Command.SHOW_GAMES:
						ShowGames();
						break;
					case Command.JOIN_GAME:
						JoinGame(int.Parse(packet.DataList[0].ToString()));
						break;
					case Command.CREATE_GAME:
						CreateGame();
						break;
					case Command.CLOSE_SOCKET:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CLOSE_SOCKET, true, new List<object>() { "Goodbye!" }));
						WritePacket(socket, jsonResponse);
						this.server.activeSockets.Remove(this.socket);
						socket.Close();
						isActive = false;
						break;
					default:
						SendError(socket, packet.Command, "Invalid option, try again.");
						break;
				}
			}
		}

		private void RegisterUser(string username)
		{
			if (string.IsNullOrEmpty(username))
			{
				SendError(socket, Command.REGISTER_USER, "Invalid Username");
				return;
			} 
			else if (server.playerMap.TryGetValue(socket, out Player temp))
			{
				SendError(socket, Command.REGISTER_USER, "Player already registered.");
				return;
			}

			Player player = new Player(username);
			server.playerMap.Add(socket, player);

			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { player }));
			WritePacket(socket, jsonResponse);
		}

		private void ChangeName(string username)
		{
			if (string.IsNullOrEmpty(username))
			{
				SendError(socket, Command.CHANGE_NAME, "New username not provided.");
				return;
			}

			bool isPlayerRegistered = server.playerMap.TryGetValue(socket, out Player player);
			if (!isPlayerRegistered)
			{
				SendError(socket, Command.CHANGE_NAME, "Player not registered.");
				return;
			}

			player.Name = username;
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CHANGE_NAME, true, new List<object>() { player }));
			WritePacket(socket, jsonResponse);
		}

		private void UnregisterUser()
		{
			if (!server.playerMap.TryGetValue(socket, out Player player))
			{
				SendError(socket, Command.UNREGISTER_USER, "Player not registered");
				return;
			}

			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, server.playerMap.Remove(socket)));
			WritePacket(socket, jsonResponse);
		}

		private void ShowGames()
		{
			List<int> availableGames = new List<int>();
			for (int i = 0; i < server.gameList.Count; i++)
			{
				if (!server.gameList[i].InProgress && server.gameList[i].PlayerList.Count < Game.MAX_PLAYERS)
				{
					availableGames.Add(i);
				}
			}
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.SHOW_GAMES, true, new List<object>() { availableGames }));
			WritePacket(socket, jsonResponse);
		}

		private void JoinGame(int gameIndex)
		{
			if (gameIndex < 0 || gameIndex >= server.gameList.Count || server.gameList[gameIndex].InProgress)
			{
				SendError(socket, Command.JOIN_GAME, "Invalid Parameters.");
				return;
			}

			if (server.playerMap.TryGetValue(socket, out Player player))
			{
				server.gameList[gameIndex].PlayerList.Add(player);
				string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, true));
				WritePacket(socket, jsonResponse);
				return;
			}

			SendError(socket, Command.CREATE_GAME, "Player not registered");
		}

		private void CreateGame()
		{
			if (server.playerMap.TryGetValue(socket, out Player player))
			{
				Game game = new Game(new List<Player>());
				game.PlayerList.Add(player);
				server.gameList.Add(game);

				string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CREATE_GAME, true));
				WritePacket(socket, jsonResponse);
				return;
			}

			SendError(socket, Command.CREATE_GAME, "Invalid parameters.");
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
		public List<Game> gameList = new List<Game>();

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
