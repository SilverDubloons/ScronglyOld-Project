using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltip;
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		if(HandScript.instance.discardButton.disabled)
		{
			if(HandScript.instance.selectedCards.Count > HandScript.instance.maxCardsDiscardedAtOnce)
			{
				tooltip.SetActive(true);
			}
		}
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		tooltip.SetActive(false);
	}
}
