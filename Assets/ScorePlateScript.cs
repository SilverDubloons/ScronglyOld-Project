using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePlateScript : MonoBehaviour
{
    //string baseValue;
	public string afterAddition;
	public float timeAlive;
	public float timeUntilDeath;
	public TMP_Text[] scoreTexts;
	public RectTransform rt;
	public Image backdropImage;
	public Color secondColor;
	private bool alreadyAnimated;
	public Vector3 endRotation;
	public float totalValue;
	public string valueToAddString;
	public float valueToAdd;
	public bool alreadyAddedValue = false;
	public RectTransform additionBorder;
	public RectTransform additionBackdrop;
	public TMP_Text[] additionTexts;
	public bool alreadyExpanded = true;
	public float mushroomProduct = 1;
	
	public RectTransform border;
	public RectTransform backdrop;
	public bool decompress;
	public bool stayDecompressed;
	public float compressionState; // 0 is fully compressed, 1 is decompressed
	public float compressionSpeed;
	public float decompressionSpeed;
	public float compressedWidth;
	public float decompressedWidth;
	public HandZone handZone;
	public bool startedCompressing;
	const float epsilon = 0.0001f;
	public bool addingToBaseValue = false;
	public bool addingToMultiplier = false;
	
    void Start()
    {
        
    }
	
	public IEnumerator Animate()
	{
		if(alreadyExpanded && addingToBaseValue)
		{
			SoundManager.instance.PlayStandardScoreSound();
		}
		if(!alreadyExpanded && addingToBaseValue)
		{
			SoundManager.instance.PlayGrowScoreSound();
		}
		float t = 0;
		Quaternion originalRotationQ = rt.localRotation;
		Quaternion destinationRotationQ = Quaternion.Euler(endRotation);
		while(t < 0.1)
		{
			t += Time.deltaTime * handZone.handValues.gameOptions.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(originalRotationQ, destinationRotationQ, t / 0.1f);
			yield return null;
		}
		t = 0;
		while(t < 0.15)
		{
			t += Time.deltaTime * handZone.handValues.gameOptions.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(destinationRotationQ, originalRotationQ, t / 0.15f);
			yield return null;
		}
	}
	
	public IEnumerator ExpandContract()
	{
		float t = 0;
		Vector3 originalScale = rt.localScale;
		Vector3 finalScale = new Vector3(1.25f, 1.25f, 1f);
		while(t < 0.1f)
		{
			t += Time.deltaTime * handZone.handValues.gameOptions.gameSpeedFactor;
			rt.localScale = Vector3.Lerp(originalScale, finalScale, t / 0.1f);
			yield return null;
		}
		t = 0;
		while(t < 0.15f)
		{
			t += Time.deltaTime * handZone.handValues.gameOptions.gameSpeedFactor;
			rt.localScale = Vector3.Lerp(finalScale, originalScale, t / 0.1f);
			yield return null;
		}
	}

    void Update()
    {
        timeAlive += Time.deltaTime * handZone.handValues.gameOptions.gameSpeedFactor;
		if(timeAlive > timeUntilDeath * 0.9 && !startedCompressing)
		{
			startedCompressing = true;
			decompress = false;
			stayDecompressed = false;
		}
		if(timeAlive > timeUntilDeath / 4f && Mathf.Abs(valueToAdd) > epsilon && !alreadyAddedValue)
		{
			alreadyAddedValue = true;
			additionBorder.gameObject.SetActive(true);
			for(int i = 0; i < additionTexts.Length; i++)
			{
				additionTexts[i].text = "+" + valueToAddString;
				additionTexts[i].ForceMeshUpdate(true, true);
			}
			additionBorder.sizeDelta = new Vector2(additionTexts[0].textBounds.size.x + 10, additionBorder.sizeDelta.y);
			additionBorder.anchoredPosition = new Vector2((border.sizeDelta.x - additionBorder.sizeDelta.x)/2, additionBorder.anchoredPosition.y);
			additionBackdrop.sizeDelta = new Vector2(additionTexts[0].textBounds.size.x + 8, additionBackdrop.sizeDelta.y);
		}
		if(!alreadyAnimated && timeAlive > timeUntilDeath / 2f)
		{
			alreadyAnimated = true;
			additionBorder.gameObject.SetActive(false);
			if(addingToBaseValue)
			{
				handZone.CallAnimateBaseValue((totalValue + valueToAdd) * mushroomProduct, 1f, true);
			}
			if(addingToMultiplier)
			{
				handZone.CallAnimateCardMultiplier((totalValue + valueToAdd) * mushroomProduct, 1f);
			}
			//backdropImage.color = secondColor;
			for(int i = 0; i < scoreTexts.Length; i++)
			{
				scoreTexts[i].text = afterAddition;
				scoreTexts[i].ForceMeshUpdate(true, true);
			}
			border.sizeDelta = new Vector2(scoreTexts[0].textBounds.size.x + 10, border.sizeDelta.y);
			backdrop.sizeDelta = new Vector2(scoreTexts[0].textBounds.size.x + 8, backdrop.sizeDelta.y);
			decompressedWidth = scoreTexts[0].textBounds.size.x + 10;
			StartCoroutine(Animate());
			//handZone.CallAnimateCardMultiplier();
		}
		if(timeAlive > timeUntilDeath / 2f && !alreadyExpanded)
		{
			alreadyExpanded = true;
			StartCoroutine(ExpandContract());
		}
		if(decompress)
		{
			if(compressionState < 1f)
			{
				compressionState += Time.deltaTime * handZone.handValues.gameOptions.gameSpeedFactor * decompressionSpeed;
			}
		}
		if(!decompress && !stayDecompressed)
		{
			if(compressionState > 0)
			{
				compressionState -= Time.deltaTime * handZone.handValues.gameOptions.gameSpeedFactor * compressionSpeed;
			}
		}
		float borderSize = Mathf.Lerp(compressedWidth, decompressedWidth, Mathf.Clamp(compressionState * 2, 0, 1));
		border.sizeDelta = new Vector2(borderSize, border.sizeDelta.y);
		backdrop.sizeDelta = new Vector2(borderSize - 2, backdrop.sizeDelta.y);
		float textScale = Mathf.Clamp((compressionState - 0.3f)*2, 0, 1);

		for(int i = 0; i < scoreTexts.Length; i++)
		{
			scoreTexts[i].GetComponent<RectTransform>().localScale = new Vector3(textScale,textScale,textScale);
		}
		if(timeAlive > timeUntilDeath)
		{
			Destroy(this.gameObject);
		}
    }
}
