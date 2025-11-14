using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour
{
	public RectTransform rt;
	public bool cardPlaced = false;
	public HandZone handZone;
	public CardScript placedCardScript;
	public int dropZoneNumber; // 0 through 6
	
	public void CardPlaced(CardScript cardScript)
	{
		cardPlaced = true;
		placedCardScript = cardScript;
		handZone.HandUpdated();
	}
	
	public void CardRemoved(bool updateHand)
	{
		cardPlaced = false;
		placedCardScript = null;
		if(updateHand)
		{
			handZone.HandUpdated();
		}
	}
}
