using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using TMPro;

public class RunVariations : MonoBehaviour
{
	public static RunVariations instance;
	public Toggle variantModeToggle;
	public RectTransform variantModeBackdrop;
	public GameObject hiddenVariantModeOptions;
	public GameObject variantModeWarning;
	public GameObject importInterface;
	//public GameObject saveInterface;
	public TMP_InputField importInputField;
	public TMP_Dropdown variantsDropdown;
	public string[] dailyVariants;
	public List<RunVariant> savedRunVariants = new List<RunVariant>();
	public MovingButton saveButton;
	public MovingButton importButton;
	public MovingButton deleteButton;
	public MovingButton exportButton;
	//public bool variantHasBeenChanged;
	public string currentVariantsManagerVersion;
	public TMP_Text[] deleteVariantNameTexts;
	public TMP_InputField exportInputField;
	public GameObject importFailedInterface;
	public GameObject variantDisplayPrefab;
	public Transform deckPickerVariantDisplayParent;
	public Transform dailyVariantDisplayParent;
	public MovingButton saveDailyVariantButton;
	//Double Stuffed|Start with an extra pack of cards and Double hand size. -3 discards, -3 hands until fatigue.|9:0_10:0_11:14||0:1_1:1_2:1_3:1_4:1_5:1_6:1_7:1_8:1_9:1_10:1_11:1_12:1_13:1_14:1_15:1_16:1_17:1_18:1_19:1_20:1_21:1_22:1_23:1_24:1_25:1_26:1_27:1_28:1_29:1_30:1_31:1_32:1_33:1_34:1_35:1_36:1_37:1_38:1_39:1_40:1_41:1_42:1_43:1_44:1_45:1_46:1_47:1_48:1_49:1_50:1_51:1|0:False:0:False|d:0
	
	// ||0:-1_1:-1_2:-1_3:-1_4:-1_5:-1_6:-1_7:-1_8:-1_9:-1_10:-1_11:-1_12:-1_13:-1_14:-1_15:-1_16:-1_17:-1_18:-1_19:-1_20:-1_21:-1_22:-1_23:-1_24:-1_25:-1_26:-1_27:-1_28:-1_29:-1_30:-1_31:-1_32:-1_33:-1_34:-1_35:-1_36:-1_37:-1_38:-1_39:-1_40:-1_41:-1_42:-1_43:-1_44:-1_45:-1_46:-1_47:-1_48:-1_49:-1_50:-1_51:-1|52:False:0:False
	// ^ 52 random cards instead of typical deck
	
	// ||0:10_1:10_2:10_3:10_4:10_5:10_6:10_7:10_8:10_9:10_10:10_11:10_12:10_13:10_14:10_15:10_16:10_17:10_18:10_19:10_20:10_21:10_22:10_23:10_24:10_25:10_26:10_27:10_28:10_29:10_30:10_31:10_32:10_33:10_34:10_35:10_36:10_37:10_38:10_39:10_40:10_41:10_42:10_43:10_44:10_45:10_46:10_47:10_48:10_49:10_50:10_51:10_52:10_53:10_54:10_55:10_56:10_57:10_58:10_59:10_60:10_61:10_62:10_63:10_64:10_65:10_66:10_67:10_68:10_69:10_70:10_71:10_72:10_73:10_74:10|99:True:99:False
	// ^ the fattest deck possible

	[System.Serializable]
    public struct RunVariant
    {
        public string variantName;
        public string variantDescription;
		public string variantSpecialOptions;
		public string variantBaubles;
		public string variantCards;
		public string variantRandomCards;
		public string variantAnteAdjustments;
    }
	
	public void SaveDailyVariantButtonClicked()
	{
		saveDailyVariantButton.ChangeDisabled(true);
		RunVariant runVariant = ConvertStringToRunVariant(MainMenu.instance.dailyVariant);
		savedRunVariants.Add(runVariant);
		UpdateDropdown(false);
		UpdateRunVariantsFile();
	}
	
	public void SetupVariantDisplay(Transform displayParent, Vector2 location, string runVariantString, float height)
	{
		RunVariant runVariant = ConvertStringToRunVariant(runVariantString);
		SetupVariantDisplay(displayParent, location, runVariant, height);
	}
	
	public void SetupVariantDisplay(Transform displayParent, Vector2 location, RunVariant runVariant, float height)
	{
		LoadRunVariant(runVariant);
		for(int i = 0; i < displayParent.childCount; i++)
		{
			Destroy(displayParent.GetChild(i).gameObject);
		}
		GameObject newVariantDisplayObject = Instantiate(variantDisplayPrefab, Vector3.zero, Quaternion.identity, displayParent);
		VariantDisplay newVariantDisplay = newVariantDisplayObject.GetComponent<VariantDisplay>();
		newVariantDisplay.rt.anchoredPosition = location;
		newVariantDisplay.SetupVariantDisplay(runVariant.variantName, runVariant.variantDescription, runVariant.variantSpecialOptions, runVariant.variantBaubles, runVariant.variantCards, runVariant.variantRandomCards, runVariant.variantAnteAdjustments, height);
	}
	
	public void ClearVariantDisplay(Transform displayParent)
	{
		for(int i = 0; i < displayParent.childCount; i++)
		{
			Destroy(displayParent.GetChild(i).gameObject);
		}
	}
	
	public void LoadSavedRunVariants()
	{
		string runVariantsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			runVariantsPath = "/idbfs/ScronglyData/" + "runVariants" + ".txt";
		#else
			runVariantsPath = Application.persistentDataPath + "/" + "runVariants" + ".txt";
		#endif
		if(File.Exists(runVariantsPath))
		{
			try
			{
				using (StreamReader reader = new StreamReader(runVariantsPath))
				{
					string variantsData = reader.ReadToEnd();
					string[] lines = variantsData.Split('\n');
					string variantsManagerVersion = lines[0].Trim();
					if(variantsManagerVersion == currentVariantsManagerVersion)
					{
						for(int i = 1; i < lines.Length; i++)
						{
							RunVariant runVariant = ConvertStringToRunVariant(lines[i]);
							savedRunVariants.Add(runVariant);
						}
					}
					else
					{
						print("outdated run variants file");
					}
				}
			}
			catch(Exception exception)
			{
				print("blorp " + exception.Message);
			}
		}
		else
		{
			UpdateRunVariantsFile();
		}
	}
	
	public void ResetRunVariantsFile()
	{
		savedRunVariants = new List<RunVariant>();
		UpdateRunVariantsFile();
	}
	
	public void UpdateRunVariantsFile()
	{
		string runVariantsPath = "";
		#if UNITY_WEBGL && !UNITY_EDITOR
			runVariantsPath = "/idbfs/ScronglyData/" + "runVariants" + ".txt";
		#else
			runVariantsPath = Application.persistentDataPath + "/" + "runVariants" + ".txt";
		#endif
		#if UNITY_WEBGL && !UNITY_EDITOR
			if(!Directory.Exists("/idbfs/ScronglyData"))
			{
				Directory.CreateDirectory("/idbfs/ScronglyData");
			}
		#endif
		if(File.Exists(runVariantsPath))
		{
			File.WriteAllText(runVariantsPath, "");
		}
		StreamWriter writer = new StreamWriter(runVariantsPath, true);
		if(savedRunVariants.Count > 0)
		{
			writer.WriteLine(currentVariantsManagerVersion);
		}
		else
		{
			writer.Write(currentVariantsManagerVersion);
		}
		//print("updating runVariants.txt, savedRunVariants.Count= " + savedRunVariants.Count);
		for(int i = 0; i < savedRunVariants.Count; i++)
		{
			if(i < savedRunVariants.Count - 1)
			{
				writer.WriteLine(ConvertRunVariantToVariantString(savedRunVariants[i]).Trim());
			}
			else
			{
				writer.Write(ConvertRunVariantToVariantString(savedRunVariants[i]).Trim());
			}
		}
		writer.Close();
		Statistics.instance.FileUpdated();
	}
	
	public void AddNewSavedRunVariant(string variantName, string variantDescription)
	{
		string runVariantString = GetCurrentRunVariantString(variantName, variantDescription);
		RunVariant runVariant = ConvertStringToRunVariant(runVariantString);
		savedRunVariants.Add(runVariant);
		UpdateDropdown(false);
		variantsDropdown.value = savedRunVariants.Count - 2;
		//variantHasBeenChanged = false;
		UpdateRunVariantsFile();
	}
	
	void UpdateDropdown(bool custom)
	{
		variantsDropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
		for(int i = 0; i < savedRunVariants.Count; i++)
		{
			TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(savedRunVariants[i].variantName);
			options.Add(option);
		}
		if(custom)
		{
			TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData("Custom");
			options.Add(option);
		}
		else
		{
			TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData("New");
			options.Add(option);
		}
		variantsDropdown.options = options;
	}
	
	public void DeleteClicked()
	{
		for(int i = 0; i < deleteVariantNameTexts.Length; i++)
		{
			deleteVariantNameTexts[i].text = savedRunVariants[variantsDropdown.value].variantName;
		}
	}
	
	public void ConfirmDeleteClicked()
	{
		savedRunVariants.RemoveAt(variantsDropdown.value);
		UpdateDropdown(false);
		UpdateRunVariantsFile();
		variantsDropdown.value = variantsDropdown.options.Count - 1;
		saveButton.ChangeDisabled(true);
		exportButton.ChangeDisabled(true);
		deleteButton.ChangeDisabled(true);
		ResetAllMutators();
		ClearVariantDisplay(deckPickerVariantDisplayParent);
	}
	
	public void ResetAllMutators()
	{
		SpecialOptions.instance.ResetSpecialOptions();
		BaubleMutators.instance.ResetBaubleMutators();
		CardSelection.instance.ResetCardSelection();
		CardSelection.instance.ResetRandomCards();
		AnteAdjustments.instance.ResetClicked();
	}
	
	public void DropdownSelectionChanged()
	{
		if(variantsDropdown.value == variantsDropdown.options.Count - 1)
		{
			if(variantsDropdown.options[variantsDropdown.options.Count - 1].text == "New")
			{
				ResetAllMutators();
				deleteButton.ChangeDisabled(true);
				exportButton.ChangeDisabled(true);
				ClearVariantDisplay(deckPickerVariantDisplayParent);
			}
			else
			{
				//print("custom");
				// should never happen? Switching to another made one should replace custom with new
			}
		}
		else
		{
			variantsDropdown.options[variantsDropdown.options.Count - 1].text = "New";
			LoadRunVariant(savedRunVariants[variantsDropdown.value]);
			//variantHasBeenChanged = false;
			deleteButton.ChangeDisabled(false);
			exportButton.ChangeDisabled(false);
			//variantDisplay.SetupVariantDisplay(savedRunVariants[variantsDropdown.value].variantName, savedRunVariants[variantsDropdown.value].variantDescription, savedRunVariants[variantsDropdown.value].variantSpecialOptions, savedRunVariants[variantsDropdown.value].variantBaubles, savedRunVariants[variantsDropdown.value].variantCards, savedRunVariants[variantsDropdown.value].variantRandomCards, savedRunVariants[variantsDropdown.value].variantAnteAdjustments);
			SetupVariantDisplay(deckPickerVariantDisplayParent, new Vector2(251, 9), savedRunVariants[variantsDropdown.value], 218);
		}
		saveButton.ChangeDisabled(true);
	}
	
	public void AnyMutatorChanged() // called when backing out of any mutator window, if something changed
	{
		// print("something changed");
		if(variantsDropdown.value == variantsDropdown.options.Count - 1)
		{
			UpdateDropdown(true);
			variantsDropdown.value = variantsDropdown.options.Count - 1;
			string curRunString = GetCurrentRunVariantString("Custom", "");
			RunVariant curRunVariant = ConvertStringToRunVariant(curRunString);
			SetupVariantDisplay(deckPickerVariantDisplayParent, new Vector2(251, 9), curRunVariant, 218);
		}
		else
		{
			SetupVariantDisplay(deckPickerVariantDisplayParent, new Vector2(251, 9), savedRunVariants[variantsDropdown.value], 218);
		}
		saveButton.ChangeDisabled(false);
		exportButton.ChangeDisabled(false);
		
	}

	public void AddCustomToDropdown()
	{
		//AddNewSavedRunVariant("Custom", "");//"Custom" + "|" + "" + "|" + GetRunVariantString());
	}
	
	public void SaveClicked()
	{
		//VariantSaveInterface.instance.gameObject.SetActive(true);
		//if(variantsDropdown.options[variantsDropdown.value].text == "New" || variantsDropdown.options[variantsDropdown.value].text == "Custom")
		if(variantsDropdown.value == variantsDropdown.options.Count - 1)
		{
			VariantSaveInterface.instance.saveInterfaceObject.SetActive(true);
		}
		else
		{
			VariantSaveInterface.instance.overwriteInterfaceObject.SetActive(true);
			VariantSaveInterface.instance.SetupOverwriteInterface(variantsDropdown.options[variantsDropdown.value].text);
		}
	}
	
	public void VariantModeToggleUpdated()
	{
		if(variantModeToggle.isOn)
		{
			variantModeBackdrop.sizeDelta = new Vector2(100, 218);
			hiddenVariantModeOptions.SetActive(true);
			variantModeWarning.SetActive(true);
			if(variantsDropdown.value != variantsDropdown.options.Count - 1)
			{
				SetupVariantDisplay(deckPickerVariantDisplayParent, new Vector2(251, 9), savedRunVariants[variantsDropdown.value], 218);
			}
		}
		else
		{
			variantModeBackdrop.sizeDelta = new Vector2(100, 30);
			hiddenVariantModeOptions.SetActive(false);
			variantModeWarning.SetActive(false);
			ClearVariantDisplay(deckPickerVariantDisplayParent);
		}
	}
	
	void Start()
	{
		SetupRunVariants();
	}
	
	public void SetupRunVariants()
	{
		VariantModeToggleUpdated();
		LoadSavedRunVariants();
		UpdateDropdown(false);
		variantsDropdown.value = variantsDropdown.options.Count - 1;
	}
	
	void Awake()
	{
		instance = this;
	}
	
	public string GetCurrentRunVariantStringWithoutName()
	{
		if(variantsDropdown.value == variantsDropdown.options.Count - 1)
		{
			return GetCurrentRunVariantString("Custom", "");
		}
		else
		{
			RunVariant runVariant = savedRunVariants[variantsDropdown.value];
			return ConvertRunVariantToVariantString(runVariant);
		}
	}
	
	public string ConvertRunVariantToVariantString(RunVariant runVariant)
	{
		return runVariant.variantName + "|" + runVariant.variantDescription + "|" + runVariant.variantSpecialOptions + "|" + runVariant.variantBaubles + "|" + runVariant.variantCards + "|" + runVariant.variantRandomCards + "|" + runVariant.variantAnteAdjustments;
	}
	
	public string GetCurrentRunVariantString(string variantName, string variantDescription)
	{
		return variantName + "|" + variantDescription + "|" + SpecialOptions.instance.ConvertSpecialOptionsToString() + "|" + BaubleMutators.instance.ConvertBaubleMutatorsToString() + "|" + CardSelection.instance.ConvertCardSelectionToString() + "|" + AnteAdjustments.instance.ConvertAnteAdjustmentsToString();
	}
	
	public void ExportButtonClicked()
	{
		if(variantsDropdown.value == variantsDropdown.options.Count - 1)
		{
			exportInputField.text = GetCurrentRunVariantString("Unnamed", "");
		}
		else
		{
			exportInputField.text = ConvertRunVariantToVariantString(savedRunVariants[variantsDropdown.value]);
		}
	}
	
	public void ImportButtonClicked()
	{
		importInterface.SetActive(true);
	}
	
	public void ImportStringClicked()
	{
 		try
		{ 
			RunVariant importedVariant = ConvertStringToRunVariant(importInputField.text);
			savedRunVariants.Add(importedVariant);
			UpdateRunVariantsFile();
			UpdateDropdown(false);
			variantsDropdown.value = variantsDropdown.options.Count - 2;
			SetupVariantDisplay(deckPickerVariantDisplayParent, new Vector2(251, 9), savedRunVariants[variantsDropdown.value], 218);
			importInterface.SetActive(false);
  		}
		catch(Exception exception)
		{
			Debug.Log("An error occurred when loading " + importInputField.text + ": " + exception.Message);
			importInterface.SetActive(false);
			importFailedInterface.SetActive(true);
		} 
	}
	
	public void ClearDailyRun()
	{
		ResetAllMutators();
		variantsDropdown.value = variantsDropdown.options.Count - 1;
		variantModeToggle.isOn = false;
		DropdownSelectionChanged();
	}
	
	public void SetupDailyRun(string input)
	{
		RunVariant runVariant = ConvertStringToRunVariant(input);
		LoadRunVariant(runVariant);
		variantModeToggle.isOn = true;
	}
	
	public void SetupDailyRunDisplay(string input)
	{
		RunVariant runVariant = ConvertStringToRunVariant(input);
		LoadRunVariant(runVariant);
		SetupVariantDisplay(dailyVariantDisplayParent, new Vector2(141.5f, 16f), runVariant, 218);
	}
	
	public void LoadRunVariant(RunVariant runVariant)
	{
		SpecialOptions.instance.ConvertStringToSpecialOptions(runVariant.variantSpecialOptions);
		BaubleMutators.instance.ConvertStringToBaubleMutators(runVariant.variantBaubles);
		CardSelection.instance.ConvertStringToCardSelection(runVariant.variantCards, runVariant.variantRandomCards);
		AnteAdjustments.instance.ConvertStringToAnteAdjustments(runVariant.variantAnteAdjustments);
	}
	
	public RunVariant ConvertStringToRunVariant(string input)
	{
		//print("ConvertStringToRunVariant= " + input);
		string[] sections = input.Split('|');
		
		return new RunVariant
		{
			variantName = sections[0],
			variantDescription = sections[1],
			variantSpecialOptions = sections[2],
			variantBaubles = sections[3],
			variantCards = sections[4],
			variantRandomCards = sections[5],
			variantAnteAdjustments = sections[6]
		};
	}
	
	public void SpecialOptionsButtonClicked()
	{
		SpecialOptions.instance.specialOptionHasChanged = false;
	}
	
	public void BaublesButtonClicked()
	{
		BaubleMutators.instance.baubleOptionHasChanged = false;
	}
	
	public void CardsButtonClicked()
	{
		CardSelection.instance.cardsSelectionHasChanged = false;
	}
	
	public void AntesButtonClicked()
	{
		AnteAdjustments.instance.anteAdjustmentsHasChanged = false;
	}
}