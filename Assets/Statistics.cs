using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System;
using System.Runtime.InteropServices;

public class Statistics : MonoBehaviour
{
	[DllImport("__Internal")]
    private static extern void JS_FileSystem_Sync();
	
	public static Statistics instance;
	public Transform bestHandCardParent;
	public DeckScript deckScript;
	public HandValues handValues;
	public NonStandardCardScript nonStandardCardScript;
	public RectTransform gameStatsRT;
	public MovingButton mainMenuButton;
	public MovingButton previousDailyRunButton;
	public MovingButton nextDailyRunButton;
	public MovingButton lastPlayedGameButton;
	public MovingButton bestGameEverButton;
	public MovingButton bestDailyGameEverButton;
	public MovingButton mostRecentDailyButton;
	public GameObject otherRunButtons;
	
	public TMP_Text[] statsTitleTexts;
	public TMP_Text[] bestHandNameTexts;
	public TMP_Text[] bestHandScoreTexts;
	public TMP_Text[] handsPlayedTexts;
	public TMP_Text[] cardsDiscardedTexts;
	public TMP_Text[] baublesPurchasedTexts;
	public TMP_Text[] zodiacsEarnedTexts;
	public TMP_Text[] cardsAddedToDeckTexts;
	public TMP_Text[] chipsEarnedTexts;
	public TMP_Text[] mostPlayedHandNameTexts;
	public TMP_Text[] mostPlayedHandQuantityTexts;
	public TMP_Text[] anteReachedTexts;
	public TMP_Text[] scoreInFinalAnteTexts;
	public TMP_Text[] seedTexts;
	public TMP_Text[] dateTexts;
	public TMP_Text[] deckNameTexts;
	public Image deckBackImage;
	public Image deckDetailImage;
	
	public GameObject seedCopiedLabel;
	
	//public bool playingDailyRun;
	//public bool playingSeededRun;
	public GameOptions gameOptions;
	
	public MovingButton continueToEndlessModeButton;
	public int handsPlayedInFatigueThisGame = 0;
	public Transform statisticsVariantParent;
	
	public void IncrementHandsPlayedInFatigue()
	{
		handsPlayedInFatigueThisGame++;
		if(handsPlayedInFatigueThisGame >= 10)
		{
			if(!Decks.instance.decks[4].unlocked && !Statistics.instance.currentRun.runIsDailyGame && !Statistics.instance.currentRun.runIsSeededGame && DeckViewer.instance.variantInUse == "")
			{
				Decks.instance.decks[4].unlocked = true;
				Decks.instance.UpdateDecksFile();
				Decks.instance.DeckKnobs[4].knobImage.sprite = Decks.instance.unlockedKnob;
				Decks.instance.DeckKnobs[4].rt.sizeDelta = new Vector2(10,10);
				UnlockNotifications.instance.CreateNewUnlockNotifier(0,4);
			}
		}
	}
	
	public void RevealEndlessModeButton()
	{
		continueToEndlessModeButton.gameObject.SetActive(true);
		continueToEndlessModeButton.ChangeDisabled(false);
		mainMenuButton.rt.anchoredPosition = new Vector2(-63,16);
		mainMenuButton.rt.sizeDelta = new Vector2(120,22);
		mainMenuButton.shadowRT.sizeDelta = new Vector2(120,12);
		mainMenuButton.buttonImageRT.sizeDelta = new Vector2(120,22);
	}
	
	public void HideEndlessModeButton()
	{
		mainMenuButton.ChangeDisabled(false);
		continueToEndlessModeButton.gameObject.SetActive(false);
		mainMenuButton.rt.anchoredPosition = new Vector2(0,16);
		mainMenuButton.rt.sizeDelta = new Vector2(150,22);
		mainMenuButton.shadowRT.sizeDelta = new Vector2(150,12);
		mainMenuButton.buttonImageRT.sizeDelta = new Vector2(150,22);
	}
	
	public void ContinueToEndlessModeClicked()
	{
		mainMenuButton.ChangeDisabled(true);
		continueToEndlessModeButton.ChangeDisabled(true);
		ScoreVial.instance.StartCoroutine(ScoreVial.instance.AnteCompleted(true));
		StartCoroutine(ScoreVial.instance.MoveOverTime(gameStatsRT, new Vector3(98, 0, 0), new Vector3(98, -360, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		StartCoroutine(DisableObjectAfterDelay(gameStatsRT.gameObject, 1f));
	}
	
	public IEnumerator DisableObjectAfterDelay(GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay / GameOptions.instance.gameSpeedFactor);
		obj.SetActive(false);
	}
	
	void Awake()
	{
		instance = this;
	}
	
	public void FileUpdated()	// tested in brave, chrome and firefox, and Will's brave worked as well.
	{
		#if UNITY_WEBGL && !UNITY_EDITOR
			//print("webgl and not editor");
			JS_FileSystem_Sync();
		#endif
	}
	
	//public void AnteCleared()
	public void UpdateRunStats()
	{
		if(currentRun.runIsDailyGame)
		{
			RunInformation lastDailyRun = LoadDailyRun(0);
			if(lastDailyRun != null)
			{
				if(lastDailyRun.dateRunTookPlace == currentRun.dateRunTookPlace)
				{
					UpdateCurrentDailyRun(currentRun);
				}
				else
				{
					AddCurrentRunToDailyRuns(currentRun);
				}
			}
			else
			{
				AddCurrentRunToDailyRuns(currentRun);
			}
		}
		CheckForBestRun();
	}
	
	public void CheckForBestRun()
	{
		RunInformation bestRun = LoadBestRun(true);
		bool bestRunEver = false;
		bool bestDailyRunEver = false;
		if(bestRun != null)
		{
			if(currentRun.anteReached > bestRun.anteReached || (currentRun.anteReached == bestRun.anteReached && currentRun.scoreInFinalAnte > bestRun.scoreInFinalAnte))
			{
				bestRunEver = true;
			}
		}
		else
		{
			bestRunEver = true;
		}
		if(currentRun.runIsDailyGame)
		{
			RunInformation bestDailyRun = LoadBestRun(false);
			if(bestDailyRun != null)
			{
				if(currentRun.anteReached > bestDailyRun.anteReached || (currentRun.anteReached == bestDailyRun.anteReached && currentRun.scoreInFinalAnte > bestDailyRun.scoreInFinalAnte))
				{
					bestDailyRunEver = true;
				}
			}
			else
			{
				bestDailyRunEver = true;
			}
		}
		if(bestRunEver || bestDailyRunEver)
		{
			SetBestRun(currentRun, bestRunEver, bestDailyRunEver);
		}
	}
	
	public void ResetCurrentRun(int seed, string date, bool daily, bool seeded, int deck, string runVariants)
	{
		currentRun = new RunInformation();
		currentRun.rngSeed = seed;
		currentRun.dateRunTookPlace = date;
		currentRun.runIsDailyGame = daily;
		currentRun.runIsSeededGame = seeded;
		currentRun.deckUsed = deck;
		currentRun.runVariations = runVariants;
	}
		
	public string ConvertRunInformationToString(RunInformation runInformation)
	{
		string runInfoString = "";
		runInfoString = runInfoString + runInformation.dateRunTookPlace + "%";
		runInfoString = runInfoString + runInformation.rngSeed + "%";
		runInfoString = runInfoString + runInformation.handsPlayed + "%";
		runInfoString = runInfoString + runInformation.cardsDiscarded + "%";
		runInfoString = runInfoString + runInformation.baublesPurchased + "%";
		runInfoString = runInfoString + runInformation.cardsAddedToDeck + "%";
		runInfoString = runInfoString + runInformation.zodiacsEarned + "%";
		runInfoString = runInfoString + runInformation.chipsEarned + "%";
		runInfoString = runInfoString + runInformation.mostMoneyHeldAtOnce + "%";
		runInfoString = runInfoString + runInformation.anteReached + "%";
		runInfoString = runInfoString + runInformation.scoreInFinalAnte.ToString() + "%";
		runInfoString = runInfoString + runInformation.deckUsed + "%";
		for(int i = 0; i < 18; i ++)
		{
			runInfoString = runInfoString + runInformation.handTypesPlayed[i] + "%";
		}
		runInfoString = runInfoString + runInformation.runIsDailyGame + "%";
		runInfoString = runInfoString + runInformation.runIsSeededGame + "%";
		runInfoString = runInfoString + runInformation.bestHandName + "%";
		runInfoString = runInfoString + runInformation.bestHandScore.ToString() + "%";
		runInfoString = runInfoString + runInformation.runVariations + "%";
		
		runInfoString = runInfoString + runInformation.chips.ToString() + "%";
		runInfoString = runInfoString + runInformation.handsUntilFatigue.ToString() + "%";
		runInfoString = runInfoString + runInformation.discards.ToString() + "%";
		runInfoString = runInfoString + runInformation.PRNGPulls.ToString() + "%";
		runInfoString = runInfoString + runInformation.inShop.ToString() + "%";
		runInfoString = runInfoString + runInformation.baublesCollected + "%";
		runInfoString = runInfoString + runInformation.deck.Count.ToString() + "%";
		for(int i = 0; i < runInformation.deck.Count; i++)
		{
			runInfoString = runInfoString + runInformation.deck[i].standardCard.ToString() + "%";
			runInfoString = runInfoString + runInformation.deck[i].nonStandardCardNumber.ToString() + "%";
			runInfoString = runInfoString + runInformation.deck[i].rankInt.ToString() + "%";
			runInfoString = runInfoString + runInformation.deck[i].suitInt.ToString() + "%";
			runInfoString = runInfoString + runInformation.deck[i].cardValue.ToString() + "%";
			runInfoString = runInfoString + runInformation.deck[i].cardValueHasBeenChanged.ToString() + "%";
			runInfoString = runInfoString + runInformation.deck[i].cardMultiplier.ToString() + "%";
			runInfoString = runInfoString + runInformation.deck[i].cardMultiplierHasChanged.ToString() + "%";
			runInfoString = runInfoString + runInformation.deck[i].cardLocation.ToString() + "%";
		}
		//runInfoString = runInfoString + "%";
		runInfoString = runInfoString + runInformation.bestHand.Count.ToString() + "%";
		
		for(int i = 0; i < runInformation.bestHand.Count; i++)
		{
			runInfoString = runInfoString + runInformation.bestHand[i].standardCard.ToString() + "%";
			runInfoString = runInfoString + runInformation.bestHand[i].nonStandardCardNumber.ToString() + "%";
			runInfoString = runInfoString + runInformation.bestHand[i].rankInt.ToString() + "%";
			runInfoString = runInfoString + runInformation.bestHand[i].suitInt.ToString() + "%";
			runInfoString = runInfoString + runInformation.bestHand[i].cardValue.ToString() + "%";
			runInfoString = runInfoString + runInformation.bestHand[i].cardValueHasBeenChanged.ToString() + "%";
			runInfoString = runInfoString + runInformation.bestHand[i].cardMultiplier.ToString() + "%";
			runInfoString = runInfoString + runInformation.bestHand[i].cardMultiplierHasChanged.ToString() + "%";
			runInfoString = runInfoString + runInformation.bestHand[i].cardLocation.ToString();
			if(i < runInformation.bestHand.Count - 1)
			{
				runInfoString = runInfoString + "%";
			}
		}
		return runInfoString;
	}
	
	public RunInformation ConvertStringToRunInformation(string runInfoString)
	{
		RunInformation runInformation = new RunInformation();
		//string[] data = runInfoString.Split(',');
		string[] data = runInfoString.Split('%');
/* 		for(int i = 0; i < data.Length; i++)
		{
			print("data[" + i + "]= " + data[i]);
		} */
		runInformation.dateRunTookPlace = data[0];
		runInformation.rngSeed = int.Parse(data[1]);
		runInformation.handsPlayed = int.Parse(data[2]);
		runInformation.cardsDiscarded = int.Parse(data[3]);
		runInformation.baublesPurchased = int.Parse(data[4]);
		runInformation.cardsAddedToDeck = int.Parse(data[5]);
		runInformation.zodiacsEarned = int.Parse(data[6]);
		runInformation.chipsEarned = int.Parse(data[7]);
		runInformation.mostMoneyHeldAtOnce = int.Parse(data[8]);
		runInformation.anteReached = int.Parse(data[9]);
		runInformation.scoreInFinalAnte = float.Parse(data[10]);
		runInformation.deckUsed = int.Parse(data[11]);
		for(int i = 0; i < 18; i++)
		{
			runInformation.handTypesPlayed[i] = int.Parse(data[12 + i]);
		}
		runInformation.runIsDailyGame = bool.Parse(data[30]);
		runInformation.runIsSeededGame = bool.Parse(data[31]);
		runInformation.bestHandName = data[32];
		runInformation.bestHandScore = float.Parse(data[33]);
		runInformation.runVariations = data[34];
		
		runInformation.chips = int.Parse(data[35]);
		runInformation.handsUntilFatigue = int.Parse(data[36]);
		runInformation.discards = int.Parse(data[37]);
		runInformation.PRNGPulls = int.Parse(data[38]);
		runInformation.inShop = bool.Parse(data[39]);
		runInformation.baublesCollected = data[40];
		List<SavedCard> deck = new List<SavedCard>();
		int deckCount = int.Parse(data[41]);
		int deckDataStep = 42;
		int savedCardDataPoints = 9;
		for(int i = deckDataStep; i < deckDataStep + deckCount * 9; i++)
		{
			deck.Add(new SavedCard(bool.Parse(data[deckDataStep + i * savedCardDataPoints]), int.Parse(data[deckDataStep + 1 + i * savedCardDataPoints]), int.Parse(data[deckDataStep + 2 + i * savedCardDataPoints]), int.Parse(data[deckDataStep + 3 + i * savedCardDataPoints]), float.Parse(data[deckDataStep + 4 + i * savedCardDataPoints]), bool.Parse(data[deckDataStep + 5 + i * savedCardDataPoints]), float.Parse(data[deckDataStep + 6 + i * 8]), bool.Parse(data[deckDataStep + 7 + i * savedCardDataPoints]), int. Parse(data[deckDataStep + 8 + i * savedCardDataPoints])));
		}
		
		int bestHandCount = int.Parse(data[deckDataStep + deckCount * 9]);
		int bestHandDataStep = deckDataStep + deckCount * 9 + 1;
		List<SavedCard> bestHandCards = new List<SavedCard>();
		for(int i = bestHandDataStep; i < bestHandDataStep + bestHandCount * 9; i++)
		{
			bestHandCards.Add(new SavedCard(bool.Parse(data[bestHandDataStep + i * savedCardDataPoints]), int.Parse(data[bestHandDataStep + 1 + i * savedCardDataPoints]), int.Parse(data[bestHandDataStep + 2 + i * savedCardDataPoints]), int.Parse(data[bestHandDataStep + 3 + i * savedCardDataPoints]), float.Parse(data[bestHandDataStep + 4 + i * savedCardDataPoints]), bool.Parse(data[bestHandDataStep + 5 + i * savedCardDataPoints]), float.Parse(data[bestHandDataStep + 6 + i * 8]), bool.Parse(data[bestHandDataStep + 7 + i * savedCardDataPoints]), int. Parse(data[bestHandDataStep + 8 + i * savedCardDataPoints])));
		}
		runInformation.bestHand = bestHandCards;
		return runInformation;
	}
	
	public class RunInformation
	{
		public string dateRunTookPlace;
		public int rngSeed;
		public int handsPlayed;
		public int cardsDiscarded;
		public int baublesPurchased;
		public int cardsAddedToDeck;
		public int zodiacsEarned;
		public int chipsEarned;
		public int mostMoneyHeldAtOnce;
		public int anteReached;
		public float scoreInFinalAnte;
		public int deckUsed;
		public int[] handTypesPlayed = new int[18]; // can extract special hands played from this
		public bool runIsDailyGame;
		public bool runIsSeededGame;
		public string bestHandName;
		public float bestHandScore;
		public string runVariations;
		
		public int chips;
		public int handsUntilFatigue;
		public int discards;
		public int PRNGPulls;
		public bool inShop;
		public string baublesCollected;		// ex 14,12,45,1,5,6,12 so the order is maintained. update this after each purchase, remember earned baubles like zodiacs from cake, spyglass, etc? or is that it?
		public List<SavedCard> deck = new List<SavedCard>(); // order matters!
		
		public List<SavedCard> bestHand = new List<SavedCard>();
	}
	
	public RunInformation currentRun = null;
	public RunInformation displayedRun = null;
	public int viewedDailyRunIndex = 0;
	
	public class SavedCard
	{
		public bool standardCard;
		public int nonStandardCardNumber;
		public int rankInt;
		public int suitInt;
		
		public float cardValue;
		public bool cardValueHasBeenChanged;
		public float cardMultiplier;
		public bool cardMultiplierHasChanged;
		public int cardLocation = 0; // 0 = shop / unknown. 1 = draw pile, 2 = hand, 3 = discard
			
		public SavedCard(bool standardCard, int nonStandardCardNumber, int rankInt, int suitInt, float cardValue, bool cardValueHasBeenChanged, float cardMultiplier, bool cardMultiplierHasChanged, int cardLocation)
		{
			this.standardCard = standardCard;
			this.nonStandardCardNumber = nonStandardCardNumber;
			this.rankInt = rankInt;
			this.suitInt = suitInt;
			this.cardValue = cardValue;
			this.cardValueHasBeenChanged = cardValueHasBeenChanged;
			this.cardMultiplier = cardMultiplier;
			this.cardMultiplierHasChanged = cardMultiplierHasChanged;
			this.cardLocation = cardLocation;
		}
	}
	
	public void SeedButtonClicked()
	{
		StartCoroutine(ShowSeedCopiedLabel());
		GUIUtility.systemCopyBuffer = seedTexts[0].text;
	}
	
	public IEnumerator ShowSeedCopiedLabel()
	{
		seedCopiedLabel.gameObject.SetActive(true);
		yield return new WaitForSeconds(1);
		seedCopiedLabel.gameObject.SetActive(false);
	}
	
	public void RevealDailyGameNavigationButtons()
	{
		
	}
	
	public bool lookingThroughDailies = false;
	
	public void RevealRelevantStatsButtons()
	{
		//if(displayedRun.runIsDailyGame && )
		if(lookingThroughDailies)
		{
			RunInformation previousDailyRun = LoadDailyRun(viewedDailyRunIndex + 1);
			if(previousDailyRun == null)
			{
				previousDailyRunButton.gameObject.SetActive(false);
			}
			else
			{
				previousDailyRunButton.gameObject.SetActive(true);
			}
			RunInformation nextDailyRun = LoadDailyRun(viewedDailyRunIndex - 1);
			if(nextDailyRun == null)
			{
				nextDailyRunButton.gameObject.SetActive(false);
			}
			else
			{
				nextDailyRunButton.gameObject.SetActive(true);
			}
			if(lookingThroughDailies)
			{
				return;
			}
		}
		else
		{
			viewedDailyRunIndex = 0;
			previousDailyRunButton.gameObject.SetActive(false);
			nextDailyRunButton.gameObject.SetActive(false);
		}
		RunInformation lastDaily = LoadDailyRun(0);
		if(lastDaily != null)
		{
			mostRecentDailyButton.gameObject.SetActive(true);
		}
		else
		{
			mostRecentDailyButton.gameObject.SetActive(false);
		}
		if(currentRun != null)
		{
			lastPlayedGameButton.gameObject.SetActive(true);
		}
		else
		{
			lastPlayedGameButton.gameObject.SetActive(false);
		}
		RunInformation bestRunEver = LoadBestRun(true);
		if(bestRunEver != null)
		{
			bestGameEverButton.gameObject.SetActive(true);
		}
		else
		{
			bestGameEverButton.gameObject.SetActive(false);
		}
		RunInformation bestDailyRunEver = LoadBestRun(false);
		if(bestDailyRunEver != null)
		{
			bestDailyGameEverButton.gameObject.SetActive(true);
		}
		else
		{
			bestDailyGameEverButton.gameObject.SetActive(false);
		}
		if(mostRecentDailyButton.gameObject.activeSelf || lastPlayedGameButton.gameObject.activeSelf || bestGameEverButton.gameObject.activeSelf || bestDailyGameEverButton.gameObject.activeSelf)
		{
			otherRunButtons.SetActive(true);
		}
		else
		{
			otherRunButtons.SetActive(false);
		}
	}
	
	public void PreviousDailyRunButtonClicked()
	{
		RunInformation previousDailyRun = LoadDailyRun(viewedDailyRunIndex + 1);
		PopulateGameStats(previousDailyRun, "Daily\n" + previousDailyRun.dateRunTookPlace);
		viewedDailyRunIndex++;
		RevealRelevantStatsButtons();
	}
	
	public void NextDailyRunButtonClicked()
	{
		RunInformation nextDailyRun = LoadDailyRun(viewedDailyRunIndex - 1);
		PopulateGameStats(nextDailyRun, "Daily\n" + nextDailyRun.dateRunTookPlace);
		viewedDailyRunIndex--;
		RevealRelevantStatsButtons();
	}
	
	public void BestRunEverButtonClicked()
	{
		RunInformation bestRunEver = LoadBestRun(true);
		if(bestRunEver.runIsSeededGame)
		{
			PopulateGameStats(bestRunEver, "Best Run Ever (Seeded)\n" + bestRunEver.dateRunTookPlace);
		}
		else
		{
			PopulateGameStats(bestRunEver, "Best Run Ever\n" + bestRunEver.dateRunTookPlace);
		}
		lookingThroughDailies = false;
		RevealRelevantStatsButtons();
	}
	
	public void BestDailyRunButtonClicked()
	{
		RunInformation bestDailyRun = LoadBestRun(false);
		PopulateGameStats(bestDailyRun, "Best Ever Daily\n" + bestDailyRun.dateRunTookPlace);
		lookingThroughDailies = false;
		RevealRelevantStatsButtons();
	}
	
	public void LastGamePlayedButtonClicked()
	{
		if(currentRun.runIsSeededGame)
		{
			PopulateGameStats(currentRun, "Last Run Played (Seeded)");
		}
		else
		{
			PopulateGameStats(currentRun, "Last Run Played");
		}
		lookingThroughDailies = false;
		RevealRelevantStatsButtons();
	}
	
	public void MostRecentDailyButtonClicked()
	{
		RunInformation mostRecentDaily = LoadDailyRun(0);
		PopulateGameStats(mostRecentDaily, "Last Played Daily\n" + mostRecentDaily.dateRunTookPlace);
		viewedDailyRunIndex = 0;
		lookingThroughDailies = true;
		RevealRelevantStatsButtons();
	}
	
	public void UpdateBestHand(List<CardScript> hand, RunInformation runInformation)
	{
		List<SavedCard> savedCards = new List<SavedCard>();
		for(int i = 0; i < hand.Count; i++)
		{
			savedCards.Add(new SavedCard(hand[i].standardCard, hand[i].nonStandardCardNumber, hand[i].rankInt, hand[i].suitInt, hand[i].cardValue, hand[i].cardValueHasBeenChanged, hand[i].cardMultiplier, hand[i].cardMultiplierHasChanged, 0));
		}
		runInformation.bestHand = savedCards;
	}
	
	public void DisableOtherStatsButtons()
	{
		previousDailyRunButton.gameObject.SetActive(false);
		nextDailyRunButton.gameObject.SetActive(false);
		lastPlayedGameButton.gameObject.SetActive(false);
		bestGameEverButton.gameObject.SetActive(false);
		bestDailyGameEverButton.gameObject.SetActive(false);
		otherRunButtons.SetActive(false);
	}
	
	public void PopulateGameStats(RunInformation runInformation, string titleText)
	{
		displayedRun = runInformation;
		
		if(runInformation.runVariations != "")
		{
			Vector2 location = new Vector2(-209,-40);
			float height = 240f;
			/* if(MainMenu.instance.playCanvas.activeSelf)
			{
				location = new Vector2(-209,-29);
				height = 218f;
			} */
			RunVariations.instance.SetupVariantDisplay(statisticsVariantParent, location, runInformation.runVariations, height);
		}
		else
		{
			RunVariations.instance.ClearVariantDisplay(statisticsVariantParent);
		}
		
		for(int i = 0; i < statsTitleTexts.Length; i++)
		{
			statsTitleTexts[i].text = titleText;
		}
		for(int i = 0; i < bestHandCardParent.childCount; i++)
		{
			Destroy(bestHandCardParent.GetChild(i).gameObject);
		}
		
		float maxWidth = 237;
		float cardWidth = 45;
		float squeezeDistance = (maxWidth - cardWidth) / (runInformation.bestHand.Count - 1);
		float distanceBetweenCards = Mathf.Min(48f, squeezeDistance);
		
		for(int i = 0; i < runInformation.bestHand.Count; i++)
		{
			float xPosition = 245f / 2f - (runInformation.bestHand.Count - 1) * (distanceBetweenCards / 2f) + (i * distanceBetweenCards);
			Vector2 cardPosition = new Vector2(xPosition, 25.5f);
			if(runInformation.bestHand[i].standardCard)
			{
				CardScript newCard = deckScript.CreateCard(runInformation.bestHand[i].suitInt, runInformation.bestHand[i].rankInt, bestHandCardParent, cardPosition, false);
				newCard.cardValue = runInformation.bestHand[i].cardValue;
				newCard.cardValueHasBeenChanged = runInformation.bestHand[i].cardValueHasBeenChanged;
				if(newCard.cardValueHasBeenChanged)
				{
					newCard.UpdateCardValueTooltip();
				}
				newCard.cardMultiplier = runInformation.bestHand[i].cardMultiplier;
				newCard.cardMultiplierHasChanged = runInformation.bestHand[i].cardMultiplierHasChanged;
				if(newCard.cardMultiplierHasChanged)
				{
					newCard.UpdateCardMultTooltip();
				}
			}
			else
			{
				deckScript.CreateNonStandardCard(nonStandardCardScript.nonStandardCards[runInformation.bestHand[i].nonStandardCardNumber].cardImage, runInformation.bestHand[i].nonStandardCardNumber, "", "", 0, bestHandCardParent, cardPosition, false, false);
			}
		}
		for(int i = 0; i < bestHandNameTexts.Length; i++)
		{
			bestHandNameTexts[i].text = runInformation.bestHandName;
		}
		for(int i = 0; i < bestHandScoreTexts.Length; i++)
		{
			bestHandScoreTexts[i].text = handValues.ConvertFloatToString(runInformation.bestHandScore);
		}
		for(int i = 0; i < handsPlayedTexts.Length; i++)
		{
			handsPlayedTexts[i].text = "" + runInformation.handsPlayed;
		}
		for(int i = 0; i < cardsDiscardedTexts.Length; i++)
		{
			cardsDiscardedTexts[i].text = "" + runInformation.cardsDiscarded;
		}
		for(int i = 0; i < baublesPurchasedTexts.Length; i++)
		{
			baublesPurchasedTexts[i].text = "" + runInformation.baublesPurchased;
		}
		for(int i = 0; i < zodiacsEarnedTexts.Length; i++)
		{
			zodiacsEarnedTexts[i].text = "" + runInformation.zodiacsEarned;
		}
		for(int i = 0; i < cardsAddedToDeckTexts.Length; i++)
		{
			cardsAddedToDeckTexts[i].text = "" + runInformation.cardsAddedToDeck;
		}
		for(int i = 0; i < chipsEarnedTexts.Length; i++)
		{
			chipsEarnedTexts[i].text = "" + runInformation.chipsEarned;
		}
		for(int i = 0; i < anteReachedTexts.Length; i++)
		{
			anteReachedTexts[i].text = "" + runInformation.anteReached;
		}
		int mostPlayedHandType = 0;
		int mostPlayedHandQuantity = 0;
		for(int i = 0; i < 18; i ++)
		{
			if(runInformation.handTypesPlayed[i] >= mostPlayedHandQuantity)
			{
				mostPlayedHandType = i;
				mostPlayedHandQuantity = runInformation.handTypesPlayed[i];
			}
		}
		for(int i = 0; i < mostPlayedHandNameTexts.Length; i++)
		{
			mostPlayedHandNameTexts[i].text = handValues.handNames[mostPlayedHandType];
		}
		for(int i = 0; i < mostPlayedHandQuantityTexts.Length; i++)
		{
			mostPlayedHandQuantityTexts[i].text = "" + mostPlayedHandQuantity;
		}
		for(int i = 0; i < scoreInFinalAnteTexts.Length; i++)
		{
			scoreInFinalAnteTexts[i].text = "" + handValues.ConvertFloatToString(runInformation.scoreInFinalAnte);
		}
		for(int i = 0; i < seedTexts.Length; i++)
		{
			seedTexts[i].text = "" + runInformation.rngSeed;
		}
		for(int i = 0; i < dateTexts.Length; i++)
		{
			dateTexts[i].text = "" + runInformation.dateRunTookPlace;
		}
		for(int i = 0; i < deckNameTexts.Length; i++)
		{
			deckNameTexts[i].text = Decks.instance.decks[runInformation.deckUsed].deckName;
		}
		deckBackImage.color = Decks.instance.decks[runInformation.deckUsed].backColor;
		deckDetailImage.sprite = Decks.instance.decks[runInformation.deckUsed].design;
		deckDetailImage.color = Decks.instance.decks[runInformation.deckUsed].designColor;
	}
	
	public void AddCurrentRunToDailyRuns(RunInformation runInformation)
	{
		
		print("AddCurrentRunToDailyRuns called");
		string dailyRunsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			dailyRunsPath = "/idbfs/ScronglyData/" + "dailyRuns" + ".txt";
		#else
			dailyRunsPath = Application.persistentDataPath + "/" + "dailyRuns" + ".txt";
		#endif
		if(File.Exists(dailyRunsPath))
		{
			try
			{
				string[] lines;
				using (StreamReader reader = new StreamReader(dailyRunsPath))
				{
					string dailyRunsData = reader.ReadToEnd();
					lines = dailyRunsData.Split('\n');
				}
				File.WriteAllText(dailyRunsPath, "");
				StreamWriter writer = new StreamWriter(dailyRunsPath, true);
				writer.WriteLine(lines[0].TrimEnd('\r', '\n'));
				if(lines.Length > 1)
				{
					writer.WriteLine(ConvertRunInformationToString(runInformation));
				}
				else
				{
					writer.Write(ConvertRunInformationToString(runInformation));
				}
				for(int i = 1; i < lines.Length - 1; i++)
				{
					writer.WriteLine(lines[i].TrimEnd('\r', '\n'));
				}
				if(lines.Length > 1)
				{
					writer.Write(lines[lines.Length - 1].TrimEnd('\r', '\n'));
				}
				writer.Close();
				FileUpdated();
			}
			catch(Exception exception)
			{
				ResetDailyRunsFile(dailyRunsPath);
				Debug.Log("An error occurred when loading " + dailyRunsPath + ": " + exception.Message);
				return;
			}
		}
	}
	
	public void UpdateCurrentDailyRun(RunInformation runInformation)
	{
		string dailyRunsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			dailyRunsPath = "/idbfs/ScronglyData/" + "dailyRuns" + ".txt";
		#else
			dailyRunsPath = Application.persistentDataPath + "/" + "dailyRuns" + ".txt";
		#endif
		
		if(File.Exists(dailyRunsPath))
		{
			try
			{
				string[] lines;
				using (StreamReader reader = new StreamReader(dailyRunsPath))
				{
					string dailyRunsData = reader.ReadToEnd();
					lines = dailyRunsData.Split('\n');
					lines[1] = ConvertRunInformationToString(runInformation);
				}
				File.WriteAllText(dailyRunsPath, "");
				StreamWriter writer = new StreamWriter(dailyRunsPath, true);
				for(int i = 0; i < lines.Length - 1; i++)
				{
					writer.WriteLine(lines[i].TrimEnd('\r', '\n'));
				}
				writer.Write(lines[lines.Length - 1].TrimEnd('\r', '\n'));
				writer.Close();
				FileUpdated();
			}
			catch(Exception exception)
			{
				ResetDailyRunsFile(dailyRunsPath);
				Debug.Log("An error occurred when loading " + dailyRunsPath + ": " + exception.Message);
				return;
			}
		}
	}
	
	public RunInformation LoadBestRun(bool bestEver)
	{
		//string bestRunsPath = Application.persistentDataPath + "/" + "bestRuns" + ".txt";
		string bestRunsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			bestRunsPath = "/idbfs/ScronglyData/" + "bestRuns" + ".txt";
		#else
			bestRunsPath = Application.persistentDataPath + "/" + "bestRuns" + ".txt";
		#endif
		
		if(File.Exists(bestRunsPath))
		{
			try
			{
				using (StreamReader reader = new StreamReader(bestRunsPath))
				{
					string bestRunsData = reader.ReadToEnd();
					string[] lines = bestRunsData.Split('\n');
					if(lines.Length < 3)
					{
						return null;
					}
					string fileManagerVersion = lines[0].Trim();
					if(fileManagerVersion == gameOptions.currentFileManagerVersion)
					{
						if(bestEver)
						{
							if(lines[1] != "none")
							{
								return ConvertStringToRunInformation(lines[1]);
							}
							else
							{
								return null;
							}
						}
						else
						{
							if(lines[2] != "none")
							{
								return ConvertStringToRunInformation(lines[2]);
							}
							else
							{
								return null;
							}
						}
					}
					else
					{
						//GameOptions.instance.fileManagerWarningInterface.SetActive(true);
						GameOptions.instance.SetupFileManagerWarningInterface(bestRunsPath);
						ResetBestRunsFile(bestRunsPath);
						Debug.Log("Trying to load a version \"" + lines[0] + "\" bestRunsData. Your version is \"" + gameOptions.currentFileManagerVersion + "\"");
						return null;
					}
				}
			}
			catch(Exception exception)
			{
				ResetBestRunsFile(bestRunsPath);
				Debug.Log("An error occurred when loading " + bestRunsPath + ": " + exception.Message);
				return null;
			}
		}
		else
		{
			return null;
		}
	}
	
	public void SetBestRun(RunInformation runInformation, bool bestEver, bool bestDaily)
	{
		string bestRunsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			bestRunsPath = "/idbfs/ScronglyData/" + "bestRuns" + ".txt";
		#else
			bestRunsPath = Application.persistentDataPath + "/" + "bestRuns" + ".txt";
		#endif
		if(File.Exists(bestRunsPath))
		{
			try
			{
				string[] lines;
				using (StreamReader reader = new StreamReader(bestRunsPath))
				{
					string bestRunsData = reader.ReadToEnd();
					lines = bestRunsData.Split('\n');
					string fileManagerVersion = lines[0].Trim();
					if(fileManagerVersion == gameOptions.currentFileManagerVersion)
					{
						if(bestEver)
						{
							lines[1] = ConvertRunInformationToString(runInformation);
						}
						if(bestDaily)
						{
							lines[2] = ConvertRunInformationToString(runInformation);
						}
					}
					else
					{
						//GameOptions.instance.fileManagerWarningInterface.SetActive(true);
						GameOptions.instance.SetupFileManagerWarningInterface(bestRunsPath);
						ResetBestRunsFile(bestRunsPath);
						Debug.Log("Trying to load a version \"" + lines[0] + "\" bestRunsData. Your version is \"" + gameOptions.currentFileManagerVersion + "\"");
						return;
					}
				}
				File.WriteAllText(bestRunsPath, "");
				StreamWriter writer = new StreamWriter(bestRunsPath, true);
				writer.WriteLine(lines[0].TrimEnd('\r', '\n'));
				writer.WriteLine(lines[1].TrimEnd('\r', '\n'));
				writer.Write(lines[2].TrimEnd('\r', '\n'));
				writer.Close();
				FileUpdated();
			}
			catch(Exception exception)
			{
				ResetBestRunsFile(bestRunsPath);
				Debug.Log("An error occurred when loading " + bestRunsPath + ": " + exception.Message);
				return;
			}
		}
		else
		{
			#if UNITY_WEBGL && !UNITY_EDITOR
			if(!Directory.Exists("/idbfs/ScronglyData"))
			{
				Directory.CreateDirectory("/idbfs/ScronglyData");
			}
			#endif
			StreamWriter writer = new StreamWriter(bestRunsPath, true);
			writer.WriteLine(gameOptions.currentFileManagerVersion);
			if(bestEver)
			{
				writer.WriteLine(ConvertRunInformationToString(runInformation));
			}
			else
			{
				writer.WriteLine("none");
			}
			if(bestDaily)
			{
				writer.Write(ConvertRunInformationToString(runInformation));
			}
			else
			{
				writer.Write("none");
			}
			writer.Close();
			FileUpdated();
		}
	}
	
	public void ResetBestRunsFile(string bestRunsPath)
	{
		#if UNITY_WEBGL && !UNITY_EDITOR
		if(!Directory.Exists("/idbfs/ScronglyData"))
		{
			Directory.CreateDirectory("/idbfs/ScronglyData");
		}
		#endif
		File.WriteAllText(bestRunsPath, "");
		StreamWriter writer = new StreamWriter(bestRunsPath, true);
		writer.WriteLine(gameOptions.currentFileManagerVersion);
		writer.WriteLine("none");
		writer.Write("none");
		writer.Close();
		FileUpdated();
	}
	
	public RunInformation LoadDailyRun(int index)	// 0 = most recent
	{
		if(index < 0)
		{
			return null;
		}
		//print("LoadDailyRun called");
		string dailyRunsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			dailyRunsPath = "/idbfs/ScronglyData/" + "dailyRuns" + ".txt";
		#else
			dailyRunsPath = Application.persistentDataPath + "/" + "dailyRuns" + ".txt";
		#endif
		//print("dailyRunsPath= " + dailyRunsPath);
		if(File.Exists(dailyRunsPath))
		{
			try
			{
				using (StreamReader reader = new StreamReader(dailyRunsPath))
				{
					string dailyRunsData = reader.ReadToEnd();
					string[] lines = dailyRunsData.Split('\n');
					//print("lines.Length= " + lines.Length);
					//print("lines.Length= " + lines.Length + " index= " + index);
					if(lines.Length - 2 < index)
					{
						return null;
					}
					string fileManagerVersion = lines[0].Trim();
					if(fileManagerVersion == gameOptions.currentFileManagerVersion)
					{
						return ConvertStringToRunInformation(lines[index + 1]);
					}
					else
					{
						//GameOptions.instance.fileManagerWarningInterface.SetActive(true);
						GameOptions.instance.SetupFileManagerWarningInterface(dailyRunsPath);
						ResetDailyRunsFile(dailyRunsPath);
						Debug.Log("Trying to load a version \"" + lines[0] + "\" dailyRuns. Your version is \"" + gameOptions.currentFileManagerVersion + "\"");
						return null;
					}
				}
			}
			catch(Exception exception)
			{
				ResetDailyRunsFile(dailyRunsPath);
				Debug.Log("An error occurred when loading " + dailyRunsPath + ": " + exception.Message);
				return null;
			}
		}
		else
		{
			ResetDailyRunsFile(dailyRunsPath);
		}
		return null;
	}
	
	public void ResetDailyRunsFile(string dailyRunsPath)
	{
		#if UNITY_WEBGL && !UNITY_EDITOR
		if(!Directory.Exists("/idbfs/ScronglyData"))
		{
			Directory.CreateDirectory("/idbfs/ScronglyData");
		}
		#endif
		File.WriteAllText(dailyRunsPath, "");
		StreamWriter writer = new StreamWriter(dailyRunsPath, true);
		writer.Write(gameOptions.currentFileManagerVersion);
		writer.Close();
		FileUpdated();
	}
	
	void Start()
	{
		
	}
}