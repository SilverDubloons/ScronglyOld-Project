using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainMovement : MonoBehaviour
{
	private float positionNormalized; // 0 to 1
	public Vector2 startPosition;
	public Vector2 endPosition;
	public RectTransform rt;
	public bool goToEnd;
	public float speed;
	
    void Start()
    {
        
    }

    void Update()
    {
        if(goToEnd)
		{
			if(positionNormalized < 1f)
			{
				positionNormalized += Time.deltaTime * speed;
				positionNormalized = Mathf.Clamp(positionNormalized, 0f, 1f);
				rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, positionNormalized);
			}
		}
		else
		{
			if(positionNormalized > 0f)
			{
				positionNormalized -= Time.deltaTime * speed;
				positionNormalized = Mathf.Clamp(positionNormalized, 0f, 1f);
				rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, positionNormalized);
			}
		}
    }
}
