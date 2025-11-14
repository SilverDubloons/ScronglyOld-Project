using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandValueLabelHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public HandValueLabel handValueLabel;
	public int type; // 0 = individual, 1 = minimum
	
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
		handValueLabel.HighlightRelevantValues(type);
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		handValueLabel.ResetHighlightedHands();
	}
}
