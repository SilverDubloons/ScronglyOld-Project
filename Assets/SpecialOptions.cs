using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpecialOptions : MonoBehaviour
{
	public static SpecialOptions instance;
    public List<ToggleHelper> specialOptionsToggles;
	public SpecialOption[] specialOptions;
	public GameObject togglePrefab;
	public GameObject sliderPrefab;
	public GameObject inputFieldPrefab;
	public Transform mutatorToggleParent;
	public RectTransform mutatorWindowRT;
	public bool specialOptionHasChanged;
	
	[System.Serializable]
	public class SpecialOption
	{
		public string mutatorName;
		public string toggleLabel;
		public string description;
		public string displayDescription;
		public int type; // 0 = toggle, 1 = slider, 2 = inputfield
		public Vector2 range;
		public bool intsOnly;
		public bool showMutatorToggle;
		public ToggleHelper toggleHelper;
		public int category; 	// determines when, during setup, this mutator gets applied or disabled
	}							// 0 = whenever, 1 = before score vial setup, 2 = after bauble reset, before bauble mutators
								// 3 = after bauble mutators 4 = just before hand values reset 5 = before deck modifiers to discards/ftg
	public void SetupSpecialOptionsToggles()
	{
		float[] heightOfColumns = new float[3];
		Vector2 curPosition = new Vector2(-110, -40);
		for(int i = 0; i < specialOptions.Length; i++)
		{
			if(specialOptions[i].showMutatorToggle)
			{
				int curColumn = 0;
				curPosition = new Vector2(-109, -5 - heightOfColumns[curColumn]);
				if(heightOfColumns[1] < heightOfColumns[0])
				{
					curColumn = 1;
					curPosition = new Vector2(0, -5 - heightOfColumns[curColumn] );
				}
				if(heightOfColumns[2] < heightOfColumns[curColumn])
				{
					curColumn = 2;
					curPosition = new Vector2(109, -5 - heightOfColumns[curColumn]);
				}
				if(specialOptions[i].type == 0)
				{
					GameObject newToggle = Instantiate(togglePrefab, new Vector3(0,0,0), Quaternion.identity, mutatorToggleParent);
					newToggle.name = specialOptions[i].mutatorName;
					ToggleHelper newToggleHelper = newToggle.GetComponent<ToggleHelper>();
					specialOptionsToggles.Add(newToggleHelper);
					specialOptions[i].toggleHelper = newToggleHelper;
					newToggleHelper.ChangeToggleLabel(specialOptions[i].toggleLabel);
					newToggleHelper.rt.anchoredPosition = curPosition;
					heightOfColumns[curColumn] += 29;
				}
				else if(specialOptions[i].type == 1)
				{
					GameObject newSlider = Instantiate(sliderPrefab, new Vector3(0,0,0), Quaternion.identity, mutatorToggleParent);
					newSlider.name = specialOptions[i].mutatorName;
					ToggleHelper newToggleHelper = newSlider.GetComponent<ToggleHelper>();
					specialOptionsToggles.Add(newToggleHelper);
					specialOptions[i].toggleHelper = newToggleHelper;
					newToggleHelper.range = specialOptions[i].range;
					newToggleHelper.intsOnly = specialOptions[i].intsOnly;
					if(newToggleHelper.intsOnly)
					{
						newToggleHelper.slider.wholeNumbers = true;
					}
					else
					{
						newToggleHelper.slider.wholeNumbers = false;
					}
					newToggleHelper.slider.minValue = specialOptions[i].range.x;
					newToggleHelper.slider.maxValue = specialOptions[i].range.y;
					newToggleHelper.ChangeToggleLabel(specialOptions[i].toggleLabel);
					newToggleHelper.ChangeSliderLabel(specialOptions[i].range.x.ToString());
					newToggleHelper.rt.anchoredPosition = curPosition;
					heightOfColumns[curColumn] += 53;
				}
				else if(specialOptions[i].type == 2)
				{
					GameObject newInputField = Instantiate(inputFieldPrefab, new Vector3(0,0,0), Quaternion.identity, mutatorToggleParent);
					newInputField.name = specialOptions[i].mutatorName;
					ToggleHelper newToggleHelper = newInputField.GetComponent<ToggleHelper>();
					specialOptionsToggles.Add(newToggleHelper);
					specialOptions[i].toggleHelper = newToggleHelper;
					newToggleHelper.range = specialOptions[i].range;
					newToggleHelper.intsOnly = specialOptions[i].intsOnly;
					if(newToggleHelper.intsOnly)
					{
						newToggleHelper.inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
					}
					else
					{
						newToggleHelper.inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
					}
					newToggleHelper.inputField.text = Mathf.RoundToInt(specialOptions[i].range.x).ToString();
					newToggleHelper.ChangeToggleLabel(specialOptions[i].toggleLabel);
					newToggleHelper.rt.anchoredPosition = curPosition;
					heightOfColumns[curColumn] += 53;
				}
			}
		}
		float tallestColumn = heightOfColumns[0];
		for(int i = 1; i < heightOfColumns.Length; i++)
		{
			if(heightOfColumns[i] > tallestColumn)
			{
				tallestColumn = heightOfColumns[i];
			}
		}
		mutatorWindowRT.sizeDelta = new Vector2(mutatorWindowRT.sizeDelta.x, tallestColumn + 32f);
	}
	
	public string ConvertSpecialOptionsToReadableString()
	{
		string output = "";
		for(int i = 0; i < specialOptions.Length; i++)
		{
			if(specialOptions[i].toggleHelper.IsOn())
			{
				if(output != "")
				{
					output += "\n";
				}
				output += "-";
				if(specialOptions[i].type == 0)
				{
					output += specialOptions[i].displayDescription;
				}
				else if(specialOptions[i].type == 1)
				{
					float val = specialOptions[i].toggleHelper.slider.value;
					output += string.Format(specialOptions[i].displayDescription, val);
				}
				else if(specialOptions[i].type == 2)
				{
					float val = float.Parse(specialOptions[i].toggleHelper.inputField.text);
					output += string.Format(specialOptions[i].displayDescription, val);
				}
			}
		}
		return output;
	}
	
	public void ApplySpecialOptions(int category)
	{
		for(int i = 0; i < specialOptions.Length; i++)
		{
			if(category == -1 || specialOptions[i].category == category)
			{
				ApplySpecialOptionEffect(i);
			}
		}
	}
	
	public void ApplySpecialOptionEffect(int mut)
	{
		SpecialOption specialOption = specialOptions[mut];
		if(!RunVariations.instance.variantModeToggle.isOn || !specialOptions[mut].toggleHelper.IsOn())
		{
			//print("disabling variant " + mut);
			TurnOffSpecialOptionEffect(mut);
			return;
		}
		//print("applying variant " + mut);
		switch(mut)
		{
			case 0:
				HandValues.instance.destroyAllPlacedCards = true;
				NonStandardCardScript.instance.nonStandardCards[0].cardCategory = 5;
			break;
			case 1:
				HandValues.instance.currentAnte = int.Parse(specialOption.toggleHelper.inputField.text) - 1;
			break;
			case 2:
				ShopScript.instance.baseStandardCardCost = Mathf.RoundToInt(specialOption.toggleHelper.slider.value);
			break;
			case 3:
				ShopScript.instance.rerollBaseCost = Mathf.RoundToInt(specialOption.toggleHelper.slider.value);
				ShopScript.instance.rerollStartingCost = ShopScript.instance.rerollBaseCost;
				BaubleScript.instance.baubles[38].maxQuantity = ShopScript.instance.rerollBaseCost - 1;
				if(BaubleScript.instance.baubles[38].maxQuantity == 0)
				{
					BaubleScript.instance.baubles[38].maxQuantity = -1;
				}
			break;
			case 4:
				ScoreVial.instance.MoneyChanged(int.Parse(specialOption.toggleHelper.inputField.text));
			break;
			case 5:
				MainMenu.instance.startNextGameInShop = true;
			break;
			case 6:
				BaubleScript.instance.UnlockAllBaubles();
			break;
			case 7:
				BaubleScript.instance.baseItemsInShop = Mathf.RoundToInt(specialOption.toggleHelper.slider.value);
				BaubleScript.instance.baubles[22].maxQuantity = 7 - BaubleScript.instance.baseItemsInShop;
				if(BaubleScript.instance.baubles[22].maxQuantity <= 0)
				{
					BaubleScript.instance.baubles[22].maxQuantity = -1;
				}
			break;
			case 8:
				ShopScript.instance.rerollScalingCost = 0;
			break;
			case 9:
				BaubleScript.instance.baseDiscardsPerAnte = Mathf.RoundToInt(specialOption.toggleHelper.slider.value);
			break;
			case 10:
				BaubleScript.instance.baseHandsUntilFatigue = Mathf.RoundToInt(specialOption.toggleHelper.slider.value);
			break;
			case 11:
				BaubleScript.instance.baseHandSize = Mathf.RoundToInt(specialOption.toggleHelper.slider.value);
			break;
			case 12:
				ScoreVial.instance.numberOfThresholds = Mathf.RoundToInt(specialOption.toggleHelper.slider.value);
			break;
			case 13:
				ShopScript.instance.collectInterest = false;
				ShopScript.instance.interestTooltipTrigger.SetActive(false);
			break;
			case 14:
				ShopScript.instance.zodiacsCannotBePurchased = true;
			break;
			case 15:
				HandValues.instance.handsHavePreplacedCard = true;
				HandValues.instance.chanceForPreplacedCardToBeNonstandard = specialOption.toggleHelper.slider.value / 100;
			break;
		}
	}
	
	public void TurnOffSpecialOptionEffect(int mut)
	{
		//SpecialOption specialOption = specialOptions[mut];
		switch(mut)
		{
			case 0:
				HandValues.instance.destroyAllPlacedCards = false;
				NonStandardCardScript.instance.nonStandardCards[0].cardCategory = 0;
			break;
			case 1:
				HandValues.instance.currentAnte = 0;
			break;
			case 2:
				ShopScript.instance.baseStandardCardCost = 1;
			break;
			case 3:
				ShopScript.instance.rerollStartingCost = 2;
				ShopScript.instance.rerollBaseCost = 2;
				BaubleScript.instance.baubles[38].maxQuantity = ShopScript.instance.rerollBaseCost - 1;
			break;
			case 4:
				ScoreVial.instance.MoneyChanged(Mathf.RoundToInt(5));
			break;
			case 5:
				MainMenu.instance.startNextGameInShop = false;
			break;
			case 6:
				// this one unlocks all baubles
			break;
			case 7:
				//print("setting base items to 4");
				BaubleScript.instance.baseItemsInShop = 4;
				BaubleScript.instance.baubles[22].maxQuantity = 7 - BaubleScript.instance.baseItemsInShop;
			break;
			case 8:
				ShopScript.instance.rerollScalingCost = 1;
			break;
			case 9:
				BaubleScript.instance.baseDiscardsPerAnte = 3;
			break;
			case 10:
				BaubleScript.instance.baseHandsUntilFatigue = 3;
			break;
			case 11:
				BaubleScript.instance.baseHandSize = 7;
			break;
			case 12:
				ScoreVial.instance.numberOfThresholds = 5;
			break;
			case 13:
				ShopScript.instance.collectInterest = true;
				ShopScript.instance.interestTooltipTrigger.SetActive(true);
			break;
			case 14:
				ShopScript.instance.zodiacsCannotBePurchased = false;
			break;
			case 15:
				HandValues.instance.handsHavePreplacedCard = false;
				HandValues.instance.chanceForPreplacedCardToBeNonstandard = 0;
			break;
		}
	}
	
	public string ConvertSpecialOptionsToString()
	{
		string runVar = "";
		for(int i = 0; i < specialOptions.Length; i++)
		{
			if(specialOptions[i].toggleHelper.IsOn())
			{
				if(runVar != "")
				{
					runVar += "_";
				}
				runVar += i.ToString();
				if(specialOptions[i].type == 1)
				{
					runVar += ":";
					if(specialOptions[i].intsOnly)
					{
						runVar += Mathf.RoundToInt(specialOptions[i].toggleHelper.slider.value).ToString();
					}
					else
					{
						runVar += specialOptions[i].toggleHelper.slider.value.ToString();
					}
				}
				else if(specialOptions[i].type == 2)
				{
					runVar += ":";
					if(specialOptions[i].intsOnly)
					{
						runVar += int.Parse(specialOptions[i].toggleHelper.inputField.text).ToString();
					}
					else
					{
						runVar += float.Parse(specialOptions[i].toggleHelper.inputField.text).ToString();
					}
				}
			}
		}
		return runVar;
	}
	
	public void ResetSpecialOptionsClicked()
	{
		ResetSpecialOptions();
		specialOptionHasChanged = true;
	}
	
	public void ResetSpecialOptions()
	{
		for(int i = 0; i < specialOptions.Length; i++)
		{
			specialOptions[i].toggleHelper.toggle.isOn = false;
			specialOptions[i].toggleHelper.ToggleUpdated();
			if(specialOptions[i].type == 1)
			{
				specialOptions[i].toggleHelper.slider.value = specialOptions[i].range.x;
				specialOptions[i].toggleHelper.SliderUpdated();
			}
			else if(specialOptions[i].type == 2)
			{
				specialOptions[i].toggleHelper.inputField.text = specialOptions[i].range.x.ToString();
				specialOptions[i].toggleHelper.InputFieldUpdated();
			}
		}
	}
	
	public void ConvertStringToSpecialOptions(string input)
	{
		if(input == "")
		{
			ResetSpecialOptions();
			return;
		}
		string[] sections = input.Split('_');
		List<int> runMutatorIndices = new List<int>();
		List<int> runMutatorValues = new List<int>();
		for(int i = 0; i < sections.Length; i++)
		{
			string[] sectionSplit = sections[i].Split(':');
			runMutatorIndices.Add(int.Parse(sectionSplit[0]));
			if(sectionSplit.Length > 1)
			{
				runMutatorValues.Add(int.Parse(sectionSplit[1]));
			}
			else
			{
				runMutatorValues.Add(-1);
			}
		}
		
		int curSection = 0;
		for(int i = 0; i < specialOptions.Length; i++)
		{
			if(curSection < runMutatorIndices.Count && runMutatorIndices[curSection] == i)
			{
				specialOptions[i].toggleHelper.toggle.isOn = true;
				specialOptions[i].toggleHelper.ToggleUpdated();
				if(specialOptions[i].type == 1)
				{
					specialOptions[i].toggleHelper.slider.value = runMutatorValues[curSection];
					specialOptions[i].toggleHelper.SliderUpdated();
				}
				else if(specialOptions[i].type == 2)
				{
					specialOptions[i].toggleHelper.inputField.text = runMutatorValues[curSection].ToString();
					specialOptions[i].toggleHelper.InputFieldUpdated();
				}
				curSection++;
			}
			else
			{
				specialOptions[i].toggleHelper.toggle.isOn = false;
				specialOptions[i].toggleHelper.ToggleUpdated();
				if(specialOptions[i].type == 1)
				{
					specialOptions[i].toggleHelper.slider.value = specialOptions[i].range.x;
					specialOptions[i].toggleHelper.SliderUpdated();
				}
				else if(specialOptions[i].type == 2)
				{
					specialOptions[i].toggleHelper.inputField.text = specialOptions[i].range.x.ToString();
					specialOptions[i].toggleHelper.InputFieldUpdated();
				}
			}
			
		}
	}
	
	public void BackButtonClicked()
	{
		if(specialOptionHasChanged)
		{
			RunVariations.instance.AnyMutatorChanged();
		}
	}
	
	void Start()
	{
		SetupSpecialOptionsToggles();
	}
	
	void Awake()
	{
		instance = this;
	}
}
