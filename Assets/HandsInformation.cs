using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandsInformation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public static HandsInformation instance;
    public GameObject handValueLabelPrefab;
	public Transform labelParent;
	public RectTransform labelParentRT;
	public RectTransform backdropRT;
	public RectTransform baseRT;
	public RectTransform columnLabelsRT;
	public HandValues handValues;
	public List<HandValueLabel> handValueLabels = new List<HandValueLabel>();
	public int numberOfVisibleHands = 9;
	public BaubleScript baubleScript;
	bool mouseOver = false;
	private Vector2 startPosition = new Vector2(0,0);
	private Vector2 endPosition = new Vector2(204+46+46,0);
	private float moveState;
	public float moveSpeed;
	public GameObject tooltipHandTypePrefab;
	public List<TooltipHandType> tooltipHandTypes = new List<TooltipHandType>();
	
	public Color handLabelBaseColor;
	public Color handLabelContainedColor;
	public Color handLabelUnplayedColor;
	
	public List<List<int>> handsContainedWithinHands = new List<List<int>>();
	public List<List<int>> handsThatContainThisHand = new List<List<int>>();
	
	public Color handValueBaseColor;
	public Color handValueContainedColor;
	public Color handValueHoveredColor;
	
	public RectTransform individualTooltipRT;
	public RectTransform minimumTooltipRT;
	
	public void ResetHighlightedHands()
	{
		for(int i = 0; i < handValueLabels.Count; i++)
		{
			handValueLabels[i].baseValueTexts[1].color = handValueBaseColor;
			handValueLabels[i].multiplierTexts[1].color = handValueBaseColor;
			handValueLabels[i].totalBaseValueTexts[1].color = handValueBaseColor;
			handValueLabels[i].totalMultiplierTexts[1].color = handValueBaseColor;
		}
	}
	
	public void HighlightHandsContainedWithinHand(int hand)
	{
		ResetHighlightedHands();
		for(int i = 0; i < handsContainedWithinHands[hand].Count; i++)
		{
			handValueLabels[handsContainedWithinHands[hand][i]].baseValueTexts[1].color = handValueContainedColor;
			handValueLabels[handsContainedWithinHands[hand][i]].multiplierTexts[1].color = handValueContainedColor;
		}
		handValueLabels[hand].totalBaseValueTexts[1].color = handValueHoveredColor;
		handValueLabels[hand].totalMultiplierTexts[1].color = handValueHoveredColor;
	}
	
	public void HighlightHandsThatContainThisHand(int hand)
	{
		ResetHighlightedHands();
		for(int i = 0; i < handsThatContainThisHand[hand].Count; i++)
		{
			handValueLabels[handsThatContainThisHand[hand][i]].totalBaseValueTexts[1].color = handValueContainedColor;
			handValueLabels[handsThatContainThisHand[hand][i]].totalMultiplierTexts[1].color = handValueContainedColor;
		}
		handValueLabels[hand].baseValueTexts[1].color = handValueHoveredColor;
		handValueLabels[hand].multiplierTexts[1].color = handValueHoveredColor;
	}
	
	void Awake()
	{
		instance = this;
	}
	
	void Update()
	{
		if(mouseOver && moveState < 1f)
		{
			moveState += moveSpeed * Time.deltaTime;
			if(moveState > 1f)
			{
				moveState = 1f;
			}
			baseRT.anchoredPosition = Vector2.Lerp(startPosition, endPosition, moveState);
		}
		if(!mouseOver && moveState > 0)
		{
			moveState -= moveSpeed * Time.deltaTime;
			if(moveState < 0)
			{
				moveState = 0;
			}
			baseRT.anchoredPosition = Vector2.Lerp(startPosition, endPosition, moveState);
		}
		
	}
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		SoundManager.instance.PlaySlideOutSound();
		transform.SetSiblingIndex(transform.parent.childCount - 1);
		mouseOver = true;
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		SoundManager.instance.PlaySlideOutSound(true);
		mouseOver = false;
	}
	
	void Start()
	{
		//CreateHandValueLabels();
		//ReorganizeValueLabels();
		//baubleScript.AssignBaubleNumbers();
		//baubleScript.SortBaublesByRarity();
		//baubleScript.SetupBaubles(); // needs to be here to the labels get created first
	}
	
	public void RecolorHandLabels(bool[] handsContained)
	{
		for(int i = 0; i < Mathf.Min(handsContained.Length, handValueLabels.Count); i++)
		{
			if(handsContained[i])
			{
				handValueLabels[i].handLabelImage.color = handLabelContainedColor;
			}
			else
			{
				if(BaubleScript.instance.handTypesPlayed[i])
				{
					handValueLabels[i].handLabelImage.color = handLabelBaseColor;
				}
				else
				{
					handValueLabels[i].handLabelImage.color = handLabelUnplayedColor;
				}
			}
		}
	}
	
	public void ReorganizeValueLabels()
	{
		int lastHandY = -16 + 8 * -18;
		for(int i = 9; i < 18; i++)
		{
			if(baubleScript.handTypesToShow[i])
			{
				lastHandY -= 18;
				handValueLabels[i].gameObject.SetActive(true);
				handValueLabels[i].rt.anchoredPosition = new Vector2(-92, lastHandY);
			}
			if(BaubleScript.instance.handTypesPlayed[i])
			{
				handValueLabels[i].handLabelImage.color = handLabelBaseColor;
				tooltipHandTypes[i].SetupTooltip("", HandValues.instance.handDescriptions[i], null, true);
			}
			else
			{
				handValueLabels[i].handLabelImage.color = handLabelUnplayedColor;
				tooltipHandTypes[i].SetupTooltip("", HandValues.instance.handDescriptions[i] + "\nThis hand has not been played yet, so any related Zodiacs and Baubles will not appear in the shop", null, true);
			}
		}
		labelParentRT.anchoredPosition = new Vector2(labelParentRT.anchoredPosition.x, Mathf.Abs(lastHandY) + (360 + lastHandY)/2);
		backdropRT.anchoredPosition = new Vector2(backdropRT.anchoredPosition.x, (360 + lastHandY)/2 - 6);
		backdropRT.sizeDelta = new Vector2(backdropRT.sizeDelta.x, Mathf.Abs(lastHandY) + 12 + 10);
		//print("lastHandY= " + lastHandY);
		columnLabelsRT.anchoredPosition = new Vector2(columnLabelsRT.anchoredPosition.x, 268f + (-160f - lastHandY) / 2f);
	}
	
	public void DestroyHandValueLabels()
	{
		for(int i = 0; i < handValueLabels.Count; i++)
		{
			Destroy(handValueLabels[i].gameObject);
		}
		handValueLabels.Clear();
		tooltipHandTypes.Clear();
	}
	
	public void CreateHandValueLabels()
	{
		ResetHandsContainedWithinHands();
		ResetHandsThatContainThisHand();
		for(int i = 0; i < 18; i++)
		{
			GameObject newLabel = Instantiate(handValueLabelPrefab, new Vector3(0,0,0), Quaternion.identity, labelParent);
			newLabel.name = handValues.handNames[i];
			HandValueLabel handValueLabel = newLabel.GetComponent<HandValueLabel>();
			handValueLabel.handNumber = i;
			handValueLabel.handsInformation = this;
			handValueLabels.Add(handValueLabel);
			for(int j = 0; j < handValueLabel.handNameTexts.Length; j++)
			{
				handValueLabel.handNameTexts[j].text = handValues.handNames[i];
			}
			UpdateBaseValueAndMultilpierLabels(i);
			UpdateTotalBaseValueAndMultiplierLabels(i);
			UpdateTotalBaseValueAndMultiplierLabels(i);
			handValueLabel.rt.anchoredPosition = new Vector2(-92, -16 + i * -18);
			GameObject newTooltip = Instantiate(tooltipHandTypePrefab, new Vector3(0,0,0), Quaternion.identity, handValueLabel.transform);
			TooltipHandType newTooltipHandType = newTooltip.GetComponent<TooltipHandType>();
			tooltipHandTypes.Add(newTooltipHandType);
			handValueLabel.tooltipSimple.tooltip = newTooltip;
			handValueLabel.tooltipSimple.tooltipRT = newTooltipHandType.rt;
			List<RectTransform> tooltipCards = new List<RectTransform>();
			switch(i)
			{
				case 0:
				CardScript newHighCard = DeckScript.instance.CreateCard(1, 12, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newHighCard.rt);
				Destroy(newHighCard);
				break;
				case 1:
				CardScript newPairCard0 = DeckScript.instance.CreateCard(0, 10, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newPairCard0.rt);
				Destroy(newPairCard0);
				CardScript newPairCard1 = DeckScript.instance.CreateCard(2, 10, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newPairCard1.rt);
				Destroy(newPairCard1);
				break;
				case 2:
				CardScript newTwoPairCard0 = DeckScript.instance.CreateCard(3, 4, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newTwoPairCard0.rt);
				Destroy(newTwoPairCard0);
				CardScript newTwoPairCard1 = DeckScript.instance.CreateCard(0, 4, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newTwoPairCard1.rt);
				Destroy(newTwoPairCard1);
				CardScript newTwoPairCard2 = DeckScript.instance.CreateCard(2, 8, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newTwoPairCard2.rt);
				Destroy(newTwoPairCard2);
				CardScript newTwoPairCard3 = DeckScript.instance.CreateCard(1, 8, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newTwoPairCard3.rt);
				Destroy(newTwoPairCard3);
				break;
				case 3:
				CardScript newThreeOAKCard0 = DeckScript.instance.CreateCard(2, 9, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newThreeOAKCard0.rt);
				Destroy(newThreeOAKCard0);
				CardScript newThreeOAKCard1 = DeckScript.instance.CreateCard(1, 9, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newThreeOAKCard1.rt);
				Destroy(newThreeOAKCard1);
				CardScript newThreeOAKCard2 = DeckScript.instance.CreateCard(0, 9, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newThreeOAKCard2.rt);
				Destroy(newThreeOAKCard2);
				break;
				case 4:
				CardScript newStraightCard0 = DeckScript.instance.CreateCard(1, 7, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightCard0.rt);
				Destroy(newStraightCard0);
				CardScript newStraightCard1 = DeckScript.instance.CreateCard(3, 8, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightCard1.rt);
				Destroy(newStraightCard1);
				CardScript newStraightCard2 = DeckScript.instance.CreateCard(0, 9, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightCard2.rt);
				Destroy(newStraightCard2);
				CardScript newStraightCard3 = DeckScript.instance.CreateCard(1, 10, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightCard3.rt);
				Destroy(newStraightCard3);
				CardScript newStraightCard4 = DeckScript.instance.CreateCard(2, 11, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightCard4.rt);
				Destroy(newStraightCard4);
				break;
				case 5:
				CardScript newFlushCard0 = DeckScript.instance.CreateCard(0, 6, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushCard0.rt);
				Destroy(newFlushCard0);
				CardScript newFlushCard1 = DeckScript.instance.CreateCard(0, 10, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushCard1.rt);
				Destroy(newFlushCard1);
				CardScript newFlushCard2 = DeckScript.instance.CreateCard(0, 0, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushCard2.rt);
				Destroy(newFlushCard2);
				CardScript newFlushCard3 = DeckScript.instance.CreateCard(0, 12, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushCard3.rt);
				Destroy(newFlushCard3);
				CardScript newFlushCard4 = DeckScript.instance.CreateCard(0, 3, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushCard4.rt);
				Destroy(newFlushCard4);
				break;
				case 6:
				CardScript newFullHouseCard0 = DeckScript.instance.CreateCard(3, 5, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFullHouseCard0.rt);
				Destroy(newFullHouseCard0);
				CardScript newFullHouseCard1 = DeckScript.instance.CreateCard(2, 5, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFullHouseCard1.rt);
				Destroy(newFullHouseCard1);
				CardScript newFullHouseCard2 = DeckScript.instance.CreateCard(0, 5, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFullHouseCard2.rt);
				Destroy(newFullHouseCard2);
				CardScript newFullHouseCard3 = DeckScript.instance.CreateCard(1, 10, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFullHouseCard3.rt);
				Destroy(newFullHouseCard3);
				CardScript newFullHouseCard4 = DeckScript.instance.CreateCard(2, 10, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFullHouseCard4.rt);
				Destroy(newFullHouseCard4);
				break;
				case 7:
				CardScript newFourOAKCard0 = DeckScript.instance.CreateCard(1, 6, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFourOAKCard0.rt);
				Destroy(newFourOAKCard0);
				CardScript newFourOAKCard1 = DeckScript.instance.CreateCard(2, 6, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFourOAKCard1.rt);
				Destroy(newFourOAKCard1);
				CardScript newFourOAKCard2 = DeckScript.instance.CreateCard(3, 6, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFourOAKCard2.rt);
				Destroy(newFourOAKCard2);
				CardScript newFourOAKCard3 = DeckScript.instance.CreateCard(0, 6, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFourOAKCard3.rt);
				Destroy(newFourOAKCard3);
				break;
				case 8:
				CardScript newStraightFlushCard0 = DeckScript.instance.CreateCard(3, 0, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightFlushCard0.rt);
				Destroy(newStraightFlushCard0);
				CardScript newStraightFlushCard1 = DeckScript.instance.CreateCard(3, 1, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightFlushCard1.rt);
				Destroy(newStraightFlushCard1);
				CardScript newStraightFlushCard2 = DeckScript.instance.CreateCard(3, 2, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightFlushCard2.rt);
				Destroy(newStraightFlushCard2);
				CardScript newStraightFlushCard3 = DeckScript.instance.CreateCard(3, 3, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightFlushCard3.rt);
				Destroy(newStraightFlushCard3);
				CardScript newStraightFlushCard4 = DeckScript.instance.CreateCard(3, 4, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newStraightFlushCard4.rt);
				Destroy(newStraightFlushCard4);
				break;
				case 9:
				CardScript newFiveOAKCard0 = DeckScript.instance.CreateCard(0, 12, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFiveOAKCard0.rt);
				Destroy(newFiveOAKCard0);
				CardScript newFiveOAKCard1 = DeckScript.instance.CreateCard(1, 12, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFiveOAKCard1.rt);
				Destroy(newFiveOAKCard1);
				CardScript newFiveOAKCard2 = DeckScript.instance.CreateCard(3, 12, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFiveOAKCard2.rt);
				Destroy(newFiveOAKCard2);
				CardScript newFiveOAKCard3 = DeckScript.instance.CreateCard(0, 12, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFiveOAKCard3.rt);
				Destroy(newFiveOAKCard3);
				CardScript newFiveOAKCard4 = DeckScript.instance.CreateCard(2, 12, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFiveOAKCard4.rt);
				Destroy(newFiveOAKCard4);
				break;
				/* case 10:
				CardScript newFlushFiveCard0 = DeckScript.instance.CreateCard(1, 3, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushFiveCard0.rt);
				Destroy(newFlushFiveCard0);
				CardScript newFlushFiveCard1 = DeckScript.instance.CreateCard(1, 3, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushFiveCard1.rt);
				Destroy(newFlushFiveCard1);
				CardScript newFlushFiveCard2 = DeckScript.instance.CreateCard(1, 3, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushFiveCard2.rt);
				Destroy(newFlushFiveCard2);
				CardScript newFlushFiveCard3 = DeckScript.instance.CreateCard(1, 3, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushFiveCard3.rt);
				Destroy(newFlushFiveCard3);
				CardScript newFlushFiveCard4 = DeckScript.instance.CreateCard(1, 3, newTooltipHandType.cardParent, new Vector2(0,0), false);
				tooltipCards.Add(newFlushFiveCard4.rt);
				Destroy(newFlushFiveCard4);
				break; */
				case 10:
				for(int j = 0; j < 6; j++)
				{
					CardScript newTripleDoubleCard = DeckScript.instance.CreateCard(j%4, (j/2) * 5, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newTripleDoubleCard.rt);
					Destroy(newTripleDoubleCard);
				}
				break;
				case 11:
				for(int j = 0; j < 6; j++)
				{
					CardScript newDoubleTripleCard = DeckScript.instance.CreateCard((j + 1) % 4, (j/3 + 2) * 4, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newDoubleTripleCard.rt);
					Destroy(newDoubleTripleCard);
				}
				break;
				case 12:
				for(int j = 0; j < 6; j++)
				{
					CardScript newStuffedHouseCard = DeckScript.instance.CreateCard((j + 2) % 4, (j < 4) ? 11 : 6, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newStuffedHouseCard.rt);
					Destroy(newStuffedHouseCard);
				}
				break;
				case 13:
				int ranRank0 = UnityEngine.Random.Range(0,13);
				for(int j = 0; j < 6; j++)
				{
					CardScript newSixOfAKindCard = DeckScript.instance.CreateCard((j + 3) % 4, ranRank0, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newSixOfAKindCard.rt);
					Destroy(newSixOfAKindCard);
				}
				break;
				/* case 15:
				int ranRank1 = UnityEngine.Random.Range(0,13);
				int ranSuit1 = UnityEngine.Random.Range(0,4);
				for(int j = 0; j < 6; j++)
				{
					CardScript newFlushSixCard = DeckScript.instance.CreateCard(ranSuit1, ranRank1, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newFlushSixCard.rt);
					Destroy(newFlushSixCard);
				}
				break; */
				case 14:
				int guestRank = 1;
				for(int j = 0; j < 7; j++)
				{
					if(j > 1)
					{
						guestRank = 6;
					}
					if(j > 4)
					{
						guestRank = 10;
					}
					CardScript newGuestHouseCard = DeckScript.instance.CreateCard(j % 4, guestRank, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newGuestHouseCard.rt);
					Destroy(newGuestHouseCard);
				}
				break;
				case 15:
				for(int j = 0; j < 7; j++)
				{
					CardScript newWideHouseCard = DeckScript.instance.CreateCard((j + 1) % 4, (j < 4) ? 7 : 9, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newWideHouseCard.rt);
					Destroy(newWideHouseCard);
				}
				break;
				case 16:
				for(int j = 0; j < 7; j++)
				{
					CardScript newHugeHouseCard = DeckScript.instance.CreateCard((j + 2) % 4, (j < 5) ? 5 : 12, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newHugeHouseCard.rt);
					Destroy(newHugeHouseCard);
				}
				break;
				case 17:
				int ranRank2 = UnityEngine.Random.Range(0,13);
				for(int j = 0; j < 7; j++)
				{
					CardScript newSevenOAKCard = DeckScript.instance.CreateCard((j + 3) % 4, ranRank2, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newSevenOAKCard.rt);
					Destroy(newSevenOAKCard);
				}
				break;
				/* case 20:
				int ranRank3 = UnityEngine.Random.Range(0,13);
				int ranSuit3 = UnityEngine.Random.Range(0,4);
				for(int j = 0; j < 7; j++)
				{
					CardScript newSevenOAKCard = DeckScript.instance.CreateCard(ranSuit3, ranRank3, newTooltipHandType.cardParent, new Vector2(0,0), false);
					tooltipCards.Add(newSevenOAKCard.rt);
					Destroy(newSevenOAKCard);
				}
				break; */
			}
			newTooltipHandType.SetupTooltip(handValues.handNames[i], handValues.handDescriptions[i], tooltipCards);
			newTooltipHandType.rt.anchoredPosition = new Vector2(newTooltipHandType.borderRT.sizeDelta.x/2+102, -49);
			handValueLabel.tooltipSimple.originalPosition = newTooltipHandType.rt.anchoredPosition; 
			newTooltipHandType.gameObject.SetActive(false);
			
			if(i > 9)
			{
				newLabel.SetActive(false);
			}
		}
	}
	
	public void ResetHandsThatContainThisHand()
	{
		handsThatContainThisHand = new List<List<int>>();
		for(int i = 0; i < 18; i++)
		{
			handsThatContainThisHand.Add(new List<int>());
		}
		handsThatContainThisHand[0].AddRange(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17});
		handsThatContainThisHand[1].AddRange(new int[] {1, 2, 3, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 17});
		handsThatContainThisHand[2].AddRange(new int[] {2, 6, 10, 11, 12, 14, 15, 16});
		handsThatContainThisHand[3].AddRange(new int[] {3, 6, 7, 9, 11, 12, 13, 14, 15, 16, 17});
		handsThatContainThisHand[4].AddRange(new int[] {4, 8});
		handsThatContainThisHand[5].AddRange(new int[] {5, 8});
		handsThatContainThisHand[6].AddRange(new int[] {6, 11, 12, 14, 15, 16});
		handsThatContainThisHand[7].AddRange(new int[] {7, 9, 12, 13, 15, 16, 17});
		handsThatContainThisHand[8].AddRange(new int[] {8});
		handsThatContainThisHand[9].AddRange(new int[] {9, 13, 16, 17});
		handsThatContainThisHand[10].AddRange(new int[] {10, 14});
		handsThatContainThisHand[11].AddRange(new int[] {11, 15});
		handsThatContainThisHand[12].AddRange(new int[] {12, 15, 16});
		handsThatContainThisHand[13].AddRange(new int[] {13, 17});
		handsThatContainThisHand[14].AddRange(new int[] {14});
		handsThatContainThisHand[15].AddRange(new int[] {15});
		handsThatContainThisHand[16].AddRange(new int[] {16});
		handsThatContainThisHand[17].AddRange(new int[] {17});
	}
	
	public void ResetHandsContainedWithinHands()
	{
		handsContainedWithinHands = new List<List<int>>();
		for(int i = 0; i < 18; i++)
		{
			handsContainedWithinHands.Add(new List<int>());
		}
		handsContainedWithinHands[0].Add(0);
		handsContainedWithinHands[1].AddRange(new int[] {0, 1});
		handsContainedWithinHands[2].AddRange(new int[] {0, 1, 2});
		handsContainedWithinHands[3].AddRange(new int[] {0, 1, 3});
		handsContainedWithinHands[4].AddRange(new int[] {0, 4});
		handsContainedWithinHands[5].AddRange(new int[] {0, 5});
		handsContainedWithinHands[6].AddRange(new int[] {0, 1, 2, 3, 6});
		handsContainedWithinHands[7].AddRange(new int[] {0, 1, 3, 7});
		handsContainedWithinHands[8].AddRange(new int[] {0, 4, 5, 8});
		handsContainedWithinHands[9].AddRange(new int[] {0, 1, 3, 7, 9});
		handsContainedWithinHands[10].AddRange(new int[] {0, 1, 2, 10});
		handsContainedWithinHands[11].AddRange(new int[] {0, 1, 2, 3, 6, 11});
		handsContainedWithinHands[12].AddRange(new int[] {0, 1, 2, 3, 6, 7, 12});
		handsContainedWithinHands[13].AddRange(new int[] {0, 1, 3, 7, 9, 13});
		handsContainedWithinHands[14].AddRange(new int[] {0, 1, 2, 3, 6, 10, 14});
		handsContainedWithinHands[15].AddRange(new int[] {0, 1, 2, 3, 6, 7, 11, 12, 15});
		handsContainedWithinHands[16].AddRange(new int[] {0, 1, 2, 3, 6, 7, 9, 12, 16});
		handsContainedWithinHands[17].AddRange(new int[] {0, 1, 3, 7, 9, 13, 17});
	}
	
	public void StraightDownToOne()
	{
		for(int i = 0; i < 18; i++)
		{
			if(i != 4 && i != 8)
			{
				handsContainedWithinHands[i].Add(4);
				handsThatContainThisHand[4].Add(i);
			}
			if(i < 4)
			{
				for(int j = 0; j < handValueLabels[i].totalObjects.Length; j++)
				{
					handValueLabels[i].totalObjects[j].SetActive(false);
				}
			}
		}
	}
	
	public void FlushDownToOne()
	{
		for(int i = 0; i < 18; i++)
		{
			if(i != 5 && i != 8)
			{
				handsContainedWithinHands[i].Add(5);
				handsThatContainThisHand[5].Add(i);
			}
			if(i < 5)
			{
				for(int j = 0; j < handValueLabels[i].totalObjects.Length; j++)
				{
					handValueLabels[i].totalObjects[j].SetActive(false);
				}
			}
		}
	}
	
	public void StraightFlushDownToOne()
	{
		for(int i = 0; i < 18; i++)
		{
			if(i != 8)
			{
				handsThatContainThisHand[8].Add(i);
				handsContainedWithinHands[i].Add(8);
			}
			if(i < 8)
			{
				for(int j = 0; j < handValueLabels[i].totalObjects.Length; j++)
				{
					handValueLabels[i].totalObjects[j].SetActive(false);
				}
			}
		}
	}
	
	public void UpdateHandValueLabel(int handNumber)
	{
		//print("handNumber= "+ handNumber);
		UpdateBaseValueAndMultilpierLabels(handNumber);
		for(int i = 0; i < handsThatContainThisHand[handNumber].Count; i++)
		{
			UpdateTotalBaseValueAndMultiplierLabels(handsThatContainThisHand[handNumber][i]);
		}
	}
	
	public void UpdateTotalBaseValueAndMultiplierLabels(int handNumber)
	{
		for(int i = 0; i < handValueLabels[handNumber].totalBaseValueTexts.Length; i++)
		{
			handValueLabels[handNumber].totalBaseValueTexts[i].text = handValues.ConvertFloatToString(GetTotalHandValue(handNumber));
		}
		for(int i = 0; i < handValueLabels[handNumber].totalMultiplierTexts.Length; i++)
		{
			handValueLabels[handNumber].totalMultiplierTexts[i].text = handValues.ConvertFloatToString(GetTotalMultiplierValue(handNumber));
		}
	}
	
	public void UpdateBaseValueAndMultilpierLabels(int handNumber)
	{
		for(int i = 0; i < handValueLabels[handNumber].baseValueTexts.Length; i++)
		{
			handValueLabels[handNumber].baseValueTexts[i].text = handValues.ConvertFloatToString(handValues.handBaseValues[handNumber]);
		}
		for(int i = 0; i < handValueLabels[handNumber].multiplierTexts.Length; i++)
		{
			handValueLabels[handNumber].multiplierTexts[i].text = handValues.ConvertFloatToString(handValues.handCardMultipliers[handNumber]);
		}
	}
	
	public float GetTotalHandValue(int handNumber)
	{
		float totalValue = 0;
		for(int i = 0; i < handsContainedWithinHands[handNumber].Count; i++)
		{
			totalValue += handValues.handBaseValues[handsContainedWithinHands[handNumber][i]];
		}
		return totalValue;
	}
	
	public float GetTotalMultiplierValue(int handNumber)
	{
		float totalMult = 0;
		for(int i = 0; i < handsContainedWithinHands[handNumber].Count; i++)
		{
			totalMult += handValues.handCardMultipliers[handsContainedWithinHands[handNumber][i]];
		}
		return totalMult;
	}
	
	public void UpdateStraightTooltip()
	{
		tooltipHandTypes[4].gameObject.SetActive(true);
		int gap = HandValues.instance.maxGapInStraights;
		int length = HandValues.instance.cardsNeededToMakeAStraight;
		bool wrap = HandValues.instance.straightsCanWrap;
		
		for(int i = 0; i < tooltipHandTypes[4].cardParent.childCount; i++)
		{
			tooltipHandTypes[4].cardParent.GetChild(i).gameObject.SetActive(false);
			Destroy(tooltipHandTypes[4].cardParent.GetChild(i).gameObject, i * 0.1f);
		}
		List<RectTransform> tooltipCards = new List<RectTransform>();
		
		int[] ranksToDisplay = new int[length];
		ranksToDisplay[0] = 0;
		if(wrap)
		{
			ranksToDisplay[0] = 10;
		}
		if(length > 1)
		{
			ranksToDisplay[1] = ranksToDisplay[0] + gap + 1;
			if(ranksToDisplay[1] > 12)
			{
				ranksToDisplay[1] -= 13;
			}
			if(length > 2)
			{
				ranksToDisplay[2] = ranksToDisplay[1] + gap + 1;
				if(ranksToDisplay[2] > 12)
				{
					ranksToDisplay[2] -= 13;
				}
				if(length > 3)
				{
					ranksToDisplay[3] = ranksToDisplay[2] + Mathf.Min(2, gap + 1);
					if(ranksToDisplay[3] > 12)
					{
						ranksToDisplay[3] -= 13;
					}
					if(length > 4)
					{
						ranksToDisplay[4] = ranksToDisplay[3] + Mathf.Min(2, gap + 1);
						if(ranksToDisplay[4] > 12)
						{
							ranksToDisplay[4] -= 13;
						}
					}
				}
			}
		}

		for(int i = 0; i < length; i++)
		{
			CardScript newStraightCard = DeckScript.instance.CreateCard(i % 4, ranksToDisplay[i], tooltipHandTypes[4].cardParent, new Vector2(0,0), false);
			tooltipCards.Add(newStraightCard.rt);
			Destroy(newStraightCard);
		}
		string description = handValues.numberStrings[length];
		if(length > 1)
		{
			description = char.ToUpper(description[0]) + description.Substring(1) + " cards of consecutive rank";
			if(gap > 0)
			{
				description = description + " with at most " + handValues.numberStrings[gap] + " gap" + ((gap > 1) ? "s" : "") + " between them";
			}
			if(wrap)
			{
				description = description + ". Straights may wrap";
			}
		}
		else
		{
			description = "All hands are considered straights";
		}
		tooltipHandTypes[4].SetupTooltip(handValues.handNames[4], description, tooltipCards);
		tooltipHandTypes[4].rt.anchoredPosition = new Vector2(tooltipHandTypes[4].borderRT.sizeDelta.x/2+102, -49);
		handValueLabels[4].tooltipSimple.originalPosition = tooltipHandTypes[4].rt.anchoredPosition;
		tooltipHandTypes[4].gameObject.SetActive(false);
	}
	
	public void UpdateStraightFlushTooltip()
	{
		tooltipHandTypes[8].gameObject.SetActive(true);
		int gap = HandValues.instance.maxGapInStraightFlushes;
		int length = HandValues.instance.cardsNeededToMakeAStraightFlush;
		bool wrap = HandValues.instance.straightFlushesCanWrap;
		
		for(int i = 0; i < tooltipHandTypes[8].cardParent.childCount; i++)
		{
			tooltipHandTypes[8].cardParent.GetChild(i).gameObject.SetActive(false);
			Destroy(tooltipHandTypes[8].cardParent.GetChild(i).gameObject, i * 0.1f);
		}
		List<RectTransform> tooltipCards = new List<RectTransform>();
		
		int[] ranksToDisplay = new int[length];
		ranksToDisplay[0] = 0;
		if(wrap)
		{
			ranksToDisplay[0] = 10;
		}
		if(length > 1)
		{
			ranksToDisplay[1] = ranksToDisplay[0] + gap + 1;
			if(ranksToDisplay[1] > 12)
			{
				ranksToDisplay[1] -= 13;
			}
			if(length > 2)
			{
				ranksToDisplay[2] = ranksToDisplay[1] + gap + 1;
				if(ranksToDisplay[2] > 12)
				{
					ranksToDisplay[2] -= 13;
				}
				if(length > 3)
				{
					ranksToDisplay[3] = ranksToDisplay[2] + Mathf.Min(2, gap + 1);
					if(ranksToDisplay[3] > 12)
					{
						ranksToDisplay[3] -= 13;
					}
					if(length > 4)
					{
						ranksToDisplay[4] = ranksToDisplay[3] + Mathf.Min(2, gap + 1);
						if(ranksToDisplay[4] > 12)
						{
							ranksToDisplay[4] -= 13;
						}
					}
				}
			}
		}
		int ranSuit = UnityEngine.Random.Range(0,4);
		for(int i = 0; i < length; i++)
		{
			CardScript newStraightFlushCard = DeckScript.instance.CreateCard(ranSuit, ranksToDisplay[i], tooltipHandTypes[8].cardParent, new Vector2(0,0), false);
			tooltipCards.Add(newStraightFlushCard.rt);
			Destroy(newStraightFlushCard);
		}
		string description = handValues.numberStrings[length];
		if(length > 1)
		{
			description = char.ToUpper(description[0]) + description.Substring(1) + " cards of consecutive rank";
			if(gap > 0)
			{
				description = description + " with at most " + handValues.numberStrings[gap] + " gap" + ((gap > 1) ? "s" : "") + " between them";
			}
			description = description + ", all of the same suit";
			if(wrap)
			{
				description = description + ". Straight flushes may wrap";
			}
			
		}
		else
		{
			description = "All hands are considered straight flushes";
		}
		tooltipHandTypes[8].SetupTooltip(handValues.handNames[8], description, tooltipCards);
		tooltipHandTypes[8].rt.anchoredPosition = new Vector2(tooltipHandTypes[8].borderRT.sizeDelta.x/2+102, -49);
		handValueLabels[8].tooltipSimple.originalPosition = tooltipHandTypes[8].rt.anchoredPosition;
		tooltipHandTypes[8].gameObject.SetActive(false);
	}
	
	public void UpdateFlushTooltip()
	{
		tooltipHandTypes[5].gameObject.SetActive(true);
		int length = HandValues.instance.cardsNeededToMakeAFlush;
		for(int i = 0; i < tooltipHandTypes[5].cardParent.childCount; i++)
		{
			tooltipHandTypes[5].cardParent.GetChild(i).gameObject.SetActive(false);
			Destroy(tooltipHandTypes[5].cardParent.GetChild(i).gameObject, i * 0.1f);
		}
		int[] ranksToDisplay = new int[5];
		ranksToDisplay[0] = 4;
		ranksToDisplay[1] = 9;
		ranksToDisplay[2] = 3;
		ranksToDisplay[3] = 12;
		ranksToDisplay[4] = 6;
		
		List<RectTransform> tooltipCards = new List<RectTransform>();
		int ranSuit = UnityEngine.Random.Range(0,4);
		for(int i = 0; i < length; i++)
		{
			CardScript newFlushCard = DeckScript.instance.CreateCard(ranSuit, ranksToDisplay[i], tooltipHandTypes[5].cardParent, new Vector2(0,0), false);
			tooltipCards.Add(newFlushCard.rt);
			Destroy(newFlushCard);
		}
		string description = handValues.numberStrings[length];
		if(length > 1)
		{
			description = char.ToUpper(description[0]) + description.Substring(1) + " cards of the same suit";
		}
		else
		{
			description = "All hands are considered flushes";
		}
		tooltipHandTypes[5].SetupTooltip(handValues.handNames[5], description, tooltipCards);
		tooltipHandTypes[5].rt.anchoredPosition = new Vector2(tooltipHandTypes[5].borderRT.sizeDelta.x/2+102, -49);
		handValueLabels[5].tooltipSimple.originalPosition = tooltipHandTypes[5].rt.anchoredPosition;
		tooltipHandTypes[5].gameObject.SetActive(false);
	}
}
