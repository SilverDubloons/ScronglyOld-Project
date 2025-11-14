using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Promotion : MonoBehaviour
{
	public RectTransform rt;
	public float moveTime;
	public CardScript cardToPromote;
	public float pixelsToMove;
	public Vector3 endRotation;
	public int ranksToIncrease;
	public Image mainImage;
	public Sprite demotionImage;
	public bool demotion = false;
	
	public IEnumerator MoveUp()
	{
		if(ranksToIncrease < 0)
		{
			mainImage.sprite = demotionImage;
		}
		float t = 0;
		Vector2 startPostion = new Vector2(0, -pixelsToMove);
		Vector2 midPosition = Vector2.zero;
		Vector2 endPosition = new Vector2(0, pixelsToMove);
		while(t < moveTime)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.anchoredPosition = Vector2.Lerp(startPostion, midPosition, t / moveTime);
			yield return null;
		}
		yield return new WaitForSeconds(moveTime / 2);
		StartCoroutine(Animate());
		if(ranksToIncrease >= 0)
		{
			SoundManager.instance.PlayPromotionSound();
		}
		else
		{
			SoundManager.instance.PlayDemotionSound();
		}
		int promotionRank = cardToPromote.rankInt + ranksToIncrease;
		if(promotionRank < 0 )
		{
			promotionRank += 13;
		}
		if(promotionRank > 12)
		{
			promotionRank -= 13;
		}
		cardToPromote.ChangeRank(promotionRank);
		cardToPromote.UpdateCardValueTooltip();
		cardToPromote.deckViewerClone.UpdateCardValueTooltip();
		yield return new WaitForSeconds(moveTime / 2);
		t = 0;
		while(t < moveTime)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.anchoredPosition = Vector2.Lerp(midPosition, endPosition, t / moveTime);
			yield return null;
		}
		Destroy(this.gameObject);
	}
	
	public IEnumerator Animate()
	{
		float t = 0;
		Quaternion originalRotationQ = rt.localRotation;
		Quaternion destinationRotationQ = Quaternion.Euler(endRotation);
		while(t < 0.1)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(originalRotationQ, destinationRotationQ, t / 0.1f);
			yield return null;
		}
		t = 0;
		while(t < 0.15)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(destinationRotationQ, originalRotationQ, t / 0.15f);
			yield return null;
		}
	}
}
