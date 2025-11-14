using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMirror : MonoBehaviour
{
	public RectTransform rt;
	public float changeTime;
	public UnityEngine.UI.Image spellImage;
	public CardScript cardToChange;
	public int newRank;
	public int newSuit;
	public bool newCardValueChanged;
	public float newValue;
	public bool newCardMultChanged;
	public float newMult;
	
    public IEnumerator Animate()
	{
		float t = 0;
		while(t < changeTime)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			spellImage.fillAmount = Mathf.Lerp(0, 1, t / changeTime);
			yield return null;
		}
		cardToChange.ChangeRank(newRank);
		cardToChange.ChangeSuit(newSuit);
		cardToChange.cardValueHasBeenChanged = newCardValueChanged;
		cardToChange.cardValue = newValue;
		cardToChange.UpdateCardValueTooltip();
		cardToChange.deckViewerClone.cardValueHasBeenChanged = newCardValueChanged;
		cardToChange.deckViewerClone.cardValue = newValue;
		cardToChange.deckViewerClone.UpdateCardValueTooltip();
		cardToChange.cardMultiplierHasChanged = newCardMultChanged;
		cardToChange.cardMultiplier = newMult;
		cardToChange.UpdateCardMultTooltip();
		cardToChange.deckViewerClone.cardMultiplierHasChanged = newCardMultChanged;
		cardToChange.deckViewerClone.cardMultiplier = newMult;
		cardToChange.deckViewerClone.UpdateCardMultTooltip();
		t = 0;
		while(t < changeTime)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			spellImage.fillAmount = Mathf.Lerp(1, 0, t / changeTime);
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
