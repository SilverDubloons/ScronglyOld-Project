using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TooltipScript : MonoBehaviour, IPointerEnterHandler
{
	public RectTransform rt;
	public RectTransform descriptionBorderRT;
    public RectTransform descriptionBackdropRT;
	public RectTransform nameRT;
	public RectTransform rarityRT;
	public TMP_Text[] descriptionTexts;
	public TMP_Text[] nameTexts;
	public TMP_Text[] rarityTexts;
	public GameObject rarityObject;
	public Image rarityBackdrop;
	public Color[] rarityColors;
	public int tutorialNumber;
	public bool isTutorialTooltip = false;
	public MovingButton gotItButton;
	
	public void GotItClicked()
	{
		//Destroy(this.gameObject);
		Tutorial.instance.TutorialGotItClicked(tutorialNumber);
	}
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		if(!isTutorialTooltip)
		{
			this.gameObject.SetActive(false);
		}
	}
	
	public void SetupTooltip(string description, string nameString, int rarity)
	{
		bool disable = false;
		if(!rt.gameObject.activeSelf)
		{
			disable = true;
			rt.gameObject.SetActive(true);
		}
		for(int i = 0; i < descriptionTexts.Length; i++)
		{
			if(i == 1)
			{
				descriptionTexts[i].text = description;
			}
			else
			{
				descriptionTexts[i].text = description.Replace("<color=red>","").Replace("<color=blue>","");
			}
			descriptionTexts[i].ForceMeshUpdate(true, true);
		}
		descriptionBorderRT.sizeDelta = new Vector2(descriptionBorderRT.sizeDelta.x, descriptionTexts[1].textBounds.size.y + 14);
		descriptionBackdropRT.sizeDelta = new Vector2(descriptionBackdropRT.sizeDelta.x, descriptionTexts[1].textBounds.size.y + 12);
		if(rarity != 5)
		{
			rarityRT.anchoredPosition = new Vector2(rarityRT.anchoredPosition.x, -descriptionBorderRT.sizeDelta.y / 2 - 10);
			rarityBackdrop.color = rarityColors[rarity];
		}
		if(nameString != "")
		{
			nameRT.anchoredPosition = new Vector2(nameRT.anchoredPosition.x, descriptionBorderRT.sizeDelta.y / 2 + 10);
			for(int i = 0; i < nameTexts.Length; i++)
			{
				nameTexts[i].text = nameString;
			}
		}
		
		for(int i = 0; i < rarityTexts.Length; i++)
		{
			switch(rarity)
			{
				case 0:
				rarityTexts[i].text = "Common";
				break;
				case 1:
				rarityTexts[i].text = "Uncommon";
				break;
				case 2:
				rarityTexts[i].text = "Rare";
				break;
				case 3:
				rarityTexts[i].text = "Legendary";
				break;
				case 4:
				rarityTexts[i].text = "Zodiac";
				break;
				case 5:
				if(rarityObject)
				{
					rarityObject.SetActive(false);
				}
				break;
				/* case 6:
				rarityTexts[i].text = "";
				break; */
			}
		}
		if(disable)
		{
			rt.gameObject.SetActive(false);
		}
	}
}
