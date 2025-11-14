using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMarker : MonoBehaviour
{
	public RectTransform rt;
	public float moveTime;
	public CardScript cardToChangeRank;
	public int rankChange;
	
	public Vector3 firstRotation;
	public Vector3 secondRotation;
	
	public Vector2 startPostion;
	public Vector2 midPosition;
	public Vector2 endPosition;
	
    public IEnumerator MoveUp()
	{
		float t = 0;
		
		while(t < moveTime)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.anchoredPosition = Vector2.Lerp(startPostion, midPosition, t / moveTime);
			yield return null;
		}
		yield return new WaitForSeconds(moveTime / 4f);
		StartCoroutine(Animate());
		SoundManager.instance.PlayMarkerSound();

		cardToChangeRank.ChangeRank(rankChange);
		cardToChangeRank.UpdateCardValueTooltip();
		cardToChangeRank.deckViewerClone.UpdateCardValueTooltip();
		
		yield return new WaitForSeconds(moveTime * 1.25f);
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
		Quaternion firstRotationQ = Quaternion.Euler(firstRotation);
		Quaternion secondRotationQ = Quaternion.Euler(secondRotation);
		while(t < moveTime / 4)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(originalRotationQ, firstRotationQ, t / (moveTime / 4));
			yield return null;
		}
		t = 0;
		while(t < moveTime / 4)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(firstRotationQ, secondRotationQ, t / (moveTime / 4));
			yield return null;
		}
		t = 0;
		while(t < moveTime / 4)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(secondRotationQ, firstRotationQ, t / (moveTime / 4));
			yield return null;
		}
		t = 0;
		while(t < moveTime / 4)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(firstRotationQ, originalRotationQ, t / (moveTime / 4));
			yield return null;
		}
	}
}
