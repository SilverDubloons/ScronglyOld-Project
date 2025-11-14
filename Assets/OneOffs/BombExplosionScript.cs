using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosionScript : MonoBehaviour
{
	public float expandTime;
	public RectTransform rt;
	public RectTransform boomRT;
	public Vector3 startingScale;
	public Vector3 endingScale;
	public BombFragmentScript[] bombFragments;
	public GameObject cardToDestroy;
	
    void Start()
    {
        StartCoroutine(Animate());
    }
	
	private int deadFragments = 0;
	
	public void FragmentDied()
	{
		deadFragments++;
		if(deadFragments >= 4)
		{
			Destroy(this.gameObject);
		}
	}
	
	public IEnumerator Animate()
	{
		float t = 0;
		while(t < expandTime)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			boomRT.localScale = Vector3.Lerp(startingScale, endingScale, t / expandTime);
			yield return null;
		}
		if(cardToDestroy)
		{
			for(int i = 0; i < bombFragments.Length; i++)
			{
				bombFragments[i].gameObject.SetActive(true);
			}
			if(cardToDestroy.GetComponent<CardScript>().tooltipScript != null)
			{
				Destroy(cardToDestroy.GetComponent<CardScript>().tooltipScript.gameObject);
			}
			Destroy(cardToDestroy);
		}
		t = 0;
		while(t < expandTime / 2)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			boomRT.localScale = Vector3.Lerp(endingScale, startingScale, t / (expandTime / 2));
			yield return null;
		}
		Destroy(boomRT.gameObject);
	}

    // Update is called once per frame
    void Update()
    {
        
		
    }
}
