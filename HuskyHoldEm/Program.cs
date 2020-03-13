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

			Console.WriteLine("\nMaking a new game.");

			List<Player> players = new List<Player> { new Player("Holly Husky") };

			Game game = new Game(players);

			Console.WriteLine("\nStarting player with two cards...\n");
			game.Deal(players.First(), 2);

			Console.WriteLine($"Showing {players.First().Name}'s hand.");
			players.First().Hand.ShowHand();

			Console.WriteLine("\n\nDealing player another card...\n");
			game.Deal(players.First(), 1);

			Console.WriteLine($"Showing {players.First().Name}'s hand.");
			players.First().Hand.ShowHand();

			Console.WriteLine("\n\nDealing player another card...\n");
			game.Deal(players.First(), 1);

			Console.WriteLine($"Showing {players.First().Name}'s hand.");
			players.First().Hand.ShowHand();

			Console.WriteLine("\n\nDealing player another card...\n");
			game.Deal(players.First(), 1);

			Console.WriteLine($"Showing {players.First().Name}'s hand.");
			players.First().Hand.ShowHand();

			//Console.WriteLine("\nShowing deck.\n");
			//game.PrintDeck();
			//Console.WriteLine();

			Console.WriteLine($"\n\n{players[0].Name}'s highest ranking:");
			players.First().Hand.PrintRanking();

			Console.WriteLine("\n");

			Console.WriteLine("Press any key to start game simulation...");
			Console.ReadLine();

			List<IPlayer> consolePlayers = new List<IPlayer> { new ConsolePlayer("Berbie"), new ConsolePlayer("joe") };

			Game testGame = new Game(consolePlayers);
			testGame.StartGame();

			Console.ReadLine();
		}
	}
}
