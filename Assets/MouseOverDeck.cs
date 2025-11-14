using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverDeck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Transform deckParent;
	public DeckPreview deckPreview;
	public int deckInt;
	public Rect deckRect;
	private bool mouseOver = false;
	
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
		Vector2 mousePos = Input.mousePosition;
		mousePos = mousePos / new Vector2(Screen.width, Screen.height);
		mousePos = mousePos * new Vector2(640, 360);
		//print(mousePos);
		if(deckRect.Contains(mousePos))
		{
			if(!deckPreview.deckViewer.isOpen)
			{
				mouseOver = true;
				SoundManager.instance.PlaySlideOutSound(false);
				deckPreview.GenerateDeckPreview(deckParent, deckInt);
				deckPreview.mouseOver = true;
				return;
			}
		}	
	}
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		if(mouseOver)
		{
			if(!deckPreview.deckViewer.isOpen)
			{
				SoundManager.instance.PlaySlideOutSound(true);
			}
			deckPreview.mouseOver = false;
			mouseOver = false;
		}
	}
}
