using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	public class ClientPlayer
	{
		// A simple player object for the client to reference its name, chips, and hand. 

		public string Name { get; set; }

		public int Chips { get; set; } = 0;

		public Hand Hand { get; private set; } = new Hand();

		public ClientPlayer(string name)
		{
			Name = name;
			Chips = 50;
		}
	}
}
