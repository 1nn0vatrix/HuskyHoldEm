using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	public class Player
	{
		public string Name { get; set; }

		public int Chips { get; set; } = 0;

		public Hand Hand = new Hand();

		public Player(string name)
		{
			Name = name;
			Chips = 50;
		}

		/// <summary>
		/// Used when a player STAYS (and needs to add to their bet) or RAISES (increases the bet)
		/// </summary>
		/// <param name="chipsBet">Amount the player wants to add to the pot.</param>
		/// <returns>True if they have enough chips to bet, false if they do not or if their bet was zero.</returns>
		public bool Bet(int chipsBet)
		{
			if (chipsBet <= 0 || chipsBet > Chips)
			{
				return false;
			}

			if (Chips - chipsBet >= 0)
			{
				Chips -= chipsBet;
				return true;
			}

			return false;
		}
	}
}
