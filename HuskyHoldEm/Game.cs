using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	public class Game
	{
		public const int MAX_PLAYERS = 10;

		public List<Player> PlayerList { get; set; }
		public bool InProgress { get; set; }
		public int CurrentRound { get; set; }
		private Deck deck;
		private int Pot { get; set; }

		public Game(List<Player> players)
		{
			PlayerList = players;
			deck = new Deck();
			deck.Shuffle();
			deck.Print();
		}

		public void Deal(Player player, int numCards = 1)
		{
			if (numCards <= 0)
			{
				DebugUtils.WriteLine($"[!] ERROR: Number of cards requested ({numCards}) to deal was less than 1.");
				return;
			}

			for (int i = 0; i < numCards; i++)
			{
				player.Hand.AddCard(deck.GetCard());
			}
		}

		public void AddToPot(int chips)
		{
			Pot += chips;
		}

		public List<Player> GetWinner()
		{
			if (PlayerList?.Count == 0)
			{
				return null;
			}

			// Order the players by their hand rankings
			List<Player> OrderedPlayerList = PlayerList.OrderBy(p => p.Hand).ToList();

			if (OrderedPlayerList[0].Hand.CompareTo(OrderedPlayerList[1].Hand) == 1)
			{
				// If the first player in the ordered list is greater than the second, then the first player wins
				return new List<Player> { OrderedPlayerList[0] };
			}
			else
			{
				// Otherwise, it was a tie, return both players
				return new List<Player> { OrderedPlayerList[0], OrderedPlayerList[1] };
			}
			// Technically, it is possible for up to four people to be tied in the following hands:
			// High Card (ie. everyone has the same 5 numbers)
			// Straight Flush / Regular Flush (ie. everyone has the same 5 numbers in their suits)
			// Royal flush (this is like, almost impossible odds)
			// The chances of any of these scenarios happening are very low.
			// But we could write up additional logic later if we want to.
		}

		// Just for testing, do not use in actual game.
		public void PrintDeck()
		{
			deck.Print();
		}
	}
}

/*
 * [[ Husky Hold 'Em ]]
 * 
 * [ Gameplay ]
 * 1. Get your players, assume we have four players in this example.
 * 2. Each player pays 2 chips to get their first two cards.
 * 3. Each player gets two cards.
 * 4. One player starts and can choose to FOLD, STAY, or RAISE.
 * 5. Each player goes in the round and chooses to FOLD, STAY, or RAISE.
 *   a. If the round gets back to the first player and the pot was RAISEd since it got to them, they must choose to FOLD, STAY, or RAISE again.
 *     i.  If the first player chooses to FOLD or STAY, GOTO Step 6.
 *     ii. If the player chooses to RAISE, GOTO Step 5.
 * 6. The next round begins.
 *	 a. If the number of cards each player has is < 5:
 *	   i.  Give each player another card.
 *	   ii. GOTO Step 5.
 *	 b. If the number of cards each player has is 5, GOTO Step 7.
 * 7. Players reveal their hands, the player with the highest ranking wins the pot.
 * 
 * [ Terminology ]
 * FOLD:  You give up for the game. You lose all the chips you bet and are out.
 * STAY:  You stay in the game, but are not increasing the amount you bet.
 * RAISE: You stay in the game, and are increasing the amount you bet. 
 *        Players after your RAISE must also increase to the amount you increased by in order from them to STAY in the game.
 */
