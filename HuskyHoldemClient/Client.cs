using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Xml.Serialization;
using static HuskyHoldEm.NetworkUtils;
using HuskyHoldEm;
using System.Collections.Generic;
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

		public static void Main()
		{
			try
			{
				Console.WriteLine("Connecting.....");
				
				TcpClient tcpClient = new TcpClient();
				tcpClient.Connect(LOCAL_HOST_IP, PORT);
				Console.WriteLine("Connected");

				Console.WriteLine(MENU);
				
				while (true)
				{
					int selection = GetUserInput();
					string jsonRequest = JsonConvert.SerializeObject(new Packet((Command)selection, false, new List<object>() { "CLIENT SENDING PACKET TO SERVER TEST" }));
					WritePacket(tcpClient.Client, jsonRequest);
					Console.WriteLine("Transmitting.....");

					// Get response back from server
					Packet packet = ReadPacket(tcpClient.Client);
					Console.WriteLine($"Packet Command: {packet.Command}, command success: {packet.Success}, response message: {(string)packet.DataList[0]}");

					if (selection == 5)
					{
						break;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error: {e.StackTrace}");
			}
		}

		private static int GetUserInput()
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
			while (selection <= 0 || selection > 5);

			return selection;
		}
	}
}
