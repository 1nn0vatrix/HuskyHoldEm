using System;
using System.Collections.Generic;
using System.Net.Sockets;
using HuskyHoldEm;
using static HuskyHoldEm.NetworkUtils;
using Newtonsoft.Json;

namespace HuskyHoldemClient
{
	/**
	 * Client Endpoint
	 */
	public class Client
	{
		private const string LOCAL_HOST_IP = "127.0.0.1";
		private const int PORT = 8070;
		private Socket socket;

		private const string MENU = "Welcome to Husky Hold'Em!\n" +
			"  _____\n" +
			" | A .  | _____\n" +
			" |  /.\\ || A ^  | _____\n" + 
			" | (_._)||  / \\ || A _  | _____\n" +
			" |   |  ||  \\ / ||  ( ) || A_ _ |\n" + 
			" | ____V||   .  || (_'_)|| ( v )|\n" +
			"         | ____V||   |  ||  \\ / |\n" + 
			"                 | ____V||   .  |\n" +
			"                         | ____V|\n\n"
			+ "Please Pick from the following options:\n"
			+ "1. Register\n"
			+ "2. Change Username\n"
			+ "3. Unregister\n"
			+ "4. Show Games\n"
			+ "5. Join Game\n"
			+ "6. Create Game\n"
			+ "0. Exit\n";

		Player Player { get; set; }

		public void Run()
		{
			try
			{
				DebugUtils.WriteLine("[CLIENT] Connecting to server...");

				// connect to the server
				TcpClient tcpClient = new TcpClient();
				tcpClient.Connect(LOCAL_HOST_IP, PORT);
				socket = tcpClient.Client;
				DebugUtils.WriteLine("[CLIENT] Connection accepted");

				Console.WriteLine(MENU);

				while (true)
				{
					Command command = GetUserInput();

					switch (command)
					{
						case Command.REGISTER_USER:
							RegisterUser();
							break;
						case Command.CHANGE_NAME:
							ChangeName();
							break;
						case Command.UNREGISTER_USER:
							UnregisterUser();
							break;
						case Command.SHOW_GAMES:
							ShowGames();
							break;
						case Command.JOIN_GAME:
							JoinGame();
							break;
						case Command.CREATE_GAME:
							CreateGame();
							break;
						case Command.CLOSE_SOCKET:
							string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.CLOSE_SOCKET));
							WritePacket(socket, jsonRequest);
							Packet packet = ReadPacket(socket);
							if (!packet.Success)
							{
								DebugUtils.WriteLine("[CLIENT] Error in closing socket connection");
								break;
							}
							return;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error: {e.StackTrace}");
			}
		}

		/**
		 * Prompts the user to select a menu option and returns the corresponding command
		 */
		private Command GetUserInput()
		{
			int selection = 0;
			do
			{
				Console.Write("\nEnter an option: ");
				try
				{
					selection = int.Parse(Console.ReadLine().Trim());
				}
				catch (Exception) { }
			}
			while (selection < 0 || selection > Enum.GetValues(typeof(Command)).Length);

			return (Command)selection;
		}

		private void RegisterUser()
		{
			string username;
			do
			{
				Console.Write("Enter a username: ");
				username = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(username))
				{
					Console.WriteLine("Invalid username. Please enter a username");
					continue;
				}

				break;
			}
			while (true);

			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { username }));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);
			if (!packet.Success)
			{
				Console.WriteLine("You are already registered");
			}
			else
			{
				Player = JsonConvert.DeserializeObject<Player>(packet.DataToString()[0]);
				DebugUtils.WriteLine("[CLIENT] Player successfully registered");
			}
		}

		private void ChangeName()
		{
			string username;
			do
			{
				Console.Write("Enter a new username: ");
				username = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(username))
				{
					Console.WriteLine("You must provide a username.");
					continue;
				}

				break;
			}
			while (true);

			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.CHANGE_NAME, true, new List<object>() { username }));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);
			if (!packet.Success)
			{
				Console.WriteLine("You are not registered");
				return;
			}

			Player = JsonConvert.DeserializeObject<Player>(packet.DataToString()[0]);
			DebugUtils.WriteLine("[CLIENT] Player name successfully changed");
		}

		private void UnregisterUser()
		{
			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, true));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);
			if (!packet.Success)
			{
				DebugUtils.WriteLine("[CLIENT] Error in deactivating the user");
				return;
			}

			Player = null;
			DebugUtils.WriteLine("[CLIENT] Account Deactivated");
		}

		private List<int> ShowGames()
		{
			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.SHOW_GAMES));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);
			if (!packet.Success)
			{
				DebugUtils.WriteLine("[CLIENT] Error in showing games");
				return null;
			}

			List<int> gameList = JsonConvert.DeserializeObject<List<int>>(packet.DataToString()[0]);
			Console.WriteLine("Games:");

			if (gameList.Count == 0)
			{
				Console.WriteLine("Sorry, no games exists at the moment.");
				return null;
			}

			foreach (int gameId in gameList)
			{
				Console.WriteLine($"Game ID: {gameId}");
			}

			return gameList;
		}

		private void JoinGame()
		{
			if (Player == null)
			{
				Console.WriteLine("You are not registered");
				return;
			}

			List<int> gameList = ShowGames();
			if (gameList == null)
			{
				return;
			}

			int gameId;
			do
			{
				Console.Write("Type in a open Game ID: ");
				try
				{
					gameId = int.Parse(Console.ReadLine().Trim());
					if (!gameList.Contains(gameId))
					{
						Console.WriteLine("Invalid Game ID");
					}
				}
				catch (Exception)
				{
					continue;
				}

				break;
			}
			while (true);

			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, true, new List<object>() { gameId }));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);
			DebugUtils.WriteLine($"[CLIENT] {(packet.Success ? "Successfully joined game" : "Error in joining game")}");

			Console.WriteLine("Waiting for players...");
			packet = ReadPacket(socket);

			if (packet.Success)
			{
				Console.WriteLine("Starting game!");
				GameLoop();
			}

		}

		private void CreateGame()
		{
			if (Player == null)
			{
				Console.WriteLine("You are not registered");
				return;
			}

			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.CREATE_GAME));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);
			DebugUtils.WriteLine($"[CLIENT] {(packet.Success ? "Successfully created a game" : "Error in creating a game")}");
			Console.WriteLine("Waiting for players...");
			packet = ReadPacket(socket);

			if (packet.Success)
			{
				Console.WriteLine("Starting game!");
				GameLoop();
			}
		}

		private void GameLoop()
		{
			DebugUtils.WriteLine($"[CLIENT] {Player.Name} in now in their GameLoop!");
			bool isGameOngoing = true;

			while (isGameOngoing)
			{
				Packet packet = ReadPacket(socket);

				switch (packet.Command)
				{
					case Command.REQUEST_MOVE:
						Console.WriteLine($"{Player.Name}, you have {Player.Chips} chips...\nYour hand is:");
						Player.Hand.ShowHand();
						Console.WriteLine("\nSTAY (0), FOLD (-1), or RAISE (#)?");
						int choice = int.Parse(Console.ReadLine()); // TODO: make this not suck. Error handling.

						string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.SEND_MOVE, true, new List<object>() { choice }));
						WritePacket(socket, jsonResponse);

						break;
					case Command.GIVE_CARD:
						Card card = JsonConvert.DeserializeObject<Card>(packet.DataToString()[0]);
						Player.Hand.AddCard(card);
						Console.WriteLine("You received a card.");
						Player.Hand.ShowHand();
						Console.WriteLine();
						break;
					case Command.ADJUST_CHIPS:
						int chips = JsonConvert.DeserializeObject<int>(packet.DataToString()[0]);
						Player.Chips = chips;
						break;
					case Command.DISPLAY_MESSAGE:
						string message = JsonConvert.DeserializeObject<string>(packet.DataToString()[0]);
						Console.WriteLine(message);
						break;
					case Command.ANNOUCE_WINNER:
						string winner = JsonConvert.DeserializeObject<string>(packet.DataToString()[0]);
						Console.WriteLine(winner);
						isGameOngoing = false;
						break;
				}
			}
		}
	}
}
