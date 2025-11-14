using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckViewer : MonoBehaviour
{
	public static DeckViewer instance;
	public RectTransform rt;
	
	public Transform cloneParent;
	public Transform drawPileParent;
	public Transform discardParent;
	public List<CardScript> clonedCards;

	public TMP_Text[] rankAmounts;
	public TMP_Text[] typeAmounts;
	public TMP_Text[] suitAmounts;
	public TMP_Text[] totalCardsTexts;
	
	public MovingButton[] deckViewButtons;
	public HandValues handValues;
	public bool isOpen;
	public int curType;
	public int curCardsCount;
	
	public RectTransform statsScreenRT;
	
	public Image cardBackground;
	public Image cardBackDetail;
	public TMP_Text[] deckNameTexts;
	public TMP_Text[] deckDescriptionTexts;
	public int deckInUse;
	public string variantInUse;
	
	void Awake()
	{
		instance = this;
	}
	
	public int GetNumberOfCardsInDeck()
	{
		return clonedCards.Count;
	}
	
	public void UpdateDeckType(int deck)
	{
		deckInUse = deck;
		for(int i = 0; i < deckNameTexts.Length; i++)
		{
			deckNameTexts[i].text = Decks.instance.decks[deck].deckName;
		}
		cardBackDetail.sprite = Decks.instance.decks[deck].design;
		cardBackDetail.color = Decks.instance.decks[deck].designColor;
		cardBackground.color = Decks.instance.decks[deck].backColor;
		for(int i = 0; i < deckDescriptionTexts.Length; i++)
		{
			deckDescriptionTexts[i].text = Decks.instance.decks[deck].description;
		}
	}
	
	public void CheckForRainbowDeckUnlock()
	{
		if(!Decks.instance.decks[2].unlocked && !Statistics.instance.currentRun.runIsDailyGame && !Statistics.instance.currentRun.runIsSeededGame && variantInUse == "")
		{
			int rainbowCards = 0;
			for(int i = 0; i < clonedCards.Count; i++)
			{
				if(clonedCards[i].suitInt == 4)
				{
					rainbowCards++;
				}
			}
			if(rainbowCards >= 13)
			{
				Decks.instance.decks[2].unlocked = true;
				Decks.instance.UpdateDecksFile();
				Decks.instance.DeckKnobs[2].knobImage.sprite = Decks.instance.unlockedKnob;
				Decks.instance.DeckKnobs[2].rt.sizeDelta = new Vector2(10,10);
				UnlockNotifications.instance.CreateNewUnlockNotifier(0,2);
			}
		}
	}
	
	public void CheckForRepublicDeckUnlock()
	{
		if(!Decks.instance.decks[3].unlocked && !Statistics.instance.currentRun.runIsDailyGame && !Statistics.instance.currentRun.runIsSeededGame && variantInUse == "")
		{
			for(int i = 0; i < clonedCards.Count; i++)
			{
				if(clonedCards[i].rankInt >= 9 && clonedCards[i].rankInt <= 11)
				{
					return;
				}
			}
			Decks.instance.decks[3].unlocked = true;
			Decks.instance.UpdateDecksFile();
			Decks.instance.DeckKnobs[3].knobImage.sprite = Decks.instance.unlockedKnob;
			Decks.instance.DeckKnobs[3].rt.sizeDelta = new Vector2(10,10);
			UnlockNotifications.instance.CreateNewUnlockNotifier(0,3);
		}
	}
	
	public void CreateDeckView(int type) // 0 = draw pile, 1 = full deck, 2 = discards
	{
		for(int i = 0; i < 3; i ++)
		{
			if(type == i)
			{
				deckViewButtons[i].ChangeSpecailState(true);
			}
			else
			{
				deckViewButtons[i].ChangeSpecailState(false);	
			}
		}
		int[] ranks = new int[13];
		int[] suits = new int[6];
		int[] types = new int[4];
		int totalCards = 0;
		clonedCards.Sort((a, b) => 
		{
			int suitSort = a.suitInt.CompareTo(b.suitInt);
			if(suitSort == 0)
			{
				return a.rankInt.CompareTo(b.rankInt);
			}
			else
			{
				return suitSort;
			}
		});
		for(int i = 0; i < clonedCards.Count; i++)
		{
			clonedCards[i].transform.SetSiblingIndex(i);
			if(type == 1 || clonedCards[i].cardLocation -1 == type)		// 0 = shop / unknown. 1 = draw pile, 2 = hand, 3 = discard
			{
				totalCards++;
				if(clonedCards[i].standardCard)
				{
					ranks[clonedCards[i].rankInt]++;
					suits[clonedCards[i].suitInt]++;
					if(clonedCards[i].rankInt == 12)
					{
						types[3]++;
					}
					else if(clonedCards[i].rankInt < 12 && clonedCards[i].rankInt >= 9)
					{
						types[2]++;
					}
					else
					{
						types[1]++;
					}
				}
				else
				{
					suits[clonedCards[i].suitInt]++;
					types[0]++;
				}
			}
		}
		for(int i = 0; i < totalCardsTexts.Length; i++)
		{
			totalCardsTexts[i].text = "" + totalCards;
		}
		for(int i = 0; i < ranks.Length; i++)
		{
			rankAmounts[i * 2].text = "" + ranks[i];
			rankAmounts[i * 2 + 1].text = "" + ranks[i];
		}
		for(int i = 0; i < types.Length; i++)
		{
			typeAmounts[i * 2].text = "" + types[i];
			typeAmounts[i * 2 + 1].text = "" + types[i];
		}
		int rows = 0;
		for(int i = 0; i < suits.Length; i++)
		{
			if(i < 5)
			{
				suitAmounts[i * 2].text = "" + suits[i];
				suitAmounts[i * 2 + 1].text = "" + suits[i];
			}
			if(suits[i] > 0)
			{
				rows++;
			}
		}
		Vector2 cardSpace = new Vector2(324f, 276f);
		int curRow = 0;
		float cardSize = 45f;	// they are square
		float ySqueezeDistance = (cardSpace.y - cardSize) / (rows - 1);
		float yDistanceBetweenCards = Mathf.Min(55f, ySqueezeDistance);
		int rowCardsPlaced = 0;
		float xSqueezeDistance = 0;
		float xDistanceBetweenCards = 0;
		int lastUsedCardSuit = -1;
		for(int i = 0; i < clonedCards.Count; i++)
		{
			if(type == 1 || clonedCards[i].cardLocation -1 == type)
			{
				clonedCards[i].gameObject.SetActive(true);
				if(lastUsedCardSuit != -1)
				{
					if(clonedCards[i].suitInt != lastUsedCardSuit)
					{
						curRow++;
						rowCardsPlaced = 0;
						lastUsedCardSuit = clonedCards[i].suitInt;
						xSqueezeDistance = (cardSpace.x - cardSize) / (suits[clonedCards[i].suitInt] - 1);
						xDistanceBetweenCards = Mathf.Min(50f, xSqueezeDistance);
					}
				}
				else
				{
					lastUsedCardSuit = clonedCards[i].suitInt;
					xSqueezeDistance = (cardSpace.x - cardSize) / (suits[clonedCards[i].suitInt] - 1);
					xDistanceBetweenCards = Mathf.Min(50f, xSqueezeDistance);
				}
				
				float yDestination = (-rows + 1) * (yDistanceBetweenCards / 2f) + (rows - curRow - 1) * yDistanceBetweenCards;
				float xDestination = (suits[clonedCards[i].suitInt] - 1) * (xDistanceBetweenCards / 2f) - (suits[clonedCards[i].suitInt] - rowCardsPlaced - 1) * xDistanceBetweenCards;
				clonedCards[i].rt.anchoredPosition = new Vector2(xDestination, yDestination);
				//print("curRow= " + curRow + " " + clonedCards[i].GetCardNameCharSuit() + " rowCardsPlaced= " + rowCardsPlaced + " suits[clonedCards[i].suitInt]= " + suits[clonedCards[i].suitInt] + " xSqueezeDistance= " + xSqueezeDistance + " xDistanceBetweenCards= " + xDistanceBetweenCards);
				rowCardsPlaced++;
			}
			else
			{
				clonedCards[i].gameObject.SetActive(false);
			}
		}
	}
	
	public void DrawPileClicked()
	{
		if(!isOpen)
		{
			OpenDeckViewer();
			isOpen = true;
		}
		if(drawPileParent.childCount > 0)
		{
			CreateDeckViewFromType(0);
		}
		else
		{
			CreateDeckViewFromType(1);
		}
	}
	
	public void DiscardPileClicked()
	{
		if(!isOpen)
		{
			OpenDeckViewer();
			isOpen = true;
		}
		if(discardParent.childCount > 0)
		{
			CreateDeckViewFromType(2);
		}
		else
		{
			CreateDeckViewFromType(1);
		}
	}
	
	public void OpenDeckViewer()
	{
		StartCoroutine(handValues.scoreVial.MoveOverTime(rt, new Vector3(0,360,0), new Vector3(0,0,0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		if(statsScreenRT.gameObject.activeSelf)
		{
			StartCoroutine(handValues.scoreVial.MoveOverTime(statsScreenRT, new Vector3(98,0,0), new Vector3(98,-360,0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		}
	}
	
	public void CloseDeckViewer()
	{
		StartCoroutine(handValues.scoreVial.MoveOverTime(rt, new Vector3(0,0,0), new Vector3(0,360,0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		if(statsScreenRT.gameObject.activeSelf)
		{
			StartCoroutine(handValues.scoreVial.MoveOverTime(statsScreenRT, new Vector3(98,-360,0), new Vector3(98,0,0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		}
	}
	
	public void BackClicked()
	{
		if(isOpen)
		{
			CloseDeckViewer();
			isOpen = false;
		}
	}
	
	public void CreateDeckViewFromType(int type)	// 0 = draw pile, 1 = full deck, 2 = discards
	{
		curType = type;
		CreateDeckView(curType);
	}
}