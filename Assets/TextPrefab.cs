using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPrefab : MonoBehaviour
{
	public RectTransform rt;
	public TMP_Text shadowText;
	public TMP_Text opaqueText;
	public void ChangeShadowText(string input)
	{
		shadowText.text = input;
	}
	public void ChangeOpaqueText(string input)
	{
		opaqueText.text = input;
	}
    public void ChangeTexts(string input)
	{
		shadowText.text = input;
		opaqueText.text = input;
	}
	public void ChangeFontSizeMax(float input)
	{
		shadowText.fontSizeMax = input;
		opaqueText.fontSizeMax = input;
	}
	public float GetDesiredHeight()
	{
		shadowText.ForceMeshUpdate(true, true);
		return shadowText.textBounds.size.y;
	}
	public void ChangeAlignmentToLeft()
	{
		shadowText.alignment = TextAlignmentOptions.Left;
		opaqueText.alignment = TextAlignmentOptions.Left;
	}
}