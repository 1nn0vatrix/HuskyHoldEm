﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	public class Game
	{
		public int MaxPlayers { get; }
		public List<IPlayer> IPlayerList { get; set; }
		public bool InProgress { get; set; }
		private Deck deck;
		private int pot = 0;

		Dictionary<IPlayer, bool> playersStayed = new Dictionary<IPlayer, bool>();
		Dictionary<IPlayer, int> playersCurrentPayments = new Dictionary<IPlayer, int>();
		List<IPlayer> connectedPlayers = new List<IPlayer>();

		public event Action<Game, IPlayer> GameFinished;

		public Game(List<IPlayer> players, int numberOfPlayers)
		{
			MaxPlayers = numberOfPlayers;
			IPlayerList = players;
			deck = new Deck();
			deck.Shuffle();
			deck.Print();
		}

		public void StartGame()
		{
			InProgress = true;

			foreach (IPlayer player in IPlayerList)
			{
				playersStayed.Add(player, false);
				playersCurrentPayments.Add(player, 0);
				connectedPlayers.Add(player);
				player.Hand.ClearHand();
			}

			GameLoop();

			InProgress = false;
		}

		// Remove disconnected players from game logic collections
		public void RemovePlayer(IPlayer player)
		{
			if (IPlayerList.IndexOf(player) != -1)
			{
				IPlayerList[IPlayerList.IndexOf(player)] = null;
			}
			playersStayed.Remove(player);
			playersCurrentPayments.Remove(player);
			connectedPlayers.Remove(player);
		}

		public void GameLoop()
		{
			for (int round = 0; round < 4; round++)
			{
				// Set the absolute maximum that can be bet for the round based on the player with the lowest coins
				int absoluteMaximum = int.MaxValue;

				// Reset each player to not being stayed and their round's payments to zero.
				// Check for the absolute maximum that can be bet.
				// Give players cards
				foreach (IPlayer player in connectedPlayers)
				{
					// Reset which players have stayed and what they're paying for this round.
					playersStayed[player] = false;
					playersCurrentPayments[player] = 0;

					string roundTitle = round < 3 ? $"Round {round + 1}" : "Final round";

					player.SendMessage("\n+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
					player.SendMessage(roundTitle + "! Handing out cards...");
					player.SendMessage("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n");

					// If the round is the starting round, be sure to collect their ante.
					if (round == 0)
					{
						player.AdjustChips(-2);  // This should be checked before they join the game. If they don't have at least two chips, they can't play.
						player.GiveCard(deck.GetCard());
						player.GiveCard(deck.GetCard());
						pot += 2;
					}
					else
					{
						player.GiveCard(deck.GetCard());
					}

					if (player.Chips < absoluteMaximum)
					{
						absoluteMaximum = player.Chips;
					}
				}

				ShowRemainingPlayers();

				// Initialize the round's player loop variables
				bool isRoundDone = false;
				IPlayer maxBetter = null;
				int playerIndex = 0;
				int currentMaxBet = 0;

				while (!isRoundDone)
				{
					// Check if there's only one player left, if so, they've won.
					if (connectedPlayers.Count == 1)
					{
						IPlayer lonelyWinner = connectedPlayers.First();
						lonelyWinner.AdjustChips(pot);
						lonelyWinner.AnnounceWinner(lonelyWinner.Name, lonelyWinner.Hand, $"\n{lonelyWinner.Name}, everyone folded! You win {pot} chips, and now have {lonelyWinner.Chips} chips.");
						GameFinished?.Invoke(this, lonelyWinner);
						return;
					}

					// Check if everyone stayed except for the current max better, if so, we can move on to the next round.
					if (!playersStayed.Where(p => p.Key != maxBetter).Any(p => p.Value == false))
					{
						isRoundDone = true;
						break;
					}

					// Makes sure to update the absolute maximum incase the lowest player bet more chips
					// Reset it to maximum value, in case the lowest player folded and there can be a bigger absolute maximum
					absoluteMaximum = int.MaxValue;
					foreach (IPlayer player in connectedPlayers)
					{
						if (player.Chips < absoluteMaximum)
						{
							absoluteMaximum = player.Chips;
						}
					}

					IPlayer currentPlayer = GetNextPlayer(ref playerIndex);

					foreach (IPlayer player in connectedPlayers)
					{
						if (!player.Equals(currentPlayer))
							player.SendMessage($"\n--> It's {currentPlayer.Name}'s turn now.");
					}

					string maxBetRound = $"\nThe maximum bet for this round is currently {currentMaxBet} chips.";
					string absMaxBet = $"The maximum bet for the game is {absoluteMaximum} chips.";

					currentPlayer.SendMessage($"{maxBetRound}\n{absMaxBet}");

					int playerChoice = currentPlayer.GetChoice();

					if (playerChoice > 0)
					{
						bool isValidChoice = false;
						// Ensure they can actually raise by what they claim.
						while (!isValidChoice)
						{
							if (playerChoice + currentMaxBet > currentPlayer.Chips + playersCurrentPayments[currentPlayer])
							{
								currentPlayer.SendMessage($"You can't raise for more than what you have plus what's currently bet! Try again.\n{maxBetRound}\n{absMaxBet}");
								playerChoice = currentPlayer.GetChoice();
							}
							else if (playerChoice > absoluteMaximum)
							{
								currentPlayer.SendMessage($"You can't raise for more than the absolute maximum so all players can play! Try again.\n{maxBetRound}\n{absMaxBet}");
								playerChoice = currentPlayer.GetChoice();
							}
							else
							{
								isValidChoice = true;
							}
						}
					}

					if (playerChoice < 0)
					{
						// Player folds
						foreach (IPlayer player in connectedPlayers)
						{
							if (!player.Equals(currentPlayer))
								player.SendMessage($"----> {currentPlayer.Name} folded!");
						}

						if (maxBetter != null && maxBetter.Equals(currentPlayer))
						{
							maxBetter = null;
						}

						RemovePlayer(currentPlayer);

						if (connectedPlayers.Count() > 0)
						{
							// Update who the current player is
							playerIndex = playerIndex >= IPlayerList.Count - 1 ? 0 : playerIndex + 1;
							currentPlayer = GetNextPlayer(ref playerIndex);
						}
						else
						{
							// Everyone folded.
							GameFinished?.Invoke(this, currentPlayer);
							return;
						}

						// Move on in the game
						continue;
					}

					if (playerChoice == 0)
					{
						foreach (IPlayer player in connectedPlayers)
						{
							if (!player.Equals(currentPlayer))
								player.SendMessage($"----> {currentPlayer.Name} stays.");
						}

						// Player stays
						playersStayed[currentPlayer] = true;
					}

					if (playerChoice > 0)
					{
						foreach (IPlayer player in connectedPlayers)
						{
							if (!player.Equals(currentPlayer))
								player.SendMessage($"----> {currentPlayer.Name} raises by {playerChoice} chips!");
						}

						// Player raises
						playersStayed[currentPlayer] = false;
						maxBetter = currentPlayer;
						currentMaxBet += playerChoice;

						// Reset the stayed players, they must all agree to stay again
						foreach (IPlayer player in connectedPlayers)
						{
							playersStayed[player] = false;
						} 
					}

					// Calculate what the player owes, have them pay, and update the pot.
					int playerOwes = currentMaxBet - playersCurrentPayments[currentPlayer];
					currentPlayer.AdjustChips(playerOwes * -1);
					pot += playerOwes;
					playersCurrentPayments[currentPlayer] += playerOwes;

					// Update who the current player is
					playerIndex = playerIndex >= IPlayerList.Count - 1 ? 0 : playerIndex + 1;
					currentPlayer = GetNextPlayer(ref playerIndex);
				}
			}

			// Display everyone's hands.
			foreach (IPlayer player in connectedPlayers)
			{
				player.ShowHands(connectedPlayers);
			}

			// Get the winner.
			// TODO: Split the pot if there's a tie.
			IPlayer winner = GetWinner()[0];
			winner.AdjustChips(pot);

			foreach (IPlayer player in connectedPlayers)
			{
				if (!player.Equals(winner))
					player.AnnounceWinner(winner.Name, winner.Hand, $"\n{winner.Name} wins {pot} chips, they now have {winner.Chips} chips.");
				else
					player.AnnounceWinner(winner.Name, winner.Hand, $"\nYou won the pot of {pot} chips! You now have {winner.Chips} chips.");
			}

			GameFinished?.Invoke(this, winner);
		}		

		// Circular player list logic
		private IPlayer GetNextPlayer(ref int currentIndex)
		{
			IPlayer nextPlayer = IPlayerList[currentIndex];
			while (nextPlayer == null)
			{
				currentIndex = currentIndex >= IPlayerList.Count - 1 ? 0 : currentIndex + 1;
				nextPlayer = IPlayerList[currentIndex];
			}
			return nextPlayer;
		}

		// Show remaining players
		private void ShowRemainingPlayers()
		{
			string remainingPlayers = "\nRemaining players: ";

			foreach (IPlayer player in connectedPlayers)
			{
				remainingPlayers += player.Name + " ";
			}

			foreach (IPlayer player in connectedPlayers)
			{
				player.SendMessage(remainingPlayers);
			}
		}

		public List<IPlayer> GetWinner()
		{
			if (connectedPlayers?.Count == 0)
			{
				return null;
			}

			// Order the players by their hand rankings
			List<IPlayer> OrderedPlayerList = connectedPlayers.OrderByDescending(p => p.Hand).ToList();

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
