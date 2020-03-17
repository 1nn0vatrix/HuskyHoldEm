using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HuskyHoldEm;
using static HuskyHoldEm.NetworkUtils;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;

namespace HuskyHoldemServer
{
	/**
	 * Additional thread related data
	 */
	public class RequestHandler
	{
		public Socket Socket { get; }
		public Server Server { get; }
		public BlockingCollection<Packet> PacketQueue { get; } = new BlockingCollection<Packet>();

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
				Packet packet = null;

				try
				{
					packet = ReadPacket(Socket);
				}
				catch (EndOfStreamException)
				{
					// Client disconnected, remove them from any games.
					if (Server.playerMap.TryGetValue(Socket, out NetworkPlayer player))
					{
						lock (Server.gameList)
						{
							foreach (Game game in Server.gameList)
							{
								if (game.IPlayerList.Contains(player))
								{
									game.RemovePlayer(player);
									game.IPlayerList.Remove(player);
								}
							} 
						}
						Server.playerMap.Remove(Socket);
					}
					isActive = false;
					continue;
				}

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
						CreateGame(int.Parse(packet.DataList[0].ToString()));
						break;
					case Command.VIEW_LEADERBOARD:
						ViewLeaderboard();
						break;
					case Command.CHEAT_CODE:
						CheatCode(int.Parse(packet.DataList[0].ToString()));
						break;
					case Command.CLOSE_SOCKET:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CLOSE_SOCKET, true, new List<object>() { "Goodbye!" }));
						WritePacket(Socket, jsonResponse);
						this.Server.activeSockets.Remove(this.Socket);
						Socket.Close();
						isActive = false;
						break;
					case Command.SEND_MOVE:
						PacketQueue.Add(packet);
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
			else if (Server.playerMap.TryGetValue(Socket, out NetworkPlayer temp))
			{
				SendError(Socket, Command.REGISTER_USER, "Player already registered.");
				return;
			}

			NetworkPlayer player = new NetworkPlayer(username, this);
			Server.playerMap.Add(Socket, player);

			ClientPlayer clientPlayerObject = new ClientPlayer(player.Name);

			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { clientPlayerObject }));
			WritePacket(Socket, jsonResponse);
		}

		private void ChangeName(string username)
		{
			if (string.IsNullOrEmpty(username))
			{
				SendError(Socket, Command.CHANGE_NAME, "New username not provided.");
				return;
			}

			bool isPlayerRegistered = Server.playerMap.TryGetValue(Socket, out NetworkPlayer player);
			if (!isPlayerRegistered)
			{
				SendError(Socket, Command.CHANGE_NAME, "Player not registered.");
				return;
			}

			player.Name = username;
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CHANGE_NAME, true));
			WritePacket(Socket, jsonResponse);
		}

		private void UnregisterUser()
		{
			if (!Server.playerMap.TryGetValue(Socket, out NetworkPlayer player))
			{
				SendError(Socket, Command.UNREGISTER_USER, "Player not registered");
				return;
			}

			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, Server.playerMap.Remove(Socket)));
			WritePacket(Socket, jsonResponse);
		}

		private void ShowGames()
		{
			lock (Server.gameList)
			{
				List<int> availableGames = new List<int>();
				List<string> playerCounts = new List<string>();
				for (int i = 0; i < Server.gameList.Count; i++)
				{
					if (!Server.gameList[i].InProgress && Server.gameList[i].IPlayerList.Count < Server.gameList[i].MaxPlayers)
					{
						availableGames.Add(i);
						playerCounts.Add(Server.gameList[i].IPlayerList.Count + "/" + Server.gameList[i].MaxPlayers);
					}
				}
				string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.SHOW_GAMES, true, new List<object>() { availableGames, playerCounts }));
				WritePacket(Socket, jsonResponse);
			}
		}

		private void JoinGame(int gameIndex)
		{
			lock (Server.gameList)
			{
				if (gameIndex < 0 || gameIndex >= Server.gameList.Count || Server.gameList[gameIndex].InProgress)
				{
					SendError(Socket, Command.JOIN_GAME, "Invalid Parameters.");
					return;
				}

				if (Server.playerMap.TryGetValue(Socket, out NetworkPlayer player))
				{
					Server.gameList[gameIndex].IPlayerList.Add(player);
					string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, true));
					WritePacket(Socket, jsonResponse);

					if (Server.gameList[gameIndex].IPlayerList.Count == Server.gameList[gameIndex].MaxPlayers)
					{
						DebugUtils.WriteLine($"[SERVER] Got {Server.gameList[gameIndex].MaxPlayers} players, starting a game.");
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.START_GAME, true));
						foreach (NetworkPlayer np in Server.gameList[gameIndex].IPlayerList)
						{
							WritePacket(np.RequestHandler.Socket, jsonResponse);
						}
						StartGameThread(Server.gameList[gameIndex]);
					}
					return;
				} 
			}

			SendError(Socket, Command.JOIN_GAME, "Player not registered");
		}

		private void CreateGame(int numberOfPlayers)
		{
			if (Server.playerMap.TryGetValue(Socket, out NetworkPlayer player))
			{
				Game game = new Game(new List<IPlayer>(), numberOfPlayers);
				game.IPlayerList.Add(player);

				lock (Server.gameList)
				{
					Server.gameList.Add(game); 
				}

				string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CREATE_GAME, true));
				WritePacket(Socket, jsonResponse);
				return;
			}

			SendError(Socket, Command.CREATE_GAME, "Invalid parameters.");
		}

		private void ViewLeaderboard()
		{
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.VIEW_LEADERBOARD, true, new List<object>() { Server.biggestWinner }));
			WritePacket(Socket, jsonResponse);
		}

		private void CheatCode(int coins)
		{
			bool success = Server.playerMap.TryGetValue(Socket, out NetworkPlayer networkPlayer);
			if (!success)
			{
				SendError(Socket, Command.CHEAT_CODE, "Invalid Parameters.");
				return;
			}

			if (networkPlayer.Chips < 2)
			{
				networkPlayer.AdjustChips(coins);
			}
			else
			{
				SendError(Socket, Command.CHEAT_CODE, "You need less than two chips for this code to work.");
			}
		}

		private void StartGameThread(Game game)
		{
			game.GameFinished += GameFinished;
			Thread gameThread = new Thread(new ThreadStart(game.StartGame));
			gameThread.IsBackground = true;
			gameThread.Start();
		}

		// Removes the game from the game list and checks if the leaderboard needs to be updated.
		private void GameFinished(Game finishedGame, IPlayer winner)
		{
			lock (Server.gameList)
			{
				DebugUtils.WriteLine($"[SERVER] GameId {Server.gameList.IndexOf(finishedGame)} finished.");
				if (winner.Chips > Server.biggestWinner.Value)
				{
					Server.biggestWinner = new KeyValuePair<string, int>(winner.Name, winner.Chips);
				}
				Server.gameList.Remove(finishedGame);
			}
		}
	}

	/**
	 * Server Endpoint
	 */
	public class Server
	{
		private const string LOCAL_HOST_IP = "127.0.0.1";
		private const int PORT = 26795;

		public List<Socket> activeSockets = new List<Socket>();
		public Dictionary<Socket, NetworkPlayer> playerMap = new Dictionary<Socket, NetworkPlayer>();
		public List<Game> gameList = new List<Game>();

		public KeyValuePair<string, int> biggestWinner = new KeyValuePair<string, int>("no one", 0);

		public void Run()
		{
			try
			{
				// initialize socket listener
				TcpListener tcpListener = new TcpListener(IPAddress.Any, PORT);
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
