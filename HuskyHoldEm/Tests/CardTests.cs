using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm.Tests
{
	class CardGameTests
	{
		public void RunAllTests()
		{
			CardValueTests();
			RankingTests();
		}

		const string CARDS_FAILED = "Card value comparison test failed.";
		const string RANKING_FAILED = "Card value comparison test failed.";


		void CardValueTests()
		{
			// Test that all suites are equal to each other for the same rank
			foreach (Suit suit1 in Enum.GetValues(typeof(Suit)))
			{
				foreach (Suit suit2 in Enum.GetValues(typeof(Suit)))
				{
					foreach (Rank rank in Enum.GetValues(typeof(Rank)))
					{
						Asserts.Assert(new Card(rank, suit1).CompareTo(new Card(rank, suit2)), 0, CARDS_FAILED);
					}
				}
			}
			// Test that A > K > Q > J > 10 > ... > 2
			foreach (Suit suit in Enum.GetValues(typeof(Suit)))
			{
				Asserts.Assert(new Card(Rank.Ace, suit).CompareTo(new Card(Rank.King, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.King, suit).CompareTo(new Card(Rank.Queen, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Queen, suit).CompareTo(new Card(Rank.Jack, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Jack, suit).CompareTo(new Card(Rank.Ten, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Ten, suit).CompareTo(new Card(Rank.Nine, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Nine, suit).CompareTo(new Card(Rank.Eight, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Eight, suit).CompareTo(new Card(Rank.Seven, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Seven, suit).CompareTo(new Card(Rank.Six, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Six, suit).CompareTo(new Card(Rank.Five, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Five, suit).CompareTo(new Card(Rank.Four, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Four, suit).CompareTo(new Card(Rank.Three, suit)), 1, CARDS_FAILED);
				Asserts.Assert(new Card(Rank.Three, suit).CompareTo(new Card(Rank.Two, suit)), 1, CARDS_FAILED);
			}
		}

		void RankingTests()
		{
			List<Card> royalFlush = new List<Card> { new Card(Rank.Ace, Suit.Club),
				                                     new Card(Rank.King, Suit.Club),
													 new Card(Rank.Queen, Suit.Club),
													 new Card(Rank.Jack, Suit.Club),
													 new Card(Rank.Ten, Suit.Club) };

			List<Card> straightFlush = new List<Card> { new Card(Rank.Nine, Suit.Club),
														new Card(Rank.King, Suit.Club),
														new Card(Rank.Queen, Suit.Club),
														new Card(Rank.Jack, Suit.Club),
														new Card(Rank.Ten, Suit.Club) };

			List<Card> straightFlush2 = new List<Card> { new Card(Rank.Nine, Suit.Club),
					                                     new Card(Rank.Eight, Suit.Club),
														 new Card(Rank.Queen, Suit.Club),
														 new Card(Rank.Jack, Suit.Club),
														 new Card(Rank.Ten, Suit.Club) };

			List<Card> fourOfAKind = new List<Card> { new Card(Rank.Nine, Suit.Club),
													  new Card(Rank.Nine, Suit.Spade),
				                                      new Card(Rank.Nine, Suit.Heart),
				                                      new Card(Rank.Nine, Suit.Diamond),
				                                      new Card(Rank.Ten, Suit.Club) };

			List<Card> fourOfAKind2 = new List<Card> { new Card(Rank.Nine, Suit.Club),
				                                       new Card(Rank.Nine, Suit.Spade),
													   new Card(Rank.Nine, Suit.Heart),
													   new Card(Rank.Nine, Suit.Diamond),
													   new Card(Rank.Eight, Suit.Club) };

			List<Card> fourOfAKind3 = new List<Card> { new Card(Rank.Seven, Suit.Club),
													   new Card(Rank.Seven, Suit.Spade),
													   new Card(Rank.Seven, Suit.Heart),
													   new Card(Rank.Seven, Suit.Diamond),
													   new Card(Rank.Eight, Suit.Club) };

			List<Card> fullHouse = new List<Card> { new Card(Rank.Nine, Suit.Club),
				                                    new Card(Rank.Nine, Suit.Spade),
													new Card(Rank.Nine, Suit.Heart),
													new Card(Rank.Ten, Suit.Diamond),
													new Card(Rank.Ten, Suit.Club) };

			List<Card> fullHouse2 = new List<Card> { new Card(Rank.Nine, Suit.Club),
													 new Card(Rank.Nine, Suit.Spade),
													 new Card(Rank.Nine, Suit.Heart),
													 new Card(Rank.Eight, Suit.Diamond),
													 new Card(Rank.Eight, Suit.Club) };

			List<Card> fullHouse3 = new List<Card> { new Card(Rank.Eight, Suit.Club),
													 new Card(Rank.Eight, Suit.Spade),
													 new Card(Rank.Eight, Suit.Heart),
													 new Card(Rank.Seven, Suit.Diamond),
													 new Card(Rank.Seven, Suit.Club) };

			List<Card> flush = new List<Card> { new Card(Rank.Nine, Suit.Club),
				                                new Card(Rank.King, Suit.Club),
												new Card(Rank.Four, Suit.Club),
												new Card(Rank.Six, Suit.Club),
												new Card(Rank.Ten, Suit.Club) };

			List<Card> flush2 = new List<Card> { new Card(Rank.Nine, Suit.Heart),
											 	 new Card(Rank.Queen, Suit.Heart),
										  		 new Card(Rank.Four, Suit.Heart),
												 new Card(Rank.Six, Suit.Heart),
												 new Card(Rank.Ten, Suit.Heart) };

			List<Card> straight = new List<Card> { new Card(Rank.Nine, Suit.Club),
				                                   new Card(Rank.King, Suit.Diamond),
												   new Card(Rank.Queen, Suit.Spade),
												   new Card(Rank.Jack, Suit.Heart),
												   new Card(Rank.Ten, Suit.Club) };

			List<Card> straight2 = new List<Card> { new Card(Rank.Eight, Suit.Club),
												    new Card(Rank.Nine, Suit.Diamond),
												    new Card(Rank.Queen, Suit.Spade),
												    new Card(Rank.Jack, Suit.Heart),
												    new Card(Rank.Ten, Suit.Club) };

			List<Card> threeOfAKind = new List<Card> { new Card(Rank.Nine, Suit.Club),
                                                       new Card(Rank.Nine, Suit.Spade),
													   new Card(Rank.Nine, Suit.Heart),
													   new Card(Rank.Three, Suit.Diamond),
													   new Card(Rank.Ten, Suit.Club) };

			List<Card> threeOfAKind2 = new List<Card> { new Card(Rank.Nine, Suit.Club),
													    new Card(Rank.Nine, Suit.Spade),
													    new Card(Rank.Nine, Suit.Heart),
													    new Card(Rank.Two, Suit.Diamond),
													    new Card(Rank.Ten, Suit.Club) };

			List<Card> threeOfAKind3 = new List<Card> { new Card(Rank.Eight, Suit.Club),
														new Card(Rank.Eight, Suit.Spade),
														new Card(Rank.Eight, Suit.Heart),
														new Card(Rank.Two, Suit.Diamond),
														new Card(Rank.Ten, Suit.Club) };

			List<Card> twoPair = new List<Card> { new Card(Rank.Nine, Suit.Club),
				                                  new Card(Rank.Nine, Suit.Spade),
												  new Card(Rank.Four, Suit.Heart),
												  new Card(Rank.Four, Suit.Diamond),
												  new Card(Rank.Ten, Suit.Club) };

			List<Card> pair = new List<Card> { new Card(Rank.Nine, Suit.Club),
				                               new Card(Rank.Nine, Suit.Spade),
											   new Card(Rank.Four, Suit.Heart),
											   new Card(Rank.Three, Suit.Diamond),
											   new Card(Rank.Ten, Suit.Club) };

			List<Card> highCard = new List<Card> { new Card(Rank.Nine, Suit.Club),
				                                   new Card(Rank.Two, Suit.Spade),
												   new Card(Rank.Four, Suit.Heart),
												   new Card(Rank.Three, Suit.Diamond),
												   new Card(Rank.Ten, Suit.Club) };


			/*List<Card> hand1cards = fourOfAKind;
			List<Card> hand2cards = fourOfAKind3;

			Hand hand1 = new Hand(hand1cards);
			Hand hand2 = new Hand(hand2cards);

			Console.WriteLine($"hand1: ");
			hand1.PrintRanking();
			Console.WriteLine($"\nhand2: ");
			hand2.PrintRanking();

			Console.WriteLine("\n\ncomparing hand1 to hand2: " + hand1.CompareTo(hand2));
			Console.WriteLine("\ncomparing hand2 to hand1: " + hand2.CompareTo(hand1));*/

			// Check ties
			Asserts.Assert(new Hand(royalFlush).CompareTo(new Hand(royalFlush)), 0, RANKING_FAILED);
			Asserts.Assert(new Hand(straightFlush).CompareTo(new Hand(straightFlush)), 0, RANKING_FAILED);
			Asserts.Assert(new Hand(fourOfAKind).CompareTo(new Hand(fourOfAKind)), 0, RANKING_FAILED);
			Asserts.Assert(new Hand(fullHouse).CompareTo(new Hand(fullHouse)), 0, RANKING_FAILED);
			Asserts.Assert(new Hand(flush).CompareTo(new Hand(flush)), 0, RANKING_FAILED);
			Asserts.Assert(new Hand(straight).CompareTo(new Hand(straight)), 0, RANKING_FAILED);
			Asserts.Assert(new Hand(threeOfAKind).CompareTo(new Hand(threeOfAKind)), 0, RANKING_FAILED);
			Asserts.Assert(new Hand(twoPair).CompareTo(new Hand(twoPair)), 0, RANKING_FAILED);
			Asserts.Assert(new Hand(pair).CompareTo(new Hand(pair)), 0, RANKING_FAILED);
			Asserts.Assert(new Hand(highCard).CompareTo(new Hand(highCard)), 0, RANKING_FAILED);
		}
	}
}
