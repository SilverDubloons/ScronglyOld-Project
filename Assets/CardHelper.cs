using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardHelper : MonoBehaviour
{
	public RectTransform rt;
    public int quantityChange;
	public MovingButton minusButton;
	public MovingButton plusButton;
	public TMP_Text[] quantityTexts;
	public bool standardCard;
	public int suitInt;
	public int rankInt;
	public int nonStandardCardNumber;
	
	public void UpdateButtonClickability()
	{
		if(standardCard)
		{
			if((suitInt <= 3 && quantityChange <= -1) || (suitInt == 4 && quantityChange <= 0))
			{
				if(!minusButton.disabled)
				{
					minusButton.ChangeDisabled(true);
				}
			}
			else
			{
				if(minusButton.disabled)
				{
					minusButton.ChangeDisabled(false);
				}
			}
			if(quantityChange >= CardSelection.instance.maxStandardCards)
			{
				if(!plusButton.disabled)
				{
					plusButton.ChangeDisabled(true);
				}
			}
			else
			{
				if(plusButton.disabled)
				{
					plusButton.ChangeDisabled(false);
				}
			}
		}
		else
		{
			if(quantityChange <= 0)
			{
				if(!minusButton.disabled)
				{
					minusButton.ChangeDisabled(true);
				}
			}
			else
			{
				if(minusButton.disabled)
				{
					minusButton.ChangeDisabled(false);
				}
			}
			if(quantityChange >= CardSelection.instance.maxNonstandardCards)
			{
				if(!plusButton.disabled)
				{
					plusButton.ChangeDisabled(true);
				}
			}
			else
			{
				if(plusButton.disabled)
				{
					plusButton.ChangeDisabled(false);
				}
			}
		}
		UpdateQuantityTexts();
	}
	
	public void MinusClicked()
	{
		quantityChange--;
		UpdateButtonClickability();
		CardSelection.instance.cardsSelectionHasChanged = true;
	}
	public void PlusClicked()
	{
		quantityChange++;
		UpdateButtonClickability();
		CardSelection.instance.cardsSelectionHasChanged = true;
	}
	public void UpdateQuantityTexts()
	{
		if(quantityChange >= 0)
		{
			quantityTexts[1].color = CardSelection.instance.positiveCardsColor;
		}
		else
		{
			quantityTexts[1].color = CardSelection.instance.negativeCardsColor;
		}
		for(int i = 0; i < quantityTexts.Length; i++)
		{
			if(quantityChange >= 0)
			{
				quantityTexts[i].text = "+" + quantityChange.ToString();	
			}
			else
			{
				quantityTexts[i].text = quantityChange.ToString();
			}
		}
		if(quantityChange == 0)
		{
			quantityTexts[0].gameObject.SetActive(false);
		}
		else
		{
			if(!quantityTexts[0].gameObject.activeSelf )
			{
				quantityTexts[0].gameObject.SetActive(true);
			}
		}
	}
}
