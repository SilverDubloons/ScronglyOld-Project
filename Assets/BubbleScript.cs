using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
	public RectTransform rt;
	public float timeAlive;
	public float verticalSpeed;
	public RectTransform fillRT;
	private float randomAngle;
	
    void Start()
    {
        randomAngle = Random.Range(0,360);
    }

    void Update()
    {
		timeAlive += Time.deltaTime;
		float sinWave = Mathf.Sin(timeAlive*4 + randomAngle) / 45;
		float verticalMovement = verticalSpeed * Time.deltaTime;
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + sinWave, rt.anchoredPosition.y + verticalMovement);
		if(rt.anchoredPosition.y > fillRT.sizeDelta.y)
		{
			Destroy(this.gameObject);
		}
    }
}
