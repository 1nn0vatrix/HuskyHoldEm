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
		public List<IPlayer> IPlayerList { get; set; }
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

		public Game(List<IPlayer> players)
		{
			IPlayerList = players;
			deck = new Deck();
			deck.Shuffle();
			deck.Print();
		}

		public void StartGame()
		{
			InProgress = true;
			CurrentRound = 0;
			//CurrentPlayer = PlayerList[0];
			GameLoop();
		}

		public void GameLoop()
		{
			for (int round = 0; round < 4; round++)
			{
				Dictionary<IPlayer, bool> playersStayed = new Dictionary<IPlayer, bool>();
				Dictionary<IPlayer, int> playersCurrentPayments = new Dictionary<IPlayer, int>();
				Console.WriteLine($"Round {round}: Handing out cards...");
				foreach (IPlayer player in IPlayerList)
				{
					playersStayed.Add(player, false);
					playersCurrentPayments.Add(player, 0);
					if (round == 0)
					{
						player.GiveCard(deck.GetCard());
						player.GiveCard(deck.GetCard());
						player.AdjustChips(-2);  // This should actually be checked before they join the game. If they don't have at least two chips, they can't play.
					}
					else
					{
						player.GiveCard(deck.GetCard());
					}
				}
				bool isRoundDone = false;
				IPlayer maxBetter = null;
				int playerIndex = 0;
				int maxBet = 0;
				while (!isRoundDone)
				{
					if (!playersStayed.Skip(1).Any(p => p.Value == false))
					{
						isRoundDone = true;
						break;
					}
					IPlayer current = IPlayerList[playerIndex];
					foreach (IPlayer player in IPlayerList)
					{
						if (!player.Equals(current))
							player.TellWhoseTurn(current.Name);
					}
					int choice = current.GetChoice();
					if (choice < 0)
					{
						// TODO: player folds...
						// make sure to remove from list and check and player index logic accordingly
					}
					if (choice == 0)
					{
						playersStayed[current] = true;
					}
					if (choice > 0)
					{
						playersStayed[current] = false;
						maxBetter = current;
						maxBet += choice;
					}
					int playerOwes = maxBet - playersCurrentPayments[current];
					current.AdjustChips(playerOwes * -1);
					Pot += playerOwes;
					playersCurrentPayments[current] += playerOwes;
					playerIndex  = playerIndex >= IPlayerList.Count - 1 ? 0 : playerIndex + 1;
					current = IPlayerList[playerIndex];
				}
			}
			IPlayer winner = GetWinner()[0];
			winner.AdjustChips(Pot);
			Console.Write("The winner is... " + winner.Name + "! with ");
			winner.Hand.PrintRanking();
			Console.WriteLine($"\n{winner.Name} wins {Pot} chips, they now have {winner.Chips} chips.");
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

		public List<IPlayer> GetWinner()
		{
			if (IPlayerList?.Count == 0)
			{
				return null;
			}

			// Order the players by their hand rankings
			List<IPlayer> OrderedPlayerList = IPlayerList.OrderByDescending(p => p.Hand).ToList();

			if (OrderedPlayerList[0].Hand.CompareTo(OrderedPlayerList[1].Hand) == 1)
			{
				// If the first player in the ordered list is greater than the second, then the first player wins
				return new List<IPlayer> { OrderedPlayerList[0] };
			}
			else
			{
				// Otherwise, it was a tie, return both players
				return new List<IPlayer> { OrderedPlayerList[0], OrderedPlayerList[1] };
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
