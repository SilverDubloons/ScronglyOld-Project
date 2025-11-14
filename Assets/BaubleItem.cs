using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BaubleItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public RectTransform rt;
	public Image baubleImage;
	public BuyScript buyScript;
	public TooltipScript tooltipScript;
	public RectTransform tooltipRT;
	public GameObject imageObject;
	private bool hovering;
	public Transform tooltipParent;
	public int baubleNumber;
	public TMP_Text[] quantityTexts;
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		transform.SetSiblingIndex(transform.parent.childCount - 1);
		tooltipScript.gameObject.SetActive(true);
		tooltipRT.anchoredPosition = new Vector2(100, 0);
		tooltipScript.transform.SetParent(tooltipParent);
		hovering = true;
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		tooltipScript.gameObject.SetActive(false);
		tooltipScript.transform.SetParent(this.transform);
		hovering = false;
	}
	
	void Update()
	{
		if(hovering)
		{
			tooltipScript.transform.SetParent(this.transform);
			tooltipRT.anchoredPosition = new Vector2(100, 0);
			tooltipScript.transform.SetParent(tooltipParent);
			PointerEventData pointerData = new PointerEventData (EventSystem.current)
			{
				pointerId = -1,
			};
			pointerData.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData,results);
			//bool overBauble = false;
			foreach (RaycastResult result in results)
			{
				if (result.gameObject != null)
				{
					if(result.gameObject.name == "DescriptionText")
					{
						tooltipScript.gameObject.SetActive(false);
						tooltipScript.transform.SetParent(this.transform);
						hovering = false;
					}
				}
			}
		}
	}
	
/*     public string baubleName;
	public string baubleDescription;
	public Sprite image;
	public int baseCost;
	public int maxQuantity;
	public int quantityOwned;
	public float costScalingMultiplier;
	public int costScalingAdditive;
	public int rarity;
	public int baubleNumber; */
}
