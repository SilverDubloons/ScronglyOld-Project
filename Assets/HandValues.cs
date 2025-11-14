using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class HandValues : MonoBehaviour
{
/*  public float highCardBaseValue; 			// 0
	public float highCardCardMultiplier
	public float onePairBaseValue;				// 1
	public float onePairCardMultiplier;
	public float twoPairBaseValue;				// 2
	public float twoPairCardMultiplier;
	public float threeOfAKindBaseValue;			// 3
	public float threeOfAKindCardMultiplier;
	public float straightBaseValue;				// 4
	public float straightCardMultiplier;
	public float flushBaseValue;				// 5
	public float flushCardMultiplier;
	public float fullHouseBaseValue;			// 6
	public float fullHouseCardMultiplier;
	public float fourOfAKindBaseValue;			// 7
	public float fourOfAKindCardMultiplier;
	public float straightFlushBaseValue;		// 8
	public float straightFlushCardMultiplier;
	public float fiveOfAKindBaseValue;			// 9
	public float fiveOfAKindCardMultiplier;
	public float flushHouseBaseValue;			// 10
	public float flushHouseCardMultiplier;
	public float flushFourBaseValue;			// 11
	public float flushFourCardMultiplier;
	public float flushFiveBaseValue;			// 12
	public float flushFiveCardMultiplier; */
	
	public static HandValues instance;
	public RectTransform rt;
	public float[] handBaseValues;
	public float[] handCardMultipliers;
	
	public float[] handPointsStarting;
	public float[] handMultStarting;
	
	public float[] handPointsPerLevel;
	public float[] handMultPerLevel;
	
	public string[] handNames;
	public string[] handDescriptions;
	public string[] numberStrings;
	public string[] suitNames;
	
	public int cardsNeededToMakeAFlush = 5;
	public int cardsNeededToMakeAStraight = 5;
	public int cardsNeededToMakeAStraightFlush = 5;
	public bool straightsCanWrap = false;
	public bool straightFlushesCanWrap = false;
	public int maxGapInStraights = 0;
	public int maxGapInStraightFlushes = 0;
	public HandZone[] handZones;
	public HandScript handScript;
	//public DeckScript deckScript;
	public int suitQuantityNeededForFlushFive = 5;
	public int suitQuantityNeededForFlushFour = 4;
	public int suitQuantityNeededForFlushHouse = 5;
	
	//public TMP_Text dataText;
	public Sprite lockedSprite;
	public Sprite unlockedSprite;
	
	public int handZonesActive;
	public ScoreVial scoreVial;
	
	public float[] antes;
	public int currentAnte = 0;
	
	public int handsUntilFatiguePerAnte;
	public int handsLeftUntilFatigue;
	public TMP_Text[] handsUntilFatigueTexts;
	public Image handsUntilFatigueImage;
	public Color[] fatigueColors; // 0 = base; 1 = 1 hand warning 2 = in fatigue
	public Color baseValueColor;
	public Color multiplierColor;
	public GameObject[] fatigueObjects; // 0 & 1 are the "hands until fatigue" label and the number of hands. 2 is the fatigued label
	public BaubleScript baubleScript;
	public BaubleNotifications baubleNotifications;
	public GameOptions gameOptions;
	public GameObject bombExplosionPrefab;
	public GameObject rainbowPaintPrefab;
	public MovingButton menuButton;
	//public Statistics statistics;
	public MainMenu mainMenu;
	public RandomNumbers randomNumbers;
	public GameObject promotionAnimationPrefab;
	public GameObject magicMarkerAnimationPrefab;
	public Transform snurfProjectileParent;
	public GameObject snurfProjectilePrefab;
	public GameObject magicMirrorAnimationPrefab;
	public GameObject dissolveCardPrefab;
	public bool destroyAllPlacedCards;
	public bool handsHavePreplacedCard;
	public float chanceForPreplacedCardToBeNonstandard;
	
	public float GetTotalBaseValue(bool[] handsContained)
	{
		float totalValue = 0;
		for(int i = 0; i < handsContained.Length; i++)
		{
			if(handsContained[i])
			{
				totalValue += handBaseValues[i];
			}
		}
		return totalValue;
	}
	
	public float GetTotalMult(bool[] handsContained)
	{
		float totalMult = 0;
		for(int i = 0; i < handsContained.Length; i++)
		{
			if(handsContained[i])
			{
				totalMult += handCardMultipliers[i];
			}
		}
		return totalMult;
	}
	
	void Awake()
	{
		instance = this;
	}
	
	public void CreateDissolvingCard(CardScript cardScript)
	{
		//print("dissolving " + cardScript.name);
		GameObject newDissolveCard = Instantiate(dissolveCardPrefab, new Vector3(0,0,0), Quaternion.identity, cardScript.transform.parent);
		DissolveCardScript dissolveCardScript = newDissolveCard.GetComponent<DissolveCardScript>();
		dissolveCardScript.CopyCard(cardScript);
	}
	
	public int GetNumberOfEmptyDropZones()
	{
		return handZones[0].GetNumberOfEmptySpaces();
	}
	
	public float[] SetupAntes(int difficulty = 0)
	{
		antes = new float[50];
		antes[0] = 300;
		antes[1] = 500;
		antes[2] = 750;
		if(difficulty == 0)
		{
			bool increaseType = true;
			for(int i = 2; i < 30; i++)
			{
				string numberString = antes[i - 1].ToString("0.##");
				if(numberString.Contains("e+"))
				{
					int index = numberString.IndexOf("e+");
					numberString = numberString.Substring(0, index);
				}
				if(numberString.StartsWith("64"))
				{
					antes[i] = Mathf.Round(antes[i - 1] * 1.5625f);
					increaseType = true;
				}
				else
				{
					if(increaseType)
					{
						antes[i] = Mathf.Round(antes[i - 1] * 1.5f);
						increaseType = false;
					}
					else
					{
						antes[i] = Mathf.Round(antes[i - 1] * 2f / 1.5f);
						increaseType = true;
					}
				}
			}
		}
		else
		{
			switch(difficulty)
			{
				case 1:
				antes[29] = 20000000f;
				break;
				case 2:
				antes[29] = 50000000f;
				break;
				case 3:
				antes[29] = 100000000f;
				break;
				case 4:
				antes[29] = 250000000f;
				break;
				case 5:
				antes[29] = 1000000000f;
				break;
			}
			float factor = Mathf.Pow(antes[29] / antes[2], 1 / 27f);
			for(int i = 3; i < 29; i++)
			{
				antes[i] = antes[i - 1] * factor;
				//print(antes[i].ToString() + " has " + numberOfDigits + " digits");
			}
			for(int i = 3; i < 29; i++)
			{
				int numberOfDigits = (int)Mathf.Floor(Mathf.Log10(antes[i])) + 1;
				int roundingNumber = Mathf.RoundToInt(Mathf.Pow(10, (numberOfDigits - 3)));
				if(roundingNumber == 10)
				{
					roundingNumber = 50;
				}
				else if(roundingNumber == 100)
				{
					roundingNumber = 500;
				}
				antes[i] = Mathf.Round(antes[i] / roundingNumber) * roundingNumber;
			}
		}
		for(int i = 30; i < 35; i++)
		{
			antes[i] = Mathf.Round(antes[i - 1] * 2f);
		}
		for(int i = 35; i < 40; i++)
		{
			antes[i] = Mathf.Round(antes[i - 1] * 5f);
		}
		for(int i = 40; i < 45; i++)
		{
			antes[i] = Mathf.Round(antes[i - 1] * 10f);
		}
		for(int i = 45; i < 50; i++)
		{
			antes[i] = Mathf.Round(antes[i - 1] * 100f);
		}
		return antes;
	}
	
	public void ResetGame()
	{
		for(int i = 0; i < handZones.Length; i++)
		{
			for(int j = 0; j < handZones[i].dropZones.Length; j++)
			{
				if(handZones[i].dropZones[j].cardPlaced)
				{
					Destroy(handZones[i].dropZones[j].placedCardScript.gameObject);
					handZones[i].dropZones[j].CardRemoved(false);
				}
			}
			handZones[i].HandUpdated();
		}
		ResetHandZones();
		for(int i = 0; i < handScript.handParent.childCount; i++)
		{
			Destroy(handScript.handParent.GetChild(i).gameObject);
		}
		handScript.cardsInHand = 0;
		handScript.visualCardsInHand = 0;
		handScript.DrawPileCountChanged(-handScript.drawPile.drawPileParent.childCount);
		handScript.selectedCards.Clear();
		for(int i = 0; i < handScript.drawPile.drawPileParent.childCount; i++)
		{
			Destroy(handScript.drawPile.drawPileParent.GetChild(i).gameObject);
		}
		handScript.DiscardPileCountChanged(-handScript.discardParent.childCount);
		for(int i = 0; i < handScript.discardParent.childCount; i++)
		{
			Destroy(handScript.discardParent.GetChild(i).gameObject);
		}
		baubleScript.handsInformation.DestroyHandValueLabels();
		baubleScript.handsInformation.CreateHandValueLabels();
		baubleScript.handsInformation.ReorganizeValueLabels();
		baubleScript.AssignBaubleNumbers();
		for(int i = 0; i < baubleScript.baubles.Length; i++)
		{
			baubleScript.baubles[i].quantityOwned = 0;
			baubleScript.baubles[i].numberOnSale = 0;
		}
		baubleScript.baublesByRarity.Clear();
		//baubleScript.SortBaublesByRarity();
		baubleScript.SetupBaubles();
		if(ShopScript.instance.layawayBuyScript != null)
		{
			if(ShopScript.instance.layawayBuyScript.itemType == 0)
			{
				Destroy(ShopScript.instance.layawayBuyScript.cardScript.gameObject);
			}
			if(ShopScript.instance.layawayBuyScript.itemType == 1)
			{
				//baubleScript.baubles[ShopScript.instance.layawayBuyScript.baubleNumber].numberOnSale--;
				Destroy(ShopScript.instance.layawayBuyScript.baubleItem.gameObject);
			}
			ShopScript.instance.buyScripts.Remove(ShopScript.instance.layawayBuyScript);
			ShopScript.instance.layawayBuyScript = null;
			ShopScript.instance.layawayImage.raycastTarget = true;
			ShopScript.instance.layawayText.SetActive(true);
		}
		for(int i = 0; i < ShopScript.instance.interestParent.childCount; i++)
		{
			Destroy(ShopScript.instance.interestParent.GetChild(i).gameObject);
		}
	}
	
	public void UpdateHandsUntilFatigue(int change, bool reset)
	{
		if(reset)
		{
			handsLeftUntilFatigue = handsUntilFatiguePerAnte;
		}
		else
		{
			handsLeftUntilFatigue += change;
		}
		if(handsLeftUntilFatigue > 1)
		{
			fatigueObjects[0].SetActive(true);
			fatigueObjects[1].SetActive(true);
			fatigueObjects[2].SetActive(false);
			handsUntilFatigueImage.color = fatigueColors[0];
		}
		else if(handsLeftUntilFatigue == 1)
		{
			fatigueObjects[0].SetActive(true);
			fatigueObjects[1].SetActive(true);
			fatigueObjects[2].SetActive(false);
			handsUntilFatigueImage.color = fatigueColors[1];
		}
		else
		{
			fatigueObjects[0].SetActive(false);
			fatigueObjects[1].SetActive(false);
			fatigueObjects[2].SetActive(true);
			handsUntilFatigueImage.color = fatigueColors[2];
		}
		for(int i = 0; i < handsUntilFatigueTexts.Length; i++)
		{
			handsUntilFatigueTexts[i].text = "" + handsLeftUntilFatigue;
		}
	}
	
	public void ScoreEntireHand()
	{
		for(int i = 0; i < handZones.Length; i++)	// This might cause issues when introducing second handzone on account of the delay
		{
			if(handZones[i].handZoneActive)
			{
				if(!handZones[i].locked)
				{
					return;
				}
				if(!handZones[i].finishedAnimating)
				{
					return;
				}
			}
		}
		float totalScore = 0;
		if(handZonesActive == 1)
		{
			totalScore = handZones[0].currentBaseValue;
		}
		Statistics.instance.currentRun.handsPlayed++;
		Statistics.instance.currentRun.handTypesPlayed[handZones[0].currentHandTier]++;
		if(totalScore > Statistics.instance.currentRun.bestHandScore)
		{
			Statistics.instance.currentRun.bestHandScore = totalScore;
			Statistics.instance.currentRun.bestHandName = handZones[0].handNameTexts[0].text;
			Statistics.instance.UpdateBestHand(handZones[0].GetAllCardsInHandZone(), Statistics.instance.currentRun);
		}
		
		handZones[0].DiscardAllCards();
		SoundManager.instance.PlayCardSlideSound();
		scoreVial.ScoreUpdated(totalScore, true, false);
		Statistics.instance.currentRun.scoreInFinalAnte = scoreVial.currentScore;
		if(scoreVial.currentScore < scoreVial.scoreTarget )
		{
			ResetHandZones();
			if(handScript.drawPile.drawPileParent.childCount == 0 && handScript.cardsInHand == 0)
			{
				StartCoroutine(ShowGameStats(true, false, false));
			}
			else
			{
				handScript.DrawToFullHand();
			}
			if(!GameOptions.instance.tutorialDone)
			{
				if(Tutorial.instance.tutorialStage == 1)
				{
					Tutorial.instance.AdvanceTutorial();
				}
				if(Tutorial.instance.tutorialStage == 2)
				{
					Tutorial.instance.AdvanceTutorial();
				}
				if(Tutorial.instance.tutorialStage == 3)
				{
					Tutorial.instance.AdvanceTutorial();
				}
			}
		}
		else
		{
			if(handScript.numberOfTopCardsVisible > 0)
			{
				handScript.TurnAllCardsInDrawPileFaceDown();
			}
			if(!GameOptions.instance.tutorialDone)
			{
				if(Tutorial.instance.tutorialStage == 1)
				{
					Tutorial.instance.AdvanceTutorial();
				}
				if(Tutorial.instance.tutorialStage == 2)
				{
					Tutorial.instance.AdvanceTutorial();
				}
				if(Tutorial.instance.tutorialStage == 3)
				{
					Tutorial.instance.AdvanceTutorial();
				}
				if(Tutorial.instance.tutorialStage == 4)
				{
					Tutorial.instance.AdvanceTutorial();
				}
				if(Tutorial.instance.tutorialStage == 5)
				{
					Tutorial.instance.AdvanceTutorial();
				}
				if(Tutorial.instance.tutorialStage == 6)
				{
					Tutorial.instance.AdvanceTutorial();
				}
			}
		}
	}
	
	public IEnumerator ShowGameStats(bool gameFailed, bool gameCompleted, bool endlessCompleted)
	{
		yield return new WaitForSeconds(3 / gameOptions.gameSpeedFactor);
		//print("ante failed!");
		if(gameFailed)
		{
			SoundManager.instance.PlayGameOverSound();
		}
		Statistics.instance.CheckForBestRun();
		if(gameCompleted)
		{
			Statistics.instance.RevealEndlessModeButton();
		}
		else
		{
			Statistics.instance.HideEndlessModeButton();
		}
		if(!GameOptions.instance.tutorialDone)
		{
			GameOptions.instance.ResetTutorial();
		}
		if(Statistics.instance.currentRun.runIsDailyGame)
		{
			Statistics.instance.UpdateCurrentDailyRun(Statistics.instance.currentRun);
		}
		StartCoroutine(scoreVial.MoveOverTime(rt, rt.anchoredPosition, new Vector3(0, 360, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		StartCoroutine(scoreVial.MoveOverTime(handScript.baseRT, handScript.baseRT.anchoredPosition, new Vector3(95, -205, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		Statistics.instance.lookingThroughDailies = false;
		Statistics.instance.gameStatsRT.gameObject.SetActive(true);
		Statistics.instance.mainMenuButton.ChangeDisabled(true);
		if(Statistics.instance.currentRun.runIsDailyGame)
		{
			if(gameFailed)
			{
				Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Daily Game Over!\n" + Statistics.instance.currentRun.dateRunTookPlace);
			}
			else if(endlessCompleted)
			{
				Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Daily Game Endless Victory! You've done the Impossible!\n" + Statistics.instance.currentRun.dateRunTookPlace);
			}
			else if(gameCompleted)
			{
				Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Daily Game Completed!\n" + Statistics.instance.currentRun.dateRunTookPlace);
			}
		}
		else
		{
			if(Statistics.instance.currentRun.runIsSeededGame)
			{
				if(gameFailed)
				{
					Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Seeded Game Over!");
				}
				else if(endlessCompleted)
				{
					Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Seeded Game Endless Victory! Now do it Unseeded ;)");
				}
				else if(gameCompleted)
				{
					Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Seeded Game Completed!");
				}
			}
			else
			{
				if(gameFailed)
				{
					Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Game Over!");
				}
				else if(endlessCompleted)
				{
					Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Endless game vicory! You've done the Impossible!");
				}
				else if(gameCompleted)
				{
					if(DeckViewer.instance.deckInUse == 3)
					{
						Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Sic semper tyrannis!");
					}
					else
					{
						Statistics.instance.PopulateGameStats(Statistics.instance.currentRun, "Game Completed!");
					}
				}
			}
		}
		//mainMenu.StartCoroutine(mainMenu.MoveOverTime(statistics.gameStatsRT, new Vector3(98, -360, 0), new Vector3(98, 0, 0), 1, 0, mainMenu.EnableStatsButtons));
		Statistics.instance.mainMenuButton.ChangeDisabled(false);
		Statistics.instance.DisableOtherStatsButtons();
		StartCoroutine(scoreVial.MoveOverTime(Statistics.instance.gameStatsRT, new Vector3(98, -360, 0), new Vector3(98, 0, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		
	}
	
	public void ResetHandZones()
	{
		for(int i = 0; i < handZones.Length; i++)
		{
			handZones[i].ResetHandZone();
		}
	}
	
	public bool AnyHandZonesAreLocked()
	{
		for(int i = 0; i < handZones.Length; i++)
		{
			if(handZones[i].locked)
			{
				return true;
			}
		}
		return false;
	}
	
	public bool AnyHandZonesAreAnimating()
	{
		for(int i = 0; i < handZones.Length; i++)
		{
			if(!handZones[i].finishedAnimating)
			{
				return true;
			}
		}
		return false;
	}
	
	public string ConvertFloatToString(float f)
	{
		string prefix = "";
		if(f < 0)
		{
			prefix = "-";
		}
		f = Mathf.Abs(f);
		string suffix = "";
		string formattedNumber = "";
		if(f >= 1000000000000000)
		{
			suffix = "e" + (Mathf.Floor(Mathf.Log10(f) / 3) * 3).ToString();
			float exponentNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3));
			if(exponentNumber > 100)
			{
				formattedNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3)).ToString("0");
			}
			else if(exponentNumber > 10)
			{
				formattedNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3)).ToString("0.#");
			}
			else
			{
				formattedNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3)).ToString("0.##");
			}
		}
		else if(f >= 100000000000000)
		{
			formattedNumber = (f/1000000000000f).ToString("0");
			suffix = "T";
		}
		else if(f >= 10000000000000)
		{
			formattedNumber = (f/1000000000000f).ToString("0.#");
			suffix = "T";
		}
		else if(f >= 1000000000000)
		{
			formattedNumber = (f/1000000000000f).ToString("0.##");
			suffix = "T";
		}
		else if(f >= 100000000000)
		{
			formattedNumber = (f/1000000000f).ToString("0");
			suffix = "B";
		}
		else if(f >= 10000000000)
		{
			formattedNumber = (f/1000000000f).ToString("0.#");
			suffix = "B";
		}
		else if(f >= 1000000000)
		{
			formattedNumber = (f/1000000000f).ToString("0.##");
			suffix = "B";
		}
		else if(f >= 100000000)
		{
			formattedNumber = (f/1000000f).ToString("0");
			suffix = "M";
		}
		else if(f >= 10000000)
		{
			formattedNumber = (f/1000000f).ToString("0.#");
			suffix = "M";
		}
		else if(f >= 1000000)
		{
			formattedNumber = (f/1000000f).ToString("0.##");
			suffix = "M";
		}
		else if(f >= 100000)
		{
			formattedNumber = (f/1000f).ToString("0");
			suffix = "K";
		}
		else if(f > 100)
		{
			formattedNumber = f.ToString("0");
		}
		else if(f > 10)
		{
			formattedNumber = f.ToString("0.#");
		}
		else if(f > 1)
		{
			formattedNumber = f.ToString("0.##");
		}
		else
		{
			formattedNumber = f.ToString("0.###");
		}
		return prefix + formattedNumber + "" + suffix; // at < 100e longest string output is 7 digits ex 3.14e18
	}
	
	public void ChangeAllXImagesToGreen()
	{
		for(int i = 0; i < handZones.Length; i++)
		{
			handZones[i].ChangeXImagesToGreen();
		}
	}
	
	public void ChangeAllXImagesToRed()
	{
		for(int i = 0; i < handZones.Length; i++)
		{
			handZones[i].ChangeXImagesToRed();
		}
	}
	
	public IEnumerator ExpandContract(List<RectTransform> objects, float animationTime, Vector3 endingScale)
	{
		Vector3 startingScale = new Vector3(1f, 1f, 1f);
		animationTime = animationTime / gameOptions.gameSpeedFactor;
		float t = 0;
		while(t < animationTime)
		{
			t += Time.deltaTime;
			for(int i = 0; i < objects.Count; i++)
			{
				objects[i].localScale = Vector3.Lerp(startingScale, endingScale, t / animationTime);
			}
			yield return null;
		}
		t = 0;
		while(t < animationTime)
		{
			t += Time.deltaTime;
			for(int i = 0; i < objects.Count; i++)
			{
				objects[i].localScale = Vector3.Lerp(endingScale, startingScale, t / animationTime);
			}
			yield return null;
		}
		for(int i = 0; i < objects.Count; i++)
		{
			objects[i].localScale = startingScale;
		}
	}
	
	public bool IsThereAPlacedCard()
	{
		for(int hz = 0; hz < handZones.Length; hz++)
		{
			for(int dz = 0; dz < handZones[hz].dropZones.Length; dz++)
			{
				if(handZones[hz].dropZones[dz].cardPlaced)
				{
					if(handZones[hz].dropZones[dz].placedCardScript.canMove)
					{
						return true;
					}
				}
			}
		}
		return false;
	}
	
	public void RecallAllPlacedCards()
	{
		for(int hz = 0; hz < handZones.Length; hz++)
		{
			for(int dz = handZones[hz].dropZones.Length - 1; dz >= 0; dz--)
			{
				if(handZones[hz].dropZones[dz].cardPlaced)
				{
					if(handZones[hz].dropZones[dz].placedCardScript.canMove)
					{
						handZones[hz].dropZones[dz].placedCardScript.dropZonePlacedIn = null;
						handZones[hz].dropZones[dz].placedCardScript.RevertToOriginalImage();
						handZones[hz].dropZones[dz].placedCardScript.transform.SetParent(handScript.handParent);
						handZones[hz].dropZones[dz].placedCardScript.transform.SetSiblingIndex(0);
						handZones[hz].dropZones[dz].CardRemoved(false);
						handScript.visualCardsInHand++;
					//print("increasing VCIH hz= " +hz+ " dz= " + dz);
					}
				}
			}
			handZones[hz].HandUpdated();
		}
		SoundManager.instance.PlayCardSlideSound();
		handScript.recallButton.ChangeDisabled(true);
		handScript.ReorganizeHand();
	}
	
	public void EvaluateHand(List<CardScript> hand, HandZone handZone, bool evaluatingOnlyCardsUsed)
	{
		//dataText.text = "";
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
		/* dataText.text = dataText.text + "cards= ";
		for(int card = 0; card < cardsInHand; card++)
		{
			dataText.text = dataText.text + hand[card].GetCardNameCharSuit() + " ";
		}
		dataText.text = dataText.text + "\n"; */
	
		bool fiveOfAKind = false; // always all five cards
		bool flushFive = false;
		if(cardsInHand >= 5)
		{
			if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt)
			{
				fiveOfAKind = true;
				for(int suit = 0; suit < 4; suit++)
				{
					if(numberOfEachSuit[4] + numberOfEachSuit[suit] >= suitQuantityNeededForFlushFive)
					{
						flushFive = true;
						//handZone.HandEvaluated(hand, "Flush Five", 12);
						//return;
					}
				}
			}
		}
		
		bool fourOfAKind = false;
		//bool flushFour = false;
		List<CardScript> fourOfAKindCards = new List<CardScript>();
		
		if(cardsInHand >= 4)
		{
			if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt)
			{
				fourOfAKind = true;
				for(int card = 0; card < 4; card++)
				{
					fourOfAKindCards.Add(hand[card]);
				}
			}
			else // if the first four are 4oaK, no need to check last four. If they are all the same too, then it's 5oaK
			{
				if(cardsInHand >= 5)
				{
					if(hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt)
					{
						fourOfAKind = true;
						for(int card = 1; card < 5; card++)
						{
							fourOfAKindCards.Add(hand[card]);
						}
					}
				}
			}
			/* if(fourOfAKind)
			{
				if(HandIsFlush(fourOfAKindCards, suitQuantityNeededForFlushFour))
				{
					flushFour = true;
					//handZone.HandEvaluated(fourOfAKindCards, "Flush Four", 11);
					//return;
				}
			} */
		}
		
		bool fullHouse = false;
		//bool flushHouse = false;
		
		if(cardsInHand >= 5)
		{
			if(((hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt) && hand[3].rankInt == hand[4].rankInt) || (hand[0].rankInt == hand[1].rankInt && (hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt)) && !fiveOfAKind)
			{
				fullHouse = true;
				if(HandIsFlush(hand, suitQuantityNeededForFlushHouse))
				{
					//flushHouse = true;
					//handZone.HandEvaluated(hand, "Flush House", 10);
					//return;
				}
			}
		}
		if(fiveOfAKind)
		{
			//handZone.HandEvaluated(hand, "Five of a Kind", 9);
			//return;
		}
		
		bool flush = false;
		bool straightFlush = false;
		bool royalFlush = false;
		List<CardScript> flushCards = new List<CardScript>();
		List<CardScript> straightFlushCards = new List<CardScript>();
		if(cardsInHand >= cardsNeededToMakeAFlush || cardsInHand >= cardsNeededToMakeAStraightFlush)
		{
			List<int> foundFlushes = new List<int>();
			List<int> foundPossibleStraightFlushes = new List<int>();
			for(int suit = 0; suit < 4; suit++)
			{
				if(numberOfEachSuit[suit] + numberOfEachSuit[4] >= cardsNeededToMakeAFlush)
				{
					flush = true;
					foundFlushes.Add(suit);
				}
				if(numberOfEachSuit[suit] + numberOfEachSuit[4] >= cardsNeededToMakeAStraightFlush)
				{
					foundPossibleStraightFlushes.Add(suit);
				}
			}
			/* dataText.text = dataText.text + "flush= " + flush + " ";
			for(int i = 0; i < numberOfEachSuit.Length; i++)
			{
				dataText.text = dataText.text + numberOfEachSuit[i] + " ";
			}
			dataText.text = dataText.text + "\n"; */
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
					/* for(int i = 0; i < flushes.Count; i++)
					{
						dataText.text = dataText.text + "flush " + i + "= ";
						for(int j = 0; j < flushes[i].Count; j++)
						{
							dataText.text = dataText.text + flushes[i][j].GetCardNameCharSuit() + " ";
						}
						dataText.text = dataText.text + "\n";
					} */
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
				List<CardScript> straightFlushCardsToCheck = DoesHandContainStraight(flushToCheck, maxGapInStraightFlushes, cardsNeededToMakeAStraightFlush, straightFlushesCanWrap);
				if(straightFlushCardsToCheck != null)
				{
					straightFlush = true;
					straightFlushes.Add(straightFlushCardsToCheck);
				}
			}
			if(straightFlush)
			{
				straightFlushes.Sort(new StraightComparer());
				for(int i = 0; i < straightFlushes.Count; i++)
				{
					/* dataText.text = dataText.text + "straight flush " + i + "= ";
					for(int j = 0; j < straightFlushes[i].Count; j++)
					{
						dataText.text = dataText.text + straightFlushes[i][j].GetCardNameCharSuit() + " ";
					}
					dataText.text = dataText.text + "\n"; */
				}
				if(straightFlushes[0].Count == 5)
				{
					if(straightFlushes[0][0].rankInt == 8 && straightFlushes[0][1].rankInt == 9 && straightFlushes[0][2].rankInt == 10 && straightFlushes[0][3].rankInt == 11 && straightFlushes[0][4].rankInt == 12)
					{
						royalFlush = true;
						//handZone.HandEvaluated(straightFlushes[0], "Royal Flush", 8);
						//return;
					}
				}
				straightFlushCards = straightFlushes[0];
				//handZone.HandEvaluated(straightFlushes[0], "Straight Flush", 8);
				//return;
			}
		}
		if(fourOfAKind)
		{
			//handZone.HandEvaluated(fourOfAKindCards, "Four of a Kind", 7);
			//return;
		}
		if(fullHouse)
		{
			//handZone.HandEvaluated(hand, "Full House", 6);
			//return;
		}
		if(flush)
		{
			//handZone.HandEvaluated(flushCards, "Flush", 5);
			//return;
		}
		bool straight = false;
		List<CardScript> straightCards = DoesHandContainStraight(hand, maxGapInStraights, cardsNeededToMakeAStraight, straightsCanWrap);
		if(straightCards != null)
		{
			straight = true;
			/* dataText.text = dataText.text + "straight= " + straight + " ";
			for(int i = 0; i < straightCards.Count; i++)
			{
				dataText.text = dataText.text + straightCards[i].GetCardNameCharSuit() + " ";
			}
			dataText.text = dataText.text + "\n"; */
			//handZone.HandEvaluated(straightCards, "Straight", 4);
			//return;
		}
		bool threeOfAKind = false;
		List<CardScript> threeOfAKindCards = new List<CardScript>();
		if(cardsInHand >= 3)
		{
			List<CardScript> threeOfAKindCheck = new List<CardScript>();
			if(hand[0].rankInt == hand[1].rankInt && hand[1].rankInt == hand[2].rankInt)
			{
				for(int card = 0; card < 3; card++)
				{
					threeOfAKindCheck.Add(hand[card]);
				}
				threeOfAKind = true;
				threeOfAKindCards = threeOfAKindCheck;
				//handZone.HandEvaluated(threeOfAKindCards, "Three of a Kind", 3);
				//return;
			}
			if(cardsInHand >= 4)
			{
				if(hand[1].rankInt == hand[2].rankInt && hand[2].rankInt == hand[3].rankInt)
				{
					for(int card = 1; card < 4; card++)
					{
						threeOfAKindCheck.Add(hand[card]);
					}
					threeOfAKind = true;
					threeOfAKindCards = threeOfAKindCheck;
					//handZone.HandEvaluated(threeOfAKindCards, "Three of a Kind", 3);
					//return;
				}
				if(cardsInHand >= 5)
				{
					if(hand[2].rankInt == hand[3].rankInt && hand[3].rankInt == hand[4].rankInt)
					{
						for(int card = 2; card < 5; card++)
						{
							threeOfAKindCheck.Add(hand[card]);
						}
						threeOfAKind = true;
						threeOfAKindCards = threeOfAKindCheck;
						//handZone.HandEvaluated(threeOfAKindCards, "Three of a Kind", 3);
						//return;
					}
				}
			}
		}
		bool twoPair = false;
		List<CardScript> twoPairCards = new List<CardScript>();
		if(cardsInHand >= 4)
		{
			List<CardScript> twoPairCheck = new List<CardScript>();
			if(hand[0].rankInt == hand[1].rankInt && hand[2].rankInt == hand[3].rankInt && hand[0].rankInt != hand[2].rankInt)
			{
				for(int card = 0; card < 4; card++)
				{
					twoPairCheck.Add(hand[card]);
				}
				twoPair = true;
				twoPairCards = twoPairCheck;
				//handZone.HandEvaluated(twoPairCards, "Two Pair", 2);
				//return;
			}
			if(cardsInHand >= 5)
			{
				if(hand[0].rankInt == hand[1].rankInt && hand[3].rankInt == hand[4].rankInt && hand[0].rankInt != hand[3].rankInt)
				{
					twoPairCheck.Add(hand[0]);
					twoPairCheck.Add(hand[1]);
					twoPairCheck.Add(hand[3]);
					twoPairCheck.Add(hand[4]);
					twoPair = true;
					twoPairCards = twoPairCheck;
					//handZone.HandEvaluated(twoPairCards, "Two Pair", 2);
					//return;
				}
				
				if(hand[1].rankInt == hand[2].rankInt && hand[3].rankInt == hand[4].rankInt && hand[1].rankInt != hand[3].rankInt)
				{
					for(int card = 1; card < 5; card++)
					{
						twoPairCheck.Add(hand[card]);
					}
					twoPair = true;
					twoPairCards = twoPairCheck;
					//handZone.HandEvaluated(twoPairCards, "Two Pair", 2);
					//return;
				}
			}
		}
		bool onePair = false;
		List<CardScript> onePairCards = new List<CardScript>();
		if(cardsInHand >= 2)
		{
			List<CardScript> onePairCheck = new List<CardScript>();
			if(hand[0].rankInt == hand[1].rankInt)
			{
				onePairCheck.Add(hand[0]);
				onePairCheck.Add(hand[1]);
				onePair = true;
				onePairCards = onePairCheck;
				//handZone.HandEvaluated(onePairCards, "One Pair", 1);
				//return;
			}
			if(cardsInHand >= 3)
			{
				if(hand[1].rankInt == hand[2].rankInt)
				{
					onePairCheck.Add(hand[1]);
					onePairCheck.Add(hand[2]);
					onePair = true;
					onePairCards = onePairCheck;
					//handZone.HandEvaluated(onePairCards, "One Pair", 1);
					//return;
				}
				if(cardsInHand >= 4)
				{
					if(hand[2].rankInt == hand[3].rankInt)
					{
						onePairCheck.Add(hand[2]);
						onePairCheck.Add(hand[3]);
						onePair = true;
						onePairCards = onePairCheck;
						//handZone.HandEvaluated(onePairCards, "One Pair", 1);
						//return;
					}
					if(cardsInHand >= 5)
					{
						if(hand[3].rankInt == hand[4].rankInt)
						{
							onePairCheck.Add(hand[3]);
							onePairCheck.Add(hand[4]);
							onePair = true;
							onePairCards = onePairCheck;
							//handZone.HandEvaluated(onePairCards, "One Pair", 1);
							//return;
						}
					}
				}
			}
		}
		bool highCard = false;
		List<CardScript> highCardCards = new List<CardScript>();
		
		bool[] handsContained = new bool[18];
		if(highCard)
		{
			handsContained[0] = true;
		}
		if(onePair)
		{
			handsContained[1] = true;
		}
		if(twoPair)
		{
			handsContained[2] = true;
		}
		if(threeOfAKind)
		{
			handsContained[3] = true;
		}
		if(straight)
		{
			handsContained[4] = true;
		}
		if(flush)
		{
			handsContained[5] = true;
		}
		if(fullHouse)
		{
			handsContained[6] = true;
		}
		if(fourOfAKind)
		{
			handsContained[7] = true;
		}
		if(straightFlush)
		{
			handsContained[8] = true;
		}
		if(fiveOfAKind)
		{
			handsContained[9] = true;
		}
		/* if(flushFive)
		{
			handsContained[10] = true;
		} */
		
		if(cardsInHand >= 1)
		{
			highCard = true;
			highCardCards.Add(hand[cardsInHand - 1]);
			//handZone.HandEvaluated(highCard, "High Card", 0);
			//handZone.HandEvaluated(highCard, highCard[0].rankString + " High");
			//return;
		}
		if(flushFive)
		{
			handZone.HandEvaluated(hand, "Flush Five", 10, evaluatingOnlyCardsUsed, handsContained);
			return;
		}
		/* if(flushFour)
		{
			handZone.HandEvaluated(fourOfAKindCards, "Flush Four", 11, evaluatingOnlyCardsUsed, onePair, twoPair, threeOfAKind, straight, flush, fullHouse, fourOfAKind, straightFlush, fiveOfAKind, flushHouse, flushFive);
			return;
		} */
		/* if(flushHouse)
		{
			handZone.HandEvaluated(hand, "Flush House", 10, evaluatingOnlyCardsUsed, handsContained);
			return;
		} */
		if(fiveOfAKind)
		{
			handZone.HandEvaluated(hand, "Five of a Kind", 9, evaluatingOnlyCardsUsed, handsContained);
			return;
		}
		if(straightFlush)
		{
			if(royalFlush)
			{
				handZone.HandEvaluated(straightFlushCards, "Royal Flush", 8, evaluatingOnlyCardsUsed, handsContained);
				return;
			}
			else
			{
				handZone.HandEvaluated(straightFlushCards, "Straight Flush", 8, evaluatingOnlyCardsUsed, handsContained);
				return;
			}
		}
		if(fourOfAKind)
		{
			handZone.HandEvaluated(fourOfAKindCards, "Four of a Kind", 7, evaluatingOnlyCardsUsed, handsContained);
			return;
		}
		if(fullHouse)
		{
			handZone.HandEvaluated(hand, "Full House", 6, evaluatingOnlyCardsUsed, handsContained);
			return;
		}
		if(flush)
		{
			handZone.HandEvaluated(flushCards, "Flush", 5, evaluatingOnlyCardsUsed, handsContained);
			return;
		}
		if(straight)
		{
			handZone.HandEvaluated(straightCards, "Straight", 4, evaluatingOnlyCardsUsed, handsContained);
			return;
		}
		if(threeOfAKind)
		{
			handZone.HandEvaluated(threeOfAKindCards, "Three of a Kind", 3, evaluatingOnlyCardsUsed, handsContained);
			return;
		}
		if(twoPair)
		{
			handZone.HandEvaluated(twoPairCards, "Two Pair", 2, evaluatingOnlyCardsUsed, handsContained);
			return;
		}
		if(onePair)
		{
			handZone.HandEvaluated(onePairCards, "One Pair", 1, evaluatingOnlyCardsUsed, handsContained);
			return;
		}
		if(highCard)
		{
			handZone.HandEvaluated(highCardCards, "High Card", 0, evaluatingOnlyCardsUsed, handsContained);			
			return;
		}
		print("evaluate hand made it to the nothin' zone somehow");
		handZone.HandEvaluated(null, "Nothin'", -1, evaluatingOnlyCardsUsed, handsContained);
	}
	
	public bool HandIsFlush(List<CardScript> hand, int minNeededForFlush)
	{
		int[] numberOfEachSuit = new int[5];
		for(int card = 0; card < hand.Count; card++)
		{
			numberOfEachSuit[hand[card].suitInt]++;
		}
		//print("numberOfEachSuit[4]= "+numberOfEachSuit[4]+" minNeededForFlush= "+minNeededForFlush);
		for(int suit = 0; suit < 4; suit++)
		{
			//print("suit= "+suit+" numberOfEachSuit[suit]= "+numberOfEachSuit[suit]);
			if(numberOfEachSuit[suit] + numberOfEachSuit[4] >= minNeededForFlush)
			{
				//print("returning true");
				return true;
			}
		}
		//print("returning false");
		return false;
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
	
	public List<CardScript> DoesHandContainStraightOldAndBad(List<CardScript> hand, int maxGaps, int minLength, bool canWrap)
	{
		bool straight = false;
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
/* 			dataText.text = dataText.text + "uniqueRanks= ";
			for(int card = 0; card < uniqueRanks.Count; card++)
			{
				dataText.text = dataText.text + uniqueRanks[card].GetCardNameCharSuit() + " ";
			}
			dataText.text = dataText.text + "\n"; */
			
			int uniqueRanksInHand = uniqueRanks.Count;
			if(uniqueRanksInHand >= minLength)
			{
				int cardsInARow = 1;
				int maxCardsInARow = 1;
				int straightHighCard = -1;
				List<CardScript> cardsUsedInStraight = new List<CardScript>();
				if(uniqueRanks[uniqueRanksInHand - 1].rankInt == 12 && !canWrap) // if there is an ace in hand
				{
					if(uniqueRanks[0].rankInt <= maxGaps)
					{
						cardsInARow++;
						if(cardsInARow > maxCardsInARow)
						{
							maxCardsInARow = cardsInARow;
						}
						cardsUsedInStraight.Add(uniqueRanks[uniqueRanksInHand - 1]);
						cardsUsedInStraight.Add(uniqueRanks[0]);
						if(cardsInARow >= minLength)
						{
							straight = true;
							straightHighCard = uniqueRanks[0].rankInt;
						}
					}
				}
				bool chainBroken = false;
				for(int card = 0; card < uniqueRanksInHand - 1; card++)
				{
					if(uniqueRanks[card + 1].rankInt - uniqueRanks[card].rankInt <= 1 + maxGaps && uniqueRanks[card + 1].rankInt - uniqueRanks[card].rankInt > 0)
					{
						cardsInARow++;
						if(cardsInARow > maxCardsInARow)
						{
							maxCardsInARow = cardsInARow;
						}
						
						if(cardsInARow >= minLength)
						{
							if(chainBroken)
							{
								if(cardsUsedInStraight.Count > 0)
								{
									if(uniqueRanks[card + 1].rankInt > cardsUsedInStraight[0].rankInt)
									{
										cardsUsedInStraight.Clear();
									}
								}
								chainBroken = false;
							}
							straight = true;
							straightHighCard = uniqueRanks[card + 1].rankInt;
						}
						if(!cardsUsedInStraight.Contains(uniqueRanks[card + 1]))
						{
							cardsUsedInStraight.Add(uniqueRanks[card + 1]);
						}
						cardsUsedInStraight.Add(uniqueRanks[card]);
					}
					else
					{
						if(cardsUsedInStraight.Count < minLength)
						{
							cardsUsedInStraight.Clear();
						}
						chainBroken = true;
						cardsInARow = 1;
					}
				}
				if(canWrap && cardsInARow < uniqueRanksInHand)
				{
					if(uniqueRanks[uniqueRanksInHand - 1].rankInt - uniqueRanks[0].rankInt >= (12 - maxGaps))
					{
						cardsInARow++;
						if(cardsInARow > maxCardsInARow)
						{
							//chainBroken = false;
							maxCardsInARow = cardsInARow;
						}
						if(cardsInARow >= minLength)
						{
							if(chainBroken)
							{
								if(cardsUsedInStraight.Count > 0)
								{
									if(uniqueRanks[0].rankInt > cardsUsedInStraight[0].rankInt)
									{
										cardsUsedInStraight.Clear();
									}
								}
								chainBroken = false;
							}
							straight = true;
							straightHighCard = uniqueRanks[0].rankInt;
						}
						cardsUsedInStraight.Add(uniqueRanks[uniqueRanksInHand - 1]);
						cardsUsedInStraight.Add(uniqueRanks[0]);
						for(int card = 0; card < uniqueRanksInHand - 2; card++)
						{
							if(uniqueRanks[card + 1].rankInt - uniqueRanks[card].rankInt <= 1 + maxGaps && uniqueRanks[card + 1].rankInt - uniqueRanks[card].rankInt > 0)
							{
								cardsInARow++;
								if(cardsInARow > maxCardsInARow)
								{
									//chainBroken = false;
									maxCardsInARow = cardsInARow;
								}
								
								if(cardsInARow >= minLength)
								{
									if(chainBroken)
									{
										if(cardsUsedInStraight.Count > 0)
										{
											if(uniqueRanks[card + 1].rankInt > cardsUsedInStraight[0].rankInt)
											{
												cardsUsedInStraight.Clear();
											}
										}
										chainBroken = false;
									}
									straight = true;
									straightHighCard = uniqueRanks[card + 1].rankInt;
								}
								cardsUsedInStraight.Add(uniqueRanks[card + 1]);
								cardsUsedInStraight.Add(uniqueRanks[card]);
							}
							else
							{
								if(cardsUsedInStraight.Count < minLength)
								{
									cardsUsedInStraight.Clear();
								}
								chainBroken = true;
								cardsInARow = 1;
							}
						}
					}
				}
				//dataText.text = dataText.text + "straight= " + straight + " SHC= "+ straightHighCard + " CIR= " + cardsInARow + " MCIR= " + maxCardsInARow + "\n";
				cardsUsedInStraight = cardsUsedInStraight.Distinct().ToList();
				cardsUsedInStraight.Sort((a, b) => a.rankInt.CompareTo(b.rankInt));
				if(straight)
				{
					//if we need it straightHighCard still works fine.
					return cardsUsedInStraight;
					//return true;
				}
			}
		}
		if(minLength <= 1)
		{
			List<CardScript> oneCardStraight = new List<CardScript>();
			hand.Sort((a, b) => b.rankInt.CompareTo(a.rankInt));
			oneCardStraight.Add(hand[0]);
			return oneCardStraight;
		}
		return null;
		//return false;
	}
	
	CardScript DoesHandContainCard(List<CardScript> cardScripts, int rank, int suit, bool checkHighRankFirst)
	{
		List<CardScript> duplicatedList = new List<CardScript>(cardScripts);
		duplicatedList.Sort((a, b) => a.rankInt.CompareTo(b.rankInt));
		if(checkHighRankFirst)
		{
			duplicatedList.Reverse();
		}
		for(int i = 0; i < duplicatedList.Count; i++)
		{
			if(duplicatedList[i].rankInt == rank)
			{
				if((duplicatedList[i].suitInt == suit || duplicatedList[i].suitInt == 4) || suit == 4)
				{
					return duplicatedList[i];
				}
			}
		}
		return null;
	}
	
	void Start()
    {	
		//scoreVial.SetupNewBlind(0.2f, 5, antes[currentAnte], currentAnte);
		//scoreVial.ScoreUpdated(0, false);
		//UpdateHandsUntilFatigue(0, true);
        //dataText = GameObject.FindWithTag("DataText").GetComponent<TMP_Text>(); //disable for release
    }
}
