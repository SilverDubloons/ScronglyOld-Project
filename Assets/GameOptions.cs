using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using TMPro;

public class GameOptions : MonoBehaviour
{
    public bool onlyShowBaseValueIfModified = true;
	public bool showCommonTooltips = true;
	public bool cheatsOn = false;
	public bool tutorialDone = false;
	public bool rotatingBackground = true;
	public GameObject cheatMenu;
	public int gameSpeed = 0;
	public float gameSpeedFactor = 1;
	public bool restrictSelection = false;
	
	public bool alreadySeenLossConditionTooltip = false;
	
	public bool askToDeleteOldLayaway = true;
	
	public Slider gameSpeedSlider;
	public Toggle onlyShowBaseValueIfModifiedToggle;
	public Toggle showCommonTooltipsToggle;
	public Toggle cheatsToggle;
	public GameObject menuCheats;
	public Toggle rotatingBackgroundToggle;
	public Toggle soundOnToggle;
	public Slider soundVolumeSlider;
	public Toggle musicOnToggle;
	public Slider musicVolumeSlider;
	public Toggle restrictSelectionToggle;
	
	public string currentFileManagerVersion;
	
	public GameObject MainMenuCanvas;
	public MainMenu mainMenu;
	public ScoreVial scoreVial;
	//public Statistics statistics;
	public static GameOptions instance;
	public MovingButton resetTutorialButton;
	public MovingButton testSoundButton;
	
	public float soundVolume = 0.25f;
	public bool soundOn = true;
	public float musicVolume = 0.25f;
	public bool musicOn = true;
	public GameObject fileManagerWarningInterface;
	public TMP_Text[] fileManagerWarningInterfaceErrorTexts;
	
	public void SetupFileManagerWarningInterface(string error)
	{
		fileManagerWarningInterface.SetActive(true);
		for(int i = 0; i < fileManagerWarningInterfaceErrorTexts.Length; i++)
		{
			fileManagerWarningInterfaceErrorTexts[i].text = "Error: " + error;
		}
	}
	
	public void ResetTutorial()
	{
		Tutorial.instance.SetupTutorial();
		tutorialDone = false;
		alreadySeenLossConditionTooltip = false;
		resetTutorialButton.ChangeDisabled(true);
		UpdateOptionsFile();
	}
	
	public void ResetAllData()
	{
		string dailyRunsPath = "";
		string gameOptionsPath = "";
		string bestRunsPath = "";
		string decksPath = "";
		string runVariantsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			dailyRunsPath = "/idbfs/ScronglyData/" + "dailyRuns" + ".txt";
			gameOptionsPath = "/idbfs/ScronglyData/" + "gameOptions" + ".txt";
			bestRunsPath = "/idbfs/ScronglyData/" + "bestRuns" + ".txt";
			decksPath = "/idbfs/ScronglyData/" + "decks" + ".txt";
			runVariantsPath = "/idbfs/ScronglyData/" + "runVariants" + ".txt";
		#else
			dailyRunsPath = Application.persistentDataPath + "/" + "dailyRuns" + ".txt";
			gameOptionsPath = Application.persistentDataPath + "/" + "gameOptions" + ".txt";
			bestRunsPath = Application.persistentDataPath + "/" + "bestRuns" + ".txt";
			decksPath = Application.persistentDataPath + "/" + "decks" + ".txt";
			runVariantsPath = Application.persistentDataPath + "/" + "runVariants" + ".txt";
		#endif
		Statistics.instance.ResetDailyRunsFile(dailyRunsPath);
		Statistics.instance.ResetBestRunsFile(bestRunsPath);
		Decks.instance.ResetDecksFile(decksPath);
		RunVariations.instance.ResetRunVariantsFile();
		ResetGameOptionsFile();
		File.Delete(dailyRunsPath);
		File.Delete(gameOptionsPath);
		File.Delete(bestRunsPath);
		File.Delete(decksPath);
		File.Delete(runVariantsPath);
	}
	
	public void AbandonRunClicked()
	{
		MainMenuCanvas.SetActive(true);
		Statistics.instance.mainMenuButton.ChangeDisabled(true);
		if(scoreVial.gameObject.activeInHierarchy)
		{
			scoreVial.ScoreUpdated(-scoreVial.currentScore, false, true);
			mainMenu.HidePlayArea();
		}
		else
		{
			Statistics.instance.DisableOtherStatsButtons();
			mainMenu.StartCoroutine(mainMenu.MoveOverTime(Statistics.instance.gameStatsRT, new Vector3(98, 0, 0), new Vector3(98, -360, 0), 1, 0));
			mainMenu.RevealMainMenu();
		}
	}
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		//DontDestroyOnLoad(gameObject);
		//string gameOptionsPath = Application.persistentDataPath + "/" + "gameOptions" + ".txt";
		string gameOptionsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			gameOptionsPath = "/idbfs/ScronglyData/" + "gameOptions" + ".txt";
		#else
			gameOptionsPath = Application.persistentDataPath + "/" + "gameOptions" + ".txt";
		#endif
		if(File.Exists(gameOptionsPath))
		{
			try
			{
				using (StreamReader reader = new StreamReader(gameOptionsPath))
				{
					string optionsData = reader.ReadToEnd();
					string[] lines = optionsData.Split('\n');
					string fileManagerVersion = lines[0].Trim();
					if(fileManagerVersion == currentFileManagerVersion)
					{
						gameSpeed = int.Parse(lines[1].Replace("gameSpeed = ", ""));
						gameSpeedSlider.value = gameSpeed;
						//Time.timeScale = Mathf.Pow(2f, gameSpeed);
						gameSpeedFactor = Mathf.Pow(2f, gameSpeed);
						onlyShowBaseValueIfModified = bool.Parse(lines[2].Replace("onlyShowBaseValueIfModified = ", ""));
						onlyShowBaseValueIfModifiedToggle.isOn = onlyShowBaseValueIfModified;
						showCommonTooltips = bool.Parse(lines[3].Replace("showCommonTooltips = ", ""));
						showCommonTooltipsToggle.isOn = showCommonTooltips;
						cheatsOn = bool.Parse(lines[4].Replace("cheatsOn = ", ""));
						menuCheats.SetActive(cheatsOn);
						cheatsToggle.isOn = cheatsOn;
						cheatMenu.SetActive(cheatsOn);
						tutorialDone = bool.Parse(lines[5].Replace("tutorialDone = ", ""));
						rotatingBackground = bool.Parse(lines[6].Replace("rotatingBackground = ", ""));
						rotatingBackgroundToggle.isOn = rotatingBackground;
						soundOn = bool.Parse(lines[7].Replace("soundOn = ", ""));
						soundOnToggle.isOn = soundOn;
						testSoundButton.ChangeDisabled(!soundOn);
						soundVolume = float.Parse(lines[8].Replace("soundVolume = ", ""));
						soundVolumeSlider.value = soundVolume;
						musicOn = bool.Parse(lines[9].Replace("musicOn = ", ""));
						musicOnToggle.isOn = musicOn;
						musicVolume = float.Parse(lines[10].Replace("musicVolume = ", ""));
						musicVolumeSlider.value = musicVolume;
						MusicManager.instance.MusicOptionsUpdated();
						alreadySeenLossConditionTooltip = bool.Parse(lines[11].Replace("alreadySeenLossConditionTooltip = ", ""));
						restrictSelection = bool.Parse(lines[12].Replace("restrictSelection = ", ""));
						restrictSelectionToggle.isOn = restrictSelection;
					}
					else // original was 22.3.2024
					{
						
						//fileManagerWarningInterface.SetActive(true);
						SetupFileManagerWarningInterface(gameOptionsPath);
						Debug.Log("Trying to load a version \"" + lines[0] + "\" gameOptions. Your version is \"" + currentFileManagerVersion + "\"");
						UpdateOptionsFile();
					}
				}
			}
			catch(Exception exception)
			{
				Debug.Log("An error occurred when loading " + gameOptionsPath + ": " + exception.Message);
				UpdateOptionsFile();
			}
		}
		else
		{
			UpdateOptionsFile();
			gameSpeedSlider.value = gameSpeed;
			soundVolumeSlider.value = soundVolume;
			testSoundButton.ChangeDisabled(!soundOn);
			musicVolumeSlider.value = musicVolume;
			gameSpeedFactor = Mathf.Pow(2f, gameSpeed);
			onlyShowBaseValueIfModifiedToggle.isOn = onlyShowBaseValueIfModified;
			showCommonTooltipsToggle.isOn = showCommonTooltips;
			rotatingBackgroundToggle.isOn = rotatingBackground;
			soundOnToggle.isOn = soundOn;
			musicOnToggle.isOn = musicOn;
			cheatsToggle.gameObject.SetActive(cheatsOn);
			MusicManager.instance.MusicOptionsUpdated();
			cheatMenu.SetActive(cheatsOn);
			restrictSelectionToggle.isOn = restrictSelection;
		}
		if(!tutorialDone)
		{
			resetTutorialButton.ChangeDisabled(true);
			Tutorial.instance.SetupTutorial();
		}
		else
		{
			resetTutorialButton.ChangeDisabled(false);
			Tutorial.instance.tutorialGameObject.SetActive(false);
		}
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.J) && Input.GetKeyDown(KeyCode.Q))
		{
			menuCheats.SetActive(true);
		}
	}
	
	public void UpdateOptionsFile()
	{
		string gameOptionsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			gameOptionsPath = "/idbfs/ScronglyData/" + "gameOptions" + ".txt";
		#else
			gameOptionsPath = Application.persistentDataPath + "/" + "gameOptions" + ".txt";
		#endif
		#if UNITY_WEBGL && !UNITY_EDITOR
			if(!Directory.Exists("/idbfs/ScronglyData"))
			{
				Directory.CreateDirectory("/idbfs/ScronglyData");
			}
		#endif
		if(File.Exists(gameOptionsPath))
		{
			File.WriteAllText(gameOptionsPath, "");
		}
		StreamWriter writer = new StreamWriter(gameOptionsPath, true);
		writer.WriteLine(currentFileManagerVersion);
		writer.WriteLine("gameSpeed = " + gameSpeed);
		writer.WriteLine("onlyShowBaseValueIfModified = " + onlyShowBaseValueIfModified);
		writer.WriteLine("showCommonTooltips = " + showCommonTooltips);
		writer.WriteLine("cheatsOn = " + cheatsOn);
		writer.WriteLine("tutorialDone = " + tutorialDone);
		writer.WriteLine("rotatingBackground = " + rotatingBackground);
		writer.WriteLine("soundOn = " + soundOn);
		writer.WriteLine("soundVolume = " + soundVolume);
		writer.WriteLine("musicOn = " + musicOn);
		writer.WriteLine("musicVolume = " + musicVolume);
		writer.WriteLine("alreadySeenLossConditionTooltip = " + alreadySeenLossConditionTooltip);
		writer.WriteLine("restrictSelection = " + restrictSelection);
		writer.Close();
		//Application.ExternalCall("SyncFiles"); 
		MusicManager.instance.MusicOptionsUpdated();
		Statistics.instance.FileUpdated();
	}
	
	public void ResetGameOptionsFile()
	{
		gameSpeed = 0;
		gameSpeedFactor = 1;
		onlyShowBaseValueIfModified = true;
		showCommonTooltips = true;
		cheatsOn = false;
		tutorialDone = false;
		rotatingBackground = true;
		soundOn = true;
		soundVolume = 0.25f;
		musicOn = true;
		musicVolume = 0.25f;
		alreadySeenLossConditionTooltip = false;
		restrictSelection = false;
		UpdateOptionsFile();
	}
	
	public void GameSpeedSliderUpdated()
	{
		gameSpeed = (int)gameSpeedSlider.value;
		gameSpeedFactor = Mathf.Pow(2f, gameSpeedSlider.value);
	}
	
	public void SoundVolumeSliderUpdated()
	{
		//SoundManager.instance.PlayChipSound();
		soundVolume = soundVolumeSlider.value;
		if(Mathf.Abs(soundVolume) <= 0.0001f)
		{
			soundOnToggle.isOn = false;
			soundVolumeSlider.interactable = false;
			testSoundButton.ChangeDisabled(!soundOn);
		}
	}
	
	public void MusicVolumeSliderUpdated()
	{
		musicVolume = musicVolumeSlider.value;
		if(Mathf.Abs(musicVolume) <= 0.0001f)
		{
			musicOnToggle.isOn = false;
			musicVolumeSlider.interactable = false;
		}
		MusicManager.instance.MusicOptionsUpdated();
	}
	
	public void GameOptionToggleUpdated()
	{
		onlyShowBaseValueIfModified = onlyShowBaseValueIfModifiedToggle.isOn;
		showCommonTooltips = showCommonTooltipsToggle.isOn;
		cheatsOn = cheatsToggle.isOn;
		cheatMenu.SetActive(cheatsOn);
		rotatingBackground = rotatingBackgroundToggle.isOn;
		soundOn = soundOnToggle.isOn;
		SoundManager.instance.PlayButtonSound();
		testSoundButton.ChangeDisabled(!soundOn);
		if(soundOn)
		{
			soundVolumeSlider.interactable = true;
			if(Mathf.Abs(soundVolume) <= 0.0001f)
			{
				soundVolume = 0.1f;
				soundVolumeSlider.value = soundVolume;
			}
		}
		else
		{
			soundVolumeSlider.interactable = false;
		}
		musicOn = musicOnToggle.isOn;
		if(musicOn)
		{
			musicVolumeSlider.interactable = true;
			if(Mathf.Abs(musicVolume) <= 0.0001f)
			{
				musicVolume = 0.1f;
				musicVolumeSlider.value = musicVolume;
			}
		}
		else
		{
			musicVolumeSlider.interactable = false;
		}
		MusicManager.instance.MusicOptionsUpdated();
		restrictSelection = restrictSelectionToggle.isOn;
		if(restrictSelection)
		{
			HandScript.instance.maxCardsSelectedAtOnce = BaubleScript.instance.baubles[41].quantityOwned + 5;
		}
		else
		{
			HandScript.instance.maxCardsSelectedAtOnce = 9001;
		}
		HandScript.instance.SelectedCardsUpdated();
		HandScript.instance.ShouldSelectAllBeEnabled();
	}
}
