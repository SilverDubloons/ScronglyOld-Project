using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandZone : MonoBehaviour, IPointerClickHandler
{
	public DropZone[] dropZones;
	public TMP_Text[] baseHandValueTexts;
	public TMP_Text[] cardMultiplierTexts;
	public HandValues handValues;
	public TMP_Text[] handNameTexts;
	
	public RectTransform[] handNameTextRTs;
	public RectTransform backdropRT;
	public RectTransform shadowRT;
	public RectTransform rt;
	
	public MovingButton lockButton;
	public bool locked;
	public bool finishedAnimating = true;
	public Image lockButtonImage;
	public RectTransform lockButtonRT;
	public bool handZoneActive;
	
	public TMP_Text dataText;
	
	public GameObject ScorePlatePrefab;
	public Transform ScorePlateParent;
	
	public float currentCardMultiplier;
	public float currentBaseValue;
	public int currentHandTier;
	bool[] currentHandContains = new bool[18];
	List<CardScript> cardsUsedCurrently = new List<CardScript>();
	
	public Image[] cardXImages;
	public Color cardXImageGreen;
	public Color cardXImageRed;
	public float epsilon = 0.0001f;
	
	public int activeDropZones = 5;
	public RectTransform crosshairRT;
	
	public void ExpandHandZone()
	{
		dropZones[activeDropZones].gameObject.SetActive(true);
		for(int i = 0; i < handNameTextRTs.Length; i++)
		{
			handNameTextRTs[i].sizeDelta = new Vector2(handNameTextRTs[i].sizeDelta.x + 46.5f, handNameTextRTs[i].sizeDelta.y);
		}
		backdropRT.sizeDelta = new Vector2(backdropRT.sizeDelta.x + 48, backdropRT.sizeDelta.y);
		shadowRT.sizeDelta = new Vector2(shadowRT.sizeDelta.x + 48, shadowRT.sizeDelta.y);
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x -32, rt.anchoredPosition.y);
		activeDropZones++;
	}
	
	public void SetHandZoneSize(int size)
	{
		for(int i = 0; i < dropZones.Length; i++)
		{
			if(i < size)
			{
				dropZones[i].gameObject.SetActive(true);
			}
			else
			{
				dropZones[i].gameObject.SetActive(false);
			}
		}
		for(int i = 0; i < handNameTextRTs.Length; i++)
		{
			handNameTextRTs[i].sizeDelta = new Vector2(243f, handNameTextRTs[i].sizeDelta.y);
		}
		backdropRT.sizeDelta = new Vector2(318f, backdropRT.sizeDelta.y);
		shadowRT.sizeDelta = new Vector2(318f, shadowRT.sizeDelta.y);
		rt.anchoredPosition = new Vector2(198.5f, rt.anchoredPosition.y);
		activeDropZones = size;
	}
	
	public void LockButtonClicked()
	{
		locked = true;
		finishedAnimating = false;
		lockButtonImage.sprite = handValues.lockedSprite;
		lockButton.ChangeDisabled(true);
		lockButtonRT.sizeDelta = new Vector2(handValues.lockedSprite.rect.width, handValues.lockedSprite.rect.height);
		for(int dropZone = 0; dropZone < dropZones.Length; dropZone++)
		{
			if(dropZones[dropZone].cardPlaced)
			{
				dropZones[dropZone].placedCardScript.canMove = false;
			}
		}
		handValues.handScript.PlacedCardsUpdated(false);
		StartCoroutine(ScoreHand());
		handValues.menuButton.ChangeDisabled(true);
		HandScript.instance.playSelectedButton.ChangeDisabled(true);
	}
	
	public void ResetHandZone()
	{
		crosshairRT.gameObject.SetActive(false);
		for(int dropZone = 0; dropZone < dropZones.Length; dropZone++)
		{
			if(dropZones[dropZone].cardPlaced)
			{
				dropZones[dropZone].placedCardScript.dropZonePlacedIn = null;
				dropZones[dropZone].CardRemoved(false);
			}
		}
		for(int i = 0; i < baseHandValueTexts.Length; i++)
		{
			baseHandValueTexts[i].text = "Base Value";
		}
		for(int i = 0; i < cardMultiplierTexts.Length; i++)
		{
			cardMultiplierTexts[i].text = "Hand Multi";
		}
		for(int i = 0; i < handNameTexts.Length; i++)
		{
			handNameTexts[i].text = "";
		}
		lockButtonImage.sprite = handValues.unlockedSprite;
		//lockButton.ChangeDisabled(false);
		lockButtonRT.sizeDelta = new Vector2(handValues.unlockedSprite.rect.width, handValues.unlockedSprite.rect.height);
		for(int i = 0; i < cardXImages.Length; i++)
		{
			cardXImages[i].gameObject.SetActive(false);
		}
		locked = false;
		HandScript.instance.SelectedCardsUpdated();
		currentHandContains = new bool[18];
		HandsInformation.instance.RecolorHandLabels(currentHandContains);
		if(handValues.handsHavePreplacedCard)
		{
			CreatePreplacedCard(); 
		}
	}
	
	public void CreatePreplacedCard()
	{
		float ran = RandomNumbers.instance.Range(0f, 1f);
		if(ran > handValues.chanceForPreplacedCardToBeNonstandard)
		{
			int cardInt = RandomNumbers.instance.Range(0, 65);
			int suitInt = cardInt / 13;
			int rankInt = cardInt % 13;
			CardScript newCard = DeckScript.instance.CreateCard(suitInt, rankInt, dropZones[0].transform, new Vector2(22.5f, 22.5f), false);
			newCard.cannotBeMovedFromHandZone = true;
			dropZones[0].CardPlaced(newCard);
			newCard.dropZonePlacedIn = dropZones[0];
			newCard.cardLocation = 2;
			GameObject clonedCard = Instantiate(newCard.gameObject, new Vector3(0,0,0), Quaternion.identity, newCard.transform);
			CardScript clonedCardScript = clonedCard.GetComponent<CardScript>();
			newCard.deckViewerClone = clonedCardScript;
			//deckViewer.clonedCards.Add(clonedCardScript);
			clonedCardScript.cardLocation = 2;
			clonedCardScript.isDeckViewerClone = true;
			clonedCardScript.faceDown = false;
			clonedCardScript.UpdateFacing();
			clonedCardScript.gameObject.SetActive(false);
			Statistics.instance.currentRun.cardsAddedToDeck++;
			newCard.deckViewerClone.transform.SetParent(DeckViewer.instance.cloneParent);
			DeckViewer.instance.clonedCards.Add(newCard.deckViewerClone);
		}
		else
		{
			//NonStandardCardScript.instance.GetRandomNonStandardCard 
			int nsci = NonStandardCardScript.instance.GetRandomNonStandardCardInt(NonStandardCardScript.instance.commonCardWeight, NonStandardCardScript.instance.uncommonCardWeight, NonStandardCardScript.instance.rareCardWeight, NonStandardCardScript.instance.legendaryCardWeight);
			CardScript newCard = DeckScript.instance.CreateNonStandardCard(NonStandardCardScript.instance.nonStandardCards[nsci].cardImage, nsci, NonStandardCardScript.instance.nonStandardCards[nsci].cardDescription, NonStandardCardScript.instance.nonStandardCards[nsci].cardName, NonStandardCardScript.instance.nonStandardCards[nsci].cardCategory, dropZones[0].transform, new Vector2(22.5f, 22.5f), false, true, false);
			newCard.cannotBeMovedFromHandZone = true;
			dropZones[0].CardPlaced(newCard);
			newCard.dropZonePlacedIn = dropZones[0];
			newCard.cardLocation = 2;
			GameObject clonedCard = Instantiate(newCard.gameObject, new Vector3(0,0,0), Quaternion.identity, newCard.transform);
			CardScript clonedCardScript = clonedCard.GetComponent<CardScript>();
			newCard.deckViewerClone = clonedCardScript;
			//deckViewer.clonedCards.Add(clonedCardScript);
			clonedCardScript.cardLocation = 2;
			clonedCardScript.isDeckViewerClone = true;
			clonedCardScript.faceDown = false;
			clonedCardScript.UpdateFacing();
			clonedCardScript.gameObject.SetActive(false);
			Statistics.instance.currentRun.cardsAddedToDeck++;
			newCard.deckViewerClone.transform.SetParent(DeckViewer.instance.cloneParent);
			DeckViewer.instance.clonedCards.Add(newCard.deckViewerClone);
		}
	}
	
	public void DiscardAllCards()
	{
		List<CardScript> cardsToDiscard = GetAllCardsInHandZone();
		for(int card = 0; card < cardsToDiscard.Count; card++)
		{
			if(cardsToDiscard[card].cannotBeMovedFromHandZone)
			{
				cardsToDiscard[card].cannotBeMovedFromHandZone = false;
			}
			else
			{
				handValues.handScript.cardsInHand--;
			}
			cardsToDiscard[card].canMove = false;
			cardsToDiscard[card].transform.SetParent(handValues.handScript.discardParent);
			//print("moving " + cardsToDiscard[card].GetCardNameCharSuit());
			cardsToDiscard[card].StartMove(0.25f, new Vector2(22.5f, 22.5f), new Vector3(0,0,0));
			cardsToDiscard[card].cardLocation = 3;
			cardsToDiscard[card].deckViewerClone.cardLocation = 3;
			if(!cardsToDiscard[card].faceDown)
			{
				StartCoroutine(cardsToDiscard[card].Flip(0.25f));
			}
			int cardsInDiscardPile = int.Parse(handValues.handScript.discardPileTexts[0].text);
			cardsInDiscardPile++;
			for(int text = 0; text < handValues.handScript.discardPileTexts.Length; text++)
			{
				handValues.handScript.discardPileTexts[text].text = "" + cardsInDiscardPile;
			}
			
		}
	}
	
	public void ResizeBackdropForScorePlate(RectTransform border, RectTransform backdrop, TMP_Text text)
	{
		border.sizeDelta = new Vector2(text.textBounds.size.x + 10, border.sizeDelta.y);
		backdrop.sizeDelta = new Vector2(text.textBounds.size.x + 8, backdrop.sizeDelta.y);
	}
	
	public void CallAnimateBaseValue(float valueToAdd, float valueToMultiply, bool animate)
	{
		currentBaseValue += valueToAdd;
		currentBaseValue = currentBaseValue * valueToMultiply;
		
		List<RectTransform> baseTexts = new List<RectTransform>();
		for(int i = 0; i < baseHandValueTexts.Length; i++)
		{
			baseTexts.Add(baseHandValueTexts[i].GetComponent<RectTransform>());
			baseHandValueTexts[i].text = handValues.ConvertFloatToString(currentBaseValue);
		}
		if(animate)
		{
			handValues.StartCoroutine(handValues.ExpandContract(baseTexts, 0.1f, new Vector3(1.2f, 1.2f, 1f)));
		}
	}
	
	public void CallAnimateCardMultiplier(float valueToAdd, float valueToMultiply)
	{
		currentCardMultiplier += valueToAdd;
		currentCardMultiplier = currentCardMultiplier * valueToMultiply;
		
		List<RectTransform> multTexts = new List<RectTransform>();
		for(int i = 0; i < cardMultiplierTexts.Length; i++)
		{
			multTexts.Add(cardMultiplierTexts[i].GetComponent<RectTransform>());
			cardMultiplierTexts[i].text = handValues.ConvertFloatToString(currentCardMultiplier);
		}
		handValues.StartCoroutine(handValues.ExpandContract(multTexts, 0.1f, new Vector3(1.2f, 1.2f, 1f)));
	}
	
	private bool shouldNotShootSnurfGun = false;
	private bool alreadyCheckedForQueens = false;
	
	public IEnumerator ScoreHand()
	{
		if(currentHandTier > 8)
		{
			handValues.baubleScript.HighTierHandPlayed(currentHandTier);
		}
		HandsInformation.instance.RecolorHandLabels(currentHandContains);
		dataText.text = ""; 
		List<CardScript> cards = new List<CardScript>();
		if(handValues.baubleScript.baubles[33].quantityOwned == 0)
		{
			cards = cardsUsedCurrently;
		}
		else
		{
			cards = GetCountedCards();
		}
		List<CardScript> allPlacedCards = GetAllCardsInHandZone();
		bool bombPlaced = false;
		bool paintRainbow = false;
		int ranksToIncrease = 0;
		bool performPromotion = false;
		int bombLocation = 0;
		int mushroomProduct = 1;
		float addToAllBaseValue = 0;
		float addToAllMults = 0;
		bool changeAllRanks = false;
		bool magicMirrorPlaced = false;
		int zodiacsGained = 0;
		if(allPlacedCards.Count == 1 && handValues.baubleScript.baubles[57].quantityOwned >= 1 && !shouldNotShootSnurfGun)
		{
			crosshairRT.gameObject.SetActive(false);
			GameObject newProjectile = Instantiate(handValues.snurfProjectilePrefab, new Vector3(0,0,0), Quaternion.identity, handValues.snurfProjectileParent);
			SnurfProjectile snurfProjectile = newProjectile.GetComponent<SnurfProjectile>();
			snurfProjectile.rt.anchoredPosition = new Vector2(-355f, 75f);
			snurfProjectile.cardToDestroy = allPlacedCards[0];
			allPlacedCards[0].dropZonePlacedIn.CardRemoved(false);
			SoundManager.instance.PlayBottlePopSound();
			allPlacedCards[0].transform.SetParent(handValues.snurfProjectileParent);
			while(!snurfProjectile.alreadyAttached)
			{
				yield return null;
			}
			CallAnimateCardMultiplier(0, 0);
			CallAnimateBaseValue(0, 0, false);
			yield return new WaitForSeconds(1f / handValues.gameOptions.gameSpeedFactor);
			finishedAnimating = true;
			if(handValues.handsLeftUntilFatigue <= 0)
			{
				Statistics.instance.IncrementHandsPlayedInFatigue();
				handValues.handScript.DiscardCardsInHand();
			}
			handValues.UpdateHandsUntilFatigue(-1, false);
			DeckViewer.instance.CheckForRepublicDeckUnlock();
			handValues.handScript.cardsInHand--;
			handValues.ScoreEntireHand();
			yield break;
		}
		for(int i = 0; i < allPlacedCards.Count; i++)
		{
			if(!allPlacedCards[i].countCardInHand && allPlacedCards[i].countCardInScoring)
			{
				cards.Add(allPlacedCards[i]);
			}
			if(!allPlacedCards[i].standardCard)
			{
				shouldNotShootSnurfGun = true;
				switch(allPlacedCards[i].nonStandardCardNumber)
				{
					case 0:
					bombLocation = allPlacedCards[i].dropZonePlacedIn.dropZoneNumber;
					bombPlaced = true;
					break;
					case 1:
					paintRainbow = true;
					break;
					case 2:
					mushroomProduct *= 2;
					break;
					case 3:
					addToAllBaseValue += 25;
					break;
					case 4:
					addToAllMults += 3;
					break;
					case 5:
					ranksToIncrease++;
					performPromotion = true;
					break;
					case 6:
					ranksToIncrease--;
					performPromotion = true;
					break;
					case 7:
					changeAllRanks = true;
					break;
					case 8:
					magicMirrorPlaced = true;
					break;
					case 9:
					zodiacsGained++;
					break;
				}
			}
		}
		if(paintRainbow)
		{
			int cardsPainted = 0;
			for(int i = 0; i < allPlacedCards.Count; i++)
			{
				if(allPlacedCards[i].standardCard && allPlacedCards[i].suitInt < 4)
				{
					GameObject newPaint = Instantiate(handValues.rainbowPaintPrefab, new Vector3(0,0,0), Quaternion.identity, allPlacedCards[i].dropZonePlacedIn.transform);
					RectTransform newPaintRT = newPaint.GetComponent<RectTransform>();
					newPaintRT.anchoredPosition = new Vector2(-2,-2);
					RainbowPaintScript rainbowPaintScript = newPaint.GetComponent<RainbowPaintScript>();
					rainbowPaintScript.cardToChange = allPlacedCards[i];
					rainbowPaintScript.paintTime = 1f / handValues.gameOptions.gameSpeedFactor;
					cardsPainted++;
					yield return new WaitForSeconds(0.2f / handValues.gameOptions.gameSpeedFactor);
				}
			}
			if(cardsPainted > 0)
			{
				yield return new WaitForSeconds(1.2f / handValues.gameOptions.gameSpeedFactor);
				for(int i = 0; i < allPlacedCards.Count; i++)
				{
					if(!allPlacedCards[i].standardCard && allPlacedCards[i].nonStandardCardNumber == 1)
					{
						dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber].CardRemoved(false);
						handValues.CreateDissolvingCard(allPlacedCards[i]);
						Destroy(allPlacedCards[i].tooltipScript.gameObject);
						Destroy(allPlacedCards[i].gameObject);
					}
				}
				HandUpdated();
				StartCoroutine(ScoreHand());
				DeckViewer.instance.CheckForRainbowDeckUnlock();
				yield break;
			}
		}
		if(changeAllRanks)
		{
			int ranksChanged = 0;
			int rankToBecome = RandomNumbers.instance.Range(0, 13);
			for(int i = 0; i < allPlacedCards.Count; i++)
			{
				if(allPlacedCards[i].standardCard)
				{
					GameObject newMarker = Instantiate(handValues.magicMarkerAnimationPrefab, new Vector3(0,0,0), Quaternion.identity, allPlacedCards[i].dropZonePlacedIn.transform);
					MagicMarker magicMarker = newMarker.GetComponent<MagicMarker>();
					//magicMarker.rt.anchoredPosition = new Vector2(20,10);
					magicMarker.cardToChangeRank = allPlacedCards[i];
					magicMarker.rankChange = rankToBecome;
					magicMarker.StartCoroutine(magicMarker.MoveUp());
					ranksChanged++;
					yield return new WaitForSeconds(0.2f / handValues.gameOptions.gameSpeedFactor);
				}
			}
			if(ranksChanged > 0)
			{
				yield return new WaitForSeconds(1.2f / handValues.gameOptions.gameSpeedFactor);
				for(int i = 0; i < allPlacedCards.Count; i++)
				{
					if(!allPlacedCards[i].standardCard && allPlacedCards[i].nonStandardCardNumber == 7)
					{
						dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber].CardRemoved(false);
						handValues.CreateDissolvingCard(allPlacedCards[i]);
						Destroy(allPlacedCards[i].tooltipScript.gameObject);
						Destroy(allPlacedCards[i].gameObject);
					}
				}
				HandUpdated();
				StartCoroutine(ScoreHand());
				yield break;
			}
		}
		if(performPromotion)
		{
			int ranksIncreased = 0;
			for(int i = 0; i < allPlacedCards.Count; i++)
			{
				if(allPlacedCards[i].standardCard)
				{
					ranksIncreased++;
					GameObject newPromotion = Instantiate(handValues.promotionAnimationPrefab, new Vector3(0,0,0), Quaternion.identity, allPlacedCards[i].dropZonePlacedIn.transform);
					Promotion promotion = newPromotion.GetComponent<Promotion>();
					promotion.rt.anchoredPosition = new Vector2(0,0);
					promotion.cardToPromote = allPlacedCards[i];
					promotion.ranksToIncrease = ranksToIncrease;
					promotion.StartCoroutine(promotion.MoveUp());
					yield return new WaitForSeconds(0.2f / handValues.gameOptions.gameSpeedFactor);
				}
			}
			if(ranksIncreased > 0)
			{
				yield return new WaitForSeconds(1.2f / handValues.gameOptions.gameSpeedFactor);
				for(int i = 0; i < allPlacedCards.Count; i++)
				{
					if(!allPlacedCards[i].standardCard && (allPlacedCards[i].nonStandardCardNumber == 5 || allPlacedCards[i].nonStandardCardNumber == 6) )
					{
						dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber].CardRemoved(false);
						handValues.CreateDissolvingCard(allPlacedCards[i]);
						Destroy(allPlacedCards[i].tooltipScript.gameObject);
						Destroy(allPlacedCards[i].gameObject);
					}
				}
				HandUpdated();
				StartCoroutine(ScoreHand());
				yield break;
			}
		}
		if(magicMirrorPlaced)
		{
			int cardsCopied = 0;
			for(int i = 0; i < allPlacedCards.Count; i++)
			{
				if(!allPlacedCards[i].standardCard && allPlacedCards[i].nonStandardCardNumber == 8)
				{
					int cardsInCorrectLocations = 0;
					if(allPlacedCards[i].dropZonePlacedIn.dropZoneNumber > 0)
					{
						if(dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber - 1].cardPlaced)
						{
							if(dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber - 1].placedCardScript.standardCard)
							{
								cardsInCorrectLocations++;
							}
						}
					}
					if(allPlacedCards[i].dropZonePlacedIn.dropZoneNumber < activeDropZones - 1)
					{
						if(dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber + 1].cardPlaced)
						{
							if(dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber + 1].placedCardScript.standardCard)
							{
								cardsInCorrectLocations++;
							}
						}
					}
					if(cardsInCorrectLocations >= 2)
					{
						GameObject newSpell = Instantiate(handValues.magicMirrorAnimationPrefab, new Vector3(0,0,0), Quaternion.identity, dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber - 1].transform);
						MagicMirror magicMirror = newSpell.GetComponent<MagicMirror>();
						magicMirror.rt.anchoredPosition = new Vector2(37f, 16.5f);
						magicMirror.cardToChange = dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber - 1].placedCardScript;
						magicMirror.newRank = dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber + 1].placedCardScript.rankInt;
						magicMirror.newSuit = dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber + 1].placedCardScript.suitInt;
						magicMirror.newCardValueChanged = dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber + 1].placedCardScript.cardValueHasBeenChanged;
						magicMirror.newValue = dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber + 1].placedCardScript.cardValue;
						magicMirror.newCardMultChanged = dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber + 1].placedCardScript.cardMultiplierHasChanged;
						magicMirror.newMult = dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber + 1].placedCardScript.cardMultiplier;
						magicMirror.StartCoroutine(magicMirror.Animate());
						cardsCopied++;
					}
				}
			}
			if(cardsCopied > 0)
			{
				yield return new WaitForSeconds(1.2f / handValues.gameOptions.gameSpeedFactor);
				for(int i = 0; i < allPlacedCards.Count; i++)
				{
					if(!allPlacedCards[i].standardCard && allPlacedCards[i].nonStandardCardNumber == 8)
					{
						dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber].CardRemoved(false);
						handValues.CreateDissolvingCard(allPlacedCards[i]);
						Destroy(allPlacedCards[i].tooltipScript.gameObject);
						Destroy(allPlacedCards[i].gameObject);
					}
				}
				HandUpdated();
				StartCoroutine(ScoreHand());
				yield break;
			}
		}
		if(zodiacsGained > 0)
		{
			yield return new WaitForSeconds(0.75f / handValues.gameOptions.gameSpeedFactor);
			SoundManager.instance.PlaySpyglassSound();
			for(int i = 0; i < zodiacsGained; i++)
			{
				//handValues.baubleScript.BaublePurchased(handValues.baubleScript.GetBaubleIndexByHandTier(currentHandTier));
				ShopScript.instance.CheatAddBauble(handValues.baubleScript.GetBaubleIndexByHandTier(currentHandTier), Vector2.zero, false);
			}
			Statistics.instance.currentRun.zodiacsEarned += zodiacsGained;
			CallAnimateCardMultiplier(0, 1);
			CallAnimateBaseValue(0, 1, true);
			yield return new WaitForSeconds(0.5f / handValues.gameOptions.gameSpeedFactor); 
			for(int i = 0; i < allPlacedCards.Count; i++)
			{
				if(!allPlacedCards[i].standardCard && allPlacedCards[i].nonStandardCardNumber == 9)
				{
					dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber].CardRemoved(false);
					handValues.CreateDissolvingCard(allPlacedCards[i]);
					Destroy(allPlacedCards[i].tooltipScript.gameObject);
					Destroy(allPlacedCards[i].gameObject);
				}
			}
			HandUpdated();
			StartCoroutine(ScoreHand());
			yield break;
		}
		if(handValues.baubleScript.baubles[59].quantityOwned > 0)
		{
			List<CardScript> cardsHeld = handValues.handScript.GetAllCardsInHand();
			List<CardScript> spyglasses = new List<CardScript>();
			for(int i = 0; i < cardsHeld.Count; i++)
			{
				if(!cardsHeld[i].standardCard && cardsHeld[i].nonStandardCardNumber == 9)
				{
					spyglasses.Add(cardsHeld[i]);
				}
			}
			if(spyglasses.Count > 0)
			{
				//print("found " + spyglasses.Count + " spyglasses");
				handValues.baubleNotifications.Notify(59);
				yield return new WaitForSeconds(0.75f / handValues.gameOptions.gameSpeedFactor); 
				SoundManager.instance.PlaySpyglassSound();
				for(int i = 0; i < spyglasses.Count; i++)
				{
					handValues.CreateDissolvingCard(spyglasses[i]);
					ShopScript.instance.CheatAddBauble(handValues.baubleScript.GetBaubleIndexByHandTier(currentHandTier), spyglasses[i].rt.anchoredPosition + new Vector2(-225,-174), false);
					Destroy(spyglasses[i].tooltipScript.gameObject);
					Destroy(spyglasses[i].gameObject);
					handValues.handScript.visualCardsInHand--;
				}
				Statistics.instance.currentRun.zodiacsEarned += spyglasses.Count;
				CallAnimateCardMultiplier(0, 1);
				CallAnimateBaseValue(0, 1, true);
				yield return null;
				HandUpdated();
				StartCoroutine(ScoreHand());
				yield return new WaitForSeconds(1f / handValues.gameOptions.gameSpeedFactor);
				handValues.handScript.ReorganizeHand();
				yield break;
			}
		}
		
		if(handValues.baubleScript.baubles[62].quantityOwned > 0 && !alreadyCheckedForQueens)
		{	// This MUST be checked after any potential baubles that could update the hand, and before any changes to cards
			List<CardScript> queens = new List<CardScript>();
			for(int i = 0; i < cards.Count; i++)
			{
				if(cards[i].rankInt == 10)
				{
					queens.Add(cards[i]);
				}
			}
			if(queens.Count > 0)
			{
				alreadyCheckedForQueens = true;
				handValues.baubleNotifications.Notify(62);
				yield return new WaitForSeconds(0.75f / handValues.gameOptions.gameSpeedFactor); 
				SoundManager.instance.PlayQueenSound();
				for(int i = 0; i < queens.Count; i++)
				{
					ShopScript.instance.CheatAddBauble(handValues.baubleScript.GetBaubleIndexByHandTier(currentHandTier), queens[i].dropZonePlacedIn.rt.anchoredPosition + new Vector2(-163f,119.5f), false);
				}
				Statistics.instance.currentRun.zodiacsEarned += queens.Count;
				CallAnimateCardMultiplier(0, 1);
				CallAnimateBaseValue(0, 1, true);
				yield return null;
				HandUpdated();
				StartCoroutine(ScoreHand());
				yield break;
			}
		}
		
		alreadyCheckedForQueens = false;
		
		cards.Sort((a, b) => a.dropZonePlacedIn.dropZoneNumber.CompareTo(b.dropZonePlacedIn.dropZoneNumber));
		List<float> scorePlateY = new List<float>();
		const float middleY = 0;
		const float highY = 11;
		const float lowY = -11;
		bool lastHigh = false;
		shouldNotShootSnurfGun = false;
		for(int card = 0; card < cards.Count; card++)
		{	// this is the low/high scoreplates function
			//print("card= " + card);
			if(card != cards.Count - 1)
			{
				//print("not last card");
				if(cards[card + 1].dropZonePlacedIn.dropZoneNumber - cards[card].dropZonePlacedIn.dropZoneNumber == 1)
				{	// this card has a card to the right
					//print("card to the right");
					if(lastHigh)
					{
						scorePlateY.Add(lowY);
						lastHigh = false;
					}
					else
					{
						scorePlateY.Add(highY);
						lastHigh = true;
					}
				}
				else if(card - 1 >= 0)
				{
					if(cards[card].dropZonePlacedIn.dropZoneNumber - cards[card - 1].dropZonePlacedIn.dropZoneNumber == 1)
					{
						//print("card to the left");
						if(lastHigh)
						{
							scorePlateY.Add(lowY);
							lastHigh = false;
						}
						else
						{
							scorePlateY.Add(highY);
							lastHigh = true;
						}
					}
					else
					{
						//print("card has no neighbor 1");
						scorePlateY.Add(middleY);
						lastHigh = false;
					}
				}
				else
				{
					//print("card has no neighbor 2");
					scorePlateY.Add(middleY);
					lastHigh = false;
				}
			}
			else
			{
				//print("last card");
				if(cards.Count > 1)
				{
					if(cards[card].dropZonePlacedIn.dropZoneNumber - cards[card - 1].dropZonePlacedIn.dropZoneNumber == 1)
					{
						//print("last card has card to the left");
						if(lastHigh)
						{
							scorePlateY.Add(lowY);
						}
						else
						{
							scorePlateY.Add(highY);
						}
					}
					else
					{
						//print("last card has no neighbor");
						scorePlateY.Add(middleY);
					}
				}
				else
				{
					//print("solo card");
					scorePlateY.Add(middleY);
				}
			}
		}
/* 		
		dataText.text = dataText.text + "SPY.C= " + scorePlateY.Count + " ";
		for(int card = 0; card < cards.Count; card++)
		{
			dataText.text = dataText.text + cards[card].GetCardNameCharSuit() + " " + scorePlateY[card].ToString() + " ";
		}
		dataText.text = dataText.text + "\n"; */
		
		if(handValues.baubleScript.baubles[36].quantityOwned > 0)
		{
			List<CardScript> faceCardsInHand = new List<CardScript>();
			for(int i = 0; i < cards.Count; i++)
			{
				if(cards[i].rankInt >= 9 && cards[i].rankInt <= 11)
				{
					faceCardsInHand.Add(cards[i]);
				}
			}
			if(faceCardsInHand.Count > 0)
			{
				int ranCard = handValues.randomNumbers.Range(0, faceCardsInHand.Count);
				faceCardsInHand[ranCard].amountToAddToMultiplierThisHand += handValues.baubleScript.baubles[36].baubleImpact * faceCardsInHand.Count;
				CardScript chosenMonarch = faceCardsInHand[ranCard];
				float delayTime = 0f; 
				for(int i = 0; i < cards.Count; i++)
				{
					if(cards[i] == chosenMonarch)
					{
						delayTime = (i * 0.25f) / handValues.gameOptions.gameSpeedFactor;
						break;
					}
				}
				StartCoroutine(DelayBaubleNotification(delayTime, 36));
			}
		}
		
		if(handValues.baubleScript.baubles[32].quantityOwned > 0)
		{
			for(int i = 0; i < cards.Count; i++)
			{
				if(cards[i].rankInt <= 8)
				{
					cards[i].amountToAddToBaseValueThisHand += handValues.baubleScript.baubles[32].baubleImpact;
					float delayTime = (i * 0.25f) / handValues.gameOptions.gameSpeedFactor;
					StartCoroutine(DelayBaubleNotification(delayTime, 32));
				}
			}
		} 
		
		if(handValues.baubleScript.baubles[61].quantityOwned > 0 && currentHandContains[4])
		{
			for(int i = 0; i < cards.Count; i++)
			{
				if(cards[i].rankInt == 12)
				{
					cards[i].amountToAddToMultiplierThisHand += 3;//handValues.baubleScript.baubles[40].baubleImpact;
					cards[i].amountToAddToBaseValueThisHand += 15;
					float delayTime = (i * 0.25f) / handValues.gameOptions.gameSpeedFactor;
					StartCoroutine(DelayBaubleNotification(delayTime, 61));
				}
			}
		}
		
		for(int card = 0; card < cards.Count; card++)
		{
			GameObject scorePlate = Instantiate(ScorePlatePrefab, Vector3.zero, Quaternion.identity, ScorePlateParent);
			ScorePlateScript scorePlateScript = scorePlate.GetComponent<ScorePlateScript>();
			scorePlateScript.rt.anchoredPosition = new Vector2(25.5f + 48f * cards[card].dropZonePlacedIn.dropZoneNumber, 42.5f + scorePlateY[card]);
			scorePlateScript.addingToBaseValue = true;
			for(int i = 0; i < scorePlateScript.scoreTexts.Length; i++)
			{
				scorePlateScript.scoreTexts[i].text = handValues.ConvertFloatToString(cards[card].cardValue);
				scorePlateScript.scoreTexts[i].ForceMeshUpdate(true, true);
			}
			ResizeBackdropForScorePlate(scorePlateScript.border, scorePlateScript.backdrop, scorePlateScript.scoreTexts[0]);
			scorePlateScript.decompressedWidth = scorePlateScript.border.sizeDelta.x;
			scorePlateScript.totalValue = cards[card].cardValue; // before any additions
			if(Mathf.Abs(addToAllBaseValue) > epsilon)
			{
				scorePlateScript.valueToAdd += addToAllBaseValue;
				cards[card].ChangeCardValue(addToAllBaseValue, 1f);
			}
			if(Mathf.Abs(cards[card].amountToAddToBaseValueThisHand) > epsilon)
			{
				scorePlateScript.valueToAdd += cards[card].amountToAddToBaseValueThisHand;
				cards[card].ChangeCardValue(cards[card].amountToAddToBaseValueThisHand, 1f);
				cards[card].amountToAddToBaseValueThisHand = 0;
			}
			/* if(handValues.baubleScript.baubles[32].quantityOwned > 0 && cards[card].rankInt <= 8)
			{
				scorePlateScript.valueToAdd += handValues.baubleScript.baubles[32].baubleImpact;
				cards[card].ChangeCardValue(handValues.baubleScript.baubles[32].baubleImpact, 1f);
				handValues.baubleNotifications.Notify(32);
			} */
			if(mushroomProduct != 1)
			{
				scorePlateScript.alreadyExpanded = false;
				scorePlateScript.mushroomProduct = mushroomProduct;
				cards[card].ChangeCardValue(0, mushroomProduct);
			}
			scorePlateScript.valueToAddString = handValues.ConvertFloatToString(scorePlateScript.valueToAdd);
			scorePlateScript.afterAddition = handValues.ConvertFloatToString(cards[card].cardValue); // after additions
			scorePlateScript.handZone = this;
			if(Mathf.Abs(cards[card].cardMultiplier) > epsilon || Mathf.Abs(cards[card].amountToAddToMultiplierThisHand) > epsilon || Mathf.Abs(addToAllMults) > epsilon)
			{
				GameObject multScorePlate = Instantiate(ScorePlatePrefab, Vector3.zero, Quaternion.identity, ScorePlateParent);
				ScorePlateScript multScorePlateScript = multScorePlate.GetComponent<ScorePlateScript>();
				multScorePlateScript.rt.anchoredPosition = new Vector2(25.5f + 48f * cards[card].dropZonePlacedIn.dropZoneNumber, 22f + 42.5f + scorePlateY[card]);
				multScorePlateScript.backdropImage.color = handValues.multiplierColor;
				multScorePlateScript.addingToMultiplier = true;
				if(Mathf.Abs(scorePlateScript.valueToAdd) > epsilon)
				{
					multScorePlateScript.rt.anchoredPosition = multScorePlateScript.rt.anchoredPosition + new Vector2(0, 22);
				}
				for(int i = 0; i < multScorePlateScript.scoreTexts.Length; i++)
				{
					multScorePlateScript.scoreTexts[i].text = handValues.ConvertFloatToString(cards[card].cardMultiplier);
					multScorePlateScript.scoreTexts[i].ForceMeshUpdate(true, true);
				}
				ResizeBackdropForScorePlate(multScorePlateScript.border, multScorePlateScript.backdrop, multScorePlateScript.scoreTexts[0]);
				multScorePlateScript.decompressedWidth = multScorePlateScript.border.sizeDelta.x;
				multScorePlateScript.totalValue = cards[card].cardMultiplier; // before any additions
				if(Mathf.Abs(addToAllMults) > epsilon)
				{
					multScorePlateScript.valueToAdd += addToAllMults;
					cards[card].ChangeCardMultiplier(addToAllMults, 1f);
				}
				if(Mathf.Abs(cards[card].amountToAddToMultiplierThisHand) > epsilon)
				{
					multScorePlateScript.valueToAdd += cards[card].amountToAddToMultiplierThisHand;
					cards[card].ChangeCardMultiplier(cards[card].amountToAddToMultiplierThisHand, 1f);
					cards[card].amountToAddToMultiplierThisHand = 0;
				}
				if(mushroomProduct != 1)
				{
					multScorePlateScript.alreadyExpanded = false;
					cards[card].ChangeCardMultiplier(0, mushroomProduct);
				}
				multScorePlateScript.valueToAddString = handValues.ConvertFloatToString(multScorePlateScript.valueToAdd);
				multScorePlateScript.afterAddition = handValues.ConvertFloatToString(cards[card].cardMultiplier);
				multScorePlateScript.handZone = this;
			}
			yield return new WaitForSeconds(0.25f / handValues.gameOptions.gameSpeedFactor);
		}
		
		yield return new WaitForSeconds((1.75f + 0.25f * cards.Count) / handValues.gameOptions.gameSpeedFactor);
		
		if(handValues.baubleScript.baubles[56].quantityOwned > 0)
		{
			int rolledNumber = RandomNumbers.instance.Range(1, Mathf.RoundToInt(handValues.baubleScript.baubles[56].baubleImpact) + 1);
			bool maxRoll = false;
			if(rolledNumber == Mathf.RoundToInt(handValues.baubleScript.baubles[56].baubleImpact))
			{
				maxRoll = true;
			}
			handValues.baubleNotifications.NotifyDie(rolledNumber, maxRoll);
			yield return new WaitForSeconds(0.75f / handValues.gameOptions.gameSpeedFactor);
			if(rolledNumber > 1)
			{
				CallAnimateCardMultiplier(rolledNumber, 1);
			}
			yield return new WaitForSeconds(0.5f / handValues.gameOptions.gameSpeedFactor); 
			//StartCoroutine(DelayDieNotification(delayTime, rolledNumber, maxRoll));
		}
		
		if(handValues.baubleScript.baubles[35].quantityOwned > 0)
		{
			int numberOfRainbowCards = 0;
			for(int i = 0; i < cards.Count; i++)
			{
				if(cards[i].suitInt == 4)
				{
					numberOfRainbowCards++;
				}
			}
			if(numberOfRainbowCards > 0)
			{
				handValues.baubleNotifications.Notify(35);
				yield return new WaitForSeconds(0.75f / handValues.gameOptions.gameSpeedFactor); 
				SoundManager.instance.PlayViolinSound();
				CallAnimateCardMultiplier(handValues.baubleScript.baubles[35].baubleImpact * numberOfRainbowCards, 1);
				yield return new WaitForSeconds(0.5f / handValues.gameOptions.gameSpeedFactor); 
			}
		}
		
		if(handValues.baubleScript.baubles[40].quantityOwned > 0)
		{
			int numberOfKings = 0;
			for(int i = 0; i < cards.Count; i++)
			{
				if(cards[i].rankInt == 11)
				{
					numberOfKings++;
				}
			}
			if(numberOfKings > 0)
			{
				handValues.baubleNotifications.Notify(40);
				yield return new WaitForSeconds(0.75f / handValues.gameOptions.gameSpeedFactor); 
				CallAnimateCardMultiplier(handValues.baubleScript.baubles[40].baubleImpact * numberOfKings, 1);
				yield return new WaitForSeconds(0.5f / handValues.gameOptions.gameSpeedFactor); 
			}
		}
		
		if(handValues.baubleScript.baubles[39].quantityOwned > 0)
		{
			bool[] suitsFound = new bool[4];
			int numberOfRainbowCards = 0;
			for(int i = 0; i < cards.Count; i++)
			{
				if(cards[i].suitInt < 4 && cards[i].standardCard)
				{
					suitsFound[cards[i].suitInt] = true;
				}
				if(cards[i].suitInt == 4)
				{
					numberOfRainbowCards++;
				}
			}
			int numberOfUniqueSuits = 0;
			for(int i = 0; i < suitsFound.Length; i++)
			{
				if(suitsFound[i])
				{
					numberOfUniqueSuits++;
				}
			}
			if(numberOfUniqueSuits + numberOfRainbowCards >= 4)
			{
				handValues.baubleNotifications.Notify(39);
				yield return new WaitForSeconds(0.75f / handValues.gameOptions.gameSpeedFactor); 
				SoundManager.instance.PlayViolinSound();
				CallAnimateCardMultiplier(0, handValues.baubleScript.baubles[39].baubleImpact);
				yield return new WaitForSeconds(0.5f / handValues.gameOptions.gameSpeedFactor); 
			}
		}
		
		for(int i = 1; i < 18; i++)
		{
			if(currentHandContains[i])
			{
				int baubleNumber = 0;
				if(i < 6)
				{
					baubleNumber = i + 22;
				}
				else if(i == 6)
				{
					baubleNumber = 37;
				}
				else if(i > 6 && i < 9)
				{
					baubleNumber = i + 21;
				}
				else if(i >= 9)
				{
					baubleNumber = i + 38;
				}
				if(handValues.baubleScript.baubles[baubleNumber].quantityOwned > 0)
				{
					handValues.baubleNotifications.Notify(baubleNumber);
					yield return new WaitForSeconds(0.75f / handValues.gameOptions.gameSpeedFactor);
					SoundManager.instance.PlayViolinSound();
					CallAnimateCardMultiplier(0, handValues.baubleScript.baubles[baubleNumber].baubleImpact);
					yield return new WaitForSeconds(0.5f / handValues.gameOptions.gameSpeedFactor); 
				}
			}
		}
		if(bombPlaced || handValues.destroyAllPlacedCards)
		{
			if(handValues.destroyAllPlacedCards)
			{
				if(activeDropZones <=6)
				{
					bombLocation = 2;
				}
				else
				{
					bombLocation = 2;
				}
			}
			for(int i = 0; i <= 6; i++)
			{
				bool madeSoundEffect = false;
				if(bombLocation - i >= 0)
				{
					SoundManager.instance.PlayBombSound();
					madeSoundEffect = true;
					GameObject newBomb = Instantiate(handValues.bombExplosionPrefab, new Vector3(0,0,0), Quaternion.identity, dropZones[bombLocation - i].transform);
					BombExplosionScript bombExplosionScript = newBomb.GetComponent<BombExplosionScript>();
					bombExplosionScript.rt.anchoredPosition = new Vector2(0,0);
					if(dropZones[bombLocation - i].cardPlaced)
					{
						if(!dropZones[bombLocation - i].placedCardScript.noImpactOnHandSize)
						{
							handValues.handScript.cardsInHand--;
						}
						cardXImages[bombLocation - i].gameObject.SetActive(false);
						bombExplosionScript.cardToDestroy = dropZones[bombLocation - i].placedCardScript.gameObject;
						dropZones[bombLocation - i].CardRemoved(false);
					}
				}
				if(bombLocation + i <= 6 && i > 0)
				{
					if(!madeSoundEffect)
					{
						SoundManager.instance.PlayBombSound();
					}
					GameObject newBomb = Instantiate(handValues.bombExplosionPrefab, new Vector3(0,0,0), Quaternion.identity, dropZones[bombLocation + i].transform);
					BombExplosionScript bombExplosionScript = newBomb.GetComponent<BombExplosionScript>();
					bombExplosionScript.rt.anchoredPosition = new Vector2(0,0);
					if(dropZones[bombLocation + i].cardPlaced)
					{
						if(!dropZones[bombLocation + i].placedCardScript.noImpactOnHandSize)
						{
							handValues.handScript.cardsInHand--;
						}
						cardXImages[bombLocation + i].gameObject.SetActive(false);
						bombExplosionScript.cardToDestroy = dropZones[bombLocation + i].placedCardScript.gameObject;
						dropZones[bombLocation + i].CardRemoved(false);
					}
				}
				if(bombLocation - i > 0 || bombLocation + i < 7)
				{
					yield return new WaitForSeconds(0.2f / handValues.gameOptions.gameSpeedFactor);
				}
			}
			yield return new WaitForSeconds(2f / handValues.gameOptions.gameSpeedFactor);
		}
		for(int i = 0; i < allPlacedCards.Count; i ++) // destroy remaining non standard cards
		{
			if(!allPlacedCards[i].standardCard)
			{
				if(!bombPlaced && !handValues.destroyAllPlacedCards && allPlacedCards[i].nonStandardCardNumber >= 2)
				{
					dropZones[allPlacedCards[i].dropZonePlacedIn.dropZoneNumber].CardRemoved(false);
					handValues.CreateDissolvingCard(allPlacedCards[i]);
					Destroy(allPlacedCards[i].tooltipScript.gameObject);
					Destroy(allPlacedCards[i].gameObject);
				}
			}
		}
		if(handValues.baubleScript.baubles[62].quantityOwned > 0)
		{
			for(int i = 0; i < cards.Count; i++)
			{
				if(cards[i].rankInt == 10 && !bombPlaced && !handValues.destroyAllPlacedCards)
				{
					dropZones[cards[i].dropZonePlacedIn.dropZoneNumber].CardRemoved(false);
					handValues.CreateDissolvingCard(cards[i]);
					Destroy(cards[i].gameObject);
					handValues.handScript.cardsInHand--;
				}
			}
		}
		CallAnimateCardMultiplier(0, 1);
		CallAnimateBaseValue(0, currentCardMultiplier, true);
		SoundManager.instance.PlayScoreMultipliedSound();
		finishedAnimating = true;
		if(true)
		{
			if(handValues.handsLeftUntilFatigue <= 0)
			{
				Statistics.instance.IncrementHandsPlayedInFatigue();
				handValues.handScript.DiscardCardsInHand();
			}
			handValues.UpdateHandsUntilFatigue(-1, false);
		}
		yield return new WaitForSeconds(1f / handValues.gameOptions.gameSpeedFactor);
		DeckViewer.instance.CheckForRepublicDeckUnlock();
		handValues.ScoreEntireHand();
		
		
		yield return null;
	}
	
	public IEnumerator DelayBaubleNotification(float delayTime, int baubleNumber)
	{
		float t = 0;
		while(t < delayTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		handValues.baubleNotifications.Notify(baubleNumber);
	}
	
	public IEnumerator DelayDieNotification(float delayTime, int numberRolled, bool maxRoll)
	{
		float t = 0;
		while(t < delayTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		handValues.baubleNotifications.NotifyDie(numberRolled, maxRoll);
	}
	
	public List<CardScript> GetAllCardsInHandZone()
	{
		List<CardScript> cardScripts = new List<CardScript>();
		for(int dropZone = 0; dropZone < dropZones.Length; dropZone++)
		{
			if(dropZones[dropZone].cardPlaced)
			{
				cardScripts.Add(dropZones[dropZone].placedCardScript);
			}
		}
		return cardScripts;
	}
	
	public int GetNumberOfCardsInHandZone()
	{
		int cards = 0;
		for(int dropZone = 0; dropZone < dropZones.Length; dropZone++)
		{
			if(dropZones[dropZone].cardPlaced)
			{
				cards++;
			}
		}
		return cards;
	}
	
	public List<CardScript> GetCountedCards()
	{
		List<CardScript> cardScripts = new List<CardScript>();
		for(int dropZone = 0; dropZone < dropZones.Length; dropZone++)
		{
			if(dropZones[dropZone].cardPlaced)
			{
				if(dropZones[dropZone].placedCardScript.countCardInHand)
				{
					cardScripts.Add(dropZones[dropZone].placedCardScript);
				}
			}
		}
		return cardScripts;
	}
	
	void Start()
	{
		dataText = GameObject.FindWithTag("DataText").GetComponent<TMP_Text>(); //disable for release
	}
	
	public int GetNumberOfEmptySpaces()
	{
		int emptyDropZones = 0;
		for(int dz = 0; dz < dropZones.Length; dz++)
		{
			if(dropZones[dz].gameObject.activeSelf && !dropZones[dz].cardPlaced)
			{
				emptyDropZones++;
			}
		}
		return emptyDropZones;
	}
	
	public List<DropZone> GetEmptyDropZones()
	{
		List<DropZone> emptyDropZones = new List<DropZone>();
		for(int dz = 0; dz < dropZones.Length; dz++)
		{
			if(dropZones[dz].gameObject.activeSelf && !dropZones[dz].cardPlaced)
			{
				emptyDropZones.Add(dropZones[dz]);
			}
		}
		return emptyDropZones;
	}
	
	public void MoveSelectedCardsToHandZone()
	{
		if(!locked)
		{
			List<DropZone> emptyDropZones = GetEmptyDropZones();
			List<CardScript> cardsToMove = new List<CardScript>(handValues.handScript.selectedCards);
			List<CardScript> unplayableCards = new List<CardScript>();
			for(int i = 0; i < cardsToMove.Count; i++)
			{
				if(cardsToMove[i].cannotBePlayed)
				{
					unplayableCards.Add(cardsToMove[i]);
				}
			}
			for(int i = 0; i < unplayableCards.Count; i++)
			{
				cardsToMove.Remove(unplayableCards[i]);
			}
			if(cardsToMove.Count <= emptyDropZones.Count && cardsToMove.Count > 0)
			{
				cardsToMove.Sort((a, b) => a.rt.anchoredPosition.x.CompareTo(b.rt.anchoredPosition.x));
				for(int card = 0; card < cardsToMove.Count; card++)
				{
					CardScript cardScript = cardsToMove[card];
					if(cardScript.transform.parent == handValues.handScript.handParent)
					{
						handValues.handScript.visualCardsInHand--;
					}
					cardScript.transform.SetParent(emptyDropZones[card].transform);
					cardScript.dropZonePlacedIn = emptyDropZones[card];
					emptyDropZones[card].CardPlaced(cardScript);
					cardScript.StartMove(0.25f, new Vector2(22.5f, 22.5f), new Vector3(0,0,0));
					cardScript.UpdateToDropZoneImage();
				}
				handValues.handScript.selectedCards.Clear();
				handValues.handScript.SelectedCardsUpdated();
				handValues.handScript.PlacedCardsUpdated(true);
				handValues.handScript.ReorganizeHand();
				HandUpdated();
				SoundManager.instance.PlayCardDropSound();
				if(!GameOptions.instance.tutorialDone)
				{
					if(Tutorial.instance.tutorialStage == 1)
					{
						Tutorial.instance.AdvanceTutorial();
					}
				}
			}
		}
	}
	
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		MoveSelectedCardsToHandZone();
	}
	
	public void CheckForMagicMirror()
	{
		List<CardScript> cards = GetAllCardsInHandZone();
		for(int i = 0; i < cards.Count; i++)
		{
			if(!cards[i].standardCard)
			{
				if(cards[i].nonStandardCardNumber == 8)
				{
					cards[i].UpdateToDropZoneImage();
				}
			}
		}
	}
	
	public void HandUpdated()
	{
		crosshairRT.gameObject.SetActive(false);
		int cardsInHand = 0;
		List<CardScript> cardScripts = new List<CardScript>();
		cardScripts = GetCountedCards();
		cardsInHand = cardScripts.Count;
		//print("HandUpdated cardsInHand = " + cardsInHand);
		for(int i = 0; i < cardXImages.Length; i++)
		{
			cardXImages[i].gameObject.SetActive(false);
		}
		//print("cardsInHand= " + cardsInHand);
		if(cardsInHand > 0)
		{
			LargeHandEvaluation.instance.EvaluateLargeHand(cardScripts, this, false);
			CheckForMagicMirror();
			HandsInformation.instance.RecolorHandLabels(currentHandContains);
			//handValues.EvaluateHand(cardScripts, this, false);
			if(cardsInHand == 1)
			{
				if(cardScripts[0].standardCard)
				{
					if(GetNumberOfCardsInHandZone() == 1)
					{
						if(handValues.baubleScript.baubles[57].quantityOwned >= 1 && !shouldNotShootSnurfGun)
						{
							crosshairRT.gameObject.SetActive(true);
							crosshairRT.transform.SetParent(cardScripts[0].dropZonePlacedIn.transform);
							crosshairRT.transform.SetSiblingIndex(crosshairRT.transform.parent.childCount - 1);
							crosshairRT.anchoredPosition = new Vector2(0, 0);
							for(int i = 0; i < handNameTexts.Length; i++)
							{
								handNameTexts[i].text = "Blasted";
							}
							for(int i = 0; i < baseHandValueTexts.Length; i++)
							{
								baseHandValueTexts[i].text = "0";
							}
							for(int i = 0; i < cardMultiplierTexts.Length; i++)
							{
								cardMultiplierTexts[i].text = "0";
							}
						}
					}
				}
			}
			if(lockButton.disabled && !locked)
			{
				lockButton.ChangeDisabled(false);
			}
			if(cardsInHand > 1)
			{
				for(int i = 0; i < cardScripts.Count; i++)
				{
					if(!cardsUsedCurrently.Contains(cardScripts[i]))
					{
						cardXImages[cardScripts[i].dropZonePlacedIn.dropZoneNumber].gameObject.SetActive(true);
					}
				}
				/* if(cardsInHand > 5)
				{
					
				} */
			}
		}
		else
		{
			if(!lockButton.disabled)
			{
				lockButton.ChangeDisabled(true);
			}
			HandEvaluated(null, "", -1, false, new bool[18]);
			HandsInformation.instance.RecolorHandLabels(currentHandContains);
		}
	}
	
	public void ChangeXImagesToGreen()
	{
		for(int i = 0; i < cardXImages.Length; i++)
		{
			cardXImages[i].color = cardXImageGreen;
		}
	}
	
	public void ChangeXImagesToRed()
	{
		for(int i = 0; i < cardXImages.Length; i++)
		{
			cardXImages[i].color = cardXImageRed;
		}
	}
	
	//public void HandEvaluated(List<CardScript> cardsUsed, string handName, int handTier)
	//public void HandEvaluated(List<CardScript> cardsUsed, string handName, int handTier, bool evaluatingOnlyCardsUsed, bool handContainsPair, bool handContainsTwoPair, bool handContainsThreeOfAKind, bool handContainsStraight, bool handContainsFlush, bool handContainsFullHouse, bool handContainsFourOfAKind, bool handContainsStraightFlush, bool handContainsFiveOfAKind, bool handContainsFlushHouse, bool handContainsFlushFive)
	public void HandEvaluated(List<CardScript> cardsUsed, string handName, int handTier, bool evaluatingOnlyCardsUsed, bool[] handsContained)
	{
		if(cardsUsed != null)
		{
			List<CardScript> allCountedCards = GetCountedCards();
			//print("allCountedCards.Count= " + allCountedCards.Count + " cardsUsed.Count= " + cardsUsed.Count);
			if(allCountedCards.Count > cardsUsed.Count && !evaluatingOnlyCardsUsed && handValues.baubleScript.baubles[33].quantityOwned == 0)
			{
				LargeHandEvaluation.instance.EvaluateLargeHand(cardsUsed, this, true);
				//handValues.EvaluateHand(cardsUsed, this, true);
				return;
			}
		}
		for(int i = 0; i < handNameTexts.Length; i++)
		{
			handNameTexts[i].text = handName;
		}
		float handBaseValue;
		float handCardMultiplier;
		if(handTier >= 0)
		{
			//handBaseValue = handValues.handBaseValues[handTier];
			handBaseValue = handValues.GetTotalBaseValue(handsContained);//.handBaseValues[handTier];
			//handCardMultiplier = handValues.handCardMultipliers[handTier];
			handCardMultiplier = handValues.GetTotalMult(handsContained);
			cardsUsedCurrently = cardsUsed;
			currentCardMultiplier = handCardMultiplier;
			currentBaseValue = handBaseValue;
			currentHandTier = handTier;
			/* currentHandContainsPair = handContainsPair;
			currentHandContainsTwoPair = handContainsTwoPair;
			currentHandContainsThreeOfAKind = handContainsThreeOfAKind;
			currentHandContainsStraight = handContainsStraight;
			currentHandContainsFlush = handContainsFlush;
			currentHandContainsFullHouse = handContainsFullHouse;
			currentHandContainsFourOfAKind = handContainsFourOfAKind;
			currentHandContainsStraightFlush = handContainsStraightFlush; */
			currentHandContains = handsContained;
		}
		else
		{
			cardsUsedCurrently = null;
			currentCardMultiplier = 0;
			currentBaseValue = 0;
			currentHandTier = -1;
			/* currentHandContainsPair = false;
			currentHandContainsTwoPair = false;
			currentHandContainsThreeOfAKind = false;
			currentHandContainsStraight = false;
			currentHandContainsFlush = false;
			currentHandContainsFullHouse = false;
			currentHandContainsFourOfAKind = false;
			currentHandContainsStraightFlush = false; */
			for(int i = 0; i < currentHandContains.Length; i++)
			{
				currentHandContains[i] = false;
			}
			for(int i = 0; i < baseHandValueTexts.Length; i++)
			{
				baseHandValueTexts[i].text = "Base Value";
			}
			for(int i = 0; i < cardMultiplierTexts.Length; i++)
			{
				cardMultiplierTexts[i].text = "Hand Multi";
			}
			return;
		}
		//print("handTier= " + handTier + " handBaseValue= " + handBaseValue.ToString() + " handCardMultiplier= " +handCardMultiplier.ToString());
		//baseHandValueTexts[0].text = "sporgle";
		for(int i = 0; i < baseHandValueTexts.Length; i++)
		{
			baseHandValueTexts[i].text = handValues.ConvertFloatToString(handBaseValue);
			//baseHandValueTexts[i].text = handBaseValue.ToString("0");
			//baseHandValueTexts[i].text = "jufloop";
		}
		for(int i = 0; i < cardMultiplierTexts.Length; i++)
		{
			cardMultiplierTexts[i].text = handValues.ConvertFloatToString(handCardMultiplier);
			//cardMultiplierTexts[i].text = handCardMultiplier.ToString("0.0");
			//cardMultiplierTexts[i].text = "jufloop";
		}
	}
}
