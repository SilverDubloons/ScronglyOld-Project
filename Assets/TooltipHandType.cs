using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipHandType : MonoBehaviour
{
	public RectTransform rt;
	public RectTransform borderRT;
	public RectTransform backdropRT;
	public Transform cardParent;
	public RectTransform cardParentRT;
	public RectTransform[] handNameRTs;
	public RectTransform[] handDescriptionRTs;
    public TMP_Text[] handNameTexts;
	public TMP_Text[] handDescriptionTexts;
	
	public void SetupTooltip(string handName, string handDescription, List<RectTransform> cards, bool onlyChangeDescription = false)
	{
		if(!onlyChangeDescription)
		{
			float width = Mathf.Max(100f, 5f + cards.Count * 48f);
			//print(handName + " width= " + width.ToString() + " 5f + cards.Count * 48f= " + (5f + cards.Count * 48f).ToString() + " cards.Count= " + cards.Count);
			borderRT.sizeDelta = new Vector2(width, borderRT.sizeDelta.y);
			backdropRT.sizeDelta = new Vector2(borderRT.sizeDelta.x - 2, borderRT.sizeDelta.y - 2);
			cardParentRT.sizeDelta = new Vector2(borderRT.sizeDelta.x, cardParentRT.sizeDelta.y);
			for(int i = 0; i < handNameRTs.Length; i++)
			{
				handNameRTs[i].sizeDelta = new Vector2(borderRT.sizeDelta.x - 6, handNameRTs[i].sizeDelta.y);
				handNameTexts[i].text = handName;
			}
			
			for(int i = 0; i < cards.Count; i++)
			{
				float xDestination = (cards.Count - 1) * 48 - (cards.Count - i - 1) * 48 + 26;
				if(cards.Count == 1)
				{
					xDestination += 26f;
				}
				cards[i].anchoredPosition = new Vector2(xDestination, 7 + 45 + 2);
			}
		}
		for(int i = 0; i < handDescriptionRTs.Length; i++)
		{
			handDescriptionRTs[i].sizeDelta = new Vector2(borderRT.sizeDelta.x - 6, handDescriptionRTs[i].sizeDelta.y);
			handDescriptionTexts[i].text = handDescription;
		}
	}
}
