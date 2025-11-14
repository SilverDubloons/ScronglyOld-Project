using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AnteHelper : MonoBehaviour
{
	public RectTransform rt;
    public TMP_Text[] anteNumberTexts;
    public TMP_Text[] anteValueTexts;
    public TMP_InputField anteInput;
	
	public void UpdateAnteValue(float newValue)
	{
		string valString = HandValues.instance.ConvertFloatToString(newValue);
		for(int i = 0; i < anteValueTexts.Length; i++)
		{
			anteValueTexts[i].text = valString;
		}
	}
	
	public void UpdateAnteNumber(int number)
	{
		for(int i = 0; i < anteNumberTexts.Length; i++)
		{
			anteNumberTexts[i].text = number.ToString();
		}
	}
	
	public void AnteInputUpdated()
	{
		if(anteInput.text == "")
		{
			UpdateAnteValue(0);
			return;
		}
		try
		{
			UpdateAnteValue(float.Parse(anteInput.text));
		}
		catch(Exception exception)
		{
			Debug.Log("An error occurred when parsing text \"" + anteInput.text + "\" to float: " + exception.Message);
			UpdateAnteValue(0);
		}
		AnteAdjustments.instance.anteAdjustmentsHasChanged = true;
	}
	
	public void AnteInputFinished()
	{
		if(anteInput.text == "")
		{
			anteInput.text = "0";
			UpdateAnteValue(0);
			return;
		}
	}
}
