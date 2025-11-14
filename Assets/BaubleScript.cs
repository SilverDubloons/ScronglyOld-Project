using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaubleScript : MonoBehaviour
{
	public static BaubleScript instance;
	
	[System.Serializable]
	public class Bauble
	{
		public string baubleName;
		public string baubleDescription;
		public Sprite baubleImage;
		public int baseCost;
		public int maxQuantity;	// 0 = infinite
		public int numberOnSale;
		public int quantityOwned;
		public float costScalingMultiplier;
		public int costScalingAdditive;
		public int baubleCategory;
		public float baubleImpact;	// used for different arithmetic, value is based on quantityOwned but easier to test different values this way
		public bool canAppearInShop;
		public int baubleNumber;
		public int sortingOrderIndex;
		public bool isInPool;
	}
	
	public Bauble[] baubles;
	
	public int handImproverBaubleWeight;
	public int commonBaubleWeight;
	public int uncommonBaubleWeight;
	public int rareBaubleWeight;
	public int legendaryBaubleWeight;
	
	public bool[] handTypesToShow = new bool[18];
	public bool[] handTypesPlayed = new bool[18];
	public bool uniqueBaublesOnly = true;
	public bool uniqueHandLevelsOnly = true;
	
	public Color handLevelColor;
	
	public HandsInformation handsInformation;
	public List<RectTransform> handsTexts;
	public RandomNumbers randomNumbers;
	
	public int baseDiscardsPerAnte;
	public int baseHandsUntilFatigue;
	public int baseItemsInShop;
	public int baseHandSize;
	
	public TooltipScript interestTooltip;
	public Sprite[] dieSprites;
	
	public void ResetHandsToShow()
	{
		for(int i = 0; i < 18; i++)
		{
			if(i <= 8)
			{
				handTypesPlayed[i] = true;
				handTypesToShow[i] = true;
			}
			else if(i <= 9)
			{
				handTypesPlayed[i] = false;
				handTypesToShow[i] = true;
			}
			else
			{
				handTypesPlayed[i] = false;
				handTypesToShow[i] = false;
			}
		}
	}
	
	public void ResetBaublesToShow()
	{
		for(int i = 0; i < baubles.Length; i++)
		{
			if(i < 47)
			{
				baubles[i].canAppearInShop = true;
			}
			else if(i < 56) //47 - 55 baubles for 5Oak - 7oak
			{
				baubles[i].canAppearInShop = false;
			}
			else
			{
				baubles[i].canAppearInShop = true;
			}
		}
	}
	
	public void UnlockAllBaubles()
	{
		for(int i = 9; i < 18; i++)
		{
			HighTierHandPlayed(i);
		}
	}
	
	public int GetBaubleIndexByHandTier(int handTier)
	{
		/* case 0:	// high card			0
		case 1:		// one pair				1
		case 2:		// two pair				2
		case 3:		// three of a kind		3
		case 4:		// straight				4
		case 5:		// flush				5
		case 6:		// full house			6
		case 7:		// four of a kind		7
		case 8:		// straight flush		8
		case 9:		// five of a kind		9
		case 10:	// triple double		10
		case 11:	// double triple		11
		case 12:	// stuffed house		12
		case 42:	// six of a kind		13
		case 43:	// guest house			14
		case 44:	// wide house			15
		case 45:	// huge house			16
		case 46:	// seven of a kind		17 */
		if(handTier <= 12)
		{
			return handTier;
		}
		else
		{
			return handTier + 29;
		}
	}
	
	public void HighTierHandPlayed(int handTier)
	{
		baubles[38 + handTier].canAppearInShop = true;
		handTypesPlayed[handTier] = true;
		//handTypesToShow[handTier] = true;
		handsInformation.ReorganizeValueLabels();
		CheckForTelescopeDeckUnlock();
	}
	
	public void CheckForTelescopeDeckUnlock()
	{
		if(!Decks.instance.decks[5].unlocked && !Statistics.instance.currentRun.runIsDailyGame && !Statistics.instance.currentRun.runIsSeededGame && DeckViewer.instance.variantInUse == "")
		{
			for(int i = 0; i < handTypesPlayed.Length; i++)
			{
				if(!handTypesPlayed[i])
				{
					return;
				}
			}
			Decks.instance.decks[5].unlocked = true;
			Decks.instance.UpdateDecksFile();
			Decks.instance.DeckKnobs[5].knobImage.sprite = Decks.instance.unlockedKnob;
			Decks.instance.DeckKnobs[5].rt.sizeDelta = new Vector2(10,10);
			UnlockNotifications.instance.CreateNewUnlockNotifier(0,5);
		}
	}
	
	public List<List<int>> baublesByRarity = new List<List<int>>();
	
	void Awake()
	{
		instance = this;
		//SetupBaubles(); // in HandsInformation for ordering reasons
	}
	
	public void AssignBaubleNumbers()
	{
		for(int i = 0; i < baubles.Length; i++)
		{
			baubles[i].baubleNumber = i;
		}
	}
	
	public void SortBaublesByRarity()
	{
		baublesByRarity.Clear();
		List<int> commonBaubles = new List<int>();
		List<int> uncommonBaubles = new List<int>();
		List<int> rareBaubles = new List<int>();
		List<int> legendaryBaubles = new List<int>();
		List<int> handLevelBaubles = new List<int>();
		for(int i = 0; i < baubles.Length; i++)
		{
			if((baubles[i].quantityOwned < baubles[i].maxQuantity || baubles[i].maxQuantity == 0) && baubles[i].canAppearInShop && baubles[i].numberOnSale == 0 && baubles[i].isInPool)
			{
				switch(baubles[i].baubleCategory)
				{
					case 0:
					commonBaubles.Add(i);
					break;
					case 1:
					uncommonBaubles.Add(i);
					break;
					case 2:
					rareBaubles.Add(i);
					break;
					case 3:
					legendaryBaubles.Add(i);
					break;
					case 4:
					int handNumber = i;
					if(handNumber > 12)
					{
						handNumber -= 29;
					}
					if(handTypesPlayed[handNumber])
					{
						handLevelBaubles.Add(i);
					}
					break;
				}
			}
		}
		//print("common= " + commonBaubles.Count);
		baublesByRarity.Add(commonBaubles);
		baublesByRarity.Add(uncommonBaubles);
		baublesByRarity.Add(rareBaubles);
		baublesByRarity.Add(legendaryBaubles);
		baublesByRarity.Add(handLevelBaubles);
	}
	
	public int GetBaublePrice(int b)
	{
		//print("b= " + b + " " + baubles[b].baubleName + " baseCost= " + baubles[b].baseCost + " quantityOwned= " + baubles[b].quantityOwned);
		return Mathf.FloorToInt(baubles[b].baseCost * Mathf.Pow(baubles[b].costScalingMultiplier, baubles[b].quantityOwned) + baubles[b].costScalingAdditive * baubles[b].quantityOwned);
	}
	
	public void SetupSpecificBaubleItem(BaubleItem baubleItem, int num, bool free)
	{
		Bauble newBauble;
		newBauble = baubles[num];
		baubleItem.baubleImage.sprite = newBauble.baubleImage;
		baubleItem.buyScript.shopScript = shopScript;
		baubleItem.buyScript.baubleNumber = newBauble.baubleNumber;
		baubleItem.buyScript.baubleScript = this;
		baubleItem.tooltipScript.SetupTooltip(newBauble.baubleDescription, newBauble.baubleName, newBauble.baubleCategory);
		baubleItem.tooltipScript.gameObject.SetActive(false);
		baubleItem.name = newBauble.baubleName;
		baubleItem.baubleNumber = newBauble.baubleNumber;
		if(newBauble.baubleCategory == 4)
		{
			baubleItem.baubleImage.color = handLevelColor;
		}
		newBauble.numberOnSale++;
		if(free)
		{
			baubleItem.buyScript.SetupBuy(0);
		}
		else
		{
			baubleItem.buyScript.SetupBuy(GetBaublePrice(newBauble.baubleNumber));
		}
	}
	
	public void SetupRandomBaubleItem(BaubleItem baubleItem, bool handLevelOnly)
	{
		//int newBaubleIndex = GetRandomBauble();
		//Bauble newBauble = baubles[newBaubleIndex];
		Bauble newBauble;
		if(handLevelOnly)
		{
			newBauble = GetRandomBauble(0, 0, 0, 0, 1);
		}
		else
		{
			newBauble = GetRandomBauble(commonBaubleWeight, uncommonBaubleWeight, rareBaubleWeight, legendaryBaubleWeight, 0);
		}
		baubleItem.baubleImage.sprite = newBauble.baubleImage;
		if(newBauble.baubleCategory == 4)
		{
			baubleItem.baubleImage.color = handLevelColor;
		}
		baubleItem.buyScript.shopScript = shopScript;
		baubleItem.buyScript.baubleNumber = newBauble.baubleNumber;
		baubleItem.buyScript.baubleScript = this;
		baubleItem.tooltipScript.SetupTooltip(newBauble.baubleDescription, newBauble.baubleName, newBauble.baubleCategory);
		baubleItem.tooltipScript.gameObject.SetActive(false);
		baubleItem.name = newBauble.baubleName;
		baubleItem.baubleNumber = newBauble.baubleNumber;
		newBauble.numberOnSale++;
		baubleItem.buyScript.SetupBuy(GetBaublePrice(newBauble.baubleNumber));
	}
	
	public TMP_InputField cheatInput;
	
	public void CheatAddBauble()
	{
		BaublePurchased(int.Parse(cheatInput.text));
	}
	
	public void BaublePurchased(int baubleIndex)
	{
		baubles[baubleIndex].quantityOwned++;
		ApplyBauble(baubles[baubleIndex]);
		//print(baubles[baubleIndex].baubleName + " purchased. Quantity is now " + baubles[baubleIndex].quantityOwned + " " + baubles[baubleIndex].baubleDescription);
	}
	
	public Bauble GetRandomBauble(int common, int uncommon, int rare, int legendary, int handLevel)
	{
		int iterations = 0;
		if(baublesByRarity[0].Count == 0)
		{
			common = 0;
		}
		if(baublesByRarity[1].Count == 0)
		{
			uncommon = 0;
		}
		if(baublesByRarity[2].Count == 0)
		{
			rare = 0;
		}
		if(baublesByRarity[3].Count == 0)
		{
			legendary = 0;
		}
		if(common == 0 && uncommon == 0 && rare == 0 && legendary == 0)
		{
			handLevel++;
		}
		while(iterations < 100)
		{
			iterations++;
			if(iterations > 1)
			{
				print("iterations= " + iterations);
			}
			//int ran = Random.Range(0, common + uncommon + rare + legendary + handLevel);
			int ran = randomNumbers.Range(0, common + uncommon + rare + legendary + handLevel);
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
			/* if(baublesByRarity[rarity].Count > 0)
			{ */
				int baubleIndex = randomNumbers.Range(0, baublesByRarity[rarity].Count);
				//print("baubleIndex= " + baubleIndex + " rarity= " + rarity);
				if(baublesByRarity[rarity].Count <= 0)
				{
					return baubles[0];
				}
				Bauble tempBauble = baubles[baublesByRarity[rarity][baubleIndex]];
				baublesByRarity[rarity].RemoveAt(baubleIndex); // does this even matter?
				return tempBauble;
				
				/* bool reroll = false;
				if(rarity == 4)
				{
					if(baubleIndex > 8)
					{
						if(!handTypesToShow[baubleIndex])
						{
							reroll = true;
						}
					}
				}
				if(!reroll)
				{
					if(uniqueBaublesOnly)
					{
						if((checkingBauble.quantityOwned < checkingBauble.maxQuantity || checkingBauble.maxQuantity == 0) && checkingBauble.numberOnSale == 0)
						{
							return checkingBauble;
						}
					}
					else
					{
						if(checkingBauble.quantityOwned + checkingBauble.numberOnSale < checkingBauble.maxQuantity || checkingBauble.maxQuantity == 0)
						{
							return checkingBauble;
						}
					}
				} */
			//}
		}
		print("iterations surpassed 100 when checking for random bauble to put in store. add more infinitely scaling baubles");
		return GetRandomBauble(0, 0, 0, 0, 1);
		//return null;
	}
	
	
	public HandScript handScript;
	public HandValues handValues;
	public ShopScript shopScript;
	
	public void SetupBaubles()
	{
		for(int i = 0; i < baubles.Length; i++)
		{
			baubles[i].numberOnSale = 0;
			ApplyBauble(baubles[i]);
		}
		/* string baub = "";
		for(int i = 0; i < baubles.Length; i++)
		{
			baub += "bauble " + i + " = " + baubles[i].baubleName + ": " + baubles[i].baubleDescription + "\n";
		}
		print(baub); */
	}
	
	public void SetupNonZodiacBaubles()
	{
		for(int i = 0; i < baubles.Length; i++)
		{
			if(baubles[i].baubleCategory <= 3)
			{
				//print("i= " + i + " baubleName=" + baubles[i].baubleName + " quantityOwned= " + baubles[i].quantityOwned);
				ApplyBauble(baubles[i]);
			}
		}
	}
	
	void ApplyBauble(Bauble bauble)
	{
		switch(bauble.baubleNumber)
		{										// hand number
			case 0:		// high card			0
			case 1:		// one pair				1
			case 2:		// two pair				2
			case 3:		// three of a kind		3
			case 4:		// straight				4
			case 5:		// flush				5
			case 6:		// full house			6
			case 7:		// four of a kind		7
			case 8:		// straight flush		8	
			case 9:		// five of a kind		9
			case 10:	// triple double		10
			case 11:	// double triple		11
			case 12:	// stuffed house		12
			case 42:	// six of a kind		13
			case 43:	// guest house			14
			case 44:	// wide house			15
			case 45:	// huge house			16
			case 46:	// seven of a kind		17
			
			int handNumber = bauble.baubleNumber;
			if(handNumber > 12)
			{
				handNumber -= 29;
			}
			
			handValues.handBaseValues[handNumber] = handValues.handPointsStarting[handNumber] + handValues.handPointsPerLevel[handNumber] * bauble.quantityOwned;
			handValues.handCardMultipliers[handNumber] = handValues.handMultStarting[handNumber] + handValues.handMultPerLevel[handNumber] * bauble.quantityOwned;
			string usea = "";
			if(handNumber == 4 || handNumber == 5 || handNumber == 6 || handNumber == 8 || handNumber == 10 || handNumber == 11 || handNumber == 12 || handNumber == 14 || handNumber == 15 || handNumber == 16)
			{
				usea = "a ";
			}
			string andAllHands = "";
			if(handNumber < 14)
			{
				andAllHands = " (and all hands containing " + usea + handValues.handNames[handNumber] + ")";
			}
			if(handNumber == 0)
			{
				bauble.baubleDescription = "Increases points of all hands by <color=blue>" + handValues.handPointsPerLevel[handNumber] + "</color> and multiplier by <color=red>" + handValues.handMultPerLevel[handNumber] + "</color>. Currently, all hands receive <color=blue>" + handValues.handBaseValues[handNumber] + "</color> and <color=red>" + handValues.handCardMultipliers[handNumber];
			}
			else
			{
				bauble.baubleDescription = "Increases points of " + handValues.handNames[handNumber] + andAllHands + " by <color=blue>" + handValues.handPointsPerLevel[handNumber] + "</color> and multiplier by <color=red>" + handValues.handMultPerLevel[handNumber] + "</color>. Current values are <color=blue>" + handValues.handBaseValues[handNumber] + "</color> and <color=red>" + handValues.handCardMultipliers[handNumber];
			}
			//print("1 bauble.baubleNumber= " + bauble.baubleNumber);
			handsInformation.UpdateHandValueLabel(handNumber);
			handValues.StartCoroutine(handValues.ExpandContract(handsTexts, 0.1f, new Vector3(1.2f, 1.2f, 1f)));
			break;
			case 13:
			handValues.cardsNeededToMakeAStraight = 5 - bauble.quantityOwned;
			handValues.cardsNeededToMakeAStraightFlush = 5 - (Mathf.Min(bauble.quantityOwned, baubles[18].quantityOwned));
			/* if(handValues.cardsNeededToMakeAStraightFlush <= 1)
			{
				baubles[14].canAppearInShop = false;
				baubles[16].canAppearInShop = false;
				shopScript.RemoveStraightBaubles();
			} */
			//print("2");
			if(bauble.quantityOwned < 1)
			{
				//print("3");
				bauble.baubleDescription = "Decreases number of cards needed to make a straight by 1";
			}
			else
			{
				//print("4");
				bauble.baubleDescription = "Decreases number of cards needed to make a straight by 1. Currently you need " + (5 - bauble.quantityOwned);
				HandsInformation.instance.UpdateStraightTooltip();
				HandsInformation.instance.UpdateStraightFlushTooltip();
				if(bauble.quantityOwned == 4)
				{
					HandsInformation.instance.StraightDownToOne();
					if(baubles[18].quantityOwned == 4)
					{
						HandsInformation.instance.StraightFlushDownToOne();
						HandsInformation.instance.UpdateHandValueLabel(8);
					}
					HandsInformation.instance.UpdateHandValueLabel(5);
				}
			}
			//print("5");
			break;
			case 14:
			handValues.maxGapInStraights = bauble.quantityOwned;
			handValues.maxGapInStraightFlushes = bauble.quantityOwned;
			if(bauble.quantityOwned < 1)
			{
				bauble.baubleDescription = "Increases max gap in straights and straight flushes by 1. Example: 5 6 8 9 J will be considered a straight";
			}
			else
			{
				bauble.baubleDescription = "Increases max gap in straights and straight flushes by 1. Current max gap is " + bauble.quantityOwned;
				HandsInformation.instance.UpdateStraightTooltip();
				HandsInformation.instance.UpdateStraightFlushTooltip();
			}
			break;
			case 15:
			handScript.discardsPerAnte = baseDiscardsPerAnte + bauble.quantityOwned;
			bauble.baubleDescription = "Increases discards per ante by 1. Currently you have " + (baseDiscardsPerAnte + bauble.quantityOwned);
			handScript.ResetDiscards();
			break;
			case 16:
			if(bauble.quantityOwned > 0)
			{
				handValues.straightsCanWrap = true;
				handValues.straightFlushesCanWrap = true;
				HandsInformation.instance.UpdateStraightTooltip();
				HandsInformation.instance.UpdateStraightFlushTooltip();
			}
			else
			{
				handValues.straightsCanWrap = false;
				handValues.straightFlushesCanWrap = false;
			}
			bauble.baubleDescription = "Straights and straight flushes can wrap. For example, Q K A 2 3 will count as a straight";
			break;
			case 17:
			handScript.maxHandSize = baseHandSize + bauble.quantityOwned;
			bauble.baubleDescription = "Increases hand size by 1. Current size is " + (7 + bauble.quantityOwned);
			break;
			case 18:
			handValues.cardsNeededToMakeAFlush = 5 - bauble.quantityOwned;
			handValues.cardsNeededToMakeAStraightFlush = 5 - (Mathf.Min(bauble.quantityOwned, baubles[13].quantityOwned));
			/* if(handValues.cardsNeededToMakeAStraightFlush <= 1)
			{
				baubles[14].canAppearInShop = false;
				baubles[16].canAppearInShop = false;
				shopScript.RemoveStraightBaubles();
			} */
			if(bauble.quantityOwned < 1)
			{
				bauble.baubleDescription = "Decreases cards needed to make a flush by 1";
			}
			else
			{
				bauble.baubleDescription = "Decreases cards needed to make a flush by 1. Currently you need " + (5 - bauble.quantityOwned);
				HandsInformation.instance.UpdateStraightFlushTooltip();
				HandsInformation.instance.UpdateFlushTooltip();
				if(bauble.quantityOwned == 4)
				{
					HandsInformation.instance.FlushDownToOne();
					if(baubles[13].quantityOwned == 4)
					{
						HandsInformation.instance.StraightFlushDownToOne();
						HandsInformation.instance.UpdateHandValueLabel(8);
					}
					HandsInformation.instance.UpdateHandValueLabel(5);
				}
			}
			break;
			case 19:
			handValues.handsUntilFatiguePerAnte = baseHandsUntilFatigue + bauble.quantityOwned;
			handValues.UpdateHandsUntilFatigue(0, true);
			bauble.baubleDescription = "Increases hands until fatigue by 1. Currenly you have " + (baseHandsUntilFatigue + bauble.quantityOwned) + " hands until fatigue sets in";
			break;
			case 20:
			if(bauble.quantityOwned > 0)
			{
				handScript.aceDiscardBaubleOwned = true;
			}
			else
			{
				handScript.aceDiscardBaubleOwned = false;
			}
			bauble.baubleDescription = "Whenever you discard 2 or more aces at once, gain 2 chips";
			break;
			case 21:
			handScript.numberOfTopCardsVisible = bauble.quantityOwned;
			bauble.baubleDescription = "The top card of your draw pile is face up";
			break;
			case 22:
			bauble.baubleDescription = "Increase number of baubles, cards and zodiacs for sale in the store by 1. Currently there are " + (baseItemsInShop + bauble.quantityOwned) + " of each available";
			//print("baseItemsInShop= " + baseItemsInShop + " bauble.quantityOwned= " + bauble.quantityOwned);
			shopScript.numberOfCardsForSale = baseItemsInShop + bauble.quantityOwned;
			shopScript.numberOfBaublesForSale = baseItemsInShop + bauble.quantityOwned;
			shopScript.numberOfHandLevelsForSale = baseItemsInShop + bauble.quantityOwned;
			if(bauble.quantityOwned > 0) // need to change this if selling is implemented
			{
				shopScript.AddItemToEachCategory();
			}
			break;
			case 23:
			bauble.baubleImpact = 1.25f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a pair get x1.25 multiplier";
			break;
			case 24:
			bauble.baubleImpact = 1.5f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain two pair get x1.5 multiplier";
			break;
			case 25:
			bauble.baubleImpact = 2f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain three of a kind get double multiplier";
			break;
			case 26:
			bauble.baubleImpact = 2f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a straight get double multiplier";
			break;
			case 27:
			bauble.baubleImpact = 2f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a flush get double multiplier";
			break;
			case 28:
			bauble.baubleImpact = 3f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain four of a kind get triple multiplier";
			break;
			case 29:
			//bauble.baubleImpact = 99999999999f * bauble.quantityOwned;
			bauble.baubleImpact = 4f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a straight flush get quadrulple multiplier";
			break;
			case 30:
			if(bauble.quantityOwned > 0) // need to change this if selling is implemented
			{
				shopScript.chanceForCardToBeRainbow = 1f;
				shopScript.rainbowExtraCost = 0;
				shopScript.ChangeAllCardsForSaleToRainbow();
			}
			else
			{
				shopScript.chanceForCardToBeRainbow = 0.2f;
				shopScript.rainbowExtraCost = 1;
			}
			bauble.baubleDescription = "All standard cards available in the shop are rainbow, and cost the same as non-rainbow cards";
			break;
			case 31:
			if(bauble.quantityOwned > 0) // need to change this if selling is implemented
			{
				handScript.maxCardsDiscardedAtOnce = 9001;
			}
			else
			{
				handScript.maxCardsDiscardedAtOnce = 5;
			}
			bauble.baubleDescription = "You may discard any number of cards at once";
			break;
			case 32:
			if(bauble.quantityOwned == 0)
			{
				bauble.baubleDescription = "Scored numbered cards receive a permanent +3 base value";
			}
			else
			{
				bauble.baubleDescription = "Scored numbered cards receive a permanent +3 base value. Currently gaining +" + (bauble.quantityOwned * 3);
			}
			bauble.baubleImpact = 3f * bauble.quantityOwned;
			break;
			case 33:
			if(bauble.quantityOwned > 0)
			{
				handValues.ChangeAllXImagesToGreen();
			}
			else
			{
				handValues.ChangeAllXImagesToRed();
			}
			bauble.baubleDescription = "All played cards are counted as part of the scored hand";
			break;
			case 34:
			//bauble.baubleDescription = "Allows you to collect interest. Upon starting a new ante, collect 1 chip for every 5 you save. Max 5 per ante";
			interestTooltip.SetupTooltip("You earn 1 chip in interest for each 5 you save when you move on to the next ante. Max " +(bauble.quantityOwned * 5 + 5) +" per ante", "", 5);
			bauble.baubleDescription = "Increases cap on interest earned to 10";
			break;
			case 35:
			bauble.baubleImpact = 3f * bauble.quantityOwned;
			if(bauble.quantityOwned < 1)
			{
				bauble.baubleDescription = "Scoring Rainbow cards each add +3 to multiplier";
			}
			else
			{
				bauble.baubleDescription = "Scoring Rainbow cards each add +3 to multiplier. Currently gaining +" + (bauble.quantityOwned * 3);
			}
			break;
			case 36:
			bauble.baubleImpact = 1f * bauble.quantityOwned;
			if(bauble.quantityOwned < 1)
			{
				bauble.baubleDescription = "If scored hand contains any face cards, give a random one +1 multiplier for each scoring face card, permanently";
			}
			else
			{
				bauble.baubleDescription = "If scored hand contains any face cards, give a random one +1 multiplier for each scoring face card, permanently. Currently giving +" + (bauble.quantityOwned * 1);
			}
			break;
			case 37:
			bauble.baubleImpact = 2.5f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a full house get 2.5x multiplier";
			break;
			case 38:
			shopScript.rerollBaseCost = shopScript.rerollStartingCost - bauble.quantityOwned;
			shopScript.rerollCurrentCost--;
			shopScript.UpdateRerollCost();
			bauble.baubleDescription = "Decreases the base cost of rerolling in the shop by 1";
			break;
			case 39:
			bauble.baubleImpact = 2f * bauble.quantityOwned;
			bauble.baubleDescription = "Scored hands that contain at least one heart, one diamond, one club and one spade get double multiplier. Rainbow cards can fill gaps";
			break;
			case 40:
			bauble.baubleImpact = 5f * bauble.quantityOwned;
			bauble.baubleDescription = "Each scored King adds +5 to multiplier";
			break;
			case 41:
			if(bauble.quantityOwned > 2)
			{
				bauble.quantityOwned = 2;
				break;
			}
			if(bauble.quantityOwned > 0)
			{
				handValues.handZones[0].ExpandHandZone();
				for(int i = 10; i <= 13; i++)
				{
					handTypesToShow[i] = true;
					HandsInformation.instance.ReorganizeValueLabels();
				}
				HandsInformation.instance.individualTooltipRT.anchoredPosition = new Vector2(HandsInformation.instance.individualTooltipRT.anchoredPosition.x, 314);
				HandsInformation.instance.minimumTooltipRT.anchoredPosition = new Vector2(HandsInformation.instance.minimumTooltipRT.anchoredPosition.x, 314);;
				if(bauble.quantityOwned >= 2)
				{
					for(int i = 14; i <= 17; i++)
					{
						handTypesToShow[i] = true;
						HandsInformation.instance.ReorganizeValueLabels();
					}
					HandsInformation.instance.individualTooltipRT.anchoredPosition = new Vector2(HandsInformation.instance.individualTooltipRT.anchoredPosition.x, 338);
					HandsInformation.instance.minimumTooltipRT.anchoredPosition = new Vector2(HandsInformation.instance.minimumTooltipRT.anchoredPosition.x, 331);;
				}
			}
			else
			{
				handValues.handZones[0].SetHandZoneSize(5);
				HandsInformation.instance.individualTooltipRT.anchoredPosition = new Vector2(HandsInformation.instance.individualTooltipRT.anchoredPosition.x, 278);
				HandsInformation.instance.minimumTooltipRT.anchoredPosition = new Vector2(HandsInformation.instance.minimumTooltipRT.anchoredPosition.x, 278);;
			}
			if(GameOptions.instance.restrictSelection)
			{
				handScript.maxCardsSelectedAtOnce = bauble.quantityOwned + 5;
			}
			else
			{
				handScript.maxCardsSelectedAtOnce = 9001;
			}
			bauble.baubleDescription = "Increases number of playable cards by 1, unlocking new hand types";
			break;
			case 47:
			bauble.baubleImpact = 5f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain 5 of a kind get x5 multiplier";
			break;
			case 48:
			bauble.baubleImpact = 5f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a triple double get x5 multiplier";
			break;
			case 49:
			bauble.baubleImpact = 5f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a double triple get x5 multiplier";
			break;
			case 50:
			bauble.baubleImpact = 5.5f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a stuffed house get x5.5 multiplier";
			break;
			case 51:
			bauble.baubleImpact = 6f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain six of a kind get x6 multiplier";
			break;
			case 52:
			bauble.baubleImpact = 6f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a guest house get x6 multiplier";
			break;
			case 53:
			bauble.baubleImpact = 7f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a wide house get x7 multiplier";
			break;
			case 54:
			bauble.baubleImpact = 8f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain a huge house get x8 multiplier";
			break;
			case 55:
			bauble.baubleImpact = 10f * bauble.quantityOwned;
			bauble.baubleDescription = "Scoring hands that contain seven of a kind get x10 multiplier";
			break;
			case 56:
			switch(bauble.quantityOwned)
			{
				case 0:
				bauble.baubleImpact = 0f;
				bauble.baubleImage = dieSprites[5];
				bauble.baubleName = "D6";
				bauble.baubleDescription = "Before scoring a hand, roll a D6. On a 1, nothing happens. On a 2-6, add that much multiplier. On a 6, also gain a chip";
				break;
				case 1:
				bauble.baubleImpact = 6f;
				bauble.baubleName = "D8";
				bauble.baubleDescription = "Replaces D6. Before scoring a hand, roll a D8. On a 1, nothing happens. On a 2-8, add that much multiplier. On a 6 or higher, also gain a chip";
				break;
				case 2:
				bauble.baubleImpact = 8f;
				bauble.baubleName = "D10";
				bauble.baubleDescription = "Replaces D8. Before scoring a hand, roll a D10. On a 1, nothing happens. On a 2-10, add that much multiplier. On a 6 or higher, also gain a chip";
				break;
				case 3:
				bauble.baubleImpact = 10f;
				bauble.baubleName = "D12";
				bauble.baubleDescription = "Replaces D10. Before scoring a hand, roll a D12. On a 1, nothing happens. On a 2-12, add that much multiplier. On a 6 or higher, also gain a chip";
				break;
				case 4:
				bauble.baubleImpact = 12f;
				bauble.baubleName = "D20";
				bauble.baubleDescription = "Replaces D12. Before scoring a hand, roll a D20. On a 1, nothing happens. On a 2-20, add that much multiplier. On a 6 or higher, also gain a chip. On a 20, gain 10 chips";
				break;
				case 5:
				bauble.baubleImpact = 20f;
				break;
			}
			break;
			case 57:
			bauble.baubleDescription = "Whenever you play a single card as your hand, destroy it without scoring";
			break;
			case 58:
			if(bauble.quantityOwned > 0)
			{
				shopScript.chanceForCardToBeNonStandard = 0.5f;
			}
			else
			{
				shopScript.chanceForCardToBeNonStandard = 0.25f;
			}
			bauble.baubleDescription = "Doubles the chance for cards in the shop to be nonstandard. Chance goes from 25% to 50%";
			break;
			case 59:
			if(bauble.quantityOwned < 1)
			{
				bauble.baubleDescription = "When you start a new ante, add a spyglass to your deck. Spyglasses are nonstandard cards that cannot be played, but while in hand, grant a zodiac for the next hand you score";
			}
			else
			{
				bauble.baubleDescription = "When you start a new ante, add a spyglass to your deck. Spyglasses are nonstandard cards that cannot be played, but while in hand, grant a zodiac for the next hand you score. Currently gaining " + bauble.quantityOwned + " spyglass per ante";
			}
			break;
			case 60:
			bauble.baubleDescription = "Whenever you clear 2 or more chip thresholds with a single hand, gain an additional chip for each chip threshold beyond the first. Clearing them all at once nets an additional 2 chips";
			break;
			case 61:
			bauble.baubleDescription = "If played hand contains a straight, give all scoring aces +15 base value and +3 multiplier, permanently";
			break;
			case 62:
			bauble.baubleDescription = "If scored hand contains a queen, gain a zodiac for played hand for each scoring queen. After scoring, destroy all scored queens";
			break;
		}
/* 		if(bauble.quantityOwned >= bauble.maxQuantity && bauble.maxQuantity > 0)
		{
			//print("removing " + bauble.baubleName + " from category " + bauble.baubleCategory);
			//baublesByRarity[bauble.baubleCategory].Remove(bauble.baubleNumber);
		} */
	}
}