using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipSimple : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public GameObject tooltip;
	public RectTransform tooltipRT;
	public bool useMinY = false;
	public Vector2 originalPosition;
	public bool moveToTooltipLayer;
	public Transform originalParent;
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		if(GameOptions.instance.showCommonTooltips)
		{
			tooltip.SetActive(true);
			if(useMinY)
			{
				Transform canvasTransform = GameObject.FindWithTag("GameplayCanvas").GetComponent<Transform>();
				tooltipRT.anchoredPosition = originalPosition;
				Transform oldParent = tooltipRT.transform.parent;
				tooltipRT.SetParent(canvasTransform);
				
				float bottom = tooltipRT.anchoredPosition.y - tooltipRT.sizeDelta.y * tooltipRT.pivot.y;
				tooltipRT.SetParent(oldParent);
				if(bottom < 0)
				{
					tooltipRT.anchoredPosition = new Vector2(tooltipRT.anchoredPosition.x, tooltipRT.anchoredPosition.y - bottom);
				}
			}
			if(moveToTooltipLayer)
			{
				originalParent = tooltip.transform.parent;
				tooltipRT.anchoredPosition = new Vector2(80, 0);
				tooltip.transform.SetParent(GameObject.FindWithTag("TooltipParent").GetComponent<Transform>());
			}
		}
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		if(moveToTooltipLayer && originalParent != null)
		{
			tooltip.transform.SetParent(originalParent);
		}
		tooltip.SetActive(false);
	}
}
