using System;
using System.Collections.Generic;
using System.Net.Sockets;
using HuskyHoldEm;
using static HuskyHoldEm.NetworkUtils;
using Newtonsoft.Json;
using System.Linq;

namespace HuskyHoldemClient
{
	/**
	 * Client Endpoint
	 */
	public class Client
	{
		private string hostIP = "127.0.0.1";
		private int port = 26795;
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
			+ "-1. Exit";

		ClientPlayer Player { get; set; }

		public void Run()
		{
			try
			{
				Console.Write("\nEnter IP address of server you wish to connect to: ");
				try
				{
					hostIP = Console.ReadLine().Trim();
				}
				catch (Exception) { }

				DebugUtils.WriteLine("[CLIENT] Connecting to server...");

				// connect to the server
				TcpClient tcpClient = new TcpClient();
				tcpClient.Connect(hostIP, port);
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
						case Command.CHEAT_CODE:
							CheatCode();
							break;
						case Command.CLOSE_SOCKET:
							CloseSocket();
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
				catch (Exception)
				{
					selection = int.MinValue;
				}
			}
			while (selection < -1 || selection > Enum.GetValues(typeof(Command)).Length);

			return (Command)selection;
		}

		private void RegisterUser()
		{
			if (Player != null)
			{
				Console.WriteLine("You are already registered");
				return;
			}

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
			if (packet.Success)
			{
				Player = JsonConvert.DeserializeObject<ClientPlayer>(packet.DataToString()[0]);
				DebugUtils.WriteLine("[CLIENT] Player successfully registered");
			}
			else
			{
				DebugUtils.WriteLine("[CLIENT] Error in registering a user");
			}
		}

		private void ChangeName()
		{
			if (Player == null)
			{
				Console.WriteLine("You are not registered");
				return;
			}
			
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
			if (packet.Success)
			{
				Player.Name = username;
				DebugUtils.WriteLine("[CLIENT] Player name successfully changed");
			}
			else
			{
				DebugUtils.WriteLine("[CLIENT] Error in changing a user's username");
			}
		}

		private void UnregisterUser()
		{
			if (Player == null)
			{
				Console.WriteLine("You are not registered");
				return;
			}

			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, true));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);
			if (packet.Success)
			{
				Player = null;
				DebugUtils.WriteLine("[CLIENT] Account Deactivated");
			}
			else
			{
				DebugUtils.WriteLine("[CLIENT] Error in deactivating a user's account");
			}
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
			else if (Player.Chips <= 1)
			{
				Console.WriteLine("You don't have enough chips to play");
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
			else if (Player.Chips <= 1)
			{
				Console.WriteLine("You don't have enough chips to play");
				return;
			}

			int numberOfPlayers;
			do
			{
				Console.Write("How many players would you like to join: ");
				numberOfPlayers = int.Parse(Console.ReadLine().Trim());
				if (numberOfPlayers <= 1 || numberOfPlayers > 10)
				{
					Console.WriteLine("Invalid number of players, please select from 2-10");
					continue;
				}

				break;
			}
			while (true);

			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.CREATE_GAME, true, new List<object>() { numberOfPlayers }));
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

		private void CheatCode()
		{
			if (Player == null)
			{
				Console.WriteLine("You are not registered.");
				return;
			}

			Console.Write("Enter a cheat code: ");
			string cheatCode = Console.ReadLine();

			if (cheatCode.Equals("0x878470"))
			{
				string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.CHEAT_CODE, true, new List<object>() { 50 }));
				WritePacket(socket, jsonRequest);

				Packet packet = ReadPacket(socket);
				if (packet.Success)
				{
					Player.Chips += 50;
					Console.WriteLine("CHEAT CODE USED. +50 CHIPS");
					Console.WriteLine($"You now have {Player.Chips} chips");
				}
				else
				{
					Console.WriteLine("You need less than two chips for this cheat code to work.");
				}
				return;
			}

			Console.WriteLine("Invalid cheat code.");
		}

		private void CloseSocket()
		{
			Player = null;

			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.CLOSE_SOCKET));
			WritePacket(socket, jsonRequest);
			
			Packet packet = ReadPacket(socket);
			if (!packet.Success)
			{
				DebugUtils.WriteLine("[CLIENT] Error in closing socket connection");
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

						bool isValidChoice = false;
						int choice = -1;
						while (!isValidChoice)
						{
							Console.WriteLine("\nSTAY, FOLD, or RAISE? To RAISE type 'RAISE n' where n is the number you want to raise by.");
							string input = Console.ReadLine().ToLower();

							if (!String.IsNullOrEmpty(input))
							{
								List<string> stringTokens = input.Split(' ').ToList();
								string playerChoice = stringTokens[0];
								switch (playerChoice)
								{
									case "fold":
										choice = -1;
										isValidChoice = true;
										break;
									case "stay":
										choice = 0;
										isValidChoice = true;
										break;
									case "raise":
										if (stringTokens.Count != 2)
										{
											Console.WriteLine("Please enter a valid number of chips.");
											break;
										}
										try
										{
											choice = int.Parse(stringTokens[1]);
										}
										catch (Exception)
										{
											Console.WriteLine("Please enter a valid number of chips.");
											break;
										}
										if (choice <= 0)
										{
											Console.WriteLine("You must bet at least 1 chip.");
											break;
										}
										if (choice > Player.Chips)
										{
											Console.WriteLine("You have insufficient funds to place this bet, please try again.");
											break;
										}
										isValidChoice = true;
										break;
									default:
										Console.WriteLine("Invalid call made. Try again.");
										break;
								}
							}
						}

						string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.SEND_MOVE, true, new List<object>() { choice }));
						WritePacket(socket, jsonResponse);

						// If player folds, exit the function.
						if (choice == -1)
						{
							isGameOngoing = false;
						}

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
					case Command.SHOW_HANDS:
						List<KeyValuePair<string, List<Card>>> players = JsonConvert.DeserializeObject<List<KeyValuePair<string, List<Card>>>>(packet.DataToString()[0]);
						foreach (KeyValuePair<string, List<Card>> player in players)
						{
							Console.Write(player.Key + " has ");
							Hand playerHand = new Hand(player.Value);
							playerHand.ShowHand();
							Console.WriteLine();
						}
						break;
					case Command.ANNOUCE_WINNER:
						string winnerName = JsonConvert.DeserializeObject<string>(packet.DataToString()[0]);
						List<Card> winnerCards = JsonConvert.DeserializeObject<List<Card>>(packet.DataToString()[1]);
						string winnerWinnings = JsonConvert.DeserializeObject<string>(packet.DataToString()[2]);

						// Only print the ranking if they finished the entire round and have five cards.
						if (winnerCards.Count == 5)
						{
							Console.Write($"The winner is... {winnerName}! Their winning hand is ");
							Hand winnerHand = new Hand(winnerCards);
							winnerHand.PrintRanking();
						}

						Console.WriteLine(winnerWinnings);

						// Empty the player's hand for further games
						Player.Hand.ClearHand();
						isGameOngoing = false;
						break;
					case Command.REMOVE_PLAYER:
						string removeMessage = JsonConvert.DeserializeObject<string>(packet.DataToString()[0]);
						Console.WriteLine(removeMessage);
						isGameOngoing = false;
						break;
				}
			}
			Console.WriteLine("\n" + MENU);
		}
	}
}
