using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameAnimation : MonoBehaviour
{
	public static NewGameAnimation instance;
	public int newGameAnimationStage = 0;
	public RectTransform newGameAnimationRT;
	
	void Awake()
	{
		instance = this;
	}
	
    public IEnumerator Animate()
	{
		float t = 0;
		Vector3 startingSize = Vector2.zero;
		newGameAnimationRT.gameObject.SetActive(true);
		Vector3 endingSize = new Vector3(1100f,1300f,1f);
		while(t < 1)
		{
			t += Time.deltaTime;// * GameOptions.instance.gameSpeedFactor;
			newGameAnimationRT.sizeDelta = Vector2.Lerp(startingSize, endingSize, t / 1);
			yield return null;
		}
		newGameAnimationStage = 1;
		while(newGameAnimationStage < 2)
		{
			yield return null;
		}
		t = 0;
		while(t < 1)
		{
			t += Time.deltaTime;// * GameOptions.instance.gameSpeedFactor;
			newGameAnimationRT.sizeDelta = Vector2.Lerp(endingSize, startingSize, t / 1);
			yield return null;
		}
		newGameAnimationRT.gameObject.SetActive(false);
	}
}
