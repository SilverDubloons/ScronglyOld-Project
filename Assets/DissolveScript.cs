using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveScript : MonoBehaviour
{
    public Sprite[] dissolveSprites;
	public UnityEngine.UI.Image image;
	public GameObject baseObject;
	
	public void StartDissolving(float dissolveTime)
	{
		StartCoroutine(DissolveAndDestroy(dissolveTime));
	}
	
	public IEnumerator DissolveAndDestroy(float dissolveTime)
	{
		float t = 0;
		while(t < dissolveTime)
		{
			t += Time.deltaTime;
			image.sprite = dissolveSprites[Mathf.Clamp(Mathf.RoundToInt((t / dissolveTime) * (dissolveSprites.Length - 1)),0 , dissolveSprites.Length - 1)];
			yield return null;
		}
		Destroy(baseObject);
	}
}
