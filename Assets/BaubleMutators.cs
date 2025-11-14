using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaubleMutators : MonoBehaviour
{
	public static BaubleMutators instance;
    public GameObject baubleHelperPrefab;
	public Transform baubleHelperParent;
	public List<BaubleHelper> baubleHelpers = new List<BaubleHelper>();
	public RectTransform contentRT;
	public GameObject baubleOptions;
	BaubleSortingHelper[] baubleSortingHelpers;
	public bool baubleOptionHasChanged;
	public GameObject baubleViewerPrefab;
	
	public class BaubleSortingHelper
	{
		public int baubleNumber;
		public int sortingIndex;
		public int normalizedSortingIndex;
	}
	
	public void NormalizeVisualOrderIndices()
	{
		baubleSortingHelpers = new BaubleSortingHelper[BaubleScript.instance.baubles.Length - 18];
		int baubleCount = 0;
		for(int i = 0; i < BaubleScript.instance.baubles.Length; i++)
		{
			if(BaubleScript.instance.baubles[i].baubleCategory <= 3)
			{
				baubleSortingHelpers[baubleCount] = new BaubleSortingHelper();
				baubleSortingHelpers[baubleCount].baubleNumber = i;
				baubleSortingHelpers[baubleCount].sortingIndex = (BaubleScript.instance.baubles[i].sortingOrderIndex);
				baubleCount++;
			}
		}
		//cardsOfDesiredSuit.Sort((a, b) => b.rankInt.CompareTo(a.rankInt));
		//BaubleSortingIndex.Sort(baubleOrderIndices);
		Array.Sort(baubleSortingHelpers, (a, b) => a.sortingIndex.CompareTo(b.sortingIndex));
		for(int i = 0; i < baubleSortingHelpers.Length; i++)
		{
			baubleSortingHelpers[i].normalizedSortingIndex = i;
		}
		Array.Sort(baubleSortingHelpers, (a, b) => a.baubleNumber.CompareTo(b.baubleNumber));
		/* for(int i = 0; i < baubleSortingHelpers.Length; i++)
		{
			print("i= " + i + " baubleNumber= " + baubleSortingHelpers[i].baubleNumber + " baubleName= " + BaubleScript.instance.baubles[baubleSortingHelpers[i].baubleNumber].baubleName + " sortingIndex= " + baubleSortingHelpers[i].sortingIndex + " normalizedSortingIndex= " + baubleSortingHelpers[i].normalizedSortingIndex);
		} */
	}
	
	public void SetupBaubleHelpers()
	{
		Vector2 curPosition = new Vector2(-110, -40);
		int baublesMade = 0;
		for(int i = 0; i < BaubleScript.instance.baubles.Length; i++)
		{
			if(BaubleScript.instance.baubles[i].baubleCategory <= 3)
			{
				GameObject newBauble = Instantiate(baubleHelperPrefab, new Vector3(0,0,0), Quaternion.identity, baubleHelperParent);
				BaubleHelper newBaubleHelper = newBauble.GetComponent<BaubleHelper>();
				baubleHelpers.Add(newBaubleHelper);
				if(BaubleScript.instance.baubles[i].maxQuantity > 0)
				{
					newBaubleHelper.slider.maxValue = BaubleScript.instance.baubles[i].maxQuantity;
				}
				else
				{
					newBaubleHelper.slider.maxValue = 25;
				}
				if(BaubleScript.instance.baubles[i].baubleNumber == 22)
				{
					newBaubleHelper.tooltipScript.SetupTooltip(BaubleScript.instance.baubles[i].baubleDescription + ".\n\nTo change the number of items available in the shop, use the special options menu", BaubleScript.instance.baubles[i].baubleName, BaubleScript.instance.baubles[i].baubleCategory);
					Destroy(newBaubleHelper.slider.gameObject);
				}
				else if(BaubleScript.instance.baubles[i].baubleNumber == 38)
				{
					newBaubleHelper.tooltipScript.SetupTooltip(BaubleScript.instance.baubles[i].baubleDescription + ".\n\nTo change the base reroll cost in the shop, use the special options menu", BaubleScript.instance.baubles[i].baubleName, BaubleScript.instance.baubles[i].baubleCategory);
					Destroy(newBaubleHelper.slider.gameObject);
				}
				else
				{
					newBaubleHelper.tooltipScript.SetupTooltip(BaubleScript.instance.baubles[i].baubleDescription, BaubleScript.instance.baubles[i].baubleName, BaubleScript.instance.baubles[i].baubleCategory);
				}
				newBaubleHelper.baubleNumber = BaubleScript.instance.baubles[i].baubleNumber;
				newBaubleHelper.baubleImage.sprite = BaubleScript.instance.baubles[i].baubleImage;
				int baubleSortingIndex = baubleSortingHelpers[baublesMade].normalizedSortingIndex;
				int row = baubleSortingIndex / 3;
				curPosition = new Vector2(-110 + ((baubleSortingIndex + 0) % 3) * 105, -40 - 66 * row);
				newBaubleHelper.rt.anchoredPosition = curPosition;
				/* if((baublesMade + 1) % 3 == 0)
				{
					curPosition = new Vector2(-110, curPosition.y - 66);
				}
				else
				{
					curPosition = new Vector2(curPosition.x + 105, curPosition.y);
				} */
				baublesMade++;
			}
		}
		int rows = baublesMade / 3;
		contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, (rows + 1) * 66 + 5);
	}
	
	void Awake()
	{
		instance = this; 
	}
	
	void Start()
	{
		BaubleScript.instance.AssignBaubleNumbers();
		BaubleScript.instance.SetupNonZodiacBaubles();
		NormalizeVisualOrderIndices();
		SetupBaubleHelpers();
		baubleOptions.SetActive(false);
		// Array.Sort(baubleSortingHelpers, (a, b) => a.baubleNumber.CompareTo(b.baubleNumber));
		// Array.Sort(baubleHelpers, (a, b) => a.baubleNumber.CompareTo(b.baubleNumber));
		baubleHelpers.Sort(new BaubleSortingIndexComparer());
	}
	
	public class BaubleSortingIndexComparer : IComparer<BaubleHelper>
	{
		public int Compare(BaubleHelper bh1, BaubleHelper bh2)
		{
			return BaubleScript.instance.baubles[bh1.baubleNumber].sortingOrderIndex.CompareTo(BaubleScript.instance.baubles[bh2.baubleNumber].sortingOrderIndex);
		}
	}
	
	public void ResetBaublesInPool()
	{
		for(int i = 0; i < baubleHelpers.Count; i++)
		{
			BaubleScript.instance.baubles[baubleHelpers[i].baubleNumber].isInPool = true;
		}
	}
	
	public void ApplyBaubleMutators()
	{
		for(int i = 0; i < baubleHelpers.Count; i++)
		{
			if(baubleHelpers[i].IsInPool())
			{
				BaubleScript.instance.baubles[baubleHelpers[i].baubleNumber].isInPool = true;
			}
			else
			{
				BaubleScript.instance.baubles[baubleHelpers[i].baubleNumber].isInPool = false;
			}
			for(int j = 0; j < Mathf.RoundToInt(baubleHelpers[i].slider.value); j++)
			{
				ShopScript.instance.CheatAddBauble(baubleHelpers[i].baubleNumber, Vector2.zero, false);
			}
		}
	}
	
	public string ConvertBaubleMutatorsToString()
	{
		string baub = "";
		for(int i = 0; i < baubleHelpers.Count; i++)
		{
			if(!baubleHelpers[i].IsInPool() || baubleHelpers[i].GetQuantity() > 0)
			{
				if(baub != "")
				{
					baub += "_";
				}
				baub += baubleHelpers[i].baubleNumber + ":" + baubleHelpers[i].IsInPool().ToString() + ":" + baubleHelpers[i].GetQuantity().ToString();
			}
		}
		return baub;
	}
	
	public float CreateBaubleIconsForDisplay(Transform baubleParent, float startingY)
	{
		int quantityMade = 0;
		for(int i = 0; i < baubleHelpers.Count; i++)
		{
			if(!baubleHelpers[i].IsInPool() || baubleHelpers[i].GetQuantity() > 0)
			{
				GameObject newBaubleViwerObject = Instantiate(baubleViewerPrefab, Vector3.zero, Quaternion.identity, baubleParent);
				BaubleViewer newBaubleViewer = newBaubleViwerObject.GetComponent<BaubleViewer>();
				newBaubleViewer.SetupBaubleViewer(baubleHelpers[i].baubleNumber, baubleHelpers[i].GetQuantity(), baubleHelpers[i].IsInPool());
				newBaubleViewer.rt.anchoredPosition = new Vector2(3 + (quantityMade % 3) * 48, startingY - (quantityMade / 3) * 48);
				quantityMade++;
			}
		}
		return startingY - (quantityMade / 3 + 1) * 48;
	}
	
	public void ResetClicked()
	{
		ResetBaubleMutators();
		baubleOptionHasChanged = true;
	}
	
	public void ResetBaubleMutators()
	{
		for(int i = 0; i < baubleHelpers.Count; i++)
		{

			baubleHelpers[i].inPoolToggle.isOn = true;
			baubleHelpers[i].InPoolToggleUpdated();
			baubleHelpers[i].slider.value = 0;
			baubleHelpers[i].SliderUpdated();
		}
	}
	// |13:False:2,17:False:0,32:False:0,35:False:0,54:False:0,55:True:1||0:False:0:False
	public void ConvertStringToBaubleMutators(string input)
	{
		if(input == "")
		{
			ResetBaubleMutators();
			return;
		}
		string[] sections = input.Split('_');
		List<int> baubleIndices = new List<int>();
		List<bool> baublesInPool = new List<bool>();
		List<int> baubleQuantities = new List<int>();
		for(int i = 0; i < sections.Length; i++)
		{
			string[] sectionSplit = sections[i].Split(':');
			baubleIndices.Add(int.Parse(sectionSplit[0]));
			baublesInPool.Add(bool.Parse(sectionSplit[1]));
			baubleQuantities.Add(int.Parse(sectionSplit[2]));
		}
		
		int curSection = 0;
		
		for(int i = 0; i < baubleHelpers.Count; i++)
		{
			if(curSection < baubleIndices.Count && baubleHelpers[i].baubleNumber == baubleIndices[curSection])
			{
				baubleHelpers[i].inPoolToggle.isOn = baublesInPool[curSection];
				baubleHelpers[i].InPoolToggleUpdated();
				baubleHelpers[i].slider.value = baubleQuantities[curSection];
				baubleHelpers[i].SliderUpdated();
				curSection++;
			}
			else
			{
				baubleHelpers[i].inPoolToggle.isOn = true;
				baubleHelpers[i].InPoolToggleUpdated();
				baubleHelpers[i].slider.value = 0;
				baubleHelpers[i].SliderUpdated();
			}
		}
	}
	
	public void BackButtonClicked()
	{
		if(baubleOptionHasChanged)
		{
			RunVariations.instance.AnyMutatorChanged();
		}
	}
}
