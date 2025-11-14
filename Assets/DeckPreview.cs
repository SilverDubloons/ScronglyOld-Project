using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckPreview : MonoBehaviour
{
	public TMP_Text[] suitQuantitiesTexts;
	public TMP_Text[] rankQuantitiesTexts;
	public TMP_Text[] cardQuantitiesTexts;
	public TMP_Text[] nonstandardQuantitiesTexts;
	
	public RectTransform rt;
	public float timeMouseOver = 0;
	public float transitionTime;
	public bool mouseOver;
	public RectTransform handAreaRT;
	public RectTransform backdropRT;
	public RectTransform shadowRT;
	public GameObject[] rainbowObjects;
	public GameObject[] nonstandardObjects;
	private float handAreaStartY;
	public float handAreaEndY;
	public ShopScript shopScript;
	public DeckViewer deckViewer;
	public int overWhichDeck = 0; // 0 = none, 1 = draw, 2 = discard
	//public Statistics statistics;
	
    public void GenerateDeckPreview(Transform cardParent, int overType) // 0 
	{
		overWhichDeck = overType;
		//print("overType= " + overType + " cardParent.name= " + cardParent.name + " cardParent.childCount= " + cardParent.childCount);
		CardScript[] cardScripts = cardParent.GetComponentsInChildren<CardScript>();
		int[] suitQuantities = new int[5];
		int[] rankQuantities = new int[13];
		int[] cardQuantities = new int[65];
		int numberOfNonStandardCards = 0;
		for(int card = 0; card < cardScripts.Length; card++)
		{
			if(cardScripts[card].standardCard)
			{
				suitQuantities[cardScripts[card].suitInt]++;
				rankQuantities[cardScripts[card].rankInt]++;
				cardQuantities[cardScripts[card].suitInt * 13 + cardScripts[card].rankInt]++;
			}
			else
			{
				numberOfNonStandardCards++;
			}
		}
		for(int suit = 0; suit < 5; suit++)
		{
			if(suit < 4 && suitQuantities[4] > 0)
			{
				suitQuantitiesTexts[suit * 2].text = "" + suitQuantities[suit] + "(" + (suitQuantities[suit] + suitQuantities[4]) + ")";
				suitQuantitiesTexts[suit * 2 + 1].text = "" + suitQuantities[suit] + "<color=green>(" + (suitQuantities[suit] + suitQuantities[4]) + ")";
			}
			else
			{
				suitQuantitiesTexts[suit * 2].text = "" + suitQuantities[suit];
				suitQuantitiesTexts[suit * 2 + 1].text = "" + suitQuantities[suit];
			}
		}
		for(int rank = 0; rank < 13; rank++)
		{
			rankQuantitiesTexts[rank * 2].text = "" + rankQuantities[rank];
			rankQuantitiesTexts[rank * 2 + 1].text = "" + rankQuantities[rank];
		}
		for(int card = 0; card < 65; card++)
		{
			cardQuantitiesTexts[card * 2].text = "" + cardQuantities[card];
			cardQuantitiesTexts[card * 2 + 1].text = "" + cardQuantities[card];
		}
		if(suitQuantities[4] == 0)
		{
			for(int i = 0; i < rainbowObjects.Length; i++)
			{
				rainbowObjects[i].SetActive(false);
			}
			backdropRT.sizeDelta = new Vector2(backdropRT.sizeDelta.x, 109);
			handAreaEndY = 121;
			shadowRT.anchoredPosition = new Vector2(shadowRT.anchoredPosition.x, -99);
		}
		else
		{
			for(int i = 0; i < rainbowObjects.Length; i++)
			{
				rainbowObjects[i].SetActive(true);
			}
			shadowRT.anchoredPosition = new Vector2(shadowRT.anchoredPosition.x, -117);
			backdropRT.sizeDelta = new Vector2(backdropRT.sizeDelta.x, 127);
			handAreaEndY = 139;
		}
		if(numberOfNonStandardCards == 0)
		{
			for(int i = 0; i < nonstandardObjects.Length; i++)
			{
				nonstandardObjects[i].SetActive(false);
			}
		}
		else
		{
			for(int i = 0; i < nonstandardObjects.Length; i++)
			{
				nonstandardObjects[i].SetActive(true);
			}
			for(int i = 0; i < nonstandardQuantitiesTexts.Length; i++)
			{
				nonstandardQuantitiesTexts[i].text = "" + numberOfNonStandardCards;
			}
		}
	}
	
	void Start()
	{
		handAreaStartY = 5;
	}

    void Update()
	{
		if(mouseOver && !deckViewer.isOpen && !Statistics.instance.gameStatsRT.gameObject.activeSelf)
		{
			timeMouseOver = Mathf.Min(timeMouseOver +=Time.deltaTime, transitionTime);
		}
		else
		{
			overWhichDeck = 0;
			if(timeMouseOver > 0)
			{
				timeMouseOver = Mathf.Max(timeMouseOver -=Time.deltaTime, 0);
			}
		}
		if(mouseOver || rt.anchoredPosition.y > handAreaStartY - 365)
		{
			if(!shopScript.shopActive)
			{
				if(!Statistics.instance.gameStatsRT.gameObject.activeSelf)
				{
					handAreaRT.anchoredPosition = new Vector2(handAreaRT.anchoredPosition.x, Mathf.Lerp(handAreaStartY, handAreaEndY, 	timeMouseOver / transitionTime));
				}
				else
				{
					handAreaRT.anchoredPosition = new Vector3(95, -205, 0);
				}
			}
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.Lerp(handAreaStartY - 365, handAreaEndY - 365, timeMouseOver / transitionTime));
		}
	}
}
