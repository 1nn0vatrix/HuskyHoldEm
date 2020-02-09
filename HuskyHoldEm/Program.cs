using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	/*
	 * 52 / 5 = 10.4, so a game can support a maximum of 10 people.
	 */

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Making a new deck.");
			Deck deck = new Deck();
			deck.Print();

			Console.WriteLine("\nShuffling deck.");
			deck.Shuffle();
			deck.Print();
			Console.WriteLine();

			Console.WriteLine("\nMaking a new game.");

			List<Player> players = new List<Player> { new Player("barbie") };

			Game game = new Game(players);

			Console.WriteLine("\nStarting player with two cards...\n");
			game.Deal(players.First(), 2);

			Console.WriteLine("Showing player's hand.");
			players.First().Hand.ShowHand();

			Console.WriteLine("\n\nDealing player another card...\n");
			game.Deal(players.First(), 1);

			Console.WriteLine("Showing player's hand.");
			players.First().Hand.ShowHand();

			Console.WriteLine("\n\nDealing player another card...\n");
			game.Deal(players.First(), 1);

			Console.WriteLine("Showing player's hand.");
			players.First().Hand.ShowHand();

			Console.WriteLine("\n\nDealing player another card...\n");
			game.Deal(players.First(), 1);

			Console.WriteLine("Showing player's hand.");
			players.First().Hand.ShowHand();

			//Console.WriteLine("\nShowing deck.\n");
			//game.PrintDeck();
			//Console.WriteLine();

			Console.WriteLine($"\n\n{players[0].Name}'s highest ranking:");
			players.First().Hand.PrintRanking();

			Console.WriteLine("\n");

			List<Card> royalFlush = new List<Card> { new Card(Rank.Ace, Suit.Club), new Card(Rank.King, Suit.Club), new Card(Rank.Queen, Suit.Club), new Card(Rank.Jack, Suit.Club), new Card(Rank.Ten, Suit.Club) };
			List<Card> straightFlush = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.King, Suit.Club), new Card(Rank.Queen, Suit.Club), new Card(Rank.Jack, Suit.Club), new Card(Rank.Ten, Suit.Club) };
			List<Card> straightFlush2 = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.Eight, Suit.Club), new Card(Rank.Queen, Suit.Club), new Card(Rank.Jack, Suit.Club), new Card(Rank.Ten, Suit.Club) };
			List<Card> fourOfAKind = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.Nine, Suit.Spade), new Card(Rank.Nine, Suit.Heart), new Card(Rank.Nine, Suit.Diamond), new Card(Rank.Ten, Suit.Club) };
			List<Card> fourOfAKind2 = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.Nine, Suit.Spade), new Card(Rank.Nine, Suit.Heart), new Card(Rank.Nine, Suit.Diamond), new Card(Rank.Eight, Suit.Club) };
			List<Card> fourOfAKind3 = new List<Card> { new Card(Rank.Seven, Suit.Club), new Card(Rank.Seven, Suit.Spade), new Card(Rank.Seven, Suit.Heart), new Card(Rank.Seven, Suit.Diamond), new Card(Rank.Eight, Suit.Club) };
			List<Card> fullHouse = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.Nine, Suit.Spade), new Card(Rank.Nine, Suit.Heart), new Card(Rank.Ten, Suit.Diamond), new Card(Rank.Ten, Suit.Club) };
			List<Card> fullHouse2 = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.Nine, Suit.Spade), new Card(Rank.Nine, Suit.Heart), new Card(Rank.Ten, Suit.Diamond), new Card(Rank.Ten, Suit.Club) };
			List<Card> flush = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.King, Suit.Club), new Card(Rank.Four, Suit.Club), new Card(Rank.Six, Suit.Club), new Card(Rank.Ten, Suit.Club) };
			List<Card> straight = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.King, Suit.Diamond), new Card(Rank.Queen, Suit.Spade), new Card(Rank.Jack, Suit.Heart), new Card(Rank.Ten, Suit.Club) };
			List<Card> threeOfAKind = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.Nine, Suit.Spade), new Card(Rank.Nine, Suit.Heart), new Card(Rank.Two, Suit.Diamond), new Card(Rank.Ten, Suit.Club) };
			List<Card> twoPair = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.Nine, Suit.Spade), new Card(Rank.Four, Suit.Heart), new Card(Rank.Four, Suit.Diamond), new Card(Rank.Ten, Suit.Club) };
			List<Card> pair = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.Nine, Suit.Spade), new Card(Rank.Four, Suit.Heart), new Card(Rank.Three, Suit.Diamond), new Card(Rank.Ten, Suit.Club) };
			List<Card> highCard = new List<Card> { new Card(Rank.Nine, Suit.Club), new Card(Rank.Nine, Suit.Spade), new Card(Rank.Four, Suit.Heart), new Card(Rank.Three, Suit.Diamond), new Card(Rank.Ten, Suit.Club) };


			List<Card> hand1cards = fourOfAKind;
			List<Card> hand2cards = fourOfAKind3;

			Hand hand1 = new Hand(hand1cards);
			Hand hand2 = new Hand(hand2cards);

			Console.WriteLine($"hand1: ");
			hand1.PrintRanking();
			Console.WriteLine($"\nhand2: ");
			hand2.PrintRanking();

			Console.WriteLine("\n\ncomparing hand1 to hand2: " + hand1.CompareTo(hand2));
			Console.WriteLine("\ncomparing hand2 to hand1: " + hand2.CompareTo(hand1));

			Console.ReadLine();
		}
	}
}
