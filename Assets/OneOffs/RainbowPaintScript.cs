using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPaintScript : MonoBehaviour
{
    public RectTransform heightPaintMask;
	public RectTransform paintBrush;
	public float paintTime;
	public CardScript cardToChange;
	
	void Start()
	{
		StartCoroutine(Animate());
	}
	
	public IEnumerator Animate()
	{
		SoundManager.instance.PlayPaintSound();
		float t = 0;
		while(t < paintTime)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			heightPaintMask.sizeDelta = new Vector2(heightPaintMask.sizeDelta.x, Mathf.Lerp(0, 43, t / paintTime));
			paintBrush.anchoredPosition = new Vector2(paintBrush.anchoredPosition.x, Mathf.Lerp(4, -38, t / paintTime));
			yield return null;
		}
		if(cardToChange != null)
		{
			cardToChange.ChangeToRainbow();
		}
		t = 0;
		while(t < paintTime)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			heightPaintMask.sizeDelta = new Vector2(heightPaintMask.sizeDelta.x, Mathf.Lerp(43, 84, t / paintTime));
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
