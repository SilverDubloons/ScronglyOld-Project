using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardQuantityLabel : MonoBehaviour
{
    public RectTransform rt;
	public TMP_Text shadowText;
	public TMP_Text opaqueText;
	public Color negativeColor;
	
	public void ChangeQuantity(int input)
	{
		shadowText.text = input.ToString();
		opaqueText.text = input.ToString();
		if(input < 0)
		{
			shadowText.color = negativeColor;
			opaqueText.color = negativeColor;
		}
	}
}
