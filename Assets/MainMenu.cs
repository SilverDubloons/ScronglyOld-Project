using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
	public static MainMenu instance;
	public AnimationCurve moveCurve;
	public RectTransform mainMenuRT;
	public RectTransform deckPickerRT;
	public RectTransform dailyMenuRT;
	public RectTransform titleRT;
	public RectTransform creditRT;
	public MovingButton mainPlayButton;
	public MovingButton dailyGameButton;
	public MovingButton optionsButton;
	public MovingButton statsButton;
	public MovingButton deckPickerPlayButton;
	public MovingButton deckPickerBackButton;
	public MovingButton dailyGamePlayButton;
	public MovingButton dailyGameBackButton;
	public MovingButton deckPickerLeftButton;
	public MovingButton deckPickerRightButton;
	public TMP_Text[] dailyGameDateTexts;
	
	public RectTransform handZonesRT;
	public RectTransform handRT;
	public RectTransform scoreAreaRT;
	public RectTransform discardsRemainingRT;
	public RectTransform anteInfoRT;
	public RectTransform moneyCountRT;
	public RectTransform fatigueInfoRT;
	public RectTransform discardPileRT;
	public RectTransform drawPileRT;
	public RectTransform menuButtonRT;
	public RectTransform offscreenObjectsRT;
	public RectTransform shopRT;
	public RectTransform deckViewerRT;
	public GameObject playCanvas;
	public GameObject mainMenuCanvas;
	
	public HandValues handValues;
	public HandScript handScript;
	public BaubleCollection baubleCollection;
	public GameObject optionsMenu;
	public GameObject optionsUI;
	public GameObject gameSettingsMenu;
	public GameObject audioMenu;
	public DeckViewer deckViewer;
	//public Statistics.instance Statistics.instance;
	public RandomNumbers randomNumbers;
	public Toggle seededGameToggle;
	public TMP_InputField seedInputField;
	
	public string dateToday;
	public int dailySeed;
	public int dailyDeck;
	public string dailyVariant;
	
	public delegate void CallbackFunction();
	
	public Image dailyGameCardDetail;
	public Image dailyGameCardBack;
	public TMP_Text[] dailyGameDeckTitleTexts;
	public TMP_Text[] dailyGameDeckDescriptionTexts;
	public GameObject seededGameWarningObject;
	
	public bool startNextGameInShop;
	
	public Toggle RunVariationsToggle;
	public bool dailyMenuAlreadySetup = false;
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		dateToday = GetCurrentDate();
		using (SHA256 sha256 = SHA256.Create())
		{
			byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dateToday));
			dailySeed = BitConverter.ToInt32(hashBytes, 0);
			dailySeed = Mathf.Abs(dailySeed) % (int.MaxValue - 1); // Ensure it's positive and within a valid range
		}
		RevealMainMenu();
		playCanvas.SetActive(false);
		Statistics.instance.gameStatsRT.gameObject.SetActive(false);
		optionsUI.SetActive(false);
		gameSettingsMenu.SetActive(false);
		audioMenu.SetActive(false);
		optionsMenu.SetActive(false);
		//SetupDailyMenu();
	}
	
	public void SetupDailyMenu()
	{
		dailyDeck = randomNumbers.GetDailyDeck(0, Decks.instance.decks.Length, dailySeed);
		dailyVariant = RunVariations.instance.dailyVariants[randomNumbers.GetDailyVariant(0, RunVariations.instance.dailyVariants.Length)];
		RunVariations.instance.SetupDailyRunDisplay(dailyVariant);
		
		//RunVariations.instance.SetupRunVariants();
		dailyGameCardDetail.sprite = Decks.instance.decks[dailyDeck].design;
		dailyGameCardDetail.color = Decks.instance.decks[dailyDeck].designColor;
		dailyGameCardBack.color = Decks.instance.decks[dailyDeck].backColor;
		for(int i = 0; i < dailyGameDateTexts.Length; i++)
		{
			dailyGameDateTexts[i].text = dateToday;
		}
		for(int i = 0; i < dailyGameDeckTitleTexts.Length; i++)
		{
			dailyGameDeckTitleTexts[i].text = Decks.instance.decks[dailyDeck].deckName;
		}
		for(int i = 0; i < dailyGameDeckDescriptionTexts.Length; i++)
		{
			dailyGameDeckDescriptionTexts[i].text = Decks.instance.decks[dailyDeck].description;
		}
	}
	
	public void StatsButtonClicked()
	{
		StartCoroutine(MoveOverTime(titleRT, new Vector2(0,79), new Vector2(0,270), 1f, 0));
		StartCoroutine(MoveOverTime(creditRT, new Vector2(-225,creditRT.anchoredPosition.y), new Vector2(-410,creditRT.anchoredPosition.y), 1f, 0));
		StartCoroutine(MoveOverTime(mainMenuRT, new Vector2(0,0), new Vector2(0,-100), 1f, 0));
		mainPlayButton.ChangeDisabled(true);
		dailyGameButton.ChangeDisabled(true);
		optionsButton.ChangeDisabled(true);
		statsButton.ChangeDisabled(true);
		// Statistics.instance.RunInformation bestRunEver = Statistics.instance.LoadBestRun(true);
		var bestRunEver = Statistics.instance.LoadBestRun(true);
		Statistics.instance.HideEndlessModeButton();
		Statistics.instance.gameStatsRT.gameObject.SetActive(true);
		Statistics.instance.PopulateGameStats(bestRunEver, "Best Run Ever\n" + bestRunEver.dateRunTookPlace);
		Statistics.instance.viewedDailyRunIndex = 0;
		
		Statistics.instance.mainMenuButton.ChangeDisabled(true);
		Statistics.instance.DisableOtherStatsButtons();
		StartCoroutine(MoveOverTime(Statistics.instance.gameStatsRT, new Vector3(98, -360, 0), new Vector3(98, 0, 0), 1, 0, StatsScreenRevealed));
		return;
	}
	
	public void DailyGameClicked()
	{
		if(!dailyMenuAlreadySetup)
		{
			SetupDailyMenu();
			dailyMenuAlreadySetup = true;
		}
		StartCoroutine(MoveOverTime(mainMenuRT, new Vector2(0,0), new Vector2(0,-100), 1f, 0));
		mainPlayButton.ChangeDisabled(true);
		dailyGameButton.ChangeDisabled(true);
		optionsButton.ChangeDisabled(true);
		statsButton.ChangeDisabled(true);
		//Statistics.instance.RunInformation lastDailyRun;
		var lastDailyRun = Statistics.instance.LoadDailyRun(0);
		//print("lastDailyRun.dateRunTookPlace= " + lastDailyRun.dateRunTookPlace + " dateToday= " + dateToday);
		if(lastDailyRun != null)
		{
			if(lastDailyRun.dateRunTookPlace == dateToday)
			{
				StartCoroutine(MoveOverTime(titleRT, new Vector2(0,79), new Vector2(0,270), 1f, 0));
				StartCoroutine(MoveOverTime(creditRT, new Vector2(-225,creditRT.anchoredPosition.y), new Vector2(-410,creditRT.anchoredPosition.y), 1f, 0));
				Statistics.instance.HideEndlessModeButton();
				// Statistics.instance.PopulateGameStats(lastDailyRun, "Daily\n" + lastDailyRun.dateRunTookPlace);
				Statistics.instance.viewedDailyRunIndex = 0;
				Statistics.instance.gameStatsRT.gameObject.SetActive(true);
				Statistics.instance.mainMenuButton.ChangeDisabled(true);
				Statistics.instance.DisableOtherStatsButtons();
				Statistics.instance.PopulateGameStats(lastDailyRun, "Daily\n" + lastDailyRun.dateRunTookPlace);
				StartCoroutine(MoveOverTime(Statistics.instance.gameStatsRT, new Vector3(98, -360, 0), new Vector3(98, 0, 0), 1, 0, DailyGameRevealed));
				return;
			}
		}
		RunVariations.instance.SetupDailyRun(dailyVariant);
		StartCoroutine(MoveOverTime(dailyMenuRT, new Vector2(0,-385), new Vector2(0,-64), 1f, 0, EnableDailyGameButtons));
	}
	
	public void DailyGameRevealed()
	{
		Statistics.instance.mainMenuButton.ChangeDisabled(false);
		Statistics.instance.lookingThroughDailies = true;
		Statistics.instance.RevealRelevantStatsButtons();
	}
	
	public void StatsScreenRevealed()
	{
		Statistics.instance.mainMenuButton.ChangeDisabled(false);
		Statistics.instance.lookingThroughDailies = false;
		Statistics.instance.RevealRelevantStatsButtons();
	}
	
	public void MainPlayClicked()
	{
		StartCoroutine(MoveOverTime(mainMenuRT, new Vector2(0,0), new Vector2(0,-100), 1f, 0));
		StartCoroutine(MoveOverTime(deckPickerRT, new Vector2(deckPickerRT.anchoredPosition.x ,-385), new Vector2(deckPickerRT.anchoredPosition.x,-72), 1f, 0, EnableDeckPickerButtons));
		mainPlayButton.ChangeDisabled(true);
		dailyGameButton.ChangeDisabled(true);
		optionsButton.ChangeDisabled(true);
		statsButton.ChangeDisabled(true);
		RunVariations.instance.variantsDropdown.value = RunVariations.instance.variantsDropdown.options.Count - 1;
	}
	
	public void DeckPickerBackClicked()
	{
		StartCoroutine(MoveOverTime(mainMenuRT, new Vector2(0,-100), new Vector2(0,0), 1f, 0, EnableMainMenuButtons));
		StartCoroutine(MoveOverTime(deckPickerRT, new Vector2(deckPickerRT.anchoredPosition.x,-72), new Vector2(deckPickerRT.anchoredPosition.x,-385), 1f, 0));
		seededGameToggle.isOn = false;
		seededGameToggle.interactable = false;
		deckPickerPlayButton.ChangeDisabled(true);
		deckPickerBackButton.ChangeDisabled(true);
		deckPickerLeftButton.ChangeDisabled(true);
		deckPickerRightButton.ChangeDisabled(true);
		RunVariations.instance.variantModeToggle.isOn = false;
		RunVariations.instance.variantModeToggle.interactable = false;
		RunVariations.instance.VariantModeToggleUpdated();
	}
	
	public void DailyGameBackClicked()
	{
		StartCoroutine(MoveOverTime(mainMenuRT, new Vector2(0,-100), new Vector2(0,0), 1f, 0, EnableMainMenuButtons));
		StartCoroutine(MoveOverTime(dailyMenuRT, new Vector2(0,-64), new Vector2(0,-385), 1f, 0));
		dailyGamePlayButton.ChangeDisabled(true);
		dailyGameBackButton.ChangeDisabled(true);
		RunVariations.instance.ClearDailyRun();
	}
	
	public void EnableMainMenuButtons()
	{
		mainPlayButton.ChangeDisabled(false);
		dailyGameButton.ChangeDisabled(false);
		optionsButton.ChangeDisabled(false);
		optionsMenu.SetActive(true);
		// Statistics.instance.RunInformation bestRunEver = Statistics.instance.LoadBestRun(true);
		var bestRunEver = Statistics.instance.LoadBestRun(true);
		if(bestRunEver == null)
		{
			statsButton.ChangeDisabled(true);
		}
		else
		{
			statsButton.ChangeDisabled(false);
		}
	}
	
	public void EnableDeckPickerButtons()
	{
		SeededRunToggleUpdated();
		deckPickerBackButton.ChangeDisabled(false);
		seededGameToggle.interactable = true;
		deckPickerLeftButton.ChangeDisabled(false);
		deckPickerRightButton.ChangeDisabled(false);
		RunVariations.instance.variantModeToggle.interactable = true;
	}
	
	public void EnableDailyGameButtons()
	{
		dailyGamePlayButton.ChangeDisabled(false);
		dailyGameBackButton.ChangeDisabled(false);
	}
	
	public void DeckPickerPlayClicked()
	{
		StartCoroutine(MoveOverTime(deckPickerRT, new Vector2(deckPickerRT.anchoredPosition.x,-72), new Vector2(deckPickerRT.anchoredPosition.x,-385), 1f, 0, BeginRandomGame));
		StartCoroutine(MoveOverTime(titleRT, new Vector2(0,79), new Vector2(0,270), 1f, 0));
		StartCoroutine(MoveOverTime(creditRT, new Vector2(-225,creditRT.anchoredPosition.y), new Vector2(-410,creditRT.anchoredPosition.y), 1f, 0));
		deckPickerPlayButton.ChangeDisabled(true);
		deckPickerBackButton.ChangeDisabled(true);
		deckPickerLeftButton.ChangeDisabled(true);
		deckPickerRightButton.ChangeDisabled(true);
		seededGameToggle.interactable = false;
		NewGameAnimation.instance.newGameAnimationStage = 0;
		NewGameAnimation.instance.StartCoroutine(NewGameAnimation.instance.Animate());
	}
	
	public void DailyGamePlayClicked()
	{
		StartCoroutine(MoveOverTime(dailyMenuRT, new Vector2(0,-64), new Vector2(0,-385), 1f, 0, BeginDailyGame));
		StartCoroutine(MoveOverTime(titleRT, new Vector2(0,79), new Vector2(0,270), 1f, 0));
		StartCoroutine(MoveOverTime(creditRT, new Vector2(-225,creditRT.anchoredPosition.y), new Vector2(-410,creditRT.anchoredPosition.y), 1f, 0));
		dailyGamePlayButton.ChangeDisabled(true);
		dailyGameBackButton.ChangeDisabled(true);
		NewGameAnimation.instance.newGameAnimationStage = 0;
		NewGameAnimation.instance.StartCoroutine(NewGameAnimation.instance.Animate());
	}
	
	public void RevealMainMenu()
	{
		StartCoroutine(MoveOverTime(mainMenuRT, new Vector2(0,-100), new Vector2(0,0), 1f, 0, EnableMainMenuButtons));
		StartCoroutine(MoveOverTime(titleRT, new Vector2(0,270), new Vector2(0,79), 1f, 0));
		StartCoroutine(MoveOverTime(creditRT, new Vector2(-410,creditRT.anchoredPosition.y), new Vector2(-225,creditRT.anchoredPosition.y), 1f, 0));
		playCanvas.SetActive(false);
	}
	
	public string GetCurrentDate()
	{
		string dayString = DateTime.Now.ToString("dd");
		if(dayString.StartsWith("0"))
		{
			dayString = dayString.Substring(1);
		}
		string monthString = DateTime.Now.ToString("MM");
		int monthInt = int.Parse(monthString);
		switch(monthInt)
		{
			case 1:
			monthString = "January";
			break;
			case 2:
			monthString = "February";
			break;
			case 3:
			monthString = "March";
			break;
			case 4:
			monthString = "April";
			break;
			case 5:
			monthString = "May";
			break;
			case 6:
			monthString = "June";
			break;
			case 7:
			monthString = "July";
			break;
			case 8:
			monthString = "August";
			break;
			case 9:
			monthString = "September";
			break;
			case 10:
			monthString = "October";
			break;
			case 11:
			monthString = "November";
			break;
			case 12:
			monthString = "December";
			break;
			default:
			monthString = "Time has fallen apart";
			break;
		}
		string yearString = DateTime.Now.ToString("yyyy");
		string dateString = dayString + " " + monthString + " " + yearString;
		return dateString;
	}
	
	public void SeededRunToggleUpdated()
	{
		seededGameWarningObject.SetActive(seededGameToggle.isOn);
		if(seededGameToggle.isOn)
		{
			SeedInputFieldUpdated();
		}
		else
		{
			if(Decks.instance.decks[Decks.instance.lastSelectedDeck].unlocked)
			{
				deckPickerPlayButton.ChangeDisabled(false);
			}
		}
	}
	
	public void SeedInputFieldUpdated()
	{
		if(seedInputField.text == "" || !Decks.instance.decks[Decks.instance.lastSelectedDeck].unlocked)
		{
			deckPickerPlayButton.ChangeDisabled(true);
		}
		else
		{
			deckPickerPlayButton.ChangeDisabled(false);
		}
	}
	
	public IEnumerator SetupNewGame(bool daily, int deck)
	{
		while(NewGameAnimation.instance.newGameAnimationStage < 1)
		{
			yield return null;
		}
		DeckViewer.instance.UpdateDeckType(deck);
		BaubleScript.instance.ResetHandsToShow();
		BaubleScript.instance.ResetBaublesToShow();
		// BaubleScript.instance.baseDiscardsPerAnte = 3;
		// BaubleScript.instance.baseHandsUntilFatigue = 3;
		SpecialOptions.instance.ApplySpecialOptions(5);
		if(deck == 0)
		{
			BaubleScript.instance.baseDiscardsPerAnte++;
			BaubleScript.instance.baseHandsUntilFatigue++;
		}
		else if(deck == 4)
		{
			BaubleScript.instance.baseDiscardsPerAnte += 4;
			BaubleScript.instance.baseHandsUntilFatigue = 0;
		}
		/* if(RunVariations.instance.hardModeToggle.isOn)
		{
			if(RunVariations.instance.smallerShopSelectionToggle.isOn)
			{
				BaubleScript.instance.baseItemsInShop = 3;
				BaubleScript.instance.baubles[22].maxQuantity = 4;
			}
			else
			{
				BaubleScript.instance.baseItemsInShop = 4;
				BaubleScript.instance.baubles[22].maxQuantity = 3;
			}
		}
		else
		{
			BaubleScript.instance.baseItemsInShop = 4;
			BaubleScript.instance.baubles[22].maxQuantity = 3;
		} */
		
		int seed = 0;
		bool seededGame = false;
		if(daily)
		{
			seed = dailySeed;
		}
		else
		{
			if(seededGameToggle.isOn)
			{
				long longSeed = long.Parse(seedInputField.text);
				seed = (int)Math.Abs(longSeed) % (int.MaxValue - 1);
				seededGame = true;
			}
			else
			{
				seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
				seed += System.DateTime.Now.Millisecond;
				seed = Mathf.Abs(seed) % (int.MaxValue - 1);
			}
		}
		//print("setting seed to " + seed);
		randomNumbers.ChangeSeed(seed);
		string runVariations = "";
		if(RunVariations.instance.variantModeToggle.isOn)
		{
			if(daily)
			{
				runVariations = dailyVariant;
			}
			else
			{
				runVariations = RunVariations.instance.GetCurrentRunVariantStringWithoutName();
			}
		}
		DeckViewer.instance.variantInUse = runVariations.Trim();
		Statistics.instance.ResetCurrentRun(seed, dateToday, daily, seededGame, deck, runVariations.Trim());
		SpecialOptions.instance.ApplySpecialOptions(4);
		handValues.ResetGame();
		HandScript.instance.lowOnCards = false;
		HandScript.instance.deckBackdropImage.color = Color.red;
		if(deck == 4)
		{
			//BaubleScript.instance.baubles[15].canAppearInShop = false;
			BaubleScript.instance.baubles[19].canAppearInShop = false;
			/* HandScript.instance.infinityDiscardsSymbol.SetActive(true);
			for(int i = 0; i < HandScript.instance.discardsRemainingTexts.Length; i++)
			{
				HandScript.instance.discardsRemainingTexts[i].gameObject.SetActive(false);
			} */
		}
		/* else
		{
			for(int i = 0; i < HandScript.instance.discardsRemainingTexts.Length; i++)
			{
				HandScript.instance.discardsRemainingTexts[i].gameObject.SetActive(true);
			}
			HandScript.instance.infinityDiscardsSymbol.SetActive(false);
		} */
		Statistics.instance.handsPlayedInFatigueThisGame = 0;
		/* int seed = 0;
		bool seededGame = false;
		if(daily)
		{
			seed = dailySeed;
		}
		else
		{
			if(seededGameToggle.isOn)
			{
				long longSeed = long.Parse(seedInputField.text);
				seed = (int)Math.Abs(longSeed) % (int.MaxValue - 1);
				seededGame = true;
			}
			else
			{
				seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
				seed += System.DateTime.Now.Millisecond;
				seed = Mathf.Abs(seed) % (int.MaxValue - 1);
			}
		}
		//print("setting seed to " + seed);
		randomNumbers.ChangeSeed(seed); */
		
		yield return null;
		Decks.instance.UpdateDecksFile(); // to make sure it remembers last selected deck
		Statistics.instance.gameStatsRT.gameObject.SetActive(false);
		handScript.drawPile.cardBackSprite = Decks.instance.decks[deck].design;
		handScript.drawPile.cardBackColors[0] = Decks.instance.decks[deck].designColor;
		if(deck == 2)
		{
			handScript.drawPile.CreateDeck(65);
			//handScript.DrawPileCountChanged(65);
		}
		else if (deck == 3)
		{
			handScript.drawPile.CreateDeck(52, true);
			//handScript.DrawPileCountChanged(40);
		}
		else
		{
			handScript.drawPile.CreateDeck(52);
			//handScript.DrawPileCountChanged(52);
		}
		handScript.drawPile.ShuffleDeck();
		
		if(RunVariations.instance.variantModeToggle.isOn)
		{
			// handValues.SetupAntes(Mathf.RoundToInt(RunVariations.instance.difficultySlider.value));
			AnteAdjustments.instance.SetupCustomAntes();
		}
		else
		{
			handValues.SetupAntes();
		}
		handValues.currentAnte = 0;
		BossAntes.instance.SetupBossAntes();
		handValues.scoreVial.numberOfThresholds = 5;
		SpecialOptions.instance.ApplySpecialOptions(0);
		SpecialOptions.instance.ApplySpecialOptions(1);
		handScript.StartCoroutine(handScript.StartGame(1f, startNextGameInShop));
		if(deck == 6)
		{
			handValues.scoreVial.numberOfThresholds++;
		}
		handValues.scoreVial.SetupNewBlind(0.2f, handValues.antes[handValues.currentAnte], handValues.currentAnte);
		handValues.scoreVial.ScoreUpdated(0, false, true);
		handValues.scoreVial.allThresholdsInARow = 0;
		handValues.UpdateHandsUntilFatigue(0, true);
		handValues.scoreVial.MoneyChanged(-handValues.scoreVial.currentMoney);
		baubleCollection.ResetBaubleCollection();
		SpecialOptions.instance.ApplySpecialOptions(2);
		if(RunVariationsToggle.isOn)
		{
			BaubleMutators.instance.ApplyBaubleMutators();
		}
		else
		{
			BaubleMutators.instance.ResetBaublesInPool();
		}
		SpecialOptions.instance.ApplySpecialOptions(3);
		handScript.SelectedCardsUpdated();
		if(deck == 1)
		{
			ShopScript.instance.CheatAddBauble(34, Vector2.zero, false);
			handValues.scoreVial.MoneyChanged(10);
		}
/* 		else
		{
			handValues.scoreVial.MoneyChanged(5);
		} */
		if(deck == 5)
		{
			ShopScript.instance.CheatAddBauble(41, Vector2.zero, false);
			ShopScript.instance.CheatAddBauble(41, Vector2.zero, false);
		}
		if(deck == 6)
		{
			ShopScript.instance.CheatAddBauble(60, Vector2.zero, false);
			BaubleScript.instance.baubles[34].canAppearInShop = false;
			ShopScript.instance.interestTooltipTrigger.SetActive(false);
		}
		deckViewer.isOpen = false;
		optionsMenu.SetActive(true);
	}
	
	public void BeginRandomGame()
	{
		int deck = Decks.instance.lastSelectedDeck;
		playCanvas.SetActive(true);
		StartCoroutine(SetupNewGame(false, deck));
		RevealPlayArea();
	}
	
	public void BeginDailyGame()
	{
		playCanvas.SetActive(true);
		StartCoroutine(SetupNewGame(true, dailyDeck));
		RevealPlayArea();
	}
	
	public void RevealPlayArea()
	{
		StartCoroutine(MoveOverTime(handZonesRT, new Vector2(0,360), new Vector2(0,0), 1f, 0));
		StartCoroutine(MoveOverTime(handRT, new Vector2(95,-205), new Vector2(95,5), 1f, 0));
		StartCoroutine(MoveOverTime(scoreAreaRT, new Vector2(100,0), new Vector2(0,0), 1f, 0));
		StartCoroutine(MoveOverTime(discardsRemainingRT, new Vector2(-65,99), new Vector2(4,99), 1f, 0));
		StartCoroutine(MoveOverTime(anteInfoRT, new Vector2(-65,161), new Vector2(-16,161), 1f, 0));
		StartCoroutine(MoveOverTime(moneyCountRT, new Vector2(-65,135), new Vector2(4,135), 1f, 0));
		StartCoroutine(MoveOverTime(fatigueInfoRT, new Vector2(-65,251), new Vector2(4,251), 1f, 0));
		StartCoroutine(MoveOverTime(discardPileRT, new Vector2(5,5), new Vector2(-56,5), 1f, 0));
		StartCoroutine(MoveOverTime(drawPileRT, new Vector2(-55,5), new Vector2(5,5), 1f, 0));
		StartCoroutine(MoveOverTime(menuButtonRT, new Vector2(-65,327), new Vector2(4,327), 1f, 0));
		StartCoroutine(MoveOverTime(offscreenObjectsRT, new Vector2(-70,0), new Vector2(0,0), 1f, 0, TurnOffMainMenuCanvas));
		if(!GameOptions.instance.tutorialDone)
		{
			Tutorial.instance.RevealTutorial();
		}
	}
	
	public void HidePlayArea()
	{
		StartCoroutine(MoveOverTime(handZonesRT, handZonesRT.anchoredPosition, new Vector2(0,360), 1f, 0));
		StartCoroutine(MoveOverTime(handRT, handRT.anchoredPosition, new Vector2(95,-205), 1f, 0));
		StartCoroutine(MoveOverTime(scoreAreaRT, new Vector2(0,0), new Vector2(100,0), 1f, 0));
		StartCoroutine(MoveOverTime(discardsRemainingRT, new Vector2(4,99), new Vector2(-65,99), 1f, 0));
		StartCoroutine(MoveOverTime(anteInfoRT, new Vector2(-16,161), new Vector2(-65,161), 1f, 0));
		StartCoroutine(MoveOverTime(moneyCountRT, new Vector2(4,135), new Vector2(-65,135), 1f, 0));
		StartCoroutine(MoveOverTime(fatigueInfoRT, new Vector2(4,251), new Vector2(-65,251), 1f, 0));
		StartCoroutine(MoveOverTime(discardPileRT, new Vector2(-56,5), new Vector2(5,5), 1f, 0));
		StartCoroutine(MoveOverTime(drawPileRT, new Vector2(5,5), new Vector2(-55,5), 1f, 0));
		StartCoroutine(MoveOverTime(menuButtonRT, new Vector2(5,327), new Vector2(-65,327), 1f, 0));
		StartCoroutine(MoveOverTime(offscreenObjectsRT, new Vector2(0,0), new Vector2(-70,0), 1f, 0));
		StartCoroutine(MoveOverTime(shopRT, shopRT.anchoredPosition, new Vector2(0,360), 1f, 0));
		if(Statistics.instance.gameStatsRT.gameObject.activeSelf)
		{
			StartCoroutine(MoveOverTime(Statistics.instance.gameStatsRT, Statistics.instance.gameStatsRT.anchoredPosition, new Vector3(98, -360, 0), 1f, 0));
		}
		StartCoroutine(MoveOverTime(deckViewerRT, deckViewerRT.anchoredPosition, new Vector2(0,360), 1f, 0, RevealMainMenu));
	}
	
	public void TurnOffMainMenuCanvas()
	{
		mainMenuCanvas.SetActive(false);
	}

	public void FollowMeClicked()
	{
		Application.OpenURL("https://twitter.com/SilverDubloons");
	}
	
    public IEnumerator MoveOverTime(RectTransform rt, Vector2 startPosition, Vector2 endPosition, float moveTime, float delayTime, CallbackFunction endFunction = null)
	{
		float t = 0;
		while(t < delayTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		t = 0;
		while(t < moveTime)
		{
			t += Time.deltaTime;
			rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, moveCurve.Evaluate(t / moveTime));
			yield return null;
		}
		rt.anchoredPosition = endPosition;
		if(endFunction != null)
		{
			endFunction();
		}
	}
}
