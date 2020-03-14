﻿using HuskyHoldEm;
using static HuskyHoldEm.NetworkUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HuskyHoldemServer
{
	public class NetworkPlayer : IPlayer
	{
		public string Name { get; set; }

		public int Chips { get; set; } = 0;

		public Hand Hand { get; private set; } = new Hand();

		public RequestHandler RequestHandler { get; }

		public NetworkPlayer(string name, RequestHandler reqHand)
		{
			Name = name;
			Chips = 50;
			RequestHandler = reqHand;
		}

		public int GetChoice()
		{
			// Ask the player for their choice and wait for the result (read the network packet)
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.REQUEST_MOVE, true, new List<object>() { "pls make move" }));
			WritePacket(RequestHandler.Socket, jsonResponse);

			// The Take method blocks if there are no items in the collection and unblocks as soon as a new item is added to the collection.
			Packet packet = RequestHandler.PacketQueue.Take();
			return int.Parse(packet.DataList[0].ToString());
		}

		public void GiveCard(Card card)
		{
			Hand.AddCard(card);
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.GIVE_CARD, true, new List<object>() { card }));
			WritePacket(RequestHandler.Socket, jsonResponse);
		}

		public void AdjustChips(int amount)
		{
			Chips += amount;
			// Tell the player their new Chip amount
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.ADJUST_CHIPS, true, new List<object>() { Chips }));
			WritePacket(RequestHandler.Socket, jsonResponse);
		}

		public void SendMessage(string message)
		{
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.DISPLAY_MESSAGE, true, new List<object>() { message }));
			WritePacket(RequestHandler.Socket, jsonResponse);
		}

		public void AnnounceWinner(string winnerName, Hand winnerHand, string winnerWinnings)
		{
			// TODO: Send the nice message with the hand and all. For now, just send a string like SendMessage.
			string jsonResponse = JsonConvert.SerializeObject(new Packet(Command.DISPLAY_MESSAGE, true, new List<object>() { $"{winnerName} won!" }));
			WritePacket(RequestHandler.Socket, jsonResponse);
		}
	}
}
