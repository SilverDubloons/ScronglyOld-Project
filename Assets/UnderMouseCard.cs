using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderMouseCard : MonoBehaviour
{
    public RectTransform rt;
	public GameObject tempCard;
	
	public void DestroyTempCard()
	{
		if(tempCard != null)
		{
			Destroy(tempCard);
		}
	}

    // Update is called once per frame
    void Update()
    {
		Vector2 mousePos = new Vector2((Input.mousePosition.x/Screen.width)*640,((Input.mousePosition.y/Screen.height))*360);
		rt.anchoredPosition = mousePos;
    }
}
