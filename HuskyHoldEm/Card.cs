using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum Rank
{
	Two = 2,
	Three = 3,
	Four = 4,
	Five = 5,
	Six = 6,
	Seven = 7,
	Eight = 8,
	Nine = 9,
	Ten = 10,
	Jack = 11,
	Queen = 12,
	King = 13,
	Ace = 14  // Aces high!
}

enum Suit
{
	Spade = 1,
	Heart = 2,
	Club = 3,
	Diamond = 4
}

namespace HuskyHoldEm
{
	[DebuggerDisplay("{Rank} {Suit}")]
	class Card : IComparable<Card>
	{
		// Properties with just a get are readonly.
		public Rank Rank { get; }
		public Suit Suit { get; }

		public Card(Rank rank, Suit suit)
		{
			this.Rank = rank;
			this.Suit = suit;
		}

		// Print the appearance of the card
		public void Print()
		{
			string output = "";
			if ((int)Rank > 10)
			{
				switch (Rank)
				{
					case Rank.Jack:
						output = "J";
						break;
					case Rank.Queen:
						output = "Q";
						break;
					case Rank.King:
						output = "K";
						break;
					case Rank.Ace:
						output = "A";
						break;
				}
			}
			else
			{
				output = ((int)Rank).ToString();
			}
			Console.ForegroundColor = ConsoleColor.White;
			switch (Suit)
			{
				case Suit.Spade:
					output += "S";
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case Suit.Heart:
					output += "H";
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case Suit.Club:
					output += "C";
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case Suit.Diamond:
					output += "D";
					Console.ForegroundColor = ConsoleColor.Cyan;
					break;
			}
			Console.Write(output + " ");
			Console.ResetColor();
		}

		public int CompareTo(Card c)
		{
			if (c == null)
			{
				return 1;
			}

			if (Rank > c.Rank)
			{
				return 1;

			}
			if (Rank < c.Rank)
			{
				return -1;
			}
			return 0;
		}
	}
}
