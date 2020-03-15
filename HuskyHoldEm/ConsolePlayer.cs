using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	class ConsolePlayer : IPlayer
	{
		public string Name { get; set; }

		public int Chips { get; set; } = 0;

		public Hand Hand { get; } = new Hand();

		public ConsolePlayer(string name)
		{
			Name = name;
			Chips = 50;
		}

		public int GetChoice()
		{
			Console.WriteLine($"{Name}, you have {Chips} chips...\nYour hand is:");
			Hand.ShowHand();
			Console.WriteLine("\nSTAY (0), FOLD (-1), or RAISE (#)?");
			return int.Parse(Console.ReadLine());
			// TODO: Ensure they don't bet more than they have
		}

		public void GiveCard(Card card)
		{
			Hand.AddCard(card);
		}

		public void AdjustChips(int amount)
		{
			Chips += amount;
		}

		public void SendMessage(string message)
		{
			Console.WriteLine(message);
		}

		public void ShowHands(List<IPlayer> players)
		{
			foreach (IPlayer player in players)
			{
				Console.Write(player.Name + " has ");
				player.Hand.ShowHand();
				Console.WriteLine();
			}
		}

		public void AnnounceWinner(string winnerName, Hand winnerHand, string winnerWinnings)
		{
			if (winnerHand.GetCards().Count == 5)
			{
				Console.Write($"The winner is... {winnerName}! Their winning hand is ");
				winnerHand.PrintRanking();
			}
			Console.WriteLine(winnerWinnings);
		}
	}
}
