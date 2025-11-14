using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ToggleHelper : MonoBehaviour
{
	public RectTransform rt;
	public Toggle toggle;
	public TMP_Text[] toggleLabelTexts;
	public int type; // 0 = only toggle, 1 = toggle + slider, 2 = toggle + input field
	public Slider slider;
	public TMP_Text[] sliderLabelTexts;
	public TMP_InputField inputField;
	public bool intsOnly;
	public Vector2 range;
	
	public bool IsOn()
	{
		return toggle.isOn;
	}
	
	public void ChangeToggleLabel(string newText)
	{
		for(int i = 0; i < toggleLabelTexts.Length; i++)
		{
			toggleLabelTexts[i].text = newText;
		}
	}
	
	public void ChangeSliderLabel(string newText)
	{
		for(int i = 0; i < sliderLabelTexts.Length; i++)
		{
			sliderLabelTexts[i].text = newText;
		}
	}
	
	public void SliderUpdated()
	{
		if(intsOnly)
		{
			ChangeSliderLabel(slider.value.ToString());
		}
		else
		{
			ChangeSliderLabel(slider.value.ToString("F1"));
		}
		SpecialOptions.instance.specialOptionHasChanged = true;
	}
	
	public void InputFieldFinished()
	{
		if(inputField.text == "")
		{
			if(intsOnly)
			{
				inputField.text = Mathf.RoundToInt(range.x).ToString();
			}
			else
			{
				inputField.text = range.x.ToString();
			}
		}
	}
	
	public void InputFieldUpdated()
	{
		if(inputField.text == "")
		{
			return;
		}
		float valueFloat = range.x;
		int valueInt = Mathf.RoundToInt(range.x);
		try
		{
			if(intsOnly)
			{
				valueInt = int.Parse(inputField.text);
			}
			else
			{
				valueFloat = float.Parse(inputField.text);
			}
		}
		catch(Exception exception)
		{
			Debug.Log("An error occurred when interpreting " + name +  " inputField: " + exception.Message);
			valueFloat = range.x;
			valueInt = Mathf.RoundToInt(range.x);
		}
		if(intsOnly)
		{
			if(valueInt < Mathf.RoundToInt(range.x))
			{
				inputField.text = Mathf.RoundToInt(range.x).ToString();
			}
			if(valueInt > Mathf.RoundToInt(range.y))
			{
				inputField.text = Mathf.RoundToInt(range.y).ToString();
			}
		}
		else
		{
			if(valueFloat < range.x)
			{
				inputField.text = range.x.ToString();
			}
			if(valueFloat > range.y)
			{
				inputField.text = range.y.ToString();
			}
		}
		SpecialOptions.instance.specialOptionHasChanged = true;
	}
	
	public void ToggleUpdated()
	{
		if(type == 1)
		{
			slider.interactable = IsOn();
		}
		else if(type == 2)
		{
			inputField.interactable = IsOn();
		}
		SpecialOptions.instance.specialOptionHasChanged = true;
	}
}
