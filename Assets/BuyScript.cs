using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuyScript : MonoBehaviour
{
    public TMP_Text[] costTexts;
	public MovingButton movingButton;
	public int itemCost;
	public ShopScript shopScript;
	public int itemType; // 0 = standard card, 1 = bauble
	public CardScript cardScript;
	public int baubleNumber;
	public BaubleScript baubleScript;
	public BaubleItem baubleItem;
	public MovingButton layawayButton;
	
	void Start()
	{
		StartCoroutine(ExpandContract());
	}
	
	public IEnumerator ExpandContract()
	{
		RectTransform rt = null;
		if(itemType == 0)
		{
			rt = cardScript.rt;
		}
		if(itemType == 1)
		{
			rt = baubleItem.rt;
		}
		float t = 0;
		Vector3 originalScale = rt.localScale;
		Vector3 finalScale = new Vector3(1.25f, 1.25f, 1f);
		while(t < 0.1f)
		{
			t += Time.deltaTime;
			rt.localScale = Vector3.Lerp(originalScale, finalScale, t / 0.1f);
			yield return null;
		}
		t = 0;
		while(t < 0.15f)
		{
			t += Time.deltaTime;
			rt.localScale = Vector3.Lerp(finalScale, originalScale, t / 0.1f);
			yield return null;
		}
	}
	
	public void MoveToLayaway()
	{
		layawayButton.ChangeDisabled(true);
		shopScript.NewObjectMovedToLayway(this, GameOptions.instance.askToDeleteOldLayaway);
	}
	
	public void SetupBuy(int cost)
	{
		itemCost = cost;
		for(int i = 0; i < costTexts.Length; i++)
		{
			costTexts[i].text = "" + cost;
		}
		if(shopScript.scoreVial.currentMoney < cost)
		{
			movingButton.ChangeDisabled(true);
		}
	}
	
	public void BuyClicked(bool playSound = true)
	{
		shopScript.buyScripts.Remove(this);
		if(playSound)
		{
			SoundManager.instance.PlayCashRegisterSound();
		}
		if(shopScript.layawayBuyScript == this)
		{
			shopScript.layawayBuyScript = null;
			shopScript.layawayImage.raycastTarget = true;
			shopScript.layawayText.SetActive(true);
		}
		//movingButton.ChangeDisabled(true);
		switch(itemType)
		{
			case 0:	// standard card
			cardScript.StartFlip(0.25f);
			cardScript.transform.SetParent(cardScript.handScript.drawPile.drawPileParent);
			cardScript.cardLocation = 1;
			cardScript.StartMove(0.25f, new Vector3(22.5f, 22.5f, 0), new Vector3(0,0,0));
			cardScript.handScript.DrawPileCountChanged(1);
			cardScript.cardIsInShop = false;
			Statistics.instance.currentRun.cardsAddedToDeck++;
			
			cardScript.deckViewerClone.transform.SetParent(shopScript.deckViewer.cloneParent);
			shopScript.deckViewer.clonedCards.Add(cardScript.deckViewerClone);
			if(cardScript.suitInt == 4)
			{
				DeckViewer.instance.CheckForRainbowDeckUnlock();
			}
			//Destroy(this.gameObject);
			break;
			case 1:	// bauble
			//print("baubleNumber= "+ baubleNumber + " quantityOwned= " + baubleScript.baubles[baubleNumber].quantityOwned);
			if(baubleNumber == 56 && baubleScript.baubles[baubleNumber].quantityOwned > 0)
			{
				//print("a");
				for(int i = 0; i < shopScript.baubleCollection.baublesInCollection.Count; i++)
				{
					if(shopScript.baubleCollection.baublesInCollection[i].baubleNumber == baubleNumber)
					{
						//print("b");
						shopScript.baubleCollection.baublesInCollection[i].baubleImage.sprite = baubleScript.dieSprites[baubleScript.baubles[56].quantityOwned + 9];
						shopScript.baubleCollection.baublesInCollection[i].tooltipScript.SetupTooltip(baubleScript.baubles[baubleNumber].baubleDescription, baubleScript.baubles[baubleNumber].baubleName, baubleScript.baubles[baubleNumber].baubleCategory);
						break;
					}
				}
			}
			baubleScript.baubles[baubleNumber].numberOnSale--;
			baubleScript.BaublePurchased(baubleNumber);
			bool destroyAfterMove = false;
			if(baubleScript.baubles[baubleNumber].quantityOwned > 1)
			{
				for(int i = 0; i < shopScript.baubleCollection.baublesInCollection.Count; i++)
				{
					if(shopScript.baubleCollection.baublesInCollection[i].baubleNumber == baubleNumber)
					{
						if(baubleNumber != 56)
						{
							for(int j = 0; j < shopScript.baubleCollection.baublesInCollection[i].quantityTexts.Length; j++)
							{
								shopScript.baubleCollection.baublesInCollection[i].quantityTexts[j].gameObject.SetActive(true);
								shopScript.baubleCollection.baublesInCollection[i].quantityTexts[j].text = "" + baubleScript.baubles[baubleNumber].quantityOwned;
								shopScript.baubleCollection.baublesInCollection[i].tooltipScript.SetupTooltip(baubleScript.baubles[baubleNumber].baubleDescription, baubleScript.baubles[baubleNumber].baubleName, baubleScript.baubles[baubleNumber].baubleCategory);
							}
						}
						else
						{
							if(baubleScript.baubles[56].quantityOwned <= 4)
							{
								baubleScript.baubles[56].baubleImage = baubleScript.dieSprites[baubleScript.baubles[56].quantityOwned + 9];
							}
						}
						destroyAfterMove = true;
					}
				}
			}
			if((baubleNumber > 12 && baubleNumber < 42) || baubleNumber > 46)
			{
				Statistics.instance.currentRun.baublesPurchased++;
			}
			else
			{
				Statistics.instance.currentRun.zodiacsEarned++;
				destroyAfterMove = true;
			}
			int baub = shopScript.baubleCollection.baublesInCollection.Count;
			baubleItem.transform.SetParent(shopScript.movingBaubleParent);
			Vector2 destination = new Vector2(-350, 50);
			shopScript.StartCoroutine(shopScript.scoreVial.MoveOverTime(baubleItem.rt, baubleItem.rt.anchoredPosition, destination, 1, 0, shopScript.baubleCollection.baubleParentTransform, new Vector2(27.5f + (baub % 2) * 50f, -27.5f - 50f * (baub / 2)), new Vector2(0,1), new Vector2(0,1), destroyAfterMove));
			if(!destroyAfterMove)
			{
				shopScript.baubleCollection.baublesInCollection.Add(baubleItem);
				shopScript.baubleCollection.ReorganizeBaubles();
				if(baubleNumber != 56)
				{
					baubleItem.tooltipScript.SetupTooltip(baubleScript.baubles[baubleNumber].baubleDescription, baubleScript.baubles[baubleNumber].baubleName, baubleScript.baubles[baubleNumber].baubleCategory);
				}
				else
				{
					baubleScript.baubles[56].baubleImage = baubleScript.dieSprites[baubleScript.baubles[56].quantityOwned + 9];
				}
			}
				
			/* if((baubleNumber > 12 && baubleNumber < 42) || baubleNumber > 46)
			{
				shopScript.statistics.currentRun.baublesPurchased++;
				int baub = shopScript.baubleCollection.baublesInCollection.Count;
				baubleItem.transform.SetParent(shopScript.movingBaubleParent);
				Vector2 destination = new Vector2(-350, 50);
				shopScript.StartCoroutine(shopScript.scoreVial.MoveOverTime(baubleItem.rt, baubleItem.rt.anchoredPosition, destination, 1, 0, shopScript.baubleCollection.baubleParentTransform, new Vector2(27.5f + (baub % 2) * 50f, -27.5f - 50f * (baub / 2)), new Vector2(0,1), new Vector2(0,1), destroyAfterMove));
				if(!destroyAfterMove)
				{
					shopScript.baubleCollection.baublesInCollection.Add(baubleItem);
					shopScript.baubleCollection.ReorganizeBaubles();
					if(baubleNumber != 56)
					{
						baubleItem.tooltipScript.SetupTooltip(baubleScript.baubles[baubleNumber].baubleDescription, baubleScript.baubles[baubleNumber].baubleName, baubleScript.baubles[baubleNumber].baubleCategory);
					}
					else
					{
						baubleScript.baubles[56].baubleImage = baubleScript.dieSprites[baubleScript.baubles[56].quantityOwned + 9];
					}
				}
				
			}
			else
			{
				Destroy(baubleItem.tooltipScript.gameObject);
				Destroy(baubleItem.gameObject);
				shopScript.statistics.currentRun.zodiacsEarned++;
			} */
			break;
		}
		Destroy(this.gameObject);
		shopScript.scoreVial.MoneyChanged(-itemCost);
	}
	
	public void MoneyUpdated()
	{
		//print("money updated");
		/* if(baubleNumber == 34)
		{
			print("Piggy bank calling MoneyUpdated() currentMoney= " + shopScript.scoreVial.currentMoney + " itemCost= " + itemCost);
		} */
		if(shopScript.scoreVial.currentMoney >= itemCost)
		{
			if(movingButton.disabled)
			{
				movingButton.ChangeDisabled(false);
			}
		}
		else
		{
			if(!movingButton.disabled)
			{
				movingButton.ChangeDisabled(true);
			}
		}
		if(itemType == 1)
		{
			SetupBuy(baubleScript.GetBaublePrice(baubleNumber));
		}
	}
}
