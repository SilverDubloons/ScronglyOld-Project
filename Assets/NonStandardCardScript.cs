using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonStandardCardScript : MonoBehaviour
{
	public static NonStandardCardScript instance;
	
	[System.Serializable]
	public class NonStandardCard
	{
		public string cardName;
		public string cardDescription;
		public Sprite cardImage;
		public int cardCost;
		public float baseValueProvided;
		public float multProvided;
		public float multMultiplierProvided;
		public int cardCategory; // 0 = common, 1 = uncommon, 2 = rare, 3 = legendary, 5 = not in shop
		public int numberOnSale;
		public bool cannotBePlayed;
		public int cardNumber;
	}
	public NonStandardCard[] nonStandardCards;
	
	public DeckScript deckScript;
	
	public int commonCardWeight;
	public int uncommonCardWeight;
	public int rareCardWeight;
	public int legendaryCardWeight;
	public bool uniqueCardsOnly;
	public RandomNumbers randomNumbers;
	
	public List<List<int>> cardsByRarity = new List<List<int>>();
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		AssignCardNumbers();
		SortCardsByRarity();
		/* string nsc = "";
		for(int i = 0; i < nonStandardCards.Length; i++)
		{
			nsc += "nonStandardCard " + i + " = " + nonStandardCards[i].cardName + ": " + nonStandardCards[i].cardDescription + "\n";
		}
		print(nsc); */
		//SetupBaubles();
	}
	
	void AssignCardNumbers()
	{
		for(int i = 0; i < nonStandardCards.Length; i++)
		{
			nonStandardCards[i].cardNumber = i;
		}
	}
	
	public void SortCardsByRarity()
	{
		cardsByRarity.Clear();
		List<int> commonCards = new List<int>();
		List<int> uncommonCards = new List<int>();
		List<int> rareCards = new List<int>();
		List<int> legendaryCards = new List<int>();
		for(int i = 0; i < nonStandardCards.Length; i++)
		{
			switch(nonStandardCards[i].cardCategory)
			{
				case 0:
				commonCards.Add(i);
				break;
				case 1:
				uncommonCards.Add(i);
				break;
				case 2:
				rareCards.Add(i);
				break;
				case 3:
				legendaryCards.Add(i);
				break;
			}
		}
		cardsByRarity.Add(commonCards);
		cardsByRarity.Add(uncommonCards);
		cardsByRarity.Add(rareCards);
		cardsByRarity.Add(legendaryCards);
	}
	
	public CardScript CreateSpecificNonStandardCard(int nonstandardCardNumber, Transform cardParent, Vector2 position)
	{
		NonStandardCard newCard = nonStandardCards[nonstandardCardNumber];
		return deckScript.CreateNonStandardCard(newCard.cardImage, newCard.cardNumber, newCard.cardDescription, newCard.cardName, newCard.cardCategory, cardParent, position, false, true, newCard.cannotBePlayed);
	}
	
	public CardScript CreateRandomNonStandardCardForShop(Transform cardParent, Vector2 position)
	{
		NonStandardCard newCard = GetRandomNonStandardCard(commonCardWeight, uncommonCardWeight, rareCardWeight, legendaryCardWeight);
		return deckScript.CreateNonStandardCard(newCard.cardImage, newCard.cardNumber, newCard.cardDescription, newCard.cardName, newCard.cardCategory, cardParent, position, false, true, newCard.cannotBePlayed);
	}
	
	public int GetRandomNonStandardCardInt(int common, int uncommon, int rare, int legendary)
	{
		if(cardsByRarity[0].Count == 0)
		{
			common = 0;
		}
		if(cardsByRarity[1].Count == 0)
		{
			uncommon = 0;
		}
		if(cardsByRarity[2].Count == 0)
		{
			rare = 0;
		}
		if(cardsByRarity[3].Count == 0)
		{
			legendary = 0;
		}
		int ran = randomNumbers.Range(0, common + uncommon + rare + legendary);
		int rarity = 0;
		if(ran < common)
		{
			rarity = 0;
		}
		else if(ran < common + uncommon)
		{
			rarity = 1;
		}
		else if(ran < common + uncommon + rare)
		{
			rarity = 2;
		}
		else if(ran < common + uncommon + rare + legendary)
		{
			rarity = 3;
		}
		else
		{
			rarity = 4;
		}
		if(cardsByRarity[rarity].Count > 0)
		{
			return cardsByRarity[rarity][randomNumbers.Range(0, cardsByRarity[rarity].Count)];
		}
		else
		{
			return 0;
		}
	}
	
	public NonStandardCard GetRandomNonStandardCard(int common, int uncommon, int rare, int legendary)
	{
		int iterations = 0;
		while(iterations < 100)
		{
			iterations++;
			//int ran = Random.Range(0, common + uncommon + rare + legendary);
			int ran = randomNumbers.Range(0, common + uncommon + rare + legendary);
			int rarity = 0;
			if(ran < common)
			{
				rarity = 0;
			}
			else if(ran < common + uncommon)
			{
				rarity = 1;
			}
			else if(ran < common + uncommon + rare)
			{
				rarity = 2;
			}
			else if(ran < common + uncommon + rare + legendary)
			{
				rarity = 3;
			}
			else
			{
				rarity = 4;
			}
			if(cardsByRarity[rarity].Count > 0)
			{
				//int cardIndex = Random.Range(0, cardsByRarity[rarity].Count);
				int cardIndex = randomNumbers.Range(0, cardsByRarity[rarity].Count);
				NonStandardCard checkingCard = nonStandardCards[cardsByRarity[rarity][cardIndex]];
				//bool reroll = false;
				if(uniqueCardsOnly)
				{
					if(checkingCard.numberOnSale == 0)
					{
						return checkingCard;
					}
				}
				else
				{
					return checkingCard;
				}
			}
		}
		print("iterations surpassed 100 when checking for random non standard CARD to put in store. add more non standard cards");
		return null;
	}
}
