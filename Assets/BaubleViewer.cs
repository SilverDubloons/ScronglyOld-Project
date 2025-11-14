using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BaubleViewer : MonoBehaviour
{
	public RectTransform rt;
	public Image baubleImage;
	public TMP_Text quantityTextShadow;
    public TMP_Text quantityText;
	public GameObject notInPoolObject;
	
	public void SetupBaubleViewer(int baubleNumber, int baubleQuantity, bool isInPool)
	{
		baubleImage.sprite = BaubleScript.instance.baubles[baubleNumber].baubleImage;
		if(baubleQuantity != 0)
		{
			quantityTextShadow.text = baubleQuantity.ToString();
			quantityText.text = baubleQuantity.ToString();
		}
		if(!isInPool)
		{
			notInPoolObject.SetActive(true);
		}
	}
}
