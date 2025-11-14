using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaubleHelper : MonoBehaviour
{
    public RectTransform rt;
	public Toggle inPoolToggle;
	public Slider slider;
	public TMP_Text[] sliderLabelTexts;
	public GameObject notInPoolObject;
	public int baubleNumber;
	public Image baubleImage;
	public TooltipScript tooltipScript;
	
	public bool IsInPool()
	{
		return inPoolToggle.isOn;
	}
	
	public int GetQuantity()
	{
		return Mathf.RoundToInt(slider.value);
	}
	
	public void SliderUpdated()
	{
		int sliderInt = Mathf.RoundToInt(slider.value);
		for(int i = 0; i < sliderLabelTexts.Length; i++)
		{
			sliderLabelTexts[i].text = sliderInt.ToString();
		}
		if(sliderInt != 0 && sliderInt == BaubleScript.instance.baubles[baubleNumber].maxQuantity)
		{
			inPoolToggle.isOn = true;
			inPoolToggle.interactable = false;
			InPoolToggleUpdated();
		}
		else
		{
			inPoolToggle.interactable = true;
		}
		if(sliderInt == 0)
		{
			for(int i = 0; i < sliderLabelTexts.Length; i++)
			{
				sliderLabelTexts[i].gameObject.SetActive(false);
			}
		}
		else
		{
			for(int i = 0; i < sliderLabelTexts.Length; i++)
			{
				sliderLabelTexts[i].gameObject.SetActive(true);
			}
		}
		BaubleMutators.instance.baubleOptionHasChanged = true;
	}
	
	public void InPoolToggleUpdated()
	{
		if(inPoolToggle.isOn)
		{
			notInPoolObject.SetActive(false);
		}
		else
		{
			notInPoolObject.SetActive(true);
		}
		BaubleMutators.instance.baubleOptionHasChanged = true;
	}
}
