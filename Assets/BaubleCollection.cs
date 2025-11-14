using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaubleCollection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	//public RectTransform baubleParentRectTransform
	public Transform baubleParentTransform;
	public RectTransform scrollviewRT;
	public RectTransform backdropRT;
	public RectTransform contentRT;
	public RectTransform baseRT;
	bool mouseOver = false;
	private Vector2 startPosition = new Vector2(0,0);
	private Vector2 endPosition = new Vector2(125,0);
	private float moveState;
	public float moveSpeed;
	public List<BaubleItem> baublesInCollection = new List<BaubleItem>();
	
	public void ResetBaubleCollection()
	{
		for(int i = 0; i < baublesInCollection.Count; i++)
		{
			Destroy(baublesInCollection[i].gameObject);
		}
		baublesInCollection.Clear();
		baseRT.gameObject.SetActive(false);
	}
	
	void Start()
	{
		//baseRT.gameObject.SetActive(false);
		//ReorganizeBaubles();
	}
	
	public void ReorganizeBaubles()
	{
		baseRT.gameObject.SetActive(true);
		int rows = baublesInCollection.Count / 2 + (baublesInCollection.Count % 2 == 0 ? 0 : 1);
		backdropRT.sizeDelta = new Vector2(backdropRT.sizeDelta.x, Mathf.Min((rows * 50 + 15), 360));
		backdropRT.anchoredPosition = new Vector2(backdropRT.anchoredPosition.x, Mathf.Lerp(239, 0, backdropRT.sizeDelta.y/360));
		scrollviewRT.sizeDelta = new Vector2(scrollviewRT.sizeDelta.x, Mathf.Min((rows * 50 + 5), 350));
		contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, rows * 50 + 5);
	}
	
	void Update()
	{
		if(mouseOver && moveState < 1f)
		{
			moveState += moveSpeed * Time.deltaTime;
			if(moveState > 1f)
			{
				moveState = 1f;
			}
			baseRT.anchoredPosition = Vector2.Lerp(startPosition, endPosition, moveState);
		}
		if(!mouseOver && moveState > 0)
		{
			moveState -= moveSpeed * Time.deltaTime;
			if(moveState < 0)
			{
				moveState = 0;
			}
			baseRT.anchoredPosition = Vector2.Lerp(startPosition, endPosition, moveState);
		}
		
	}
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		SoundManager.instance.PlaySlideOutSound();
		transform.SetSiblingIndex(transform.parent.childCount - 1);
		mouseOver = true;
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		SoundManager.instance.PlaySlideOutSound(true);
		mouseOver = false;
	}
}
