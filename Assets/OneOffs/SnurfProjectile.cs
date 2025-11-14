using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnurfProjectile : MonoBehaviour
{
	public RectTransform rt;
	public float moveSpeed;
	public CardScript cardToDestroy;
	public bool alreadyAttached = false;
	
    void Update()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + moveSpeed * Time.deltaTime * GameOptions.instance.gameSpeedFactor, rt.anchoredPosition.y);
		if(!alreadyAttached && rt.anchoredPosition.x >= cardToDestroy.rt.anchoredPosition.x - 22.5f - 18f - 45f)
		{
			alreadyAttached = true;
			cardToDestroy.transform.SetParent(this.transform);
			cardToDestroy.rt.anchoredPosition = new Vector2(18f + 22.5f + 18f, 0);
		}
		if(rt.anchoredPosition.x > 355)
		{
			Destroy(this.gameObject);
		}
    }
}
