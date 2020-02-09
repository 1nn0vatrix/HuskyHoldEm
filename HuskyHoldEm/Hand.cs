using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm
{
	public enum PokerHand
	{
		// If multiple players have the same hand ranking, the highest value wins. (Aces high!)
		HighCard,       // The highest value card in your hand
		Pair,           // Two cards of the same rank
		TwoPair,        // Two different pairs (2, 2, Q, Q)
		ThreeOfAKind,   // Three cards of the same rank
		Straight,       // Five cards with rank in sequence, but not the same suit (2, 3, 4, 5, 6)
		Flush,          // Five cards with the same suit, but not in a sequence
		FullHouse,      // Three of a kind with a pair (3, 3, 3, A, A)
		FourOfAKind,    // Four cards of the same rank
		StraightFlush,  // Five cards in a sequence with the same suit
		RoyalFlush      // (10, J, Q, K, A) in the same suit
	}

	public static class PokerHandExtensions
	{
		public static string ToFriendlyString(this PokerHand hand)
		{
			switch (hand)
			{
				case PokerHand.HighCard:
					return "High Card";
				case PokerHand.Pair:
					return "Pair";
				case PokerHand.TwoPair:
					return "Two Pair";
				case PokerHand.ThreeOfAKind:
					return "Three of a Kind";
				case PokerHand.Straight:
					return "Straight";
				case PokerHand.Flush:
					return "Flush";
				case PokerHand.FullHouse:
					return "Full House";
				case PokerHand.FourOfAKind:
					return "Four of a Kind";
				case PokerHand.StraightFlush:
					return "Straight Flush";
				case PokerHand.RoyalFlush:
					return "Royal Flush";
				default:
					return "Unknown hand!";
			}
		}
	}

	class Hand : IComparable<Hand>
	{
		List<Card> Cards { get; set; } = new List<Card>();

		public Hand()
		{

		}

		// Do not use this for games. It's for testing purposes only.
		public Hand(List<Card> cards)
		{
			if (cards == null || cards.Count < 5)
			{
				Console.WriteLine("Invalid hand. Need 5 cards.");
			}

			Cards = cards;
		}

		public void AddCard(Card c)
		{
			Cards.Add(c);
			if (Cards.Count == 5)
			{
				Cards.Sort();
			}
		}

		public void ShowHand()
		{
			foreach (Card c in Cards)
			{
				c.Print();
			}
		}

		public void PrintRanking()
		{
			Tuple<PokerHand, List<Card>> rankingTuple = GetRanking();
			string ranking = rankingTuple.Item1.ToFriendlyString();

			Console.Write(ranking + ": ");

			foreach (Card c in rankingTuple.Item2)
			{
				c.Print();
			}

		}

		private Tuple<PokerHand, List<Card>> GetRanking()
		{
			List<Card> RankingCards = Cards;

			RankingCards.Sort();  // Sort by rank (2, 3, 4, 5, 6, 7, 8, 9, 10, J, Q, K, A)
			var suits = Cards.GroupBy(c => c.Suit);
			var ranks = Cards.GroupBy(c => c.Rank);
			Rank start = Cards[0].Rank;  // Lowest value card rank on hand

			// Royal Flush check
			if (Cards[0].Rank == Rank.Ten)
			{
				if (suits.Count() == 1)
				{
					if (Cards[1].Rank == Rank.Jack && Cards[2].Rank == Rank.Queen && Cards[3].Rank == Rank.King && Cards[4].Rank == Rank.Ace)
					{
						return new Tuple<PokerHand, List<Card>>(PokerHand.RoyalFlush, RankingCards);
					}
				}
			}
			// Straight Flush check
			if (suits.Count() == 1)
			{
				if (Cards[1].Rank == start + 1 && Cards[2].Rank == start + 2 && Cards[3].Rank == start + 3 && Cards[4].Rank == start + 4)
				{
					return new Tuple<PokerHand, List<Card>>(PokerHand.StraightFlush, RankingCards);
				}
			}
			// Four of a Kind check
			if (ranks.Count() == 2)
			{
				if (ranks.Any(r => r.Count() == 4))
				{
					RankingCards = ranks.First(r => r.Count() == 4).ToList();
					return new Tuple<PokerHand, List<Card>>(PokerHand.FourOfAKind, RankingCards);
				}
			}
			// Full House check
			if (ranks.Count() == 2)
			{
				if (ranks.Any(r => r.Count() == 3) && ranks.Any(r => r.Count() == 2))
				{
					return new Tuple<PokerHand, List<Card>>(PokerHand.FullHouse, RankingCards);
				}
			}
			// Flush check
			if (suits.Count() == 1)
			{
				return new Tuple<PokerHand, List<Card>>(PokerHand.Flush, RankingCards);
			}
			// Straight check
			if (Cards[1].Rank == start + 1 && Cards[2].Rank == start + 2 && Cards[3].Rank == start + 3 && Cards[4].Rank == start + 4)
			{
				return new Tuple<PokerHand, List<Card>>(PokerHand.Straight, RankingCards);
			}
			// Three of a Kind check
			if (ranks.Any(r => r.Count() == 3))
			{
				RankingCards = ranks.First(r => r.Count() == 3).ToList();
				return new Tuple<PokerHand, List<Card>>(PokerHand.ThreeOfAKind, RankingCards);
			}
			int pairs = 0;
			foreach (var rank in ranks)
			{
				if (rank.Count() == 2)
				{
					pairs++;
				}
			}
			// Two Pair check
			if (pairs == 2)
			{
				RankingCards = new List<Card>();
				foreach (var pair in ranks.Where(r => r.Count() == 2))
				{
					RankingCards.AddRange(pair);
				}
				return new Tuple<PokerHand, List<Card>>(PokerHand.TwoPair, RankingCards);
			}
			// Pair check
			if (pairs == 1)
			{
				RankingCards = ranks.First(r => r.Count() == 2).ToList();
				return new Tuple<PokerHand, List<Card>>(PokerHand.Pair, RankingCards);
			}
			// High Card
			return new Tuple<PokerHand, List<Card>>(PokerHand.HighCard, new List<Card> { RankingCards.Last() });
		}

		public int CompareTo(Hand theirs)
		{
			if (theirs == null)
			{
				return 1;
			}

			var _myRanking = GetRanking();
			var _theirRanking = theirs.GetRanking();

			PokerHand myRanking = _myRanking.Item1;
			List<Card> myRankingCards = _myRanking.Item2;

			PokerHand theirRanking = _theirRanking.Item1;
			List<Card> theirRankingCards = _theirRanking.Item2;

			var myRanks = myRankingCards.GroupBy(c => c.Rank);
			var theirRanks = theirRankingCards.GroupBy(c => c.Rank);

			if (myRanking > theirRanking)
			{
				return 1;

			}
			if (myRanking < theirRanking)
			{
				return -1;
			}

			List<Card> myLeftoverCards = Cards.Except(myRankingCards).ToList();
			myLeftoverCards.Sort();

			List<Card> theirLeftoverCards = theirs.Cards.Except(theirRankingCards).ToList();
			theirLeftoverCards.Sort();

			//Both the same ranking, use tie rules.
			switch (myRanking)
			{
				case PokerHand.RoyalFlush:
					return 0;
				case PokerHand.StraightFlush:
					{
						int compare = myRankingCards.Last().CompareTo(theirRankingCards.Last());
						if (compare != 0)
						{
							return compare;
						}
						return 0;
					}
				case PokerHand.FourOfAKind:
					{
						int compare = myRankingCards.Last().CompareTo(theirRankingCards.Last());
						if (compare != 0)
						{
							return compare;
						}
						return myLeftoverCards.Last().CompareTo(theirLeftoverCards.Last());
					}
				case PokerHand.FullHouse:
					{
						// Compare Three of a Kind
						int compare = myRanks.Where(r => r.Count() == 3).First().First().CompareTo(theirRanks.Where(r => r.Count() == 3).First().First());
						if (compare != 0)
						{
							return compare;
						}
						// Compare Pair
						return myRanks.Where(r => r.Count() == 2).First().First().CompareTo(theirRanks.Where(r => r.Count() == 2).First().First());
					}
				case PokerHand.Flush:
					{
						int compare = myRankingCards[4].CompareTo(theirRankingCards[4]);
						if (compare != 0)
						{
							compare = myRankingCards[3].CompareTo(theirRankingCards[3]);
							if (compare != 0)
							{
								compare = myRankingCards[2].CompareTo(theirRankingCards[2]);
								if (compare != 0)
								{
									compare = myRankingCards[1].CompareTo(theirRankingCards[1]);
									if (compare != 0)
									{
										compare = myRankingCards[0].CompareTo(theirRankingCards[0]);
										if (compare != 0)
										{
											return compare;
										}
									}
								}
							}
						}
						return 0;
					}
				case PokerHand.Straight:
					{
						int compare = myRankingCards.Last().CompareTo(theirRankingCards.Last());
						if (compare != 0)
						{
							return compare;
						}
						return 0;
					}
				case PokerHand.ThreeOfAKind:
					{
						int compare = myRankingCards.Last().CompareTo(theirRankingCards.Last());
						if (compare != 0)
						{
							return compare;
						}
						compare = myLeftoverCards[1].CompareTo(theirLeftoverCards[1]);
						if (compare != 0)
						{
							compare = myLeftoverCards[0].CompareTo(theirLeftoverCards[0]);
							if (compare != 0)
							{
								return compare;
							}
						}
						return 0;
					}
				case PokerHand.TwoPair:
					{
						// Compare 1st highest pair
						int compare = myRanks.Where(r => r.Count() == 2).Last().First().CompareTo(theirRanks.Where(r => r.Count() == 2).Last().First());
						if (compare != 0)
						{
							return compare;
						}
						// Compare 2nd highest pair
						return myRanks.Where(r => r.Count() == 2).First().First().CompareTo(theirRanks.Where(r => r.Count() == 2).First().First());
					}
				case PokerHand.Pair:
					{
						int compare = myRankingCards.Last().CompareTo(theirRankingCards.Last());
						if (compare != 0)
						{
							return compare;
						}
						compare = myLeftoverCards[2].CompareTo(theirLeftoverCards[2]);
						if (compare != 0)
						{
							compare = myLeftoverCards[1].CompareTo(theirLeftoverCards[1]);
							if (compare != 0)
							{
								compare = myLeftoverCards[0].CompareTo(theirLeftoverCards[0]);
								if (compare != 0)
								{
									return compare;
								}
							}
						}
						return 0;
					}
				case PokerHand.HighCard:
					{
						int compare = myRankingCards.Last().CompareTo(theirRankingCards.Last());
						if (compare != 0)
						{
							return compare;
						}
						compare = myLeftoverCards[3].CompareTo(theirLeftoverCards[3]);
						if (compare != 0)
						{
							compare = myLeftoverCards[2].CompareTo(theirLeftoverCards[2]);
							if (compare != 0)
							{
								compare = myLeftoverCards[1].CompareTo(theirLeftoverCards[1]);
								if (compare != 0)
								{
									compare = myLeftoverCards[0].CompareTo(theirLeftoverCards[0]);
									if (compare != 0)
									{
										return compare;
									}
								}
							}
						}
						return 0;
					}
			}
			return 0; // How? Insanity. 
		}

		/*
		 * [ Ties ]
		 * Royal Flush:
		 *    The chance of a royal flush tie is extremely rare. If this happens, the winners split the pot.
		 *    
		 * Straight Flush:
		 *    The Straight Flush with the highest rank wins. If all hands have the same highest rank, the winners split the pot.
		 *    
		 * Four of a Kind:
		 *    The Four of a Kind with the highest rank wins. If all hands have the same highest rank, the 5th card is used to break the tie. If the 5th card is a tie, the winners split the pot.
		 *    
		 * Full House:
		 *    The Three of a Kind with hte highest rank wins. If the 3oaK have the same highest rank, the Pair with the highest rank wins. If the Pair is a tie, the winners split the pot.
		 *    
		 * Flush:
		 *    The Flush with the highest rank wins. If the highest rank is a tie, the 2nd highest rank wins. If the 2nd highest is a tie, the 3rd highest rank wins, etc. 
		 *    
		 * Straight:
		 *	  The Straight with the highest rank wins. If the highest rank is a tie, the winners split the pot.
		 *	  
		 * Three of a Kind:
		 *    The Three of a Kind with the highest rank wins. If the highest rank is a tie, the remaining card(s) are used to break the additional ties.
		 *    
		 * Two Pair:
		 *     The Pair with the highest rank wins. If the first pairs have the same rank, the 2nd pair highest rank wins. The remaining card(s) are used to break additional ties.
		 *     
		 * Pair:
		 *     The Pair with the highest rank wins. The remaining card(s) are used to break additional ties.
		 * 
		 * High Card:
		 *	   The highest rank wins. The remaining card(s) are used to break additional ties.
		 */
	}
}
