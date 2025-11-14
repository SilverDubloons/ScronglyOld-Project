using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class LargeHandEvaluation : MonoBehaviour
{
    public static LargeHandEvaluation instance;
	public TMP_Text dataText;
	
	void Awake()
	{
		instance = this;
	}
	
	public void EvaluateLargeHand(List<CardScript> hand, HandZone handZone, bool evaluatingOnlyCardsUsed)
	{
		int cardsInHand = hand.Count;
		hand.Sort((a, b) => 
		{
			int rankSort = a.rankInt.CompareTo(b.rankInt);
			if(rankSort == 0)
			{
				return a.suitInt.CompareTo(b.suitInt);
			}
			else return rankSort;
		});
		int[] numberOfEachSuit = new int[5];
		for(int card = 0; card < cardsInHand; card++)
		{
			numberOfEachSuit[hand[card].suitInt]++;
		}
		
		bool sevenOfAKind = false;
		//bool flushSeven = false;
		bool hugeHouse = false; // 5oak + pair
		bool wideHouse = false; // 4oak + 3oak
		bool guestHouse = false; // 3oak + 2 pair
		
		// List<handcipt> specialhand = new List<handcript>(); // probably best to do these individually for futureproofing
		
		if(cardsInHand >= 7)
		{
			if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
			{
				sevenOfAKind = true;
				/* for(int suit = 0; suit < 4; suit++)
				{
					if(numberOfEachSuit[4] + numberOfEachSuit[suit] >= 7)
					{
						flushSeven = true;
						break;
					}
				} */
			}
			else
			{
				if((hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt != hand[5].rankInt && hand[5].rankInt == hand[6].rankInt) || (hand[0].rankInt == hand[1].rankInt && hand[1].rankInt != hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt))
				{
					hugeHouse = true;
				}
				else
				{
					if((hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt != hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt) || (hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt != hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt))
					{
						wideHouse = true;
					}
					else
					{
						if((hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt != hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt != hand[5].rankInt && hand[5].rankInt == hand[6].rankInt) || (hand[0].rankInt == hand[1].rankInt && hand[1].rankInt != hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt != hand[5].rankInt && hand[5].rankInt == hand[6].rankInt) || (hand[0].rankInt == hand[1].rankInt && hand[1].rankInt != hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt != hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt))
						{
							guestHouse = true;
						}
					}
				}
			}
		}
		
		bool sixOfAKind = false;
		//bool flushSix = false;
		bool stuffedHouse = false; // 4oak + pair
		bool doubleTriple = false; // two 3oak
		bool tripleDouble = false; // 3 pair
		
		List<CardScript> sixHand = new List<CardScript>();
		
		if(cardsInHand >= 6)
		{
			/* if(flushSeven)
			{
				sixOfAKind = true;
				flushSix = true;
			}
			else */ if(sevenOfAKind)
			{
				sixOfAKind = true;
				/* if(HandIsFlush(hand, 6))
				{
					flushSix = true;
				} */
			}
			else if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt)
			{
				sixOfAKind = true;
				for(int i = 0; i < 6; i++)
				{
					sixHand.Add(hand[i]);
				}
				/* if(HandIsFlush(sixHand, 6))
				{
					flushSix = true;
				} */
			}
			else
			{
				if(cardsInHand >= 7)
				{
					if(hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
					{
						sixOfAKind = true;
						for(int i = 1; i < 7; i++)
						{
							sixHand.Add(hand[i]);
						}
					}
				}
				/* if(sixOfAKind)
				{
					if(HandIsFlush(sixHand, 6))
					{
						flushSix = true;
					}
				}
				else */
				if(!sixOfAKind)
				{
					if(hugeHouse || wideHouse || guestHouse)
					{
						if(hugeHouse || wideHouse)
						{
							stuffedHouse = true;
						}
						if(wideHouse)
						{
							doubleTriple = true;
						}
						if(guestHouse)
						{
							tripleDouble = true;
						}
					}
					else
					{
						if((hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt != hand[4].rankInt && hand[4].rankInt == hand[5].rankInt) || (hand[0].rankInt == hand[1].rankInt && hand[1].rankInt != hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt))
						{
							stuffedHouse = true;
							for(int i = 0; i < 6; i++)
							{
								sixHand.Add(hand[i]);
							}
						}
						else if(cardsInHand >= 7)
						{
							if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[5].rankInt == hand[6].rankInt)
							{
								stuffedHouse = true;
								for(int i = 0; i < 7; i++)
								{
									if(i != 4)
									{
										sixHand.Add(hand[i]);
									}
								}
							}
							else if(hand[0].rankInt == hand[1].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
							{
								stuffedHouse = true;
								for(int i = 0; i < 7; i++)
								{
									if(i != 2)
									{
										sixHand.Add(hand[i]);
									}
								}
							}
							else if((hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[5].rankInt == hand[6].rankInt) || (hand[1].rankInt == hand[2].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt))
							{
								stuffedHouse = true;
								for(int i = 1; i < 7; i++)
								{
									sixHand.Add(hand[i]);
								}
							}
						}
						if(!stuffedHouse)
						{
							
							if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt)
							{
								doubleTriple = true;
								for(int i = 0; i < 6; i++)
								{
									sixHand.Add(hand[i]);
								}
							}
							else
							{
								if(cardsInHand >= 7)
								{
									if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
									{
										doubleTriple = true;
										for(int i = 0; i < 7; i++)
										{
											if(i != 3)
											{
												sixHand.Add(hand[i]);
											}
										}
									}
									else if(hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
									{
										doubleTriple = true;
										for(int i = 1; i < 7; i++)
										{
											sixHand.Add(hand[i]);
										}
									}
								}
							}
							if(!doubleTriple)
							{
								if(hand[0].rankInt == hand[1].rankInt && hand[2].rankInt == hand[3].rankInt && hand[4].rankInt == hand[5].rankInt)
								{
									tripleDouble = true;
									for(int i = 0; i < 6; i++)
									{
										sixHand.Add(hand[i]);
									}
								}
								else
								{
									if(cardsInHand >= 7)
									{
										if(hand[0].rankInt == hand[1].rankInt && hand[2].rankInt == hand[3].rankInt && hand[5].rankInt == hand[6].rankInt)
										{
											tripleDouble = true;
											for(int i = 0; i < 7; i++)
											{
												if(i != 4)
												{
													sixHand.Add(hand[i]);
												}
											}
										}
										else if(hand[0].rankInt == hand[1].rankInt && hand[3].rankInt == hand[4].rankInt && hand[5].rankInt == hand[6].rankInt)
										{
											tripleDouble = true;
											for(int i = 0; i < 7; i++)
											{
												if(i != 2)
												{
													sixHand.Add(hand[i]);
												}
											}
										}
										else if(hand[1].rankInt == hand[2].rankInt && hand[3].rankInt == hand[4].rankInt && hand[5].rankInt == hand[6].rankInt)
										{
											tripleDouble = true;
											for(int i = 1; i < 7; i++)
											{
												sixHand.Add(hand[i]);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		
		bool fiveOfAKind = false;
		//bool flushFive = false;
		
		List<CardScript> fiveOfAKindCards = new List<CardScript>();
		
		if(cardsInHand >= 5)
		{
			/* if(flushSix || flushSeven)
			{
				fiveOfAKind = true;
				flushFive = true;
			}
			else if(sevenOfAKind)
			{
				if(HandIsFlush(hand, 5))
				{
					flushFive = true;
				}
			}
			else  */
			if(sixOfAKind)
			{
				fiveOfAKind = true;
			}
			else if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt)
			{
				fiveOfAKind = true;
				for(int i = 0; i < 5; i++)
				{
					fiveOfAKindCards.Add(hand[i]);
				}
				/* if(HandIsFlush(fiveOfAKindCards, 5))
				{
					flushFive = true;
				} */
			}
			else if(cardsInHand >= 6)
			{
				if(hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt)
				{
					fiveOfAKind = true;
					for(int i = 1; i < 6; i++)
					{
						fiveOfAKindCards.Add(hand[i]);
					}
					/* if(HandIsFlush(fiveOfAKindCards, 5))
					{
						flushFive = true;
					} */
				}
				else if(cardsInHand >= 7)
				{
					if(hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
					{
						fiveOfAKind = true;
						for(int i = 2; i < 7; i++)
						{
							fiveOfAKindCards.Add(hand[i]);
						}
						/* if(HandIsFlush(fiveOfAKindCards, 5))
						{
							flushFive = true;
						} */
					}
				}
			}
		}
		
		bool fourOfAKind = false;
		//bool flushFour = false;
		List<CardScript> fourOfAKindCards = new List<CardScript>();
		
		if(cardsInHand >= 4)
		{
			if(fiveOfAKind)
			{
				fourOfAKind = true;
			}
			else if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt)
			{
				fourOfAKind = true;
				for(int i = 0; i < 4; i++)
				{
					fourOfAKindCards.Add(hand[i]);
				}
			}
			else if(cardsInHand >= 5)
			{
				if(hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt)
				{
					fourOfAKind = true;
					for(int i = 1; i < 5; i++)
					{
						fourOfAKindCards.Add(hand[i]);
					}
				}
				else if(cardsInHand >= 6)
				{
					if(hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt)
					{
						fourOfAKind = true;
						for(int i = 2; i < 6; i++)
						{
							fourOfAKindCards.Add(hand[i]);
						}
					}
					else if(cardsInHand >= 7)
					{
						if(hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
						{
							fourOfAKind = true;
							for(int i = 3; i < 7; i++)
							{
								fourOfAKindCards.Add(hand[i]);
							}
						}
					}
				}
			}
		}
		
		bool fullHouse = false;
		//bool flushHouse = false; // maybe get rid of flush house? it sets a weird expectation that there will be flush version of all 'house' style hands
		List<CardScript> fullHouseCards = new List<CardScript>();
		
		if(cardsInHand >= 5)
		{
			if(hugeHouse || wideHouse || guestHouse || stuffedHouse || doubleTriple)
			{
				fullHouse = true;
			}
			else if(!fourOfAKind) 	// if you have four of a kind (or five of a kind, etc, which means you have four of a kind),
			{						// then you can't have a full house without also having one of the bigger housees
				if((hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[3].rankInt == hand[4].rankInt) || (hand[0].rankInt == hand[1].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt))
				{
					fullHouse = true;
					for(int i = 0; i < 5; i++)
					{
						fullHouseCards.Add(hand[i]);
					}
				}
				else if(cardsInHand >= 6)
				{
					if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[4].rankInt == hand[5].rankInt)
					{
						fullHouse = true;
						for(int i = 0; i < 6; i++)
						{
							if(i != 3)
							{
								fullHouseCards.Add(hand[i]);
							}
						}
					}
					else if(hand[0].rankInt == hand[1].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt)
					{
						fullHouse = true;
						for(int i = 0; i < 6; i++)
						{
							if(i != 2)
							{
								fullHouseCards.Add(hand[i]);
							}
						}
					}
					else if((hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[4].rankInt == hand[5].rankInt) || (hand[1].rankInt == hand[2].rankInt && hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt))
					{
						fullHouse = true;
						for(int i = 1; i < 6; i++)
						{
							fullHouseCards.Add(hand[i]);
						}
					}
					else if(cardsInHand >= 7)
					{
						if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[5].rankInt == hand[6].rankInt)
						{
							fullHouse = true;
							for(int i = 0; i < 7; i++)
							{
								if(i != 3 && i != 4)
								{
									fullHouseCards.Add(hand[i]);
								}
							}
						}
						else if(hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[5].rankInt == hand[6].rankInt)
						{
							fullHouse = true;
							for(int i = 1; i < 7; i++)
							{
								if(i != 4)
								{
									fullHouseCards.Add(hand[i]);
								}
							}
						}
						else if((hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt && hand[5].rankInt == hand[6].rankInt) || (hand[2].rankInt == hand[3].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt))
						{
							fullHouse = true;
							for(int i = 2; i < 7; i++)
							{
								fullHouseCards.Add(hand[i]);
							}
						}
						else if(hand[0].rankInt == hand[1].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
						{
							fullHouse = true;
							for(int i = 0; i < 7; i++)
							{
								if(i != 2 && i != 3)
								{
									fullHouseCards.Add(hand[i]);
								}
							}
						}
						else if(hand[1].rankInt == hand[2].rankInt && hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
						{
							fullHouse = true;
							for(int i = 1; i < 7; i++)
							{
								if(i != 3)
								{
									fullHouseCards.Add(hand[i]);
								}
							}
						}
					}
				}
			}
		}
		
		bool flush = false;
		bool straightFlush = false;
		bool royalFlush = false;
		List<CardScript> flushCards = new List<CardScript>();
		List<CardScript> straightFlushCards = new List<CardScript>();
		if(cardsInHand >= HandValues.instance.cardsNeededToMakeAFlush || cardsInHand >= HandValues.instance.cardsNeededToMakeAStraightFlush)
		{
			List<int> foundFlushes = new List<int>();
			List<int> foundPossibleStraightFlushes = new List<int>();
			for(int suit = 0; suit < 4; suit++)
			{
				if(numberOfEachSuit[suit] + numberOfEachSuit[4] >= HandValues.instance.cardsNeededToMakeAFlush)
				{
					flush = true;
					foundFlushes.Add(suit);
				}
				if(numberOfEachSuit[suit] + numberOfEachSuit[4] >= HandValues.instance.cardsNeededToMakeAStraightFlush)
				{
					foundPossibleStraightFlushes.Add(suit);
				}
			}
			if(flush)
			{
				List<List<CardScript>> flushes = new List<List<CardScript>>();
				for(int curFlush = 0; curFlush < foundFlushes.Count; curFlush++)
				{
					List<CardScript> flushHand = GetAllCardsOfSuit(hand, foundFlushes[curFlush], false, true);
					flushes.Add(flushHand);
				}
				if(flushes.Count >= 1)
				{
					flushes.Sort(new HighCardComparer());
					flushCards = flushes[0];
				}
			}
			List<List<CardScript>> straightFlushes = new List<List<CardScript>>();
			for(int suit = 0; suit < foundPossibleStraightFlushes.Count; suit++)
			{
				List<CardScript> flushToCheck = new List<CardScript>();
				for(int card = 0; card < cardsInHand; card++)
				{
					if(hand[card].suitInt == foundPossibleStraightFlushes[suit] || hand[card].suitInt == 4)
					{
						flushToCheck.Add(hand[card]);
					}
				}
				List<CardScript> straightFlushCardsToCheck = DoesHandContainStraight(flushToCheck, HandValues.instance.maxGapInStraightFlushes, HandValues.instance.cardsNeededToMakeAStraightFlush, HandValues.instance.straightFlushesCanWrap);
				if(straightFlushCardsToCheck != null)
				{
					straightFlush = true;
					straightFlushes.Add(straightFlushCardsToCheck);
				}
			}
			if(straightFlush)
			{
				straightFlushes.Sort(new StraightComparer());
				if(straightFlushes[0].Count == 5)
				{
					if(straightFlushes[0][0].rankInt == 8 && straightFlushes[0][1].rankInt == 9 && straightFlushes[0][2].rankInt == 10 && straightFlushes[0][3].rankInt == 11 && straightFlushes[0][4].rankInt == 12)
					{
						royalFlush = true;
					}
				}
				straightFlushCards = straightFlushes[0];
			}
		}
		
		bool straight = false;
		List<CardScript> straightCards = DoesHandContainStraight(hand, HandValues.instance.maxGapInStraights, HandValues.instance.cardsNeededToMakeAStraight, HandValues.instance.straightsCanWrap);
		if(straightCards != null)
		{
			straight = true;
		}
		
		bool threeOfAKind = false;
		List<CardScript> threeOfAKindCards = new List<CardScript>();
		
		if(cardsInHand >= 3)
		{
			if(fourOfAKind)
			{
				threeOfAKind = true;
			}
			else if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt)
			{
				threeOfAKind = true;
				for(int i = 0; i < 3; i++)
				{
					threeOfAKindCards.Add(hand[i]);
				}
			}
			else if(cardsInHand >= 4)
			{
				if(hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt)
				{
					threeOfAKind = true;
					for(int i = 1; i < 4; i++)
					{
						threeOfAKindCards.Add(hand[i]);
					}
				}
				else if(cardsInHand >= 5)
				{
					if(hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt)
					{
						threeOfAKind = true;
						for(int i = 2; i < 5; i++)
						{
							threeOfAKindCards.Add(hand[i]);
						}
					}
					else if(cardsInHand >= 6)
					{
						if(hand[3].rankInt == hand[4].rankInt && hand[4].rankInt == hand[5].rankInt)
						{
							threeOfAKind = true;
							for(int i = 3; i < 6; i++)
							{
								threeOfAKindCards.Add(hand[i]);
							}
						}
						else if(cardsInHand >= 7)
						{
							if(hand[4].rankInt == hand[5].rankInt && hand[5].rankInt == hand[6].rankInt)
							{
								threeOfAKind = true;
								for(int i = 4; i < 7; i++)
								{
									threeOfAKindCards.Add(hand[i]);
								}
							}
						}
					}
				}
			}
		}
		
		bool twoPair = false;
		List<CardScript> twoPairCards = new List<CardScript>();
		if(cardsInHand >= 4)
		{
			if(fullHouse || tripleDouble || doubleTriple || stuffedHouse || guestHouse || wideHouse || hugeHouse)
			{
				twoPair = true;
			}
			else if(!threeOfAKind)	// if you have three+ of a kind, then you can't have two pair without having one of the
			{						// bigger houses
				if(hand[0].rankInt == hand[1].rankInt && hand[2].rankInt == hand[3].rankInt)
				{
					twoPair = true;
					for(int i = 0; i < 4; i++)
					{
						twoPairCards.Add(hand[i]);
					}
				}
				else if(cardsInHand >= 5)
				{
					if(hand[0].rankInt == hand[1].rankInt && hand[3].rankInt == hand[4].rankInt)
					{
						twoPair = true;
						for(int i = 0; i < 5; i++)
						{
							if(i != 2)
							{
								twoPairCards.Add(hand[i]);
							}
						}
					}
					else if(hand[1].rankInt == hand[2].rankInt && hand[3].rankInt == hand[4].rankInt)
					{
						twoPair = true;
						for(int i = 1; i < 5; i++)
						{
							twoPairCards.Add(hand[i]);
						}
					}
					else if(cardsInHand >= 6)
					{
						if(hand[0].rankInt == hand[1].rankInt && hand[4].rankInt == hand[5].rankInt)
						{
							twoPair = true;
							for(int i = 0; i < 6; i++)
							{
								if(i != 2 && i != 3)
								{
									twoPairCards.Add(hand[i]);
								}
							}
						}
						else if(hand[1].rankInt == hand[2].rankInt && hand[4].rankInt == hand[5].rankInt)
						{
							twoPair = true;
							for(int i = 1; i < 6; i++)
							{
								if(i != 3)
								{
									twoPairCards.Add(hand[i]);
								}
							}
						}
						else if(hand[2].rankInt == hand[3].rankInt && hand[4].rankInt == hand[5].rankInt)
						{
							twoPair = true;
							for(int i = 2; i < 6; i++)
							{
								twoPairCards.Add(hand[i]);
							}
						}
						else if(cardsInHand >= 7)
						{
							if(hand[0].rankInt == hand[1].rankInt && hand[5].rankInt == hand[6].rankInt)
							{
								twoPair = true;
								for(int i = 0; i < 7; i++)
								{
									if(i != 2 && i != 3 && i != 4)
									{
										twoPairCards.Add(hand[i]);
									}
								}
							}
							else if(hand[1].rankInt == hand[2].rankInt && hand[5].rankInt == hand[6].rankInt)
							{
								twoPair = true;
								for(int i = 1; i < 7; i++)
								{
									if(i != 3 && i != 4)
									{
										twoPairCards.Add(hand[i]);
									}
								}
							}
							else if(hand[2].rankInt == hand[3].rankInt && hand[5].rankInt == hand[6].rankInt)
							{
								twoPair = true;
								for(int i = 2; i < 7; i++)
								{
									if(i != 4)
									{
										twoPairCards.Add(hand[i]);
									}
								}
							}
							else if(hand[3].rankInt == hand[4].rankInt && hand[5].rankInt == hand[6].rankInt)
							{
								twoPair = true;
								for(int i = 3; i < 7; i++)
								{
									twoPairCards.Add(hand[i]);
								}
							}
						}
					}
				}
			}
		}
		
		bool onePair = false;
		List<CardScript> onePairCards = new List<CardScript>();
		if(cardsInHand >= 2)
		{
			if(twoPair || threeOfAKind || fullHouse || tripleDouble || doubleTriple || stuffedHouse || guestHouse || wideHouse || hugeHouse)
			{
				onePair = true;
			}
			else
			{
				if(hand[0].rankInt == hand[1].rankInt)
				{
					onePair = true;
					onePairCards.Add(hand[0]);
					onePairCards.Add(hand[1]);
				}
				else if(cardsInHand >= 3)
				{
					if(hand[1].rankInt == hand[2].rankInt)
					{
						onePair = true;
						onePairCards.Add(hand[1]);
						onePairCards.Add(hand[2]);
					}
					else if(cardsInHand >= 4)
					{
						if(hand[2].rankInt == hand[3].rankInt)
						{
							onePair = true;
							onePairCards.Add(hand[2]);
							onePairCards.Add(hand[3]);
						}
						else if(cardsInHand >= 5)
						{
							if(hand[3].rankInt == hand[4].rankInt)
							{
								onePair = true;
								onePairCards.Add(hand[3]);
								onePairCards.Add(hand[4]);
							}
							else if(cardsInHand >= 6)
							{
								if(hand[4].rankInt == hand[5].rankInt)
								{
									onePair = true;
									onePairCards.Add(hand[4]);
									onePairCards.Add(hand[5]);
								}
								else if(cardsInHand >= 7)
								{
									if(hand[5].rankInt == hand[6].rankInt)
									{
										onePair = true;
										onePairCards.Add(hand[5]);
										onePairCards.Add(hand[6]);
									}
								}
							}
						}
					}
				}
			}
		}
		
		bool highCard = false;
		List<CardScript> highCardCards = new List<CardScript>();
		if(cardsInHand >= 1)
		{
			highCard = true;
			highCardCards.Add(hand[cardsInHand - 1]);
		}
		
		bool[] handsContained = new bool[18];
		
		handsContained[0] = highCard;
		handsContained[1] = onePair;
		handsContained[2] = twoPair;
		handsContained[3] = threeOfAKind;
		handsContained[4] = straight;
		handsContained[5] = flush;
		handsContained[6] = fullHouse;
		handsContained[7] = fourOfAKind;
		handsContained[8] = straightFlush;
		handsContained[9] = fiveOfAKind;
		//handsContained[10] = flushFive;
		handsContained[10] = tripleDouble;
		handsContained[11] = doubleTriple;
		handsContained[12] = stuffedHouse;
		handsContained[13] = sixOfAKind;
		//handsContained[15] = flushSix;
		handsContained[14] = guestHouse;
		handsContained[15] = wideHouse;
		handsContained[16] = hugeHouse;
		handsContained[17] = sevenOfAKind;
		//handsContained[20] = flushSeven;
		
		dataText.text = "";
		for(int i = 17; i >= 0; i--)
		{
			if(handsContained[i])
			{
				dataText.text += HandValues.instance.handNames[i] + ", ";
			}
		}
		
		for(int i = 17; i >= 0; i--)
		{
			if(handsContained[i])
			{
				if(i >= 14)
				{
					handZone.HandEvaluated(hand, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i >= 10)
				{
					handZone.HandEvaluated(sixHand, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i >= 9)
				{
					handZone.HandEvaluated(fiveOfAKindCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 8)
				{
					if(royalFlush)
					{
						handZone.HandEvaluated(straightFlushCards, "Royal Flush", i, evaluatingOnlyCardsUsed, handsContained);
					}
					else
					{
						handZone.HandEvaluated(straightFlushCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
					}
				}
				else if(i == 7)
				{
					handZone.HandEvaluated(fourOfAKindCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 6)
				{
					handZone.HandEvaluated(fullHouseCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 5)
				{
					handZone.HandEvaluated(flushCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 4)
				{
					handZone.HandEvaluated(straightCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 3)
				{
					handZone.HandEvaluated(threeOfAKindCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 2)
				{
					handZone.HandEvaluated(twoPairCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 1)
				{
					handZone.HandEvaluated(onePairCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 0)
				{
					handZone.HandEvaluated(highCardCards, HandValues.instance.handNames[i], i, evaluatingOnlyCardsUsed, handsContained);
				}
				return;
			}
		}
		print("LargeHandEvaluated reached an endpoint with no hand evaluated");
	}
	
	public class StraightComparer : IComparer<List<CardScript>>
	{
		public int Compare(List<CardScript> straight1, List<CardScript> straight2)
		{
			int highCard1 = straight1.Count > 0 ? straight1[straight1.Count - 1].rankInt : -1;
			int highCard2 = straight2.Count > 0 ? straight2[straight2.Count - 1].rankInt : -1;
			if(highCard1 != highCard2)
			{
				return highCard2.CompareTo(highCard1);
			}
			int i = -2;
			while(straight1.Count + i >= 0 && straight2.Count + i >= 0)
			{
				highCard1 = straight1[straight1.Count + i].rankInt;
				highCard2 = straight2[straight2.Count + i].rankInt;
				if(highCard1 != highCard2)
				{
					return highCard2.CompareTo(highCard1);
				}
				i--;
			}
			return straight2.Count.CompareTo(straight1.Count);
		}
	}
	
	public List<CardScript> DoesHandContainStraight(List<CardScript> hand, int maxGaps, int minLength, bool canWrap)
	{
		if(hand.Count >= minLength)
		{
			List<CardScript> uniqueRanks = new List<CardScript>();
			for(int card = 0; card < hand.Count; card++)
			{
				bool addToUniqueRanks = true;
				for(int uniqueRank = 0; uniqueRank < uniqueRanks.Count; uniqueRank++)
				{
					if(uniqueRanks[uniqueRank].rankInt == hand[card].rankInt)
					{
						addToUniqueRanks = false;
						break;
					}
				}
				if(addToUniqueRanks)
				{
					uniqueRanks.Add(hand[card]);
				}
			}
			int uniqueRanksInHand = uniqueRanks.Count;
			if(uniqueRanksInHand >= minLength)
			{
				List<CardScript> cardsUsedInStraight = new List<CardScript>();
				List<List<CardScript>> straights = new List<List<CardScript>>();
				if(minLength <= 1)
				{
					List<CardScript> oneCardStraight = new List<CardScript>();
					oneCardStraight.Add(hand[hand.Count - 1]);
					straights.Add(oneCardStraight);
				}
				int cardsInARow = 1;
				if(uniqueRanks[uniqueRanksInHand - 1].rankInt == 12 && !canWrap) // if there is an ace in hand
				{
					if(uniqueRanks[0].rankInt <= maxGaps)
					{
						cardsInARow++;
						cardsUsedInStraight.Add(uniqueRanks[uniqueRanksInHand - 1]);
						cardsUsedInStraight.Add(uniqueRanks[0]);
					}
				}
				for(int card = 0; card < uniqueRanksInHand - 1; card++)
				{
					if(uniqueRanks[card + 1].rankInt - uniqueRanks[card].rankInt <= 1 + maxGaps && uniqueRanks[card + 1].rankInt - uniqueRanks[card].rankInt > 0)
					{
						cardsInARow++;
						if(!cardsUsedInStraight.Contains(uniqueRanks[card]))
						{
							cardsUsedInStraight.Add(uniqueRanks[card]);
						}
						cardsUsedInStraight.Add(uniqueRanks[card + 1]);
					}
					else
					{
						if(cardsUsedInStraight.Count >= minLength)
						{
							straights.Add(cardsUsedInStraight.ToList());
						}
						cardsInARow = 1;
						cardsUsedInStraight.Clear();
					}
				}
				if(cardsUsedInStraight.Count >= minLength)	//		this here's the bit that makes it so wraps suck, but it is logical. Best straight  
				{											//		is best straight. Like with JQKA2 and straight length of 4, JQKA is an ace high
					straights.Add(cardsUsedInStraight.ToList());//	straight whereas JQKA2 is a 2 high straight, according to prescedent set by wheel.
				}
				if(canWrap && cardsInARow < uniqueRanksInHand)
				{
					if(uniqueRanks[uniqueRanksInHand - 1].rankInt - uniqueRanks[0].rankInt >= (12 - maxGaps))
					{
						cardsInARow++;
						if(!cardsUsedInStraight.Contains(uniqueRanks[uniqueRanksInHand - 1]))
						{
							cardsUsedInStraight.Add(uniqueRanks[uniqueRanksInHand - 1]);
						}
						if(!cardsUsedInStraight.Contains(uniqueRanks[0]))
						{
							cardsUsedInStraight.Add(uniqueRanks[0]);
						}
					}
					else
					{
						if(cardsUsedInStraight.Count >= minLength)
						{
							straights.Add(cardsUsedInStraight.ToList());
						}
						cardsInARow = 1;
						cardsUsedInStraight.Clear();
					}
					for(int card = 0; card < uniqueRanksInHand - 2; card++)
					{
						if(uniqueRanks[card + 1].rankInt - uniqueRanks[card].rankInt <= 1 + maxGaps && uniqueRanks[card + 1].rankInt - uniqueRanks[card].rankInt > 0)
						{
							cardsInARow++;
							if(!cardsUsedInStraight.Contains(uniqueRanks[card]))
							{
								cardsUsedInStraight.Add(uniqueRanks[card]);
							}
							if(!cardsUsedInStraight.Contains(uniqueRanks[card + 1]))
							{
								cardsUsedInStraight.Add(uniqueRanks[card + 1]);
							}
						}
						else
						{
							if(cardsUsedInStraight.Count >= minLength)
							{
								straights.Add(cardsUsedInStraight.ToList());
							}
							cardsInARow = 1;
							cardsUsedInStraight.Clear();
						}
					}
					if(cardsUsedInStraight.Count >= minLength)
					{
						straights.Add(cardsUsedInStraight.ToList());
					}
				}
				else
				{
					if(cardsUsedInStraight.Count >= minLength)
					{
						straights.Add(cardsUsedInStraight.ToList());
					}
				}
				if(straights.Count > 0)
				{
					straights.Sort(new StraightComparer());
					//print("There were " + straights.Count + " straights");
					//print("straights[0][0]" + straights[0][0].rankInt);
					return straights[0];
				}
			}
			else
			{
				return null;
			}
		}
		else
		{
			return null;
		}
		return null;
	}
	
	public class HighCardComparer : IComparer<List<CardScript>>
	{
		public int Compare(List<CardScript> hand1, List<CardScript> hand2)
		{
			int rank1 = hand1.Count > 0 ? hand1[0].rankInt : -1;
			int rank2 = hand2.Count > 0 ? hand2[0].rankInt : -1;
			
			if(rank1 != rank2)
			{
				return rank2.CompareTo(rank1);
			}
			
			for(int i = 1; i < Mathf.Min(hand1.Count, hand2.Count); i++)
			{
				rank1 = hand1[i].rankInt;
				rank2 = hand2[i].rankInt;
				if(rank1 != rank2)
				{
					return rank2.CompareTo(rank1);
				}
			}
			return hand2.Count.CompareTo(hand1.Count);
		}
	}
	
	public List<CardScript> GetAllCardsOfSuit(List<CardScript> hand, int suit, bool uniqueRanksOnly, bool highestFirst)
	{
		List<CardScript> cardsOfDesiredSuit = new List<CardScript>();
		for(int card = 0; card < hand.Count; card++)
		{
			if(hand[card].suitInt == suit || hand[card].suitInt == 4)
			{
				if(uniqueRanksOnly)
				{
					bool rankIsUnique = true;
					for(int i = 0; i < cardsOfDesiredSuit.Count; i++)
					{
						if(cardsOfDesiredSuit[i].rankInt == hand[card].rankInt)
						{
							rankIsUnique = false;
							break;
						}
					}
					if(rankIsUnique)
					{
						cardsOfDesiredSuit.Add(hand[card]);
					}
				}
				else
				{
					cardsOfDesiredSuit.Add(hand[card]);
				}
			}
		}
		if(highestFirst)
		{
			cardsOfDesiredSuit.Sort((a, b) => b.rankInt.CompareTo(a.rankInt));
		}
		else
		{
			cardsOfDesiredSuit.Sort((a, b) => a.rankInt.CompareTo(b.rankInt));
		}
		return cardsOfDesiredSuit;
	}
	
	public bool HandIsFlush(List<CardScript> hand, int minNeededForFlush)
	{
		int[] numberOfEachSuit = new int[5];
		for(int card = 0; card < hand.Count; card++)
		{
			numberOfEachSuit[hand[card].suitInt]++;
		}
		for(int suit = 0; suit < 4; suit++)
		{
			if(numberOfEachSuit[suit] + numberOfEachSuit[4] >= minNeededForFlush)
			{
				return true;
			}
		}
		return false;
	}
	
	void Start()
    {	
        //dataText = GameObject.FindWithTag("DataText").GetComponent<TMP_Text>();
    }
}
