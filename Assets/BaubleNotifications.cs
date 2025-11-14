using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaubleNotifications : MonoBehaviour
{
	public static BaubleNotifications instance;
    public BaubleScript baubleScript;
	public GameObject baubleNotifierPrefab;
	public Transform baubleNotifierParent;
	
	void Awake()
	{
		instance = this;
	}
	
	public void Notify(int baubleNumber, int chipsToSpawn = 0)
	{
		GameObject newNotifierObject = Instantiate(baubleNotifierPrefab, new Vector3(0,0,0), Quaternion.identity, baubleNotifierParent);
		BaubleNotifier newNotifier = newNotifierObject.GetComponent<BaubleNotifier>();
		newNotifier.baubleNumber = baubleNumber;
		newNotifier.baubleImage.sprite = baubleScript.baubles[baubleNumber].baubleImage;
		newNotifier.chipsToSpawn = chipsToSpawn;
	}
	
	public void NotifyDie(int rolledNumber, bool maxRoll)
	{
		GameObject newNotifierObject = Instantiate(baubleNotifierPrefab, new Vector3(0,0,0), Quaternion.identity, baubleNotifierParent);
		BaubleNotifier newNotifier = newNotifierObject.GetComponent<BaubleNotifier>();
		newNotifier.baubleNumber = 56;
		if(maxRoll)
		{
			newNotifier.rollType = 2;
		}
		else if(rolledNumber == 1)
		{
			newNotifier.rollType = 0;
		}
		else
		{
			newNotifier.rollType = 1;
		}
		if(rolledNumber >=6)
		{
			newNotifier.chipsToSpawn = 1;
		}
		if(rolledNumber >= 20)
		{
			newNotifier.chipsToSpawn = 10;
		}
		if(baubleScript.baubles[56].quantityOwned == 1)
		{
			newNotifier.baubleImage.sprite = baubleScript.dieSprites[rolledNumber - 1];
		}
		else
		{
			newNotifier.numberObject.SetActive(true);
			if(baubleScript.baubles[56].quantityOwned == 2)
			{
				newNotifier.numberRT.anchoredPosition = new Vector2(1.5f, -0.5f);
				for(int i = 0; i < newNotifier.numberTexts.Length; i++)
				{
					newNotifier.numberTexts[i].fontSize = 10;
				}
			}
			else if(baubleScript.baubles[56].quantityOwned == 3)
			{
				newNotifier.numberRT.anchoredPosition = new Vector2(1f, -5f);
				for(int i = 0; i < newNotifier.numberTexts.Length; i++)
				{
					newNotifier.numberTexts[i].fontSize = 8;
				}
			}
			else if(baubleScript.baubles[56].quantityOwned == 4)
			{
				newNotifier.numberRT.anchoredPosition = new Vector2(1f, 0);
				for(int i = 0; i < newNotifier.numberTexts.Length; i++)
				{
					newNotifier.numberTexts[i].fontSize = 11;
				}
			}
			else if(baubleScript.baubles[56].quantityOwned == 5)
			{
				newNotifier.numberRT.anchoredPosition = new Vector2(1f, -3f);
				for(int i = 0; i < newNotifier.numberTexts.Length; i++)
				{
					newNotifier.numberTexts[i].fontSize = 6;
				}
			}
			
			for(int i = 0; i < newNotifier.numberTexts.Length; i++)
			{
				newNotifier.numberTexts[i].text = "" + rolledNumber;
			}
			newNotifier.baubleImage.sprite = baubleScript.dieSprites[baubleScript.baubles[56].quantityOwned + 4];
		}
	}
}
