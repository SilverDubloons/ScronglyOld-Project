using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VariantSaveInterface : MonoBehaviour
{
	public static VariantSaveInterface instance;
	public TMP_Text[] titleTexts;
    public TMP_InputField nameInput;
    public TMP_InputField descriptionInput;
	public MovingButton saveButton;
	public TMP_Text[] overwriteVariantNameTexts;
	public GameObject saveInterfaceObject;
	public GameObject overwriteInterfaceObject;
	
	void Awake()
	{
		instance = this;
		saveInterfaceObject.SetActive(false);
	}
	
	public void OverwriteClicked()
	{
		for(int i = 0; i < RunVariations.instance.savedRunVariants.Count; i++)
		{
			if(RunVariations.instance.savedRunVariants[i].variantName == overwriteVariantNameTexts[0].text)
			{
				string updatedRunVariantString = RunVariations.instance.GetCurrentRunVariantString(RunVariations.instance.savedRunVariants[i].variantName, RunVariations.instance.savedRunVariants[i].variantDescription);
				RunVariations.instance.savedRunVariants[i] = RunVariations.instance.ConvertStringToRunVariant(updatedRunVariantString);
				RunVariations.instance.UpdateRunVariantsFile();
				RunVariations.instance.saveButton.ChangeDisabled(true);
				return;
			}
		}
	}
/* 	
	public void SetupSaveInterface(bool saveOver = false, string oldSaveName = "")
	{
		
	}
	 */
	public void NameInputUpdated()
	{
		if(nameInput.text == "")
		{
			saveButton.ChangeDisabled(true);
		}
		else
		{
			if(saveButton.disabled)
			{
				saveButton.ChangeDisabled(false);
			}
		}
	}
	
	public string RemoveBadCharacters(string input)
	{
		string output = input;
		string[] badChars = {"_", ":", "|", "%"};
		for(int i = 0; i < badChars.Length; i++)
		{
			output = output.Replace(badChars[i], " ");
		}
		return output;
	}
	
	public void SetupOverwriteInterface(string variantName)
	{
		for(int i = 0; i < overwriteVariantNameTexts.Length; i++)
		{
			overwriteVariantNameTexts[i].text = variantName;
		}
	}
	
	public void SaveClicked()
	{
		for(int i = 0; i < RunVariations.instance.savedRunVariants.Count; i++)
		{
			print("nameInput.text= " + nameInput.text + " savedRunVariants[i].variantName= " + RunVariations.instance.savedRunVariants[i].variantName);
			if(RunVariations.instance.savedRunVariants[i].variantName == nameInput.text)
			{
				print("they were the same");
				SetupOverwriteInterface(RunVariations.instance.savedRunVariants[i].variantName);
				overwriteInterfaceObject.SetActive(true);
				nameInput.text = "";
				return;
			}
		}
		if(nameInput.text == "Custom" || nameInput.text == "New")
		{
			nameInput.text = "custom";
		}
		RunVariations.instance.AddNewSavedRunVariant(RemoveBadCharacters(nameInput.text), RemoveBadCharacters(descriptionInput.text));
		RunVariations.instance.UpdateRunVariantsFile();
		RunVariations.instance.variantsDropdown.value = RunVariations.instance.variantsDropdown.options.Count - 2;
		RunVariations.instance.SetupVariantDisplay(RunVariations.instance.deckPickerVariantDisplayParent, new Vector2(251, 9), RunVariations.instance.savedRunVariants[RunVariations.instance.variantsDropdown.options.Count - 2], 218);
		nameInput.text = "";
		descriptionInput.text = "";
		RunVariations.instance.saveButton.ChangeDisabled(true);
	}
}
