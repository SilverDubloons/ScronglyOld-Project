using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardSelection : MonoBehaviour
{
    public static CardSelection instance;
	public Transform cardParent;
	public Transform nonstandardCardParent;
	public GameObject cardHelperPrefab;
	public Color negativeCardsColor;
	public Color positiveCardsColor;
	public List<CardHelper> cardHelpers = new List<CardHelper>();
	public TMP_InputField randomStandardCardsInputField;
	public TMP_InputField randomNonstandardCardsInputField;
	public Toggle includeRainbowToggle;
	public Toggle considerRarityToggle;
	public GameObject cardSelectionInterface;
	public int maxStandardCards;
	public int maxNonstandardCards;
	public bool cardsSelectionHasChanged;
	public GameObject cardQuantityLabelPrefab;
	
	void Awake()
	{
		instance = this;
	}
	
	public float CreateCardIconsForDisplay(Transform cardParent, float startingY)
	{
		int quantityMade = 0;
		for(int i = 0; i < cardHelpers.Count; i++)
		{
			if(cardHelpers[i].quantityChange != 0)
			{
				if(cardHelpers[i].standardCard)
				{
					CardScript newCard = DeckScript.instance.CreateCard(cardHelpers[i].suitInt, cardHelpers[i].rankInt, cardParent, Vector2.zero, false);
					newCard.rt.anchorMin = new Vector2(0,1);
					newCard.rt.anchorMax = new Vector2(0,1);
					newCard.rt.anchoredPosition = new Vector2(3 + 22.5f + (quantityMade % 3) * 48, startingY - 22.5f - (quantityMade / 3) * 48);
					newCard.justForShow = true;
					Destroy(newCard.cardBackImage.gameObject);
					Destroy(newCard.cardValueTooltipObject);
					Destroy(newCard.cardMultiplierTooltipRT.gameObject);
					GameObject cardQuantityLabelObject = Instantiate(cardQuantityLabelPrefab, Vector3.zero, Quaternion.identity, newCard.transform);
					CardQuantityLabel cardQuantityLabel = cardQuantityLabelObject.GetComponent<CardQuantityLabel>();
					cardQuantityLabel.rt.anchoredPosition = new Vector2(0, 15);
					cardQuantityLabel.ChangeQuantity(cardHelpers[i].quantityChange);
				}
				else
				{
					CardScript newCard = DeckScript.instance.CreateNonStandardCard(NonStandardCardScript.instance.nonStandardCards[cardHelpers[i].nonStandardCardNumber].cardImage, cardHelpers[i].nonStandardCardNumber, NonStandardCardScript.instance.nonStandardCards[cardHelpers[i].nonStandardCardNumber].cardDescription, NonStandardCardScript.instance.nonStandardCards[cardHelpers[i].nonStandardCardNumber].cardName, NonStandardCardScript.instance.nonStandardCards[cardHelpers[i].nonStandardCardNumber].cardCategory, cardParent, Vector2.zero, false, false, NonStandardCardScript.instance.nonStandardCards[cardHelpers[i].nonStandardCardNumber].cannotBePlayed);
					newCard.rt.anchorMin = new Vector2(0,1);
					newCard.rt.anchorMax = new Vector2(0,1);
					newCard.rt.anchoredPosition = new Vector2(3 + 22.5f + (quantityMade % 3) * 48, startingY - 22.5f - (quantityMade / 3) * 48);
					newCard.justForShow = true;
					Destroy(newCard.cardBackImage.gameObject);
					Destroy(newCard.cardValueTooltipObject);
					Destroy(newCard.cardMultiplierTooltipRT.gameObject);
					GameObject cardQuantityLabelObject = Instantiate(cardQuantityLabelPrefab, Vector3.zero, Quaternion.identity, newCard.transform);
					CardQuantityLabel cardQuantityLabel = cardQuantityLabelObject.GetComponent<CardQuantityLabel>();
					cardQuantityLabel.rt.anchoredPosition = new Vector2(0, 15);
					cardQuantityLabel.ChangeQuantity(cardHelpers[i].quantityChange);
				}
				quantityMade++;
			}
		}
		return startingY - (quantityMade / 3 + 1) * 48;
	}
	
	public void SetupCardSelection()
	{
		for(int i = 0; i < 65; i++)
		{
			int suitInt = i / 13;
			int rankInt = i % 13;
			CardScript newCard = DeckScript.instance.CreateCard(suitInt, rankInt, cardParent, Vector2.zero, false);
			newCard.rt.anchorMin = new Vector2(0,1);
			newCard.rt.anchorMax = new Vector2(0,1);
			newCard.rt.anchoredPosition = new Vector2(27.5f + rankInt * 47f, -27.5f - suitInt * 47f);
			newCard.justForShow = true;
			
			GameObject newCardHelperObject = Instantiate(cardHelperPrefab, Vector3.zero, Quaternion.identity, newCard.transform);
			CardHelper newCardHelper = newCardHelperObject.GetComponent<CardHelper>();
			cardHelpers.Add(newCardHelper);
			newCardHelper.rt.anchoredPosition = new Vector2(0,0);
			newCardHelper.standardCard = true;
			newCardHelper.suitInt = suitInt;
			newCardHelper.rankInt = rankInt;
			if(i >= 52)
			{
				newCardHelper.minusButton.ChangeDisabled(true);
			}
			
			Destroy(newCard.cardBackImage.gameObject);
			Destroy(newCard.cardValueTooltipObject);
			Destroy(newCard.cardMultiplierTooltipRT.gameObject);
			//Destroy(newCard);
		}
		for(int i = 0; i < NonStandardCardScript.instance.nonStandardCards.Length; i++)
		{
			CardScript newCard = DeckScript.instance.CreateNonStandardCard(NonStandardCardScript.instance.nonStandardCards[i].cardImage, i, NonStandardCardScript.instance.nonStandardCards[i].cardDescription, NonStandardCardScript.instance.nonStandardCards[i].cardName, NonStandardCardScript.instance.nonStandardCards[i].cardCategory, nonstandardCardParent, Vector2.zero, false, true, NonStandardCardScript.instance.nonStandardCards[i].cannotBePlayed);
			newCard.rt.anchorMin = new Vector2(0,1);
			newCard.rt.anchorMax = new Vector2(0,1);
			newCard.rt.anchoredPosition = new Vector2(27.5f + i * 47f, -27.5f - 5 * 47f);
			newCard.justForShow = true;
			
			GameObject newCardHelperObject = Instantiate(cardHelperPrefab, Vector3.zero, Quaternion.identity, newCard.transform);
			CardHelper newCardHelper = newCardHelperObject.GetComponent<CardHelper>();
			cardHelpers.Add(newCardHelper);
			newCardHelper.rt.anchoredPosition = new Vector2(0,0);
			newCardHelper.standardCard = false;
			newCardHelper.nonStandardCardNumber = i;
			newCardHelper.minusButton.ChangeDisabled(true);
			
			Destroy(newCard.cardBackImage.gameObject);
			Destroy(newCard.cardValueTooltipObject);
			Destroy(newCard.cardMultiplierTooltipRT.gameObject);
		}
	}
	
	void Start()
	{
		SetupCardSelection();
		cardSelectionInterface.SetActive(false);
	}
	
	public string ConvertCardSelectionToString()
	{
		string cardSel = "";
		for(int i = 0; i < cardHelpers.Count; i++)
		{
			if(cardHelpers[i].quantityChange != 0)
			{
				if(cardSel != "")
				{
					cardSel += "_";
				}
				cardSel += i.ToString() + ":" + cardHelpers[i].quantityChange;
			}
		}
		cardSel += "|" + randomStandardCardsInputField.text + ":" + includeRainbowToggle.isOn.ToString() + ":" + randomNonstandardCardsInputField.text + ":" + considerRarityToggle.isOn.ToString();
		return cardSel;
	}
	
	public void ResetClicked()
	{
		ResetCardSelection();
		ResetRandomCards();
		cardsSelectionHasChanged = true;
	}
	
	public void ResetCardSelection()
	{
		for(int i = 0; i < cardHelpers.Count; i++)
		{
			cardHelpers[i].quantityChange = 0;
			cardHelpers[i].UpdateButtonClickability();
		}
	}
	
	public void ResetRandomCards()
	{
		randomStandardCardsInputField.text = "0";
		includeRainbowToggle.isOn = false;
		randomNonstandardCardsInputField.text = "0";
		considerRarityToggle.isOn = false;
	}
	
	public void ConvertStringToCardSelection(string input1, string input2)
	{
		//print("input1= " + input1 + " input2= " + input2);
		string[] ranStrings = input2.Split(':');
		randomStandardCardsInputField.text = ranStrings[0];
		includeRainbowToggle.isOn = bool.Parse(ranStrings[1]);
		randomNonstandardCardsInputField.text = ranStrings[2];
		considerRarityToggle.isOn = bool.Parse(ranStrings[3]);
		if(input1 == "")
		{
			ResetCardSelection();
			return;
		}
		string[] sections = input1.Split('_');
		List<int> cardIndices = new List<int>();
		List<int> cardQuantities = new List<int>();
		for(int i = 0; i < sections.Length; i++)
		{
			string[] sectionSplit = sections[i].Split(':');
			cardIndices.Add(int.Parse(sectionSplit[0]));
			cardQuantities.Add(int.Parse(sectionSplit[1]));
		}
		
		int curSection = 0;
		
		for(int i = 0; i < cardHelpers.Count; i++)
		{
			if(curSection < cardIndices.Count && i == cardIndices[curSection])
			{
				cardHelpers[i].quantityChange = cardQuantities[curSection];
				cardHelpers[i].UpdateButtonClickability();
				curSection++;
			}
			else
			{
				cardHelpers[i].quantityChange = 0;
				cardHelpers[i].UpdateButtonClickability();
			}
		}
	}
	
	public void RandomStandardCardsInputFieldFinished()
	{
		if(randomStandardCardsInputField.text == "")
		{
			randomStandardCardsInputField.text = "0";
		}
		cardsSelectionHasChanged = true;
	}
	
	public void RandomNonstandardCardsInputFieldFinished()
	{
		if(randomNonstandardCardsInputField.text == "")
		{
			randomNonstandardCardsInputField.text = "0";
		}
		cardsSelectionHasChanged = true;
	}
	
	public void ToggleUpdated()
	{
		cardsSelectionHasChanged = true;
	}
	
	public void BackButtonClicked()
	{
		if(cardsSelectionHasChanged)
		{
			RunVariations.instance.AnyMutatorChanged();
		}
	}
}
