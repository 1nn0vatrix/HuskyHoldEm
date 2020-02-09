using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	class Player
	{
		public string Name { get; set; }

		public int Chips { get; set; } = 0;

		public Hand Hand = new Hand();

		public Player(string name)
		{
			Name = name;
		}
	}
}
