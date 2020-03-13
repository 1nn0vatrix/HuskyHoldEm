using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	class Deck
	{
		//NOTE: Random is not thread-safe.
		static Random _random = new Random();

		List<Card> cards = new List<Card>();

		public Deck()
		{
			for (int i = 0; i < 52; i++)
			{
				if (i < 13)
				{
					cards.Add(new Card((Rank)i + 2, Suit.Spade));
					continue;
				}
				if (i < 13 * 2)
				{
					cards.Add(new Card((Rank)i - 13 + 2, Suit.Heart));
					continue;
				}
				if (i < 13 * 3)
				{
					cards.Add(new Card((Rank)i - 13 * 2 + 2, Suit.Club));
					continue;
				}
				if (i < 13 * 4)
				{
					cards.Add(new Card((Rank)i - 13 * 3 + 2, Suit.Diamond));
					continue;
				}
			}
		}

		/*
		 * Fisher-Yates Shuffle
		 * for i from n−1 down to 1 do
		 * j <-- random integer such that 0 ≤ j ≤ i
		 * exchange a[j] and a[i]
		 */
		public void Shuffle()
		{
			int n = cards.Count;

			for (int i = 0; i < (n - 1); i++)
			{
				int r = i + _random.Next(n - i);
				Card temp = cards[r];
				cards[r] = cards[i];
				cards[i] = temp;
			}
		}

		public Card GetCard()
		{
			if (cards.Count > 0)
			{
				Card topCard = cards[cards.Count - 1];
				cards.RemoveAt(cards.Count - 1);
				return topCard;
			}
			else
			{
				DebugUtils.WriteLine("[!] ERROR: Tried to remove card from empty deck.");
				return null;
			}
		}

		// Print the deck. Why? Idk debugging. lol. You'd never really want to do this in the game.
		public void Print()
		{
			foreach (Card c in cards)
			{
				c.Print();
			}
		}
	}
}
