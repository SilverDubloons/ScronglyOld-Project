using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnteAdjustments : MonoBehaviour
{
    public static AnteAdjustments instance;
	public Slider anteSlider;
	public Toggle customToggle;
	public Transform anteHelperParent;
	public GameObject anteHelperPrefab;
	public List<AnteHelper> anteHelpers = new List<AnteHelper>();
	public TMP_Text[] difficultyTexts;
	public bool anteAdjustmentsHasChanged;
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		SetupAnteHelpers();
	}
	
	public void SetupAnteHelpers()
	{
		float[] defaultAntes = HandValues.instance.SetupAntes(0);
		for(int i = 0; i < 30; i++)
		{
			GameObject newAnteObject = Instantiate(anteHelperPrefab, Vector3.zero, Quaternion.identity, anteHelperParent);
			AnteHelper newAnteHelper = newAnteObject.GetComponent<AnteHelper>();
			anteHelpers.Add(newAnteHelper);
			newAnteHelper.rt.anchoredPosition = new Vector2(55 + 105 * (i % 5), -60 - 31 * (i / 5));
			
			newAnteHelper.UpdateAnteNumber(i + 1);
			newAnteHelper.UpdateAnteValue(defaultAntes[i]);
		}
	}
	
	public void CustomToggleUpdated()
	{
		if(customToggle.isOn)
		{
			anteSlider.interactable = false;
			int difficulty = Mathf.RoundToInt(anteSlider.value);
			float[] antes = HandValues.instance.SetupAntes(difficulty);
			for(int i = 0; i < 30; i++)
			{
				anteHelpers[i].anteInput.interactable = true;
				anteHelpers[i].anteInput.text = antes[i].ToString("F0");
			}
		}
		else
		{
			anteSlider.interactable = true;
			for(int i = 0; i < 30; i++)
			{
				anteHelpers[i].anteInput.text = "";
				anteHelpers[i].anteInput.interactable = false;
			}
			SliderUpdated();
		}
		UpdateDifficultyTexts();
		anteAdjustmentsHasChanged = true;
	}
	
	public void SliderUpdated()
	{
		int difficulty = Mathf.RoundToInt(anteSlider.value);
		float[] antes = HandValues.instance.SetupAntes(difficulty);
		for(int i = 0; i < 30; i++)
		{
			anteHelpers[i].UpdateAnteValue(antes[i]);
		}
		UpdateDifficultyTexts();
		anteAdjustmentsHasChanged = true;
	}
	
	public void ResetClicked()
	{
		customToggle.isOn = false;
		anteSlider.value = 0;
		CustomToggleUpdated();
		anteAdjustmentsHasChanged = true;
	}
	
	public void UpdateDifficultyTexts()
	{
		string difficultyText = "";
		if(customToggle.isOn)
		{
			difficultyText = "Custom Difficulty";
		}
		else
		{
			int difficultyLevel = Mathf.RoundToInt(anteSlider.value) + 1;
			difficultyText = "Difficulty " + difficultyLevel.ToString();
		}
		for(int i = 0; i < difficultyTexts.Length; i++)
		{
			difficultyTexts[i].text = difficultyText;
		}
	}
	
	public void ConvertStringToAnteAdjustments(string input)
	{
		string[] sections = input.Split(':');
		if(sections[0] == "c")
		{
			customToggle.isOn = true;
			CustomToggleUpdated();
			string[] anteStrings = sections[1].Split("_");
			for(int i = 0; i < anteStrings.Length; i++)
			{
				anteHelpers[i].anteInput.text = anteStrings[i];
			}
		}
		else if(sections[0] == "d")
		{
			customToggle.isOn = false;
			CustomToggleUpdated();
			int difficulty = int.Parse(sections[1]);
			anteSlider.value = difficulty;
			SliderUpdated();
		}
	}
	
	public string ConvertAnteAdjustmentsToString()
	{
		string anteAdjustments = "";
		if(customToggle.isOn)
		{
			anteAdjustments += "c:";
			for(int i = 0; i < 30; i++)
			{
				if(i > 0)
				{
					anteAdjustments += "_";
				}
				anteAdjustments += anteHelpers[i].anteInput.text;
			}
		}
		else
		{
			anteAdjustments += "d:";
			anteAdjustments += Mathf.RoundToInt(anteSlider.value).ToString();
		}
		return anteAdjustments;
	}
	
	public void SetupCustomAntes()
	{
		if(customToggle.isOn)
		{
			float[] antes = new float[50];
			for(int i = 0; i < 30; i ++)
			{
				antes[i] = float.Parse(anteHelpers[i].anteInput.text);
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
			HandValues.instance.antes = antes;
		}
		else
		{
			HandValues.instance.SetupAntes(Mathf.RoundToInt(anteSlider.value));
		}
	}
	
	public void BackButtonClicked()
	{
		if(anteAdjustmentsHasChanged)
		{
			RunVariations.instance.AnyMutatorChanged();
		}
	}
}
