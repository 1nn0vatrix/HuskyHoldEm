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

		private const string MENU = "Welcome to Husky Hold'Em!\n"
			+ "  _____\n | A .  | _____\n |  /.\\ || A ^  | _____\n" 
			+ " | (_._)||  / \\ || A _  | _____\n |   |  ||  \\ / ||  ( ) || A_ _ |\n" 
			+ " | ____V||   .  || (_'_)|| ( v )|\n         | ____V||   |  ||  \\ / |\n" 
			+ "                 | ____V||   .  |\n                         | ____V|\n" 
			+ "\nPlease Pick from the following options:\n"
			+ "1. Register\n2. Join A Game\n3. Create a Game\n4. Unregister\n5. Exit";

		private static string Username { get; set; }

		public static void Main()
		{
			try
			{
				Console.WriteLine("[CLIENT] Connecting to server...");
				
				// connect to the server
				TcpClient tcpClient = new TcpClient();
				tcpClient.Connect(LOCAL_HOST_IP, PORT);
				Console.WriteLine("[CLIENT] Connection accepted");

				Console.WriteLine(MENU);
				
				while (true)
				{
					Command command = GetUserInput();

					switch (command)
					{
						case Command.REGISTER_USER:
							RegisterUser(tcpClient.Client);
							break;
						case Command.CLOSE_SOCKET:
							string closeSocketRequest = JsonConvert.SerializeObject(new Packet(Command.CLOSE_SOCKET, true));
							WritePacket(tcpClient.Client, closeSocketRequest);
							Packet packet = ReadPacket(tcpClient.Client);
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

		private static void RegisterUser(Socket socket)
		{
			do
			{
				Console.Write("Enter a username: ");
				string username = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(username))
				{
					Console.WriteLine("Invalid username. Please enter a username");
					continue;
				}

				string jsonRequest = JsonConvert.SerializeObject(new Packet(Command.REGISTER_USER, true, new List<object>() { username }));
				WritePacket(socket, jsonRequest);

				Packet packet = ReadPacket(socket);
				if (!packet.Success)
				{
					Console.WriteLine("That username is already taken. Try Again");
					continue;
				}

				Username = username;
				Console.WriteLine("Player successfully registered");
				break;
			}
			while (true);
		}
	}
}
