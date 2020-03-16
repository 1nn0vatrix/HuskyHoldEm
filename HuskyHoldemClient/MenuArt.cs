using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldemClient
{
	public static class MenuArt
	{
		static string MENU_ART = 
		"  _____\n" +
		" | A .  | _____\n" +
		" |  /.\\ || A ^  | _____\n" +
		" | (_._)||  / \\ || A _  | _____\n" +
		" |   |  ||  \\ / ||  ( ) || A_ _ |\n" +
		" | ____V||   .  || (_'_)|| ( v )|\n" +
		"         | ____V||   |  ||  \\ / |\n" +
		"                 | ____V||   .  |\n" +
		"                         | ____V|\n\n";

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
