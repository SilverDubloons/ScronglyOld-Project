using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
	public static ShopScript instance;
	public RectTransform rt;
	public bool shopActive;
	public int numberOfCardsForSale;	// max 7 fits on table if in center vertically
	public int numberOfBaublesForSale;
	public int numberOfHandLevelsForSale;
	public List<BuyScript> buyScripts = new List<BuyScript>();
	public DeckScript deckScript;
	public float chanceForCardToBeRainbow; 		// 0 to 1;
	public int baseStandardCardCost = 1;
	public int rainbowExtraCost = 1;
	public float chanceForCardToBeNonStandard;
	public Transform itemParent;
	//public MonoBehaviour BuyScript;
	public GameObject buyPrefab;
	public ScoreVial scoreVial;
	public GameObject baublePrefab;
	public BaubleScript baubleScript;
	public BaubleCollection baubleCollection;
	public Transform tooltipParent;
	public NonStandardCardScript nonStandardCardScript;
	public Transform interestParent;
	public BaubleNotifications baubleNotifications;
	public HandValues handValues;
	public MovingButton shuffleAndContinueButton;
	//public Statistics statistics;
	public RandomNumbers randomNumbers;
	public int rerollStartingCost;
	public int rerollBaseCost;
	public int rerollCurrentCost;
	public int rerollScalingCost;
	public TMP_Text[] rerollCostTexts;
	public MovingButton rerollButton;
	public TMP_InputField nonstandardCardCheatInputField;
	public TMP_InputField baubleCheatInputField;
	public DeckViewer deckViewer;
	public Transform movingBaubleParent;
	//public GameObject layawayObject;
	public BuyScript layawayBuyScript = null;
	public Vector2 layawayOrigin;
	public int layawayAnte;
	public Image layawayImage;
	public GameObject layawayText;
	public GameObject laywayAreYouSureMenu;
	public BuyScript layawayTemp = null;
	private int shopGenerationStage = 0;
	public GameObject interestTooltipTrigger;
	public bool collectInterest;
	public bool zodiacsCannotBePurchased;
	
	public void ConfirmDeleteOldLayway()
	{
		NewObjectMovedToLayway(layawayTemp, false);
	}
	
	public void DenyDeleteOldLayaway()
	{
		layawayTemp.layawayButton.ChangeDisabled(false);
		layawayTemp = null;
	}
	
	public void NewObjectMovedToLayway(BuyScript buyScript, bool askIfSure)
	{
		if(layawayBuyScript != null)
		{
			if(layawayAnte == handValues.currentAnte)
			{
				if(layawayBuyScript.itemType == 0)
				{
					StartCoroutine(scoreVial.MoveOverTime(layawayBuyScript.cardScript.rt, layawayBuyScript.cardScript.rt.anchoredPosition, layawayOrigin, 1, 0, null, new Vector2(11f,11f), new Vector2(11f,11f), new Vector2(11f,11f), false));
				}
				if(layawayBuyScript.itemType == 1)
				{
					StartCoroutine(scoreVial.MoveOverTime(layawayBuyScript.baubleItem.rt, layawayBuyScript.baubleItem.rt.anchoredPosition, layawayOrigin, 1, 0, null, new Vector2(11f,11f), new Vector2(11f,11f), new Vector2(11f,11f), false));
				}
				layawayBuyScript.layawayButton.ChangeDisabled(false);
			}
			else
			{
				if(askIfSure)
				{
					layawayTemp = buyScript;
					laywayAreYouSureMenu.SetActive(true);
					return;
				}
				if(layawayBuyScript.itemType == 0)
				{
					Destroy(layawayBuyScript.cardScript.gameObject);
				}
				if(layawayBuyScript.itemType == 1)
				{
					baubleScript.baubles[layawayBuyScript.baubleNumber].numberOnSale--;
					Destroy(layawayBuyScript.baubleItem.gameObject);
				}
				buyScripts.Remove(layawayBuyScript);
			}
		}
		layawayTemp = null;
		layawayImage.raycastTarget = false;
		layawayText.SetActive(false);
		layawayBuyScript = buyScript;
		layawayAnte = handValues.currentAnte;
		if(buyScript.itemType == 0)
		{
			layawayOrigin = buyScript.cardScript.rt.anchoredPosition;
			StartCoroutine(scoreVial.MoveOverTime(buyScript.cardScript.rt, buyScript.cardScript.rt.anchoredPosition, new Vector2(-210,120), 1, 0, null, new Vector2(11f,11f), new Vector2(11f,11f), new Vector2(11f,11f), false));
			//layawayObject = buyScript.cardScript.gameObject;
		}
		if(buyScript.itemType == 1)
		{
			layawayOrigin = buyScript.baubleItem.rt.anchoredPosition;
			StartCoroutine(scoreVial.MoveOverTime(buyScript.baubleItem.rt, buyScript.baubleItem.rt.anchoredPosition, new Vector2(-210,120), 1, 0, null, new Vector2(11f,11f), new Vector2(11f,11f), new Vector2(11f,11f), false));
			//layawayObject = buyScript.baubleItem.gameObject;
		}
		
	}
	
	void Awake()
	{
		instance = this;
	}
	
	public void CheckAffordability()
	{
		if(shopActive)
		{
			for(int i = 0; i < buyScripts.Count; i++)
			{
				buyScripts[i].MoneyUpdated();
			}
		}
		UpdateInterest();
		UpdateRerollCost();
	}
	
	public void UpdateInterest()
	{
/* 		if(baubleScript.baubles[34].quantityOwned > 0)
		{ */
		if(DeckViewer.instance.deckInUse == 6 || !collectInterest)
		{
			return;
		}
		int interestToEarn = scoreVial.currentMoney / 5;
		if(interestToEarn == 0)
		{
			interestTooltipTrigger.SetActive(false);
		}
		else
		{
			interestTooltipTrigger.SetActive(true);
		}
		int maxInterest = 5 + baubleScript.baubles[34].quantityOwned * 5;
		interestToEarn = Mathf.Min(interestToEarn, maxInterest);
		int currentInterestShown = interestParent.childCount;
		if(currentInterestShown != interestToEarn)
		{
			List<RectTransform> chipRTs = new List<RectTransform>();
			for(int i = 0; i < interestParent.childCount; i++)
			{
				chipRTs.Add(interestParent.GetChild(i).GetComponent<RectTransform>());
			}
			if(currentInterestShown < interestToEarn)
			{
				for(int i = 0; i < interestToEarn - currentInterestShown; i++)
				{
					PokerChip newChipScript = scoreVial.SpawnPokerChip(new Vector2(0,0), interestParent);
					newChipScript.moving = false;
					chipRTs.Add(newChipScript.rt);
				}
			}
			else
			{
				for(int i = 0; i < currentInterestShown - interestToEarn; i++)
				{
					chipRTs.Remove(interestParent.GetChild(i).GetComponent<RectTransform>());
					Destroy(interestParent.GetChild(i).gameObject);
				}
			}
			float maxWidth = 80f;
			float pokerChipWidth = 16f;
			float squeezeDistance = (maxWidth - pokerChipWidth) / (chipRTs.Count - 1);
			float distanceBetweenChips = Mathf.Min(20f, squeezeDistance);
			
			for(int i = 0; i < chipRTs.Count; i++)
			{
				float xDestination = (interestToEarn - 1) * (distanceBetweenChips / 2f) - (interestToEarn - i - 1) * distanceBetweenChips;
				chipRTs[i].anchoredPosition = new Vector2(xDestination, 0);
			}
		}
		//}
	}
	
	public void RemoveStraightBaubles() // this doesn't work but I decided not to use it before fixing it
	{
		for(int i = 0; i < buyScripts.Count; i++)
		{
			if(buyScripts[i].itemType == 1)
			{
				if(buyScripts[i].baubleNumber == 14 || buyScripts[i].baubleNumber == 16)
				{
					if(layawayBuyScript == buyScripts[i])
					{
						layawayBuyScript = null;
						layawayImage.raycastTarget = true;
						layawayText.SetActive(true);
					}
					buyScripts.Remove(buyScripts[i]);
					Destroy(buyScripts[i].gameObject);
				}
			}
		}
	}
	
	public void CollectInterest()
	{
		if(interestParent.childCount > 0)
		{
			handValues.menuButton.ChangeDisabled(true);
			
			if(interestParent.childCount > 5 && baubleScript.baubles[34].quantityOwned >= 1)
			{
				baubleNotifications.Notify(34);
				if(interestParent.childCount >= 10)
				{
					if(!Decks.instance.decks[1].unlocked  && !Statistics.instance.currentRun.runIsDailyGame && !Statistics.instance.currentRun.runIsSeededGame && DeckViewer.instance.variantInUse == "")
					{
						Decks.instance.decks[1].unlocked = true;
						Decks.instance.UpdateDecksFile();
						Decks.instance.DeckKnobs[1].knobImage.sprite = Decks.instance.unlockedKnob;
						Decks.instance.DeckKnobs[1].rt.sizeDelta = new Vector2(10,10);
						UnlockNotifications.instance.CreateNewUnlockNotifier(0,1);
					}
				}
			}
		}
		for(int i = interestParent.childCount - 1; i >= 0; i--)
		{
			PokerChip pokerChip = interestParent.GetChild(i).GetComponent<PokerChip>();
			pokerChip.transform.SetParent(pokerChip.movingChipParent);
			pokerChip.startPosition = pokerChip.rt.anchoredPosition;
			pokerChip.moving = true;
		}
	}
	
	public void ChangeAllCardsForSaleToRainbow()
	{
		for(int i = 0; i < buyScripts.Count; i++)
		{
			if(buyScripts[i].itemType == 0)
			{
				if(buyScripts[i].cardScript.standardCard)
				{
					buyScripts[i].cardScript.ChangeToRainbow();
					buyScripts[i].SetupBuy(baseStandardCardCost);
				}
			}
		}
	}
	
	public void AddItemToEachCategory()
	{
		BaubleScript.instance.SortBaublesByRarity();
		for(int i = 0; i < buyScripts.Count; i++)
		{
			if(buyScripts[i].itemType == 0 && buyScripts[i] != layawayBuyScript)
			{
				buyScripts[i].cardScript.rt.anchoredPosition = buyScripts[i].cardScript.rt.anchoredPosition + new Vector2(-25, 0);
			}
			else if(buyScripts[i].itemType == 1 && buyScripts[i] != layawayBuyScript)
			{
				buyScripts[i].baubleItem.rt.anchoredPosition = buyScripts[i].baubleItem.rt.anchoredPosition + new Vector2(-25, 0);
			}
		}
		if(layawayBuyScript != null)
		{
			layawayOrigin = layawayOrigin + new Vector2(-25,0);
		}
		rerollButton.ChangeDisabled(true);
		shopGenerationStage = 0;
		StartCoroutine(CreateBaublesForSale(numberOfBaublesForSale - 1));
		StartCoroutine(CreateCardsForSale(numberOfCardsForSale - 1));
		if(!zodiacsCannotBePurchased)
		{
			StartCoroutine(CreateHandLevelsForSale(numberOfHandLevelsForSale - 1));
		}
	}
		
	public void NextAnteClicked()
	{
		//print("next ante");
		if(BaubleScript.instance.baubles[59].quantityOwned >= 1)
		{
			baubleNotifications.Notify(59);
			for(int i = 0; i < BaubleScript.instance.baubles[59].quantityOwned; i++)
			{
				CheatAddNonStandardCard(9, false);
			}
		}
		shuffleAndContinueButton.ChangeDisabled(true);
		rerollButton.ChangeDisabled(true);
		CollectInterest();
		shopActive = false;
		handValues.ResetHandZones();
		//scoreVial.handScript.StartCoroutine(scoreVial.handScript.AddAllCardsInDiscardPileToDeck());
		scoreVial.StartCoroutine(scoreVial.StartNextAnte());
		if(!GameOptions.instance.tutorialDone)
		{
			if(Tutorial.instance.tutorialStage == 7)
			{
				Tutorial.instance.AdvanceTutorial();
			}
			if(Tutorial.instance.tutorialStage == 8)
			{
				Tutorial.instance.AdvanceTutorial();
			}
		}
	}
	
	List<GameObject> GetAllItemsForSale()
	{
		List<GameObject> itemsForSale = new List<GameObject>();
		for(int item = 0; item < itemParent.childCount; item++)
		{
			itemsForSale.Add(itemParent.GetChild(item).gameObject);
		}
		return itemsForSale;
	}
	
	void DeleteOldItems()
	{
		List<GameObject> oldItems = GetAllItemsForSale();
		GameObject doNotDestroy = null;
		if(layawayBuyScript != null)
		{
			if(layawayBuyScript.itemType == 0)
			{
				doNotDestroy = layawayBuyScript.cardScript.gameObject;
			}
			if(layawayBuyScript.itemType == 1)
			{
				doNotDestroy = layawayBuyScript.baubleItem.gameObject;
			}
		}
		oldItems.Remove(doNotDestroy);
		for(int item = 0; item < oldItems.Count; item++)
		{
			oldItems[item].SetActive(false);
			Destroy(oldItems[item], item*0.05f);
		}
	}
	
	public void CheatAddBaubleClicked()
	{
		int baubleNumber = int.Parse(baubleCheatInputField.text);
		if(baubleNumber < baubleScript.baubles.Length)
		{
			CheatAddBauble(baubleNumber, Vector2.zero, true);
		}
	}
	
	public void CheatAddBauble(int baubleNumber, Vector2 spawnLocation, bool playSound = true)
	{
		GameObject newBauble = Instantiate(baublePrefab, new Vector3(0,0,0), Quaternion.identity, movingBaubleParent);
		BaubleItem newBaubleItem = newBauble.GetComponent<BaubleItem>();
		newBaubleItem.tooltipParent = tooltipParent;
		newBaubleItem.rt.anchoredPosition = spawnLocation;
		baubleScript.SetupSpecificBaubleItem(newBaubleItem, baubleNumber, true);
		buyScripts.Add(newBaubleItem.buyScript);
		newBaubleItem.buyScript.BuyClicked(playSound);
	}
	
	public void CheatAddNonStandardCardButtonClicked()
	{
		int cardNumber = int.Parse(nonstandardCardCheatInputField.text);
		if(cardNumber < nonStandardCardScript.nonStandardCards.Length)
		{
			//print("cardNumber= " +cardNumber + " nonStandardCardScript.nonStandardCards.Length= "+nonStandardCardScript.nonStandardCards.Length);
			CheatAddNonStandardCard(cardNumber);
		}
	}
		
	public void CheatAddNonStandardCard(int cardNumber, bool playSound = true)
	{
		CardScript cardScript = nonStandardCardScript.CreateSpecificNonStandardCard(cardNumber, itemParent, new Vector2(200,200));
		cardScript.canMove = false;
		cardScript.cardIsInShop = true;
		cardScript.cardLocation = 1;
		GameObject clonedCard = Instantiate(cardScript.gameObject, new Vector3(0,0,0), Quaternion.identity, deckViewer.cloneParent);
		CardScript clonedCardScript = clonedCard.GetComponent<CardScript>();
		cardScript.deckViewerClone = clonedCardScript;
		//deckViewer.clonedCards.Add(clonedCardScript);
		clonedCardScript.cardLocation = 1;
		clonedCardScript.isDeckViewerClone = true;
		clonedCardScript.faceDown = false;
		clonedCardScript.UpdateFacing();
		clonedCardScript.gameObject.SetActive(false);
		GameObject newBuy = Instantiate(buyPrefab, new Vector3(0,0,0), Quaternion.identity, cardScript.transform);
		newBuy.transform.SetSiblingIndex(0);
		RectTransform newBuyRT = newBuy.GetComponent<RectTransform>();
		newBuyRT.anchoredPosition = Vector2.zero;
		BuyScript buyScript = newBuy.GetComponent<BuyScript>();
		buyScript.shopScript = this;
		buyScript.cardScript = cardScript;
		buyScript.itemType = 0;
		buyScript.SetupBuy(0);
		buyScript.BuyClicked(playSound);
	}
	
	public IEnumerator CreateCardsForSale(int iStart = 0)
	{
		while(shopGenerationStage < 1)
		{
			yield return null;
		}
		float startingPosX = -25f * (numberOfCardsForSale - 1);
		for(int i = iStart; i < numberOfCardsForSale; i++)
		{
			int cardCost = baseStandardCardCost;
			Vector2 position = new Vector2(startingPosX + i * 50f, 0f);
			CardScript cardScript;
			//float ran = Random.Range(0f, 1f);
			float ran = randomNumbers.Range(0f, 1f);
			if(ran < chanceForCardToBeNonStandard)
			{
				cardScript = nonStandardCardScript.CreateRandomNonStandardCardForShop(itemParent, position);
				cardCost = nonStandardCardScript.nonStandardCards[cardScript.nonStandardCardNumber].cardCost;
			}
			else
			{
				//ran = Random.Range(0f, 1f);
				ran = randomNumbers.Range(0f, 1f);
				if(ran < chanceForCardToBeRainbow)
				{
					cardCost += rainbowExtraCost;
					//cardScript = deckScript.CreateCard(4, Random.Range(0, 13), itemParent, position, false);
					cardScript = deckScript.CreateCard(4, randomNumbers.Range(0, 13), itemParent, position, false);
				}
				else
				{
					//cardScript = deckScript.CreateCard(Random.Range(0, 4), Random.Range(0, 13), itemParent, position, false);
					cardScript = deckScript.CreateCard(randomNumbers.Range(0, 4), randomNumbers.Range(0, 13), itemParent, position, false);
				}
			}
			
			GameObject clonedCard = Instantiate(cardScript.gameObject, new Vector3(0,0,0), Quaternion.identity, cardScript.transform);
			CardScript clonedCardScript = clonedCard.GetComponent<CardScript>();
			cardScript.deckViewerClone = clonedCardScript;
			//deckViewer.clonedCards.Add(clonedCardScript);
			clonedCardScript.cardLocation = 1;
			clonedCardScript.isDeckViewerClone = true;
			clonedCardScript.faceDown = false;
			clonedCardScript.UpdateFacing();
			clonedCardScript.gameObject.SetActive(false);
			
			cardScript.canMove = false;
			cardScript.cardIsInShop = true;
			GameObject newBuy = Instantiate(buyPrefab, new Vector3(0,0,0), Quaternion.identity, cardScript.transform);
			newBuy.transform.SetSiblingIndex(0);
			RectTransform newBuyRT = newBuy.GetComponent<RectTransform>();
			newBuyRT.anchoredPosition = Vector2.zero;
			BuyScript buyScript = newBuy.GetComponent<BuyScript>();
			buyScript.shopScript = this;
			buyScript.cardScript = cardScript;
			buyScript.itemType = 0;
			buyScript.SetupBuy(cardCost);
			buyScripts.Add(buyScript);
			yield return null;
		}
		shopGenerationStage = 2;
	}
	
	public IEnumerator CreateBaublesForSale(int iStart = 0)
	{
		//yield return new WaitForSeconds(3f);
		float startingPosX = -25f * (numberOfBaublesForSale - 1);
		for(int i = iStart; i < numberOfBaublesForSale; i++)
		{
			GameObject newBauble = Instantiate(baublePrefab, new Vector3(0,0,0), Quaternion.identity, itemParent);
			BaubleItem newBaubleItem = newBauble.GetComponent<BaubleItem>();
			newBaubleItem.tooltipParent = tooltipParent;
			newBaubleItem.rt.anchoredPosition = new Vector2(startingPosX + i * 50f, 85f);
			baubleScript.SetupRandomBaubleItem(newBaubleItem, false);
			buyScripts.Add(newBaubleItem.buyScript);
			yield return null;
		}
		shopGenerationStage = 1;
		if(zodiacsCannotBePurchased)
		{
			UpdateRerollCost();
		}
	}
	
	public IEnumerator CreateHandLevelsForSale(int iStart = 0)
	{
		while(shopGenerationStage < 2)
		{
			yield return null;
		}
		float startingPosX = -25f * (numberOfHandLevelsForSale - 1);
		for(int i = iStart; i < numberOfHandLevelsForSale; i++)
		{
			GameObject newBauble = Instantiate(baublePrefab, new Vector3(0,0,0), Quaternion.identity, itemParent);
			BaubleItem newBaubleItem = newBauble.GetComponent<BaubleItem>();
			newBaubleItem.tooltipParent = tooltipParent;
			newBaubleItem.rt.anchoredPosition = new Vector2(startingPosX + i * 50f, -85f);
			baubleScript.SetupRandomBaubleItem(newBaubleItem, true);
			buyScripts.Add(newBaubleItem.buyScript);
			yield return null;
		}
		UpdateRerollCost();
	}
	
	public void SetupShop(bool reroll = false)
	{
		int layawayBaubleNumber = -1;
		if(layawayBuyScript != null)
		{
			if(layawayBuyScript.itemType == 1)
			{
				layawayBaubleNumber = layawayBuyScript.baubleNumber;
			}
			layawayBuyScript.MoneyUpdated();
		}
		for(int i = 0; i < buyScripts.Count; i++)
		{
			if(buyScripts[i].itemType == 1)
			{
				if(layawayBaubleNumber != buyScripts[i].baubleNumber)
				{
					baubleScript.baubles[buyScripts[i].baubleNumber].numberOnSale = 0;
				}
			}
		}
		if(!reroll)
		{
			rerollCurrentCost = rerollBaseCost;
		}
		baubleScript.SortBaublesByRarity();
		UpdateRerollCost();
		buyScripts.Clear();
		if(layawayBuyScript != null)
		{
			buyScripts.Add(layawayBuyScript);
		}
		DeleteOldItems();
		rerollButton.ChangeDisabled(true);
		shopGenerationStage = 0;
		NonStandardCardScript.instance.SortCardsByRarity();
		StartCoroutine(CreateBaublesForSale());
		StartCoroutine(CreateCardsForSale());
		if(!zodiacsCannotBePurchased)
		{
			StartCoroutine(CreateHandLevelsForSale());
		}
	}
	
	public void RerollClicked()
	{
		layawayAnte--;
		scoreVial.MoneyChanged(-rerollCurrentCost);
		SoundManager.instance.PlayRerollSound();
		rerollCurrentCost += rerollScalingCost;
		//UpdateRerollCost();
		SetupShop(true);
	}
	
	public void UpdateRerollCost()
	{
		for(int i = 0; i < rerollCostTexts.Length; i++)
		{
			rerollCostTexts[i].text = "" + rerollCurrentCost;
		}
		if(scoreVial.currentMoney < rerollCurrentCost)
		{
			rerollButton.ChangeDisabled(true);
		}
		else
		{
			rerollButton.ChangeDisabled(false);
		}
	}
}
