using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolveCardScript : MonoBehaviour
{
	public RectTransform rt;
	//public RectTransform borderRT;
	//public Image borderImage;
	public Image rankImage;
	public RectTransform rankImageRT;
	public Image bigSuitImage;
	public RectTransform bigSuitImageRT;
	public Image detailImage;
	public RectTransform detailImageRT;
	public Image backgroundImage;
	public Image cardBackImage;
	public Image dissolveImage;
	public Sprite[] dissolveSprites;
	
    public void CopyCard(CardScript cardScript)
	{
		rt.anchoredPosition = cardScript.rt.anchoredPosition;
		if(cardScript.standardCard)
		{
			rankImage.sprite = cardScript.rankImage.sprite;
			rankImage.color = cardScript.rankImage.color;
			rankImageRT.anchoredPosition = cardScript.rankImage.GetComponent<RectTransform>().anchoredPosition;
			rankImageRT.sizeDelta = cardScript.rankImage.GetComponent<RectTransform>().sizeDelta;
			bigSuitImage.sprite = cardScript.bigSuitImage.sprite;
			bigSuitImage.color = cardScript.bigSuitImage.color;
			bigSuitImageRT.anchoredPosition = cardScript.bigSuitImage.GetComponent<RectTransform>().anchoredPosition;
			bigSuitImageRT.sizeDelta = cardScript.bigSuitImage.GetComponent<RectTransform>().sizeDelta;
		}
		else
		{
			rankImage.gameObject.SetActive(false);
			bigSuitImage.gameObject.SetActive(false);
		}
		detailImage.sprite = cardScript.detailImage.sprite;
		detailImage.color = cardScript.detailImage.color;
		detailImageRT.anchoredPosition = cardScript.detailImage.GetComponent<RectTransform>().anchoredPosition;
		detailImageRT.sizeDelta = cardScript.detailImage.GetComponent<RectTransform>().sizeDelta;
		backgroundImage.color = cardScript.backgroundImage.color;
		cardBackImage.sprite = cardScript.cardBackImage.sprite;
		cardBackImage.color = cardScript.cardBackImage.color;
		SoundManager.instance.PlayCardDissolveSound();
		StartCoroutine(DissolveAndDestroy(1f));
	}
	
	public IEnumerator DissolveAndDestroy(float dissolveTime)
	{
		float t = 0;
		while(t < dissolveTime)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			dissolveImage.sprite = dissolveSprites[Mathf.Clamp(Mathf.RoundToInt((t / dissolveTime) * (dissolveSprites.Length - 1)),0 , dissolveSprites.Length - 1)];
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
