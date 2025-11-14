using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class DeckScript : MonoBehaviour
{
	public static DeckScript instance;
	public GameObject cardPrefab;
	public Transform drawPileParent;
	private const int cardWidth = 45;
	public Sprite[] rankSprites;
	public Sprite[] bigSuitSprites;
	public Sprite[] cardDetails;
	public Sprite[] nonStandardCardDetails;
	public Color[] suitColors; // ♥ = Red ♦ = Blue ♣ = Green ♠ = Black
	public HandScript handScript;
	public GameObject tooltipPrefab;
	public Sprite cardBackSprite;
	public Color[] cardBackColors;
	public RandomNumbers randomNumbers;
	public DeckViewer deckViewer;
	
	void Awake()
	{
		instance = this;
	}
	
	public void ShuffleDeck()
	{
		Transform[] cards = new Transform[drawPileParent.childCount];
		int[] deckOrder = new int[cards.Length];
		for(int i = 0; i <cards.Length; i++)
		{
			deckOrder[i] = i;
		}
		for(int j = 0; j < cards.Length; j++)
		{
			//int k = UnityEngine.Random.Range(0, j+1);
			//int k = Random.NextInt(0, j + 1);
			int k = randomNumbers.Range(0, j + 1);
			int temp = deckOrder[j];
			deckOrder[j] = deckOrder[k];
			deckOrder[k] = temp;
		}
		
		for(int l = 0; l < cards.Length; l++)
		{
			cards[l] = drawPileParent.GetChild(l);
		}
		for(int m = 0; m < cards.Length; m++)
		{
			cards[m].SetSiblingIndex(deckOrder[m]);
		}
	}
	
	public float GetBaseValueBasedOnRank(int rank)
	{
		if(rank <= 8)
		{
			return rank + 2f;
		}
		else if(rank <= 11)
		{
			return 10f;
		}
		else if(rank == 12)
		{
			return 15f;
		}
		else
		{
			print("GetBaseValueBasedOnRank was called for a rank > 12");
			return -1f;
		}
	}
	
	public CardScript CreateNonStandardCard(Sprite cardSprite, int cardNumber, string description, string cardName, int rarity, Transform cardParent, Vector2 position, bool faceDown, bool makeTooltip = true, bool cannotBePlayed = false)
	{
		GameObject newCard = Instantiate(cardPrefab, new Vector3(0,0,0), Quaternion.identity, cardParent);
		newCard.name = cardName;
		RectTransform newRect = newCard.GetComponent<RectTransform>();
		newRect.anchoredPosition = position;
		CardScript cardScript = newCard.GetComponent<CardScript>();
		cardScript.rankInt = -cardNumber - 1;	// for sorting
		cardScript.suitInt = 5;					// for deckviewer
		cardScript.nonStandardCardNumber = cardNumber;
		cardScript.cannotBePlayed = cannotBePlayed;
		
		cardScript.borderImage.color = Color.black;
		cardScript.backgroundImage.color = Color.white;
		cardScript.rankImage.color = Color.clear;
		cardScript.bigSuitImage.color = Color.clear;
		cardScript.cardBackImage.sprite = cardBackSprite;
		cardScript.cardBackImage.color = cardBackColors[0];
		
		cardScript.handScript = handScript;
		cardScript.UpdateCardValueTooltip();
		cardScript.UpdateCardMultTooltip();
		cardScript.cardValueTooltipObject.SetActive(false);
		cardScript.cardMultiplierTooltipRT.gameObject.SetActive(false);
		cardScript.noImpactOnHandSize = true;
		
		cardScript.detailImage.sprite = cardSprite;
		RectTransform cardDetailRT = cardScript.detailImage.GetComponent<RectTransform>();
		cardDetailRT.sizeDelta = new Vector2(cardSprite.rect.width, cardSprite.rect.height);
		int xPos = Mathf.CeilToInt((45 - cardSprite.rect.width) / 2f);
		int yPos = Mathf.CeilToInt((45 - cardSprite.rect.height) / 2f);
		cardDetailRT.anchoredPosition = new Vector2(xPos, yPos);
		if(makeTooltip)
		{
			GameObject newTooltip = Instantiate(tooltipPrefab, new Vector3(0,0,0), Quaternion.identity, newCard.transform);
			TooltipScript newTooltipScript = newTooltip.GetComponent<TooltipScript>();
			newTooltipScript.SetupTooltip(description, cardName, rarity);
			cardScript.tooltipScript = newTooltipScript;
			newTooltipScript.gameObject.SetActive(false);
		}
		cardScript.faceDown = faceDown;
		cardScript.UpdateFacing();
		
		return cardScript;
	}
	
	public CardScript CreateCard(int suitInt, int rankInt, Transform cardParent, Vector2 position, bool faceDown)
	{
		GameObject newCard = Instantiate(cardPrefab, new Vector3(0,0,0), Quaternion.identity, cardParent);
		RectTransform newRect = newCard.GetComponent<RectTransform>();
		newRect.anchoredPosition = position;
		CardScript cardScript = newCard.GetComponent<CardScript>();
		cardScript.rankInt = rankInt;
		int cardNumber = suitInt * 13 + rankInt;
		
		cardScript.borderImage.color = Color.black;
		
		if(suitInt < 4)
		{
			cardScript.rankImage.sprite = rankSprites[rankInt];
			cardScript.rankImage.GetComponent<RectTransform>().sizeDelta = new Vector2(rankSprites[rankInt].rect.width, 13);
			cardScript.rankImage.color = suitColors[suitInt];
		}
		else if(suitInt == 4)
		{
			cardScript.rankImage.sprite = rankSprites[rankInt + 13];
			cardScript.rankImage.GetComponent<RectTransform>().sizeDelta = new Vector2(rankSprites[rankInt].rect.width, 15);
			cardScript.rankImage.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 2);
		}
		
		cardScript.bigSuitImage.sprite = bigSuitSprites[suitInt];
		cardScript.bigSuitImage.GetComponent<RectTransform>().sizeDelta = new Vector2(bigSuitSprites[suitInt].rect.width, bigSuitSprites[suitInt].rect.height);
		if(suitInt < 4)
		{
			cardScript.bigSuitImage.color = suitColors[suitInt];
		}
		
		cardScript.detailImage.sprite = cardDetails[cardNumber];
		cardScript.detailImage.GetComponent<RectTransform>().sizeDelta = new Vector2(cardDetails[cardNumber].rect.width, cardDetails[cardNumber].rect.height);
		if((rankInt <= 8 || rankInt == 12) && suitInt < 4)
		{
			cardScript.detailImage.color = suitColors[suitInt];
		}
		int xPos = 17 + Mathf.CeilToInt((24 - cardDetails[cardNumber].rect.width) / 2f);
		int yPos = 5 + Mathf.CeilToInt((35 - cardDetails[cardNumber].rect.height) / 2f);
		cardScript.detailImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
		cardScript.cardBackImage.sprite = cardBackSprite;
		cardScript.cardBackImage.color = cardBackColors[0];
		
		cardScript.backgroundImage.color = Color.white;
		cardScript.suitInt = suitInt;
		switch(suitInt)
		{
			case 0:
			cardScript.suitChar = 'h';
			cardScript.suitSymbol = '♥';	
			break;
			case 1:
			cardScript.suitChar = 'd';
			cardScript.suitSymbol = '♦';
			break;
			case 2:
			cardScript.suitChar = 'c';
			cardScript.suitSymbol = '♣';
			break;
			case 3:
			cardScript.suitChar = 's';
			cardScript.suitSymbol = '♠';
			break;
			case 4:
			cardScript.suitChar = 'r';
			cardScript.suitSymbol =  '☼';
			break;
		}
		switch(rankInt)
		{
			case 0:
			cardScript.rankChar = (char)((rankInt+2) + '0');
			cardScript.cardValue = 2f + rankInt;
			cardScript.rankString = "Deuce";
			break;
			case 1:
			cardScript.rankChar = (char)((rankInt+2) + '0');
			cardScript.cardValue = 2f + rankInt;
			cardScript.rankString = "Three";
			break;
			case 2:
			cardScript.rankChar = (char)((rankInt+2) + '0');
			cardScript.cardValue = 2f + rankInt;
			cardScript.rankString = "Four";
			break;
			case 3:
			cardScript.rankChar = (char)((rankInt+2) + '0');
			cardScript.cardValue = 2f + rankInt;
			cardScript.rankString = "Five";
			break;
			case 4:
			cardScript.rankChar = (char)((rankInt+2) + '0');
			cardScript.cardValue = 2f + rankInt;
			cardScript.rankString = "Six";
			break;
			case 5:
			cardScript.rankChar = (char)((rankInt+2) + '0');
			cardScript.cardValue = 2f + rankInt;
			cardScript.rankString = "Seven";
			break;
			case 6:
			cardScript.rankChar = (char)((rankInt+2) + '0');
			cardScript.cardValue = 2f + rankInt;
			cardScript.rankString = "Eight";
			break;
			case 7:
			cardScript.rankChar = (char)((rankInt+2) + '0');
			cardScript.cardValue = 2f + rankInt;
			cardScript.rankString = "Nine";
			break;
			case 8:
			cardScript.rankChar = 'T';
			cardScript.cardValue = 10f;
			cardScript.rankString = "Ten";
			break;
			case 9:
			cardScript.rankChar = 'J';
			cardScript.cardValue = 10f;
			cardScript.rankString = "Jack";
			break;
			case 10:
			cardScript.rankChar = 'Q';
			cardScript.cardValue = 10f;
			cardScript.rankString = "Queen";
			break;
			case 11:
			cardScript.rankChar = 'K';
			cardScript.cardValue = 10f;
			cardScript.rankString = "King";
			break;
			case 12:
			cardScript.rankChar = 'A';
			cardScript.cardValue = 15f;
			cardScript.rankString = "Ace";
			break;
			default:
			print("Mixup on DeckScript");
			cardScript.rankChar = (char)((rankInt+2) + '0');
			cardScript.cardValue = 2 + rankInt;
			break;
		}
		cardScript.handScript = handScript;
		cardScript.UpdateCardValueTooltip();
		cardScript.UpdateCardMultTooltip();
		cardScript.cardValueTooltipObject.SetActive(false);
		cardScript.cardMultiplierTooltipRT.gameObject.SetActive(false);
		if(rankInt < 8)
		{
			newCard.name = (rankInt + 2).ToString() + cardScript.suitChar;
		}
		else
		{
			char nameChar = 'X';
			switch(rankInt)
			{
				case 8:
				nameChar = 'T';
				break;
				case 9:
				nameChar = 'J';
				break;
				case 10:
				nameChar = 'Q';
				break;
				case 11:
				nameChar = 'K';
				break;
				case 12:
				nameChar = 'A';
				break;
			}
			newCard.name = nameChar.ToString() + cardScript.suitChar;
		}
		cardScript.faceDown = faceDown;
		cardScript.UpdateFacing();
		cardScript.countCardInHand = true;
		cardScript.countCardInScoring = true;
		cardScript.standardCard = true;
		if(cardParent == drawPileParent)
		{
			cardScript.cardLocation = 1;
			GameObject clonedCard = Instantiate(newCard, new Vector3(0,0,0), Quaternion.identity, deckViewer.cloneParent);
			CardScript clonedCardScript = clonedCard.GetComponent<CardScript>();
			cardScript.deckViewerClone = clonedCardScript;
			deckViewer.clonedCards.Add(clonedCardScript);
			clonedCardScript.cardLocation = 1;
			clonedCardScript.isDeckViewerClone = true;
			clonedCardScript.faceDown = false;
			clonedCardScript.UpdateFacing();
			clonedCardScript.gameObject.SetActive(false);
		}			
		return cardScript;
	}
	
	public void CreateDeck(int numberOfCards, bool noFaceCards = false)
	{
		int cardsMade = 0;
		for(int i = 0; i < 65; i++)
		{
			int suitInt = i / 13;
			int rankInt = i % 13;
			if((rankInt < 9 || rankInt > 11) || !noFaceCards)
			{
				if(RunVariations.instance.variantModeToggle.isOn)
				{
					if(CardSelection.instance.cardHelpers[i].quantityChange >= 0)
					{
						int cardsInBaseDeck = 0;
						if(i < numberOfCards)
						{
							cardsInBaseDeck++;
						}
						for(int j = 0; j < CardSelection.instance.cardHelpers[i].quantityChange + cardsInBaseDeck; j++)
						{
							CreateCard(suitInt, rankInt, drawPileParent, new Vector2(22.5f, 22.5f), true);
							cardsMade++;
						}
					}
				}
				else
				{
					if(i < numberOfCards)
					{
						CreateCard(suitInt, rankInt, drawPileParent, new Vector2(22.5f, 22.5f), true);
						cardsMade++;
					}
				}
			}
		}
		if(RunVariations.instance.variantModeToggle.isOn)
		{
			for(int i = 65; i < CardSelection.instance.cardHelpers.Count; i++)
			{
				if(CardSelection.instance.cardHelpers[i])
				{
					if(CardSelection.instance.cardHelpers[i].quantityChange > 0)
					{
						for(int j = 0; j < CardSelection.instance.cardHelpers[i].quantityChange; j++)
						{
							ShopScript.instance.CheatAddNonStandardCard(CardSelection.instance.cardHelpers[i].nonStandardCardNumber, false);
							cardsMade++;
						}
					}
				}
			}
			int maxCardInt = 52;
			if(CardSelection.instance.includeRainbowToggle.isOn)
			{
				maxCardInt = 65;
			}
			for(int i = 0; i < int.Parse(CardSelection.instance.randomStandardCardsInputField.text); i++)
			{
				int cardInt = randomNumbers.Range(0,maxCardInt);
				{
					int suitInt = cardInt / 13;
					int rankInt = cardInt % 13;
					if(noFaceCards && rankInt >= 9 && rankInt <= 11)
					{
						i--;
					}
					else
					{
						CreateCard(suitInt, rankInt, drawPileParent, new Vector2(22.5f, 22.5f), true);
						cardsMade++;
					}
				}
			}
			for(int i = 0; i < int.Parse(CardSelection.instance.randomNonstandardCardsInputField.text); i++)
			{
				if(CardSelection.instance.considerRarityToggle.isOn)
				{
					ShopScript.instance.CheatAddNonStandardCard(NonStandardCardScript.instance.GetRandomNonStandardCardInt(NonStandardCardScript.instance.commonCardWeight, NonStandardCardScript.instance.uncommonCardWeight, NonStandardCardScript.instance.rareCardWeight, NonStandardCardScript.instance.legendaryCardWeight), false);
				}
				else
				{
					ShopScript.instance.CheatAddNonStandardCard(randomNumbers.Range(0, NonStandardCardScript.instance.nonStandardCards.Length), false);
				}
			}
		}
		handScript.DrawPileCountChanged(cardsMade);
	}
	
    void Start()
    {
        //CreateDeck();
		//ShuffleDeck();
    }
	
    void Update()
    {
        
    }
}
