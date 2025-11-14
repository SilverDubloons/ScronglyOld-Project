using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class HandScript : MonoBehaviour
{
	public static HandScript instance;
	public RectTransform baseRT;
    public DeckScript drawPile;
	public int cardsInHand = 0;
	public int visualCardsInHand = 0;
	public int maxHandSize = 7;
	public Transform handParent;
	public int discardsPerAnte = 4;
	
	public TMP_Text[] drawPileTexts;
	public TMP_Text[] discardPileTexts;
	public TMP_Text[] discardsRemainingTexts;
	public GameObject infinityDiscardsSymbol;
	
	public AnimationCurve handParabola;
	public AnimationCurve handRotationParabola;
	
	public int cardToNotMove = -1;
	
	public TMP_Text handData;
	
	public UnderMouseCard underMouseCard;
	
	public List<CardScript> selectedCards;
	public Transform discardParent;
	public RectTransform discardParentRT;
	
	public int alwaysSortType; // 0 = no always sort, 1 = by rank, 2 = by suit
	public MovingButton[] sortButtons;
	public MovingButton discardButton;
	public MovingButton recallButton;
	
	float timeBetweenCards = 0.25f;
	
	public int maxCardsSelectedAtOnce = 5;
	public int maxCardsDiscardedAtOnce = 5;
	
	public HandValues handValues;
	
	public TMP_Text dataText;
	
	public bool aceDiscardBaubleOwned = false;
	public ScoreVial scoreVial;
	
	public int numberOfTopCardsVisible = 0;
	
	public Transform tooltipParent;
	
	public DeckPreview deckPreview;
	public DeckViewer deckViewer;
	//public Statistics statistics;
	
	public MovingButton selectAllButton;
	public MovingButton playSelectedButton;
	public bool lowOnCards = false;
	public Image deckBackdropImage;
	public float deckBackdropIntensity;
	public GameObject lossConditionTooltip;
	
	void Awake()
	{
		instance = this;
	}
	
	public void PlaySelectedClicked()
	{
		handValues.handZones[0].MoveSelectedCardsToHandZone();
	}
	
	public void SelectAllClicked()
	{
		List<CardScript> cardsInHand = GetAllCardsInHand();
		int standardCardsSelected = 0;
		int standardCards = 0;
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			if(cardsInHand[i].standardCard)
			{
				standardCards++;
				if(!selectedCards.Contains(cardsInHand[i]))
				{
					selectedCards.Add(cardsInHand[i]);
					cardsInHand[i].rt.anchoredPosition = cardsInHand[i].rt.anchoredPosition + Vector2.up * 20;
				}
				else
				{
					standardCardsSelected++;
				}
			}
			else
			{
				if(selectedCards.Contains(cardsInHand[i]))
				{
					selectedCards.Remove(cardsInHand[i]);
					cardsInHand[i].rt.anchoredPosition = cardsInHand[i].rt.anchoredPosition - Vector2.up * 20;
				}
			}
		}
		if(standardCards == standardCardsSelected && standardCardsSelected > 0)
		{
			for(int i = 0; i < cardsInHand.Count; i++)
			{
				if(cardsInHand[i].standardCard)
				{
					if(selectedCards.Contains(cardsInHand[i]))
					{
						selectedCards.Remove(cardsInHand[i]);
						cardsInHand[i].rt.anchoredPosition = cardsInHand[i].rt.anchoredPosition - Vector2.up * 20;
					}
				}
			}
			SoundManager.instance.PlayCardDropSound();
		}
		else
		{
			if(standardCards > 0)
			{
				SoundManager.instance.PlayCardPickupSound();
			}
		}
		SelectedCardsUpdated();
	}
	
	public void ShouldSelectAllBeEnabled()
	{
		List<CardScript> cardsInHand = GetAllCardsInHand(false);
		int standardCards = 0;
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			if(cardsInHand[i].standardCard)
			{
				standardCards++;
			}
		}
		if(standardCards <= Mathf.Max(maxCardsDiscardedAtOnce, maxCardsSelectedAtOnce) && standardCards > 0)
		{
			if(selectAllButton.disabled)
			{
				selectAllButton.ChangeDisabled(false);
			}
		}
		else
		{
			if(!selectAllButton.disabled)
			{
				selectAllButton.ChangeDisabled(true);
			}
		}
	}
	
	public void DrawToFullHand()
	{
		//print("drawing MHS= " +  maxHandSize + " CIH= " + cardsInHand);
		StartCoroutine(DrawCards(maxHandSize - cardsInHand));
	}
	
	public void PlacedCardsUpdated(bool justPlaced)
	{
		if(justPlaced)
		{
			if(recallButton.disabled)
			{
				recallButton.ChangeDisabled(false);
			}
		}
		else
		{
			if(handValues.IsThereAPlacedCard())
			{
				if(recallButton.disabled)
				{
					recallButton.ChangeDisabled(false);
				}
			}
			else
			{
				if(!recallButton.disabled)
				{
					recallButton.ChangeDisabled(true);
				}
			}
		}
	}
	
	public void SelectedCardsUpdated()
	{
		if(int.Parse(discardsRemainingTexts[0].text) > 0)
		{
			if(discardButton.disabled && selectedCards.Count > 0 && selectedCards.Count <= maxCardsDiscardedAtOnce)
			{
				discardButton.ChangeDisabled(false);
			}
			else
			{
				if(!discardButton.disabled && (selectedCards.Count == 0 || selectedCards.Count > maxCardsDiscardedAtOnce))
				{
					discardButton.ChangeDisabled(true);
				}
			}
		}
		if(selectedCards.Count > 0 && selectedCards.Count <= handValues.GetNumberOfEmptyDropZones() && !handValues.handZones[0].locked)
		{
			if(playSelectedButton.disabled)
			{
				playSelectedButton.ChangeDisabled(false);
			}
		}
		else
		{
			if(!playSelectedButton.disabled)
			{
				playSelectedButton.ChangeDisabled(true);
			}
		}
	}
	
	public void DiscardClicked()
	{
		//selectedCards.Sort((a, b) => a.GetComponent<RectTransform>().anchoredPosition.x.CompareTo(b.GetComponent<RectTransform>().anchoredPosition.x));
		//print("discarding " + selectedCards.Count + " cards");
		SoundManager.instance.PlayCardSlideSound();
		List<CardScript> cardsToDiscard = new List<CardScript>(selectedCards);
		int numberOfAces = 0;
		List<Vector2> locationsOfAces = new List<Vector2>();
		for(int card = 0; card < cardsToDiscard.Count; card++)
		{
			if(cardsToDiscard[card].rankInt == 12 && aceDiscardBaubleOwned)
			{
				numberOfAces++;
				locationsOfAces.Add(cardsToDiscard[card].rt.anchoredPosition);
			}
			cardsToDiscard[card].cardLocation = 3;
			cardsToDiscard[card].deckViewerClone.cardLocation = 3;
			cardsToDiscard[card].canMove = false;
			cardsToDiscard[card].transform.SetParent(discardParent);
			//print("moving " + cardsToDiscard[card].GetCardNameCharSuit());
			cardsToDiscard[card].StartMove(0.25f, new Vector2(22.5f, 22.5f), new Vector3(0,0,0));
			if(!cardsToDiscard[card].faceDown)
			{
				StartCoroutine(cardsToDiscard[card].Flip(0.25f));
			}
			if(!cardsToDiscard[card].noImpactOnHandSize)
			{
				cardsInHand--;
			}
			visualCardsInHand--;
			DiscardPileCountChanged(1);
			/* int cardsInDiscardPile = int.Parse(discardPileTexts[0].text);
			cardsInDiscardPile++;
			for(int text = 0; text < discardPileTexts.Length; text++)
			{
				discardPileTexts[text].text = "" + cardsInDiscardPile;
			} */
		}
		int discardsRemaining = int.Parse(discardsRemainingTexts[0].text);
		discardsRemaining--;
		for(int i = 0; i < discardsRemainingTexts.Length; i++)
		{
			discardsRemainingTexts[i].text = "" + discardsRemaining;
		}
		if(numberOfAces >= 2 && aceDiscardBaubleOwned)
		{
			handValues.baubleNotifications.Notify(20);
			handValues.menuButton.ChangeDisabled(true);
			//for(int i = 0; i < numberOfAces; i++)
			for(int i = 0; i < 2; i++)
			{
				scoreVial.SpawnPokerChip(locationsOfAces[i] + new Vector2(95, -125), scoreVial.movingChipParent);
			}
		}
		selectedCards.Clear();
		SelectedCardsUpdated();
		DrawToFullHand();
		ReorganizeHand();
		if(!GameOptions.instance.tutorialDone)
		{
			if(Tutorial.instance.tutorialStage == 4)
			{
				Tutorial.instance.AdvanceTutorial();
			}
		}
	}
	
	public void ResetDiscards()
	{
		for(int i = 0; i < discardsRemainingTexts.Length; i++)
		{
			discardsRemainingTexts[i].text = "" + discardsPerAnte;
		}
	}
	
	public void ChangeAlwaysSortType(int sortType)
	{
		alwaysSortType = sortType;
		for(int i = 0; i < sortButtons.Length; i++)
		{
			sortButtons[i].ChangeSpecailState(false);
		}
		if(sortType > 0)
		{
			sortButtons[sortType - 1].ChangeSpecailState(true);
		}
		//ReorganizeHand();
	}
	
	public void SortHand(int sortType) // 1 = rank, 2 = suit
	{
		if(sortType != alwaysSortType)
		{
			ChangeAlwaysSortType(0);
		}
		Transform[] children = new Transform[handParent.childCount];
		for(int i = 0; i < handParent.childCount; i++)
		{
			children[i] = handParent.GetChild(i);
		}
		switch(sortType)
		{
			case 1:
				//print("sorting by rank");
				System.Array.Sort(children, (a,b) => 
				{
					int rankComparison = a.GetComponent<CardScript>().rankInt.CompareTo(b.GetComponent<CardScript>().rankInt);
					if(rankComparison != 0)
					{
						return rankComparison;
					}
					else
					{
						return a.GetComponent<CardScript>().suitInt.CompareTo(b.GetComponent<CardScript>().suitInt);
					}
				});
				break;
			case 2:
				//print("sorting by suit");
				System.Array.Sort(children, (a,b) =>
				{
					int suitComparison = a.GetComponent<CardScript>().suitInt.CompareTo(b.GetComponent<CardScript>().suitInt);
					if(suitComparison != 0)
					{
						return suitComparison;
					}
					else
					{
						return a.GetComponent<CardScript>().rankInt.CompareTo(b.GetComponent<CardScript>().rankInt);
					}
				});
				
				break;
			default:
				print("bad sorting parameter");
				break;
		}
		for(int j = 0; j < children.Length; j++)
		{
			//children[j].SetSiblingIndex(children.Length - j - 1);
			children[j].SetSiblingIndex(j);
		}
		//ReorganizeHand();
	}
	
	public void ReorganizeHand() // Rightmost card is child 0
	{
		if(alwaysSortType != 0)
		{
			SortHand(alwaysSortType);
		}
		float distanceBetweenCards = 50f;
		if(visualCardsInHand > 9)
		{
			distanceBetweenCards = 450f / visualCardsInHand;
		}
		for(int i = 0; i < handParent.childCount; i++)
		//for(int i = handParent.childCount - 1; i >= 0; i--)
		{
			if(i != cardToNotMove)
			{
				float xDestination = 225 + (visualCardsInHand - 1) * (distanceBetweenCards / 2f) - (handParent.childCount - i - 1) * distanceBetweenCards;
				float yDestination = 75 + handParabola.Evaluate((450 - xDestination) / 450f)*30;
				
				Vector2 destination = new Vector2(xDestination, yDestination);
				float handRotationNormalized = (450f - xDestination) / 450f;
				float handRotationParabolaEvaluation = handRotationParabola.Evaluate(handRotationNormalized);
				float destinationRotationZ = handRotationParabolaEvaluation*-30f;
				Vector3 destinationRotation = new Vector3(0f,0f,destinationRotationZ);
				//print("xDestination= " +xDestination+ " yDestination= " + yDestination + " destinationRotationZ= " + destinationRotationZ + " handRotationNormalized= " + handRotationNormalized + " handRotationParabolaEvaluation= "+handRotationParabolaEvaluation);
				CardScript cardScript = handParent.GetChild(i).GetComponent<CardScript>();
				handParent.GetChild(i).GetComponent<CardScript>().StartMove(0.1f, destination, destinationRotation);
			}
		}
		ShouldSelectAllBeEnabled();
	}
	
	public IEnumerator DrawCards(int numberOfCards)
	{
		//print("DrawCards called NOC= " + numberOfCards + " TBC= " + timeBetweenCards);
		for(int i = 0; i < numberOfCards; i++)
		{
			if(drawPile.drawPileParent.childCount > 0)
			{
				SoundManager.instance.PlayCardPickupSound();
				Transform topCard = drawPile.drawPileParent.GetChild(drawPile.drawPileParent.childCount - 1);
				CardScript cardScript = topCard.GetComponent<CardScript>();
				cardScript.canMove = true;
				if(cardScript.faceDown)
				{
					cardScript.StartFlip(0.25f);
				}
				topCard.SetParent(handParent);
				cardScript.cardLocation = 2;
				cardScript.deckViewerClone.cardLocation = 2;
				//int siblingIndex = handParent.childCount -1;
				int siblingIndex = 0;
				topCard.SetSiblingIndex(siblingIndex);
				//print("Sibling index for " + cardScript.GetCardNameCharSuit() + " set to " + siblingIndex);
				
				DrawPileCountChanged(-1);
				float t = 0;
				if(!cardScript.noImpactOnHandSize)
				{
					cardsInHand++;
				}
				else
				{
					i--;
				}
				visualCardsInHand++;
				ReorganizeHand();
				while(t < timeBetweenCards)
				{
					t += Time.deltaTime * handValues.gameOptions.gameSpeedFactor;
					yield return null;
				}
			}
			else
			{
				// if all cards in hand are non-standard, ante failed.
				// HandValues.StartCoroutine(HandValues.ShowGameStats(true, false, false));
				// yield break;
			}
		}
		if(drawPile.drawPileParent.childCount > 0 && numberOfTopCardsVisible > 0)
		{
			Transform topCard = drawPile.drawPileParent.GetChild(drawPile.drawPileParent.childCount - 1);
			CardScript cardScript = topCard.GetComponent<CardScript>();
			if(cardScript.faceDown)
			{
				cardScript.StartFlip(0.25f);
			}
		}
		//if( < 18)
		if(drawPile.drawPileParent.childCount < DeckViewer.instance.GetNumberOfCardsInDeck() / 3)
		{
			lowOnCards = true;
			if(!GameOptions.instance.alreadySeenLossConditionTooltip)
			{
				lossConditionTooltip.SetActive(true);
				GameOptions.instance.alreadySeenLossConditionTooltip = true;
				GameOptions.instance.UpdateOptionsFile();
			}
		}
		//ReorganizeHand();
	}
	
	public List<CardScript> GetAllCardsInDrawPile()
	{
		List<CardScript> cardScripts = new List<CardScript>();
		for(int i = 0; i < drawPile.drawPileParent.childCount; i++)
		{
			if(drawPile.drawPileParent.GetChild(i).GetComponent<CardScript>())
			{
				cardScripts.Add(drawPile.drawPileParent.GetChild(i).GetComponent<CardScript>());
			}
		}
		return cardScripts;
	}
	
	public void TurnAllCardsInDrawPileFaceDown()
	{
		TurnAllCardsFaceDown(GetAllCardsInDrawPile());
	}
	
	public void TurnAllCardsFaceDown(List<CardScript> cardScripts)
	{
		for(int card = 0; card < cardScripts.Count; card++)
		{
			if(!cardScripts[card].faceDown)
			{
				cardScripts[card].StartFlip(0.25f);
			}
			if(cardScripts[card].borderRT.localScale.x > 1.0001f)
			{
				cardScripts[card].borderRT.localScale = new Vector3(1f,1f,1f);
			}
		}
	}
	
	public List<CardScript> GetAllCardsInHand(bool includeLooseCards = true)
	{
		List<CardScript> cardScripts = new List<CardScript>();
		for(int i = 0; i < handParent.childCount; i++)
		{
			if(handParent.GetChild(i).GetComponent<CardScript>())
			{
				cardScripts.Add(handParent.GetChild(i).GetComponent<CardScript>());
			}
		}
		if(includeLooseCards)
		{
			Transform looseCardsParent = GameObject.FindWithTag("LooseCards").transform;
			for(int i = 0; i < looseCardsParent.childCount; i++)
			{
				if(looseCardsParent.GetChild(i).GetComponent<CardScript>())
				{
					cardScripts.Add(looseCardsParent.GetChild(i).GetComponent<CardScript>());
				}
			}
		}
		//print("there were " + cardScripts.Count + " cards in hand");
		return cardScripts;
	}
	
	public void FreezeCardsInHand()
	{
		List<CardScript> cardsToFreeze = GetAllCardsInHand();
		for(int card = 0; card < cardsToFreeze.Count; card++)
		{
			cardsToFreeze[card].canMove = false;
		}
	}
	
	public List<CardScript> GetAllCardsInDiscardPile()
	{
		List<CardScript> cardScripts = new List<CardScript>();
		for(int i = 0; i < discardParent.childCount; i++)
		{
			if(discardParent.GetChild(i).GetComponent<CardScript>())
			{
				cardScripts.Add(discardParent.GetChild(i).GetComponent<CardScript>());
			}
		}
		return cardScripts;
	}
	
	public void CheatGetDiscardsBack()
	{
		StartCoroutine(AddAllCardsInDiscardPileToDeck());
	}
	
	public IEnumerator AddAllCardsInDiscardPileToDeck()
	{
		List<CardScript> cardsInDiscardPile = GetAllCardsInDiscardPile();
		DiscardPileCountChanged(-cardsInDiscardPile.Count);
		DrawPileCountChanged(cardsInDiscardPile.Count);
		SoundManager.instance.PlayShuffleSound();
		for(int card = 0; card < cardsInDiscardPile.Count; card++)
		{
			cardsInDiscardPile[card].transform.SetParent(drawPile.drawPileParent);
		}
		lowOnCards = false;
		deckBackdropImage.color = Color.red;
		for(int card = 0; card < cardsInDiscardPile.Count; card++)
		{
			cardsInDiscardPile[card].StartMove(0.25f, new Vector2(22.5f, 22.5f), new Vector3(0,0,0));
			cardsInDiscardPile[card].cardLocation = 1;
			cardsInDiscardPile[card].deckViewerClone.cardLocation = 1;
			yield return new WaitForSeconds(0.7f / cardsInDiscardPile.Count / GameOptions.instance.gameSpeedFactor);
		}
	}
	
	public void DiscardCardsInHand()
	{
		SoundManager.instance.PlayCardSlideSound();
		underMouseCard.DestroyTempCard();
		List<CardScript> cardsToDiscard = GetAllCardsInHand();
		DiscardPileCountChanged(cardsToDiscard.Count);
		int numberOfAces = 0;
		List<Vector2> locationsOfAces = new List<Vector2>();		
		for(int card = 0; card < cardsToDiscard.Count; card++)
		{
			if(cardsToDiscard[card].rankInt == 12 && aceDiscardBaubleOwned)
			{
				numberOfAces++;
				locationsOfAces.Add(cardsToDiscard[card].rt.anchoredPosition);
			}
			cardsToDiscard[card].cardLocation = 3;
			cardsToDiscard[card].deckViewerClone.cardLocation = 3;
			cardsToDiscard[card].canMove = false;
			cardsToDiscard[card].transform.SetParent(discardParent);
			cardsToDiscard[card].StartMove(0.25f, new Vector2(22.5f, 22.5f), new Vector3(0,0,0));
			if(!cardsToDiscard[card].faceDown)
			{
				StartCoroutine(cardsToDiscard[card].Flip(0.25f));
			}
			if(!cardsToDiscard[card].noImpactOnHandSize)
			{
				cardsInHand--;
			}
			visualCardsInHand--;
		}
		if(numberOfAces >= 2 && aceDiscardBaubleOwned)
		{
			handValues.baubleNotifications.Notify(20);
			handValues.menuButton.ChangeDisabled(true);
			//for(int i = 0; i < numberOfAces; i++)
			for(int i = 0; i < 2; i++)
			{
				scoreVial.SpawnPokerChip(locationsOfAces[i] + new Vector2(95, -125), scoreVial.movingChipParent);
			}
		}
	}
	
	public void DiscardPileCountChanged(int change)
	{
		if(change > 0)
		{
			Statistics.instance.currentRun.cardsDiscarded += change;
		}
		if(deckPreview.timeMouseOver > 0 && deckPreview.overWhichDeck == 2)
		{
			deckPreview.GenerateDeckPreview(discardParent, 2);
		}
		if(deckViewer.isOpen && deckViewer.curType == 2)
		{
			deckViewer.CreateDeckViewFromType(2);
		}
		int cardsInDiscardPile = int.Parse(discardPileTexts[0].text);
		cardsInDiscardPile += change;
		for(int text = 0; text < discardPileTexts.Length; text++)
		{
			discardPileTexts[text].text = "" + cardsInDiscardPile;
		}
	}
	
	
	public void DrawPileCountChanged(int change)
	{
		if(deckPreview.timeMouseOver > 0 && deckPreview.overWhichDeck == 1)
		{
			deckPreview.GenerateDeckPreview(drawPile.drawPileParent, 1);
		}
		if(deckViewer.isOpen && deckViewer.curType == 0)
		{
			deckViewer.CreateDeckViewFromType(0);
		}
		int cardsInDrawPile = int.Parse(drawPileTexts[0].text);
		cardsInDrawPile += change;
		for(int text = 0; text < drawPileTexts.Length; text++)
		{
			drawPileTexts[text].text = "" + cardsInDrawPile;
		}
	}
	
	public IEnumerator StartGame(float waitTime, bool startInShop)
	{
		float t = 0;
		while(t < waitTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		NewGameAnimation.instance.newGameAnimationStage = 2;
		if(startInShop)
		{
			ScoreVial.instance.StartCoroutine(ScoreVial.instance.AnteCompleted(false, true));
		}
		else
		{
			StartCoroutine(DrawCards(maxHandSize - cardsInHand));
		}
	}
	
	void Start()
	{
		dataText = GameObject.FindWithTag("DataText").GetComponent<TMP_Text>(); //disable for release
	}

    void Update()
    {
		if(lowOnCards)
		{
			deckBackdropIntensity = Mathf.PingPong(Time.time, 1f);
			Color deckColor = deckBackdropImage.color;
			deckColor.r = 0.5f + deckBackdropIntensity * 0.5f;
			deckBackdropImage.color = deckColor;
		}
		if(handValues.gameOptions.cheatsOn)
		{
			if(Input.GetKeyDown(KeyCode.Z))
			{
				StartCoroutine(DrawCards(7));
			}
		}
		//handData.text = "VCIH= " + visualCardsInHand + "\n"+cardsInHand+"/"+maxHandSize + " ST="+alwaysSortType;
    }
}
