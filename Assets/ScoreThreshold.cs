using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScoreThreshold : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public RectTransform rt;
    public RectTransform border;
	public RectTransform backdrop;
	public Image backdropImage;
	public Image chipImage;
	public PokerChip chipScript;
	public TMP_Text[] scoreTexts;
	public float compressionState; // 0 is fully compressed, 1 is decompressed
	public float compressionSpeed;
	public float decompressionSpeed;
	public bool stayDecompressed;
	public bool decompress;
	public float compressedWidth;
	public float decompressedWidth;
	public float decompressedChipTransparency;
	public GameObject pointer;
	public float score;
	public int handsRemaining;
	public DissolveScript dissolveScript;
	
    void Start()
    {
        
    }
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		if(!stayDecompressed)
		{
			SoundManager.instance.PlaySlideOutSound();
		}
		decompress = true;
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		if(!stayDecompressed)
		{
			SoundManager.instance.PlaySlideOutSound(true);
		}
		decompress = false;
	}

    void Update()
    {
        if(decompress)
		{
			if(compressionState < 1f)
			{
				compressionState += Time.deltaTime * decompressionSpeed;
			}
		}
		if(!decompress && !stayDecompressed)
		{
			if(compressionState > 0)
			{
				compressionState -= Time.deltaTime * compressionSpeed;
			}
		}
		float borderSize = Mathf.Lerp(compressedWidth, decompressedWidth, Mathf.Clamp(compressionState * 2, 0, 1));
		border.sizeDelta = new Vector2(borderSize, border.sizeDelta.y);
		backdrop.sizeDelta = new Vector2(borderSize - 2, backdrop.sizeDelta.y);
		float textScale = Mathf.Clamp((compressionState - 0.3f)*2, 0, 1);
		//float textScale = Mathf.Clamp(compressionState, 0, 1);
		Color newColor = chipImage.color;
		newColor.a = Mathf.Lerp(1, decompressedChipTransparency, compressionState);
		chipImage.color = newColor;
		
		for(int i = 0; i < scoreTexts.Length; i++)
		{
			scoreTexts[i].GetComponent<RectTransform>().localScale = new Vector3(textScale,textScale,textScale);
		}
    }
}
