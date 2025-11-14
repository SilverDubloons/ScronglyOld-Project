using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltip;
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		tooltip.SetActive(true);
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		tooltip.SetActive(false);
	}
}
