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
using System.Collections.Concurrent;
using HuskyHoldemServer;

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
                        RegisterUser();
						break;
					case Command.JOIN_GAME:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, true, new List<object>() { "Join Game" }));
						WritePacket(clientSocketDescriptor, jsonResponse);
						break;
					case Command.CREATE_GAME:
						jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CREATE_GAME, true, new List<object>() { "Create Game" }));
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
        /**
         * Registers the given player
         */
        private void RegisterUser()
        {
            string jsonResponse;
            // if the socket exists in the player map, the user is registered
            if (server.playerMap.ContainsKey(clientSocketDescriptor))
            {
                jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, false, new List<object>() { "User is already registered." }));
                WritePacket(clientSocketDescriptor, jsonResponse);
            }
            // if not prompt for credentials
            jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { "Enter your username: " }));
            WritePacket(clientSocketDescriptor, jsonResponse);

            // read the preferred username
            Packet packet = ReadPacket(clientSocketDescriptor);
            // create a new player
            Player newPlayer = new Player((string)packet.DataList[0]);
            // provides 50 chips upon registering
            newPlayer.Chips += 50;
            Console.WriteLine($"Player chips = { newPlayer.Chips}");
            // add them to the server's player map
            server.playerMap.TryAdd(clientSocketDescriptor, newPlayer);
            // return success
            jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { $"Successfully registered {newPlayer.Name}"+
                $" 50 chips have been added to {newPlayer.Name}'s wallet" }));
            WritePacket(clientSocketDescriptor, jsonResponse);
        }

        /**
         * Unregisters the given player
         */
        private void UnregisterUser(Player player)
        {
            server.playerMap.TryRemove(clientSocketDescriptor, out player);

            string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { $"Successfully unregistered {player.Name}" }));
            WritePacket(clientSocketDescriptor,jsonResponse);
        }

        /**
         * Updates the given player and updates the leaderboard if necessary
         */
  /*      private void UpdateUser(Player player)
        {
            server.playerMap.TryUpdate(clientSocketDescriptor, player, player);
            if (player.Chips > server.leaderChips)
            {
                server.leaderChips = player.Chips;
                server.leaderUsername = player.Name;
            }

            string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.UPDATE_USER, true, new List<object>() { "Successfully updated player" }));
            WritePacket(clientSocketDescriptor,jsonResponse);
        }


        /**
         * Gets a list of sessions
         */
   /*     private void GetSessionList()
        {
            string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.GET_SESSION_LIST, true, new List<object>() { server.sessionList }));
            WritePacket(clientSocketDescriptor,jsonResponse);
        }

        /**
         * Creates a session
         */
   /*     private void CreateSession(Player player)
        {
            Session session = new Session(false, new List<Player>(), -1);
            session.PlayerList.Add(player);
            server.sessionList.Add(session);

            string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.CREATE_GAME, true, new List<object>() { "Successfully created the game." }));
            WritePacket(clientSocketDescriptor,jsonResponse);
        }

        /**
         * Updates the details of a session identified by the given sessionIndex
         */
 /*       private void UpdateSession(int sessionIndex, Session session)
        {
            string jsonResponse;
            if (sessionIndex < 0 || sessionIndex >= server.sessionList.Count)
            {
                jsonResponse = JsonConvert.SerializeObject(new Packet(Command.UPDATE_SESSION, false, new List<object>() { "Invalid session request." }));
            }
            else
            {
                server.sessionList[sessionIndex] = session;
                jsonResponse = JsonConvert.SerializeObject(new Packet(Command.UPDATE_SESSION, false, new List<object>() { "Successfully updated the session." }));
            }

            WritePacket(clientSocketDescriptor, jsonResponse);
        }

        /**
         * Puts the given player into a Session identified by the given sessionIndex
         */
  /*      private void JoinSession(int sessionIndex, Player player)
        {
            string jsonResponse;
            if (sessionIndex < 0 || sessionIndex >= server.sessionList.Count)
            {
                jsonResponse = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, false, new List<object>() { "Invalid session request." }));
            }
            else
            {
                server.sessionList[sessionIndex].PlayerList.Add(player);
                jsonResponse = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, true, new List<object>(){ "Successfully joined the game."}));
            }

            WritePacket(clientSocketDescriptor, jsonResponse);
        }

        /**
         * Gets the leaderboard
         */
        private void GetLeaderboard()
        {
            string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.VIEW_LEADERBOARD, true, new List<object>(){ $"{server.leaderUsername}: {server.leaderChips}" }));
            WritePacket(clientSocketDescriptor, jsonResponse);
        }
    }

}

/**
 * Server Endpoint
 */
public class Server
	{
		public const string LOCAL_HOST_IP = "127.0.0.1";

        public ConcurrentDictionary<Socket, Player> playerMap = new ConcurrentDictionary<Socket, Player>();
        // public List<Session> sessionList = new List<Session>();

        public string leaderUsername;
        public int leaderChips;
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
