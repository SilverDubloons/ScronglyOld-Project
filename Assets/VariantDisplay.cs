using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantDisplay : MonoBehaviour
{
	public RectTransform rt;
	public Transform contentParent;
	public RectTransform contentRT;
	public GameObject textPrefab;
	
	public void DestroyOldObjects()
	{
		for(int i = 0; i < contentParent.childCount; i++)
		{
			Destroy(contentParent.GetChild(i).gameObject);
		}
	}
	
    public void SetupVariantDisplay(string variantName, string variantDescription, string variantSpecialOptions, string variantBaubles, string variantCards, string variantRandomCards, string variantAnteAdjustments, float height)
	{
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
		DestroyOldObjects();
		float nextY = -3f;
		GameObject newTitleText = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, contentParent);
		TextPrefab titleTextPrefab = newTitleText.GetComponent<TextPrefab>();
		titleTextPrefab.ChangeTexts(variantName);
		titleTextPrefab.rt.anchoredPosition = new Vector2(-1.5f, nextY);
		titleTextPrefab.rt.sizeDelta = new Vector2(141, 20);
		titleTextPrefab.ChangeFontSizeMax(20);
		nextY -= 23f;
		if(variantDescription != "")
		{
			GameObject newDescriptionText = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, contentParent);
			TextPrefab descriptionTextPrefab = newDescriptionText.GetComponent<TextPrefab>();
			descriptionTextPrefab.ChangeTexts(variantDescription);
			descriptionTextPrefab.rt.anchoredPosition = new Vector2(-1.5f, nextY);
			descriptionTextPrefab.ChangeFontSizeMax(12);
			descriptionTextPrefab.rt.sizeDelta = new Vector2(141, descriptionTextPrefab.GetDesiredHeight());
			descriptionTextPrefab.ChangeAlignmentToLeft();
			nextY -= descriptionTextPrefab.GetDesiredHeight() + 3;
		}
		if(variantSpecialOptions != "")
		{
			GameObject newSpecialOptionsText = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, contentParent);
			TextPrefab specialOptionsTextPrefab = newSpecialOptionsText.GetComponent<TextPrefab>();
			specialOptionsTextPrefab.ChangeTexts(SpecialOptions.instance.ConvertSpecialOptionsToReadableString());
			specialOptionsTextPrefab.rt.anchoredPosition = new Vector2(-1.5f, nextY);
			specialOptionsTextPrefab.rt.sizeDelta = new Vector2(141, 20);
			specialOptionsTextPrefab.ChangeFontSizeMax(12);
			nextY -= specialOptionsTextPrefab.GetDesiredHeight() + 3;
		}
		if(variantBaubles != "")
		{
			nextY = BaubleMutators.instance.CreateBaubleIconsForDisplay(contentParent, nextY);
		}
		if(variantCards != "")
		{
			nextY = CardSelection.instance.CreateCardIconsForDisplay(contentParent, nextY);
		}
		if(variantRandomCards != "0:False:0:False")
		{
			string[] ranStrings = variantRandomCards.Split(':');
			int randomStandardCards = int.Parse(ranStrings[0]);
			if(randomStandardCards > 0)
			{
				string rsc = "-Add " + randomStandardCards + " random standard ";
				bool includeRainbowCards = bool.Parse(ranStrings[1]);
				if(!includeRainbowCards)
				{
					rsc += "non-rainbow ";
				}
				rsc += "cards";
				GameObject newRandomCardsTextObject = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, contentParent);
				TextPrefab newRandomCardsText = newRandomCardsTextObject.GetComponent<TextPrefab>();
				newRandomCardsText.ChangeTexts(rsc);
				newRandomCardsText.rt.anchoredPosition = new Vector2(-1.5f, nextY);
				newRandomCardsText.ChangeFontSizeMax(12);
				newRandomCardsText.rt.sizeDelta = new Vector2(141, newRandomCardsText.GetDesiredHeight());
				newRandomCardsText.ChangeAlignmentToLeft();
				nextY -= newRandomCardsText.GetDesiredHeight() + 3;
			}
			int randomNonstandardCards = int.Parse(ranStrings[2]);
			if(randomNonstandardCards > 0)
			{
				string rnsc = "-Add " + randomNonstandardCards + " random nonstandard cards";
				bool considerRarity = bool.Parse(ranStrings[3]);
				if(considerRarity)
				{
					rnsc += ", considering rarity";
				}
				GameObject newRandomCardsTextObject = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, contentParent);
				TextPrefab newRandomCardsText = newRandomCardsTextObject.GetComponent<TextPrefab>();
				newRandomCardsText.ChangeTexts(rnsc);
				newRandomCardsText.rt.anchoredPosition = new Vector2(-1.5f, nextY);
				newRandomCardsText.ChangeFontSizeMax(12);
				newRandomCardsText.rt.sizeDelta = new Vector2(141, newRandomCardsText.GetDesiredHeight());
				newRandomCardsText.ChangeAlignmentToLeft();
				nextY -= newRandomCardsText.GetDesiredHeight() + 3;
			}
		}
		if(variantAnteAdjustments.Trim() != "d:0")
		{
			print("variantAnteAdjustments= \"" +variantAnteAdjustments +"\"");
			string[] sections = variantAnteAdjustments.Split(':');
			if(sections[0] == "c")
			{
				string customAntes = "Custom antes:";
				string customAntesShadow = "Custom antes:";
				string[] anteStrings = sections[1].Split("_");
				for(int i = 0; i < anteStrings.Length; i++)
				{
					customAntes += "\n";
					if(i < 9)
					{
						customAntes += " ";
					}
					customAntes += "<color=red>" + (i + 1) + "</color>" + ": " +  anteStrings[i];
					customAntesShadow += "\n";
					if(i < 9)
					{
						customAntesShadow += " ";
					}
					customAntesShadow += (i + 1) + ": " + anteStrings[i];
				}
				GameObject newAntesText = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, contentParent);
				TextPrefab anteTextPrefab = newAntesText.GetComponent<TextPrefab>();
				//anteTextPrefab.ChangeTexts(customAntes);
				anteTextPrefab.shadowText.text = customAntesShadow;
				anteTextPrefab.opaqueText.text = customAntes;
				anteTextPrefab.rt.anchoredPosition = new Vector2(-1.5f, nextY);
				anteTextPrefab.ChangeAlignmentToLeft();
				anteTextPrefab.ChangeFontSizeMax(12);
				anteTextPrefab.rt.sizeDelta = new Vector2(141, anteTextPrefab.GetDesiredHeight());
				nextY -= anteTextPrefab.GetDesiredHeight() + 3;
			}
			else if(sections[0] == "d")
			{
				int difficulty = int.Parse(sections[1]) + 1;
				GameObject newDifficultyText = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, contentParent);
				TextPrefab difficultyTextPrefab = newDifficultyText.GetComponent<TextPrefab>();
				difficultyTextPrefab.ChangeTexts("Difficulty " + difficulty);
				difficultyTextPrefab.rt.anchoredPosition = new Vector2(-1.5f, nextY);
				difficultyTextPrefab.rt.sizeDelta = new Vector2(141, 20);
				difficultyTextPrefab.ChangeFontSizeMax(12);
				nextY -= difficultyTextPrefab.GetDesiredHeight() + 3;
			}
		}
		
		contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, -nextY);
	}
}
