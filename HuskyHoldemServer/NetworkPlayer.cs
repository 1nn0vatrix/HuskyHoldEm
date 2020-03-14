using HuskyHoldEm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldemServer
{
	public class NetworkPlayer : IPlayer
	{
		public string Name { get; set; }

		public int Chips { get; set; } = 0;

		public Hand Hand { get; private set; } = new Hand();

		public NetworkPlayer(string name) // TODO: add to constructor network stream. the reference to the client socket so we can send/receive packets from here.
		{
			Name = name;
			Chips = 50;
		}

		public int GetChoice()
		{
			// TODO: ask the player for their choice and wait for the result (read the network packet)
			throw new NotImplementedException();
		}

		public void GiveCard(Card card)
		{
			Hand.AddCard(card);
			// TODO: send the card to the client/player
			throw new NotImplementedException();
		}

		public void AdjustChips(int amount)
		{
			// TODO: notify the player their new chip count
			Chips += amount;
			throw new NotImplementedException();
		}

		public void SendMessage(string message)
		{
			// TODO: send a message to the client/player
			throw new NotImplementedException();
		}
	}
}
