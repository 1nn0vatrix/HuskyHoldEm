﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using HuskyHoldEm;
using static HuskyHoldEm.NetworkUtils;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.IO;

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

		ClientPlayer Player { get; set; }

		public void Run()
		{
			try
			{
				ConnectToServers();

				MenuUtils.ShowUnregisteredMenu();

				while (true)
				{
					Command command = GetUserMenuInput();

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
						case Command.VIEW_LEADERBOARD:
							ViewLeaderboard();
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
			catch (SocketException)
			{
				Console.WriteLine("Could not connect to any servers. Please try again later.");
				Console.ReadLine();
			}
			catch (Exception e)
			{
				Console.WriteLine($"[!] Error {e.GetType().Name}\n {e.StackTrace} \nPress any key to exit...");
				Console.ReadLine();
			}
		}

		private void ConnectToServers()
		{
			WebClient client = new WebClient();
			String content;

			using (Stream stream = client.OpenRead("http://anyabiryukova.github.io/huskyholdemserver.txt"))
			{
				StreamReader reader = new StreamReader(stream);
				content = reader.ReadToEnd();
			}

			List<String> availableServers = content.Split('\n').ToList();

			TcpClient tcpClient = new TcpClient();

			foreach (string server in availableServers)
			{
				string[] serverSocket = server.Split(':');

				string serverIP = serverSocket[0];

				// Attempt to connect to the servers
				DebugUtils.WriteLine($"[CLIENT] Connecting to {server}...");
				
				if (serverSocket.Count() == 1)
				{
					tcpClient.ConnectAsync(serverIP, port).Wait(1000);
				}
				else
				{
					int serverPort;
					if (int.TryParse(serverSocket[1], out serverPort))
					{
						tcpClient.ConnectAsync(serverIP, serverPort).Wait(1000);
					}
				}

				if (!tcpClient.Connected)
				{
					DebugUtils.WriteLine($"[CLIENT] Could not connect to server {server}...");
					tcpClient.Close();
					tcpClient = new TcpClient();
				}
				else
				{
					break;
				}
			}

			if (!tcpClient.Connected)
			{
				Console.Write("\nCould not connect to any established servers.\nEnter IP address of server you wish to connect to: ");
				try
				{
					hostIP = Console.ReadLine().Trim();
				}
				catch (Exception) { }

				// Connect to the given server
				tcpClient.Connect(hostIP, port);
			}

			socket = tcpClient.Client;

			DebugUtils.WriteLine("[CLIENT] Connection accepted");
		}

		/**
		 * Prompts the user to select a menu option and returns the corresponding command
		 */
		private Command GetUserMenuInput()
		{
			int selection = 0;
			do
			{
				Console.Write("\nEnter a menu option: ");
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
				MenuUtils.ShowRegisteredMenu(Player);
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
				Console.WriteLine("Player name successfully changed!");
			}
			else
			{
				Console.WriteLine("Error in changing name.");
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
				MenuUtils.ShowUnregisteredMenu();
			}
			else
			{
				DebugUtils.WriteLine("[CLIENT] Error in deactivating a user's account");
			}
		}

		private List<int> ShowGames()
		{
			if (Player == null)
			{
				Console.WriteLine("You are not registered");
				return null;
			}

			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.SHOW_GAMES));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);
			if (!packet.Success)
			{
				DebugUtils.WriteLine("[CLIENT] Error in showing games");
				return null;
			}

			List<int> gameList = JsonConvert.DeserializeObject<List<int>>(packet.DataToString()[0]);
			List<string> gamePlayerCount = JsonConvert.DeserializeObject<List<string>>(packet.DataToString()[1]);
			Console.WriteLine("Games:");

			if (gameList == null || gameList.Count == 0)
			{
				Console.WriteLine("Sorry, no games exists at the moment.");
				return null;
			}

			for (int i = 0; i < gameList.Count; i++)
			{
				Console.WriteLine($"Game ID: {gameList[i]}\t{gamePlayerCount[i]} players");
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
			else if (Player.Chips < 2)
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
				Console.Write("Type in an open Game ID: ");
				try
				{
					gameId = int.Parse(Console.ReadLine().Trim());
					if (!gameList.Contains(gameId))
					{
						Console.WriteLine("Invalid Game ID.");
						return;
					}
				}
				catch (Exception)
				{
					Console.WriteLine("Invalid Game ID.");
					return;
				}

				break;
			}
			while (true);

			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.JOIN_GAME, true, new List<object>() { gameId }));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);

			if (packet.Success)
			{
				Console.WriteLine($"Joined game {gameId}");

				Console.WriteLine("Waiting for players...");
				packet = ReadPacket(socket);

				if (packet.Success)
				{
					Console.WriteLine("Starting game!");
					GameLoop();
				}
			}
			else
			{
				Console.WriteLine($"Error in joining game {gameId}");
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
				Console.Write("How many players would you like to join? (Type -1 to cancel.) ");
				try
				{
					numberOfPlayers = int.Parse(Console.ReadLine().Trim());
				}
				catch (FormatException)
				{
					Console.WriteLine("Invalid number of players, please select from 2-10");
					continue;
				}

				if (numberOfPlayers == -1)
				{
					return;
				}

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

		void ViewLeaderboard()
		{
			string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.VIEW_LEADERBOARD));
			WritePacket(socket, jsonRequest);

			Packet packet = ReadPacket(socket);
			if (!packet.Success)
			{
				DebugUtils.WriteLine("[CLIENT] Error in showing leaderboard");
				return;
			}

			KeyValuePair<string, int> winner = JsonConvert.DeserializeObject<KeyValuePair<string, int>>(packet.DataToString()[0]);
			Console.WriteLine($"The biggest winner is {winner.Key} with {winner.Value} chips!");
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
						// Clear any previous user input.
						while (Console.KeyAvailable)
						{
							Console.ReadKey(false);
						}

						Console.WriteLine($"\n{Player.Name}, you have {Player.Chips} chips...\nYour hand is:");
						Player.Hand.ShowHand();

						bool isValidChoice = false;
						int choice = -1;

						while (!isValidChoice)
						{
							Console.WriteLine("\n\nSTAY, FOLD, or RAISE? \nTo RAISE type 'RAISE n' where n is the number you want to raise by.");
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
							Console.Write($"\nThe winner is... {winnerName}! Their winning hand is ");
							Hand winnerHand = new Hand(winnerCards);
							winnerHand.PrintRanking();
						}

						Console.WriteLine(winnerWinnings);
						isGameOngoing = false;
						break;
					case Command.REMOVE_PLAYER:
						string removeMessage = JsonConvert.DeserializeObject<string>(packet.DataToString()[0]);
						Console.WriteLine(removeMessage);
						isGameOngoing = false;
						break;
				}
			}
			// Empty the player's hand for further games
			Player.Hand.ClearHand();
			Console.WriteLine("\nPress any key to return to main menu.");
			Console.ReadLine();
			MenuUtils.ShowRegisteredMenu(Player);
		}
	}
}
