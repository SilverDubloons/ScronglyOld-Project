using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandValueLabel : MonoBehaviour, IPointerEnterHandler
{
	public RectTransform rt;
	public TMP_Text[] handNameTexts;
	public TMP_Text[] baseValueTexts;
	public TMP_Text[] multiplierTexts;
	public TMP_Text[] totalBaseValueTexts;
	public TMP_Text[] totalMultiplierTexts;
	public GameObject[] totalObjects;
	public TooltipSimple tooltipSimple;
	public Image handLabelImage;
	public int handNumber;
	public HandsInformation handsInformation;
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		rt.transform.SetSiblingIndex(transform.parent.childCount - 1);
	}
	
	public void HighlightRelevantValues(int type)
	{
		if(type == 0)
		{
			handsInformation.HighlightHandsThatContainThisHand(handNumber);
		}
		else if(type == 1)
		{
			handsInformation.HighlightHandsContainedWithinHand(handNumber);
		}
	}
	
	public void ResetHighlightedHands()
	{
		handsInformation.ResetHighlightedHands();
	}
}
