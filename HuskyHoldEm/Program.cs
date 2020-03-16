using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuskyHoldEm.Tests;

namespace HuskyHoldEm
{
	/*
	 * 52 / 5 = 10.4, so a game can support a maximum of 10 people.
	 */

	class Program
	{
		static void Main(string[] args)
		{
			new CardGameTests().RunAllTests();

			Console.WriteLine("Making a new deck.");
			Deck deck = new Deck();
			deck.Print();
			Console.WriteLine();

			Console.WriteLine("\nShuffling deck.");
			deck.Shuffle();
			deck.Print();
			Console.WriteLine();

			Console.WriteLine("Press any key to start game simulation...");
			Console.ReadLine();

			List<IPlayer> consolePlayers = new List<IPlayer> { new ConsolePlayer("Berbie", 75), new ConsolePlayer("joe", 20), new ConsolePlayer("Llama") };

			Game testGame = new Game(consolePlayers, consolePlayers.Count());
			testGame.StartGame();

			Console.ReadLine();
		}
	}
}
