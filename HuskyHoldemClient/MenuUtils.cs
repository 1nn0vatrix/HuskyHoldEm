using HuskyHoldEm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldemClient
{
	public static class MenuUtils
	{
		private const string MENU_WELCOME = "Welcome to Husky Hold'Em!\n";

		// This is updated whenever ShowRegisteredMenu() is called.
		private static string MENU_CUSTOM_WELCOME = "Hello, {Player.Name}, you have {Player.Chips} chips!\n";

		private const string MENU_PROMPT = "Please pick from the following options:\n";

		private const string MENU_REGISTER = "1. Register\n";
		private const string MENU_CHANGE_USER = "2. Change Username\n";
		private const string MENU_UNREGISTER = "3. Unregister\n";
		private const string MENU_SHOW_GAMES = "4. Show Games\n";
		private const string MENU_JOIN_GAME = "5. Join Game\n";
		private const string MENU_CREATE_GAME = "6. Create Game\n";
		private const string MENU_EXIT = "-1. Exit";

		//static string MENU_ART = 
		//"  _____\n" +
		//" | A .  | _____\n" +
		//" |  /.\\ || A ^  | _____\n" +
		//" | (_._)||  / \\ || A _  | _____\n" +
		//" |   |  ||  \\ / ||  ( ) || A_ _ |\n" +
		//" | ____V||   .  || (_'_)|| ( v )|\n" +
		//"         | ____V||   |  ||  \\ / |\n" +
		//"                 | ____V||   .  |\n" +
		//"                         | ____V|\n\n";

		static string[] MENU_ART_ARR = new string[]
		{
			"  _____                          ",
			" | A .  | _____                  ",
			" |  /.\\ || A ^  | _____          ",
			" | (_._)||  / \\ || A _  | _____  ",
			" |   |  ||  \\ / ||  ( ) || A_ _ |",
			" | ____V||   .  || (_'_)|| ( v )|",
			"         | ____V||   |  ||  \\ / |",
			"                 | ____V||   .  |",
			"                         | ____V|"
		};

		public static void ShowUnregisteredMenu()
		{
			Console.WriteLine(MENU_WELCOME);
			MenuUtils.ShowColoredArt();
			Console.WriteLine(MENU_PROMPT + MENU_REGISTER + MENU_EXIT);
		}

		public static void ShowRegisteredMenu(ClientPlayer player)
		{
			MENU_CUSTOM_WELCOME = $"Hello, {player.Name}, you have {player.Chips} chips!\n";
			Console.WriteLine(MENU_CUSTOM_WELCOME);
			MenuUtils.ShowColoredArt();
			Console.WriteLine(MENU_PROMPT + MENU_CHANGE_USER + MENU_UNREGISTER + MENU_SHOW_GAMES + MENU_JOIN_GAME + MENU_CREATE_GAME + MENU_EXIT);

		}

		public static void ShowColoredArt()
		{
			// 9 lines tall
			// 32 chars wide
			for (int height = 0; height < MENU_ART_ARR.Length; height++)
			{
				for (int width = 0; width < MENU_ART_ARR[0].Length; width++)
				{
					if (height == 1)
					{
						if (width == 3 || width == 5)
						{
							Console.ForegroundColor = ConsoleColor.White;
						}
						else
						{
							Console.ResetColor();
						}
					}
					if (height == 2)
					{
						if (width >= 4 && width <= 7)
						{
							Console.ForegroundColor = ConsoleColor.White;
						}
						else if (width >= 11 && width <= 14)
						{
							Console.ForegroundColor = ConsoleColor.Cyan;
						}
						else
						{
							Console.ResetColor();
						}
					}
					if (height == 3)
					{
						if (width >= 3 && width <= 7)
						{
							Console.ForegroundColor = ConsoleColor.White;
						}
						else if (width >= 11 && width <= 14)
						{
							Console.ForegroundColor = ConsoleColor.Cyan;
						}
						else if (width >= 19 && width <= 21)
						{
							Console.ForegroundColor = ConsoleColor.Green;
						}
						else
						{
							Console.ResetColor();
						}
					}
					if (height == 4)
					{
						if (width >= 3 && width <= 7)
						{
							Console.ForegroundColor = ConsoleColor.White;
						}
						else if (width >= 11 && width <= 14)
						{
							Console.ForegroundColor = ConsoleColor.Cyan;
						}
						else if (width >= 19 && width <= 23)
						{
							Console.ForegroundColor = ConsoleColor.Green;
						}
						else if (width >= 27 && width <= 31)
						{
							Console.ForegroundColor = ConsoleColor.Red;
						}
						else
						{
							Console.ResetColor();
						}
					}
					if (height == 5)
					{
						if (width == 7)
						{
							Console.ForegroundColor = ConsoleColor.White;
						}
						else if (width >= 11 && width <= 14)
						{
							Console.ForegroundColor = ConsoleColor.Cyan;
						}
						else if (width >= 19 && width <= 23)
						{
							Console.ForegroundColor = ConsoleColor.Green;
						}
						else if (width >= 27 && width <= 31)
						{
							Console.ForegroundColor = ConsoleColor.Red;
						}
						else
						{
							Console.ResetColor();
						}
					}
					if (height == 6)
					{
						if (width == 15)
						{
							Console.ForegroundColor = ConsoleColor.Cyan;
						}
						else if (width >= 19 && width <= 23)
						{
							Console.ForegroundColor = ConsoleColor.Green;
						}
						else if (width >= 27 && width <= 31)
						{
							Console.ForegroundColor = ConsoleColor.Red;
						}
						else
						{
							Console.ResetColor();
						}
					}
					if (height == 7)
					{
						if (width == 23)
						{
							Console.ForegroundColor = ConsoleColor.Green;
						}
						else if (width >= 27 && width <= 31)
						{
							Console.ForegroundColor = ConsoleColor.Red;
						}
						else
						{
							Console.ResetColor();
						}
					}
					if (height == 8)
					{
						if (width == 31)
						{
							Console.ForegroundColor = ConsoleColor.Red;
						}
						else
						{
							Console.ResetColor();
						}
					}

					Console.Write(MENU_ART_ARR[height][width]);
				}
				Console.WriteLine();
			}
			Console.ResetColor();
		}
	}
}
