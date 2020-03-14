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
		public Socket Socket { get; }
		public Server Server { get; }

		public RequestHandler(Socket socket, Server server)
		{
			this.Socket = socket;
			this.Server = server;
		}

		/**
		 * Opening options for the menu
		 */
		public void ProcessRequest()
		{
			//---get the incoming data through a network stream---
			DebugUtils.WriteLine("[SERVER] Connection Accepted: " + Socket.RemoteEndPoint);
			
			bool isActive = true;
			while (isActive)
			{
				Packet packet = ReadPacket(Socket);
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
						WritePacket(Socket, jsonResponse);
						this.Server.activeSockets.Remove(this.Socket);
						Socket.Close();
						isActive = false;
						break;
					default:
						SendError(Socket, packet.Command, "Invalid option, try again.");
						break;
				}
			}
		}

		private void RegisterUser(string username)
		{
			if (string.IsNullOrEmpty(username))
			{
				SendError(Socket, Command.REGISTER_USER, "Invalid Username");
				return;
			} 
			else if (Server.playerMap.TryGetValue(Socket, out Player temp))
			{
				SendError(Socket, Command.REGISTER_USER, "Player already registered.");
				return;
			}

			Player player = new Player(username);
			Server.playerMap.Add(Socket, player);

			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { player }));
			WritePacket(Socket, jsonResponse);
		}

		private void ChangeName(string username)
		{
			if (string.IsNullOrEmpty(username))
			{
				SendError(Socket, Command.CHANGE_NAME, "New username not provided.");
				return;
			}

			bool isPlayerRegistered = Server.playerMap.TryGetValue(Socket, out Player player);
			if (!isPlayerRegistered)
			{
				SendError(Socket, Command.CHANGE_NAME, "Player not registered.");
				return;
			}

			player.Name = username;
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CHANGE_NAME, true, new List<object>() { player }));
			WritePacket(Socket, jsonResponse);
		}

		private void UnregisterUser()
		{
			if (!Server.playerMap.TryGetValue(Socket, out Player player))
			{
				SendError(Socket, Command.UNREGISTER_USER, "Player not registered");
				return;
			}

			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, Server.playerMap.Remove(Socket)));
			WritePacket(Socket, jsonResponse);
		}

		private void ShowGames()
		{
			List<int> availableGames = new List<int>();
			for (int i = 0; i < Server.gameList.Count; i++)
			{
				if (!Server.gameList[i].InProgress && Server.gameList[i].PlayerList.Count < Game.MAX_PLAYERS)
				{
					availableGames.Add(i);
				}
			}
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.SHOW_GAMES, true, new List<object>() { availableGames }));
			WritePacket(Socket, jsonResponse);
		}

		private void JoinGame(int gameIndex)
		{
			if (gameIndex < 0 || gameIndex >= Server.gameList.Count || Server.gameList[gameIndex].InProgress)
			{
				SendError(Socket, Command.JOIN_GAME, "Invalid Parameters.");
				return;
			}

			if (Server.playerMap.TryGetValue(Socket, out Player player))
			{
				Server.gameList[gameIndex].PlayerList.Add(player);
				string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, true));
				WritePacket(Socket, jsonResponse);
				return;
			}

			SendError(Socket, Command.CREATE_GAME, "Player not registered");
		}

		private void CreateGame()
		{
			if (Server.playerMap.TryGetValue(Socket, out Player player))
			{
				Game game = new Game(new List<Player>());
				game.PlayerList.Add(player);
				Server.gameList.Add(game);

				string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CREATE_GAME, true));
				WritePacket(Socket, jsonResponse);
				return;
			}

			SendError(Socket, Command.CREATE_GAME, "Invalid parameters.");
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

				DebugUtils.WriteLine($"[SERVER] Listening to port: {PORT}");
				DebugUtils.WriteLine($"[SERVER] Local Endpoint: {tcpListener.LocalEndpoint}");

				while (true)
				{
					DebugUtils.WriteLine("[SERVER] Waiting for a connection...");

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
