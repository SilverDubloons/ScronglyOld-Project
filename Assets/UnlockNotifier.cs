using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnlockNotifier : MonoBehaviour
{
    public RectTransform rt;
	public MovingButton closeButton;
	public bool movingIn = true;
	public float moveSpeed;
	public bool movingOut = false;
	public TooltipScript tooltipScript;
	public TMP_Text[] titleTexts;
	public Image cardBorder;
	public Image cardBack;
	public Image cardDetail;
	
	public void SetupUnlockNotifier(int type, int num) // type 0 = deck, 1 = bauble | num = relative deck/bauble/etc
	{
		string titleText = "error!";
		tooltipScript.gameObject.SetActive(true);
		switch(type)
		{
			case 0:
			titleText = "New Deck Unlocked!";
			tooltipScript.SetupTooltip(Decks.instance.decks[num].description, Decks.instance.decks[num].deckName, 5);
			cardDetail.sprite = Decks.instance.decks[num].design;
			cardBack.color = Decks.instance.decks[num].backColor;
			cardDetail.color = Decks.instance.decks[num].designColor;
			break;
			case 1:
			titleText = "New Bauble Unlocked!";
			tooltipScript.SetupTooltip(BaubleScript.instance.baubles[num].baubleDescription, BaubleScript.instance.baubles[num].baubleName, BaubleScript.instance.baubles[num].baubleCategory);
			cardBack.gameObject.SetActive(false);
			cardDetail.gameObject.SetActive(false);
			cardBorder.sprite = BaubleScript.instance.baubles[num].baubleImage;
			break;
		}
		for(int i = 0; i < titleTexts.Length; i++)
		{
			titleTexts[i].text = titleText;
		}
		tooltipScript.gameObject.SetActive(false);
	}
	
	void Update()
	{
		if(movingIn && rt.anchoredPosition.x > -45f)
		{
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - moveSpeed * Time.deltaTime, rt.anchoredPosition.y);
		}
		if(movingIn && rt.anchoredPosition.x <= -45f)
		{
			rt.anchoredPosition = new Vector2(-45f, rt.anchoredPosition.y);
			movingIn = false;
			closeButton.ChangeDisabled(false);
		}
		if(movingOut && rt.anchoredPosition.x < 42f)
		{
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + moveSpeed * Time.deltaTime, rt.anchoredPosition.y);
		}
		if(movingOut && rt.anchoredPosition.x > 42f)
		{
			Destroy(this.gameObject);
		}
	}
	
	public void CloseClicked()
	{
		movingOut = true;
		closeButton.ChangeDisabled(true);
	}
}
