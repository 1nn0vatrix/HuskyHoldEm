using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	public interface IPlayer
	{
		// Player's hand
		string Name { get; }

		// Player's chips
		int Chips { get; }

		// Player's hand
		Hand Hand { get; }

		// Get the player's choice (STAY (0), FOLD (-1), RAISE(int value of how much to raise))
		int GetChoice();

		// Add and subtract the player's chips
		void AdjustChips(int amount);

		// Get the player a card
		void GiveCard(Card card);

		// Send the player a message
		void SendMessage(string message);

		// Send the player all the player names and their hands
		void ShowHands(List<IPlayer> players);

		// Display the winner
		void AnnounceWinner(string winnerName, Hand winnerHand, string winnerWinnings);
	}
}
