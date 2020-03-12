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
		private static Socket socket;

		private const string MENU = "Welcome to Husky Hold'Em!\n"
			+ "  _____\n | A .  | _____\n |  /.\\ || A ^  | _____\n"
			+ " | (_._)||  / \\ || A _  | _____\n |   |  ||  \\ / ||  ( ) || A_ _ |\n"
			+ " | ____V||   .  || (_'_)|| ( v )|\n         | ____V||   |  ||  \\ / |\n"
			+ "                 | ____V||   .  |\n                         | ____V|\n\n"
			+ "Please Pick from the following options:\n"
			+ "1. Register\n"
			+ "2. Change Username\n"
			+ "3. Unregister\n"
			+ "4. Show Games\n"
			+ "5. Join Game\n"
			+ "6. Create Game\n"
			+ "7. Exit\n";

		private static Player Player { get; set; }

		public static void Main()
		{
			try
			{
				Console.WriteLine("[CLIENT] Connecting to server...");
				
				// connect to the server
				TcpClient tcpClient = new TcpClient();
				tcpClient.Connect(LOCAL_HOST_IP, PORT);
				socket = tcpClient.Client;
				Console.WriteLine("[CLIENT] Connection accepted");

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
						case Command.CREATE_GAME:
							break;
						case Command.CLOSE_SOCKET:
							string closeSocketRequest = JsonConvert.SerializeObject(new Packet(Command.CLOSE_SOCKET, true));
							WritePacket(socket, closeSocketRequest);
							Packet packet = ReadPacket(socket);
							if (!packet.Success)
							{
								Console.WriteLine("[CLIENT] Error in closing socket connection");
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
		private static Command GetUserInput()
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
			while (selection <= 0 || selection > Enum.GetValues(typeof(Command)).Length);

			return (Command)selection;
		}

		private static void RegisterUser()
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
				Player = JsonConvert.DeserializeObject<Player>(JsonConvert.SerializeObject(packet.DataList[0]));
				Console.WriteLine("[CLIENT] Player successfully registered");
			}
		}

		private static void ChangeName()
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

			Player = JsonConvert.DeserializeObject<Player>(JsonConvert.SerializeObject(packet.DataList[0]));
			Console.WriteLine("[CLIENT] Player name successfully changed");
		}

		private static void UnregisterUser()
		{
			string unregisterUserRequest = JsonConvert.SerializeObject(new Packet(Command.UNREGISTER_USER, true));
			WritePacket(socket, unregisterUserRequest);

			Packet packet = ReadPacket(socket);
			Console.WriteLine(packet.Success ? "Account Deactivated" : "Failed to unregister the current user");
		}
	}
}
