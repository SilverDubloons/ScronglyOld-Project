using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

public class Decks : MonoBehaviour
{
	public static Decks instance;
	public GameObject deckKnobPrefab;
	public Sprite lockedKnob;
	public Color lockedColor;
	public Sprite unlockedKnob;
	public Color unlockedColor;
	public Color selectedColor;
	public Transform deckKnobParent;
	public List<DeckKnob> DeckKnobs = new List<DeckKnob>();
	public int lastSelectedDeck;
	
	public Image cardBackDetail;
	public Image cardBackground;
	public TMP_Text[] deckNameTexts;
	public TMP_Text[] deckDescriptionTexts;
	public MovingButton playButton;
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		LoadDecks();
		SetupSelectableDecks();
	}
	
	private void AssignDeckNumbers()
	{
		for(int i = 0; i < decks.Length; i++)
		{
			decks[i].deckInt = i;
		}
	}
	
	[System.Serializable]
    public class Deck
	{
		public string deckName;
		public string description;
		public string howToUnlock;
		public Sprite design;
		public Color designColor;
		public Color backColor;
		public bool unlocked;
		public int deckInt;
	}
	
	public Deck[] decks;
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.N))
		{
			if(GameOptions.instance.cheatsOn)
			{
				Decks.instance.UnlockAllDecks();
			}
		}
	}
	
	public void UnlockAllDecks()
	{
		//print("unlocking decks");
		for(int i = 1; i < decks.Length; i++)
		{
			Decks.instance.decks[i].unlocked = true;
			Decks.instance.DeckKnobs[i].knobImage.sprite = Decks.instance.unlockedKnob;
			Decks.instance.DeckKnobs[i].rt.sizeDelta = new Vector2(10,10);
		}
		Decks.instance.UpdateDecksFile();
	}
	
	public void LoadDecks()
	{
		//string decksPath = Application.persistentDataPath + "/" + "decks" + ".txt";
		string decksPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			decksPath = "/idbfs/ScronglyData/" + "decks" + ".txt";
		#else
			decksPath = Application.persistentDataPath + "/" + "decks" + ".txt";
		#endif
		#if UNITY_WEBGL && !UNITY_EDITOR
			if(!Directory.Exists("/idbfs/ScronglyData"))
			{
				Directory.CreateDirectory("/idbfs/ScronglyData");
			}
		#endif
		if(File.Exists(decksPath))
		{
			try
			{
				using (StreamReader reader = new StreamReader(decksPath))
				{
					string decksData = reader.ReadToEnd();
					string[] lines = decksData.Split('\n');
					string fileManagerVersion = lines[0].Trim();
					if(fileManagerVersion != GameOptions.instance.currentFileManagerVersion)
					{
						//GameOptions.instance.fileManagerWarningInterface.SetActive(true);
						GameOptions.instance.SetupFileManagerWarningInterface(decksPath);
						ResetDecksFile(decksPath);
						Debug.Log("Trying to load a version \"" + lines[0] + "\" decks. Your version is \"" + GameOptions.instance.currentFileManagerVersion + "\"");
						return;
					}
					lastSelectedDeck = int.Parse(lines[1].Trim());
					for(int i = 0; i < decks.Length; i++)
					{
						decks[i].unlocked = bool.Parse(lines[i + 2].Replace(i + "=", ""));
					}
				}
			}
			catch(Exception exception)
			{
				ResetDecksFile(decksPath);
				Debug.Log("An error occurred when loading " + decksPath + ": " + exception.Message);
				return;
			}
		}
		else
		{
			ResetDecksFile(decksPath);
		}
	}
	
	public void ResetDecksFile(string decksPath)
	{
		#if UNITY_WEBGL && !UNITY_EDITOR
		if(!Directory.Exists("/idbfs/ScronglyData"))
		{
			Directory.CreateDirectory("/idbfs/ScronglyData");
		}
		#endif
		File.WriteAllText(decksPath, "");
		StreamWriter writer = new StreamWriter(decksPath, true);
		writer.WriteLine(GameOptions.instance.currentFileManagerVersion);
		writer.WriteLine("" + lastSelectedDeck);
		for(int i = 0; i < decks.Length - 1; i++)
		{
			writer.WriteLine("" + i + "=" + decks[i].unlocked);
		}
		writer.Write("" + (decks.Length - 1) + "=" + decks[decks.Length -1].unlocked);
		writer.Close();
		Statistics.instance.FileUpdated();
	}
	
	public void UpdateDecksFile()
	{
		string decksPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			decksPath = "/idbfs/ScronglyData/" + "decks" + ".txt";
		#else
			decksPath = Application.persistentDataPath + "/" + "decks" + ".txt";
		#endif
		#if UNITY_WEBGL && !UNITY_EDITOR
			if(!Directory.Exists("/idbfs/ScronglyData"))
			{
				Directory.CreateDirectory("/idbfs/ScronglyData");
			}
		#endif
		File.WriteAllText(decksPath, "");
		StreamWriter writer = new StreamWriter(decksPath, true);
		writer.WriteLine(GameOptions.instance.currentFileManagerVersion);
		writer.WriteLine("" + lastSelectedDeck);
		for(int i = 0; i < decks.Length - 1; i++)
		{
			writer.WriteLine("" + i + "=" + decks[i].unlocked);
		}
		writer.Write("" + (decks.Length - 1) + "=" + decks[decks.Length -1].unlocked);
		writer.Close();
		Statistics.instance.FileUpdated();
	}
	
	public void SetupSelectableDecks()
	{
		float maxWidth = 88;
		float knobWidth = 10;
		float squeezeDistance = (maxWidth - knobWidth) / (decks.Length - 1);
		float distanceBetweenKnobs = Mathf.Min(14f, squeezeDistance);
		for(int i = 0; i < decks.Length; i++)
		{
			GameObject newKnob = Instantiate(deckKnobPrefab, new Vector3(0,0,0), Quaternion.identity, deckKnobParent);
			DeckKnob newDeckKnob = newKnob.GetComponent<DeckKnob>();
			DeckKnobs.Add(newDeckKnob);
			float xDestination = (decks.Length - 1) * (distanceBetweenKnobs / 2f) - (decks.Length - i - 1) * distanceBetweenKnobs;
			newDeckKnob.rt.anchoredPosition = new Vector2(xDestination, 0);
			if(decks[i].unlocked)
			{
				newDeckKnob.knobImage.sprite = unlockedKnob;
				newDeckKnob.knobImage.color = unlockedColor;
				newDeckKnob.rt.sizeDelta = new Vector2(10,10);
			}
			else
			{
				newDeckKnob.knobImage.sprite = lockedKnob;
				newDeckKnob.knobImage.color = lockedColor;
				newDeckKnob.rt.sizeDelta = new Vector2(8,10);
			}
		}
		ChangeSelectedDeck(lastSelectedDeck, true);
	}
	
	public void ChangeSelectedDeck(int deck, bool setup = false)
	{
		DeckKnobs[deck].knobImage.color = selectedColor;
		if(decks[deck].unlocked)
		{
			if(!setup)
			{
				MainMenu.instance.SeededRunToggleUpdated();
			}
			for(int i = 0; i < deckNameTexts.Length; i++)
			{
				deckNameTexts[i].text = decks[deck].deckName;
			}
			cardBackDetail.gameObject.SetActive(true);
			cardBackground.gameObject.SetActive(true);
			cardBackDetail.sprite = decks[deck].design;
			cardBackDetail.color = decks[deck].designColor;
			cardBackground.color = decks[deck].backColor;
			for(int i = 0; i < deckDescriptionTexts.Length; i++)
			{
				deckDescriptionTexts[i].text = decks[deck].description;
			}
		}
		else
		{
			playButton.ChangeDisabled(true);
			for(int i = 0; i < deckNameTexts.Length; i++)
			{
				deckNameTexts[i].text = "To Unlock:";
			}
			cardBackDetail.gameObject.SetActive(false);
			cardBackground.gameObject.SetActive(false);
			for(int i = 0; i < deckDescriptionTexts.Length; i++)
			{
				deckDescriptionTexts[i].text = decks[deck].howToUnlock;
			}
		}
	}
	
	public void DeckRightClicked()
	{
		if(decks[lastSelectedDeck].unlocked)
		{
			DeckKnobs[lastSelectedDeck].knobImage.color = unlockedColor;
		}
		else
		{
			DeckKnobs[lastSelectedDeck].knobImage.color = lockedColor;
		}
		lastSelectedDeck++;
		if(lastSelectedDeck > decks.Length - 1)
		{
			lastSelectedDeck = 0;
		}
		ChangeSelectedDeck(lastSelectedDeck);
	}
	
	public void DeckLeftClicked()
	{
		if(decks[lastSelectedDeck].unlocked)
		{
			DeckKnobs[lastSelectedDeck].knobImage.color = unlockedColor;
		}
		else
		{
			DeckKnobs[lastSelectedDeck].knobImage.color = lockedColor;
		}
		lastSelectedDeck--;
		if(lastSelectedDeck < 0)
		{
			lastSelectedDeck = decks.Length - 1;
		}
		ChangeSelectedDeck(lastSelectedDeck);
	}
}
