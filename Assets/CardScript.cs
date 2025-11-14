using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/*
	Started 28 Feb, 2024
	
	17 March 2024
	
	It's been feeling like a real game for a few days now. Implemented Señor Pail today, the first bauble that does more than change a variable. Not too bad! Thinking about what's left to come.
	
	- Nonstandard cards
		- Consumable cards activate when played as part of a hand, affecting other cards then disapearing. These should be quite strong for a couple reasons
			- They might not get drawn
			- They might not get drawn when you want/need them
			- They may sit in your deck for multiple antes doing nothing
		- Consumables should probably be considered 'extra' cards, meaning they do not take space in your hand.
	- Card upgrades
	- More baubles!
	- Main menu, daily run, random run, lose condition, win condition? or endless?
	- Let it ride (mechanic wherein you can choose to not shuffle after the shop and get a reward)
	- Toy box (random cards, baubles and zodiacs at a discount, purchasable once per shop)
	- Expanded deck view
	
	19 March 2024
	
	Been implementing more baubles. Had a run today that froze the game because I had bought so many baubles the store ran out of unique ones to create. Had the capability of making straight flushes with two cards, and the baubles that mulitply for straights, flushes and straight flushes, plus like 15 rainbow aces and Señor Pail were making me rich. This is the kind of thing I was thinking about when this game was just an idea, I love it.
	
	30 March 2024
	
	Over a month been working on this now, still excited to work on it each day. Poker games seem to be my thing!
	
	Today I played the daily game and got to Ante 17, it was a lot of fun! Straight flush rainbow build, but I got unlucky and didn't see any good rare baubles. Thought about a system where all bauble weights get increased by 1 each ante so that the deeper you are the more likely you are to see rare baubles.
	
	Game is surprisingly fun, I hope people enjoy it when they get their hands on it.
	
	11 April 2024
	
	More mechanics have been added, started writing patch notes on itch.io which have kind of replaced this pseudo journal. Getting good feedback, although lots of people have been finding it hard to figure out mechanically. Today's patch will help with that somewhat.
	
	
	**************************************************
	
	Things to think about before building - 
	
	New saved data? Make sure old data works
	Get DataText and HandData off the screen!
	Make sure the tutorial is still working as expected. Remember when you added the opening stinger and the tutorial was on top of it?
	Test and rebuild if there are any issues!
	
	This implementation has the side effect of the card being dragged appearing under some other cards in the hand.
	This is because the sorting algorithm in HandScript relies on the hierarchy of children, which defines which cards
	get drawn on top of each other
	Idea for workaround - have a high level duplicate just under the mouse. 
	^ Done
	
	Issue - When hands are large, the left side of the card gets covered up. It would be better if the right side were covered, so that the suit and rank were more easily conveyed.
	^ Done

	There's still the issue of churning through the deck after you've given up on getting the score thresholds. Perhaps once there is no more score thresholds, a player has X (2?) hands until 'Fatigue' sets in. While fatigued, all cards in hand are discarded after playing a hand.

	There's that weird glitch when auto sort is on where the card tries to move back to the hand, gotta fix that.
	
	The deck preview is generated when hovering over the deck, and when multiple cards are being drawn, cards that are about to not be in the deck are counted as in the deck.
	
	Since the idea of handzones is becoming increasingly problematic with the design, consider having them be optional purchases and/or need to be upgraded. Like they could start out three cards only, and be additive. Upgrades increase number of accepted cards and change to multiplicative
	
	GridPoker as a bonus round? lol
	Another idea for bonus round - reels. 5x3 grid and you can adjust them vertically to make three hands. I'm never doing any of these
	
	Idea for bauble UI - a little knob on the left of the screen opens a drawer that has all knobs. When opened they jostle around a bit like they have physics, but when moused over they go upright and in order.
	
	Cards that aren't counted in scoring should be differentiated - making them fainter should do it.
	^ Done, used an X
	
	Add bubbles to the slime vial
	^ Done
	
	Create a preview window showing cards remaining in deck, show when hovered over.
	^ Done
	
	In the shop, have an area where the player can drag an item to save for later. Since the shop is always refreshing it's stock, it'd be nice to have the option to buy something later. Maybe it costs double? Call it layaway or something
	
	What if instead of missing a threshold causing the topmost to be gone, it lowers the rest of them to the next lowest threshhold. More intuitive and better UI, nearly the same outcome
	^ Done
	
	Mechanics Ideas
		- To keep players from playing one card at a time to churn through deck, whenever all hands are played, discard the remaining cards in hand. Perhaps items can mitigate this. (keep rightmost card as extra card when playing hand or something)
		- Let it ride. If you still have > 1/2 your deck at the end of an ante, you can choose to not reshuffle and get good
		gift right away
	
	Name ideas
		- Deuce Heaven
		- Wacky Cards. Could call non-standard cards wacky
		
	Art Ideas
		- The background seems to be green. Makes sense to pick one color and not make any UI elements that.
		- With a green background, could do like that childrens playmat with roads and trees and houses. A lot of work, though
		- Original idea is to just have some random lasers or particle systems going wacky in the background. Can speed them up as the game gets deeper?
		- When the player gets a massive hand that beats the score target by a large margin, maybe have the top of the vial fly off and spew goo everywhere. It would be funny if some got on the screen and a windshield wiper came to take it off
		- What about little animations that play when the threshold is reached? Like the goo vial gets a little squirt top on it and it squirts into a bowl and a cat comes and licks it up, then gives a thumbs up and says "YUM!"
		- Idea for silly ending thing - Bring up a screen that allows the player to select one of three emojis to describe how
		they feel. (Surprised face, sad face and angry face) when they pick one, a garbage can appears on the side of the screen
		and the emoji is deposited into it. Your feelings have been placed in the garbage, as they are worthless.
		
	Ideas for items (Baubles?)
		- Whenever you discard 2 or fewer cards (increase by 2 for each copy), they instead go on the bottom of your deck
		✓ Decrease cards needed for a straight and sraight flush by 1. Straights can wrap
		✓ Increase max gap in straights by 1. Straights can wrap
		✓ Decrease cards needed for a flush by 1
		- Once per ante, choose two cards from your discard pile and add them back to your draw pile, then shuffle your deck
		✓ See the top card of your deck (additional copies allow to see more on hover, will take some UI work)
		- Whenever you play a single card for a hand, increase that cards value by 50 permanently
		✓ Increase hand size by 1
		✓ Increase max number of cards discarded at once by 1
			- Changed to a one time purchase that allows any number to be discarded at once.
		✓ Increase discards per ante by 1
		✓ All cards purchasable in the shop are rainbow, and only cost 1 chip (max 1)
		- Increase sell value of all items by 1 (max 1)
		- Send your deuces to deuce heaven (what does this mean?)
		- Whenever you shuffle your deck, draw two extra cards
		- Whenever you discard an ace, increase it's base value by 15
		✓ Whenever you discard two aces (or more) at once, earn one chip
		- When you finish an ante, cards in your hand go into your draw pile instead of your discard pile (cheap) (unlocks after successfully letting it ride one time)
		- Connect 4 piece - for each 4 in a hand, do something??, or something with 4 of a kind.
			- One could be for four of a kind in played hand, other could check if you have four of a kind in your held hand.
		- Whenever you destroy a card, level up hand that was played when it was destroyed.
		✓ Piggy bank - turn on interest mechanic
		- Upgrade the hand of your first discard, if you discard 5 or less cards.
		- Arcane Octosphere - after playing a hand with an eight, shake the arcane octoshpere. Save this for more mechanics. Maybe add a "temporary" powerful card to hand/deck
		
		Items are going to be pieces from other games.
		Item name ideas:
		- Hankerin' Hankerin' Hippopotamus
		- Skrabble Pieces
		- Horseshoe
		- Yahtzee
		- Twister
		- Guess Who
		- Mouse Trap
		- Operation (funny bone?)
		- Crocodile Dentist (Alligator Orthodontist)
		- Mr Bucket
		- Crossfire
		- Mastermind
		- Jenga
		- Dominoes
		- Lego
		- Marbles
		- Battletech mech figurine
		- DnD character
		- Tamagochi aka Digital Hamster
		- Nintendo controller
		- Pog
		- Super soaker
		- Moon shoes
		- Finger Monster
		- Pikachu
		- Etch a Sketch
		- Nerf blaster
		- Rubik's cube
		- Magic 8 Ball
		- Battleship
		- Slinky
		- Furby
		- Pez, 90's candy in general
		- d20, d6 etc?
		
	Cards
		- Slot machine card. When placed in a dropzone, it gets randomly assigned a rank and suit, but has a high base value
		and card upgrades, and gets locked so you have to play it.
		- Bomb card. After scoring, destroys other cards in hand zone
		- Glue. If played with exactly two other cards, fuse them together. Add all ranks, values and enhancements (or maybe it happens to the cards on either side)
		- Gromlins - they hide in booster packs. Automatically added to deck. No points, but killed when played in a hand. Maybe certain cards get boosts based on number of gromlins killed? Maybe those items don't show up until you've killed at least one gromlin.
		
	Tips
		- Double click on the rank or suit sorting options to keep your hand sorted
		- Nonstandard cards do not take up space in your hand
		- Progress will not be saved if you close your browser tab mid run. Abandon your run or run out of cards!
		- You cannot get unlocks when playing daily or seeded games
*/

public class CardScript : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	public int rankInt; // 0 to 12
	public char rankChar; // 2, 3, 4, 5, 6, 7, 8, 9, T, J, Q, K, A
	public string rankString; // Duece, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
	public char suitChar; // h, d, c, s, r (c and h taken for chromatic) maybe just call it rainbow - easy suit graphic
	public char suitSymbol; // ♥ ♦ ♣ ♠ ☼
	public int suitInt;	//  h = 0, d = 1, c = 2, s = 3, 4 = chromatic | For sorting purposes.
	
	public RectTransform rt;
	public RectTransform borderRT;
	public Image borderImage;
	public Image rankImage;
	public Image bigSuitImage;
	public Image detailImage;
	public Image backgroundImage;
	public Image cardBackImage;
	
	public bool canMove = false;
	//public bool isPlaced = false;
	
	private bool moving;
	private IEnumerator moveCoroutine;
	
	public TMP_Text dataText;
	
	private Vector2 startedDraggingFrom;
	
	public HandScript handScript;
	
	public DropZone dropZonePlacedIn;
	
	public float cardValue;
	public bool cardValueHasBeenChanged = false;
	
	public float cardMultiplier = 0;
	public bool cardMultiplierHasChanged = false;
	//public bool addingToCardMultiplierThisHand;
	public float amountToAddToBaseValueThisHand = 0;
	public float amountToAddToMultiplierThisHand = 0;
	
	public bool faceDown = true;
	public bool countCardInScoring;
	public bool countCardInHand;
	public bool standardCard; // card has typical suit and rank
	public bool noImpactOnHandSize;
	public bool cannotBePlayed = false;
	
	public bool cardIsInShop = false;
	public int cardLocation = 0; // 0 = shop / unknown. 1 = draw pile, 2 = hand, 3 = discard
	
	public GameObject cardValueTooltipObject;
	public RectTransform cardValueTooltipBorder;
	public RectTransform cardValueTooltipBackdrop;
	public TMP_Text[] cardValueTooltipTexts;
	public RectTransform cardMultiplierTooltipRT;
	public RectTransform cardMultiplierTooltipBorder;
	public RectTransform cardMultiplierTooltipBackdrop;
	public TMP_Text[] cardMultiplierTooltipTexts;
	
	//private bool mouseOver = false;
	public TooltipScript tooltipScript;
	
	public int nonStandardCardNumber;
	public bool isDeckViewerClone = false;
	public int oldSiblingIndex = 0;
	public float epsilon = 0.0001f;
	
	public CardScript deckViewerClone = null;
	public bool justForShow = false;
	public bool cannotBeMovedFromHandZone = false;
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		if(isDeckViewerClone)
		{
			oldSiblingIndex = transform.GetSiblingIndex();
			transform.SetSiblingIndex(transform.parent.childCount - 1);
		}
		//mouseOver = true;
		if(!justForShow)
		{
			if(((handScript.handValues.gameOptions.onlyShowBaseValueIfModified && cardValueHasBeenChanged) || !handScript.handValues.gameOptions.onlyShowBaseValueIfModified) && !faceDown && countCardInScoring)
			{
				cardValueTooltipObject.SetActive(true);
				cardMultiplierTooltipRT.anchoredPosition = new Vector2(cardMultiplierTooltipRT.anchoredPosition.x, 31.5f + 18f);  
			}
			else
			{
				cardMultiplierTooltipRT.anchoredPosition = new Vector2(cardMultiplierTooltipRT.anchoredPosition.x, 31.5f);
			}
			if(Mathf.Abs(cardMultiplier) > epsilon && !faceDown)
			{
				cardMultiplierTooltipRT.gameObject.SetActive(true);
			}
		}
		if(tooltipScript != null && !faceDown)
		{
			tooltipScript.gameObject.SetActive(true);
			tooltipScript.transform.SetParent(this.transform);
			if(IsCardRotated())
			{
				tooltipScript.rt.anchoredPosition = new Vector2(115, 0);
			}
			else
			{
				tooltipScript.rt.anchoredPosition = new Vector2(105, 0);
			}
			tooltipScript.transform.SetParent(handScript.tooltipParent);
		}
		if(!cardIsInShop && !faceDown)
		{
			StartCoroutine(Expand());
		}
    }
	
	private bool IsCardRotated()
	{
		return Quaternion.Angle(borderRT.rotation, Quaternion.identity) > 0.001f;
	}
	
	public IEnumerator Expand()
	{
		SoundManager.instance.PlayTickSound();
		float t = 0;
		Vector3 startingScale = new Vector3(1f,1f,1f);
		Vector3 endingScale = new Vector3(1.1f,1.1f,1f);
		while (t < 0.1f)
		{
			t += Time.deltaTime;
			borderRT.localScale = Vector3.Lerp(startingScale, endingScale, t/0.1f);
			yield return null;
		}
		/* t = 0;
		while(t < 0.1f)
		{
			t += Time.deltaTime;
			borderRT.localScale = Vector3.Lerp(endingScale, startingScale, t/0.1f);
			yield return null;
		} */
		borderRT.localScale = endingScale;
	}
	
	public IEnumerator Contract()
	{
		Vector3 startingScale = new Vector3(1f,1f,1f);
		Vector3 endingScale = new Vector3(1.1f,1.1f,1f);
		float t = 0;
		while(t < 0.1f)
		{
			t += Time.deltaTime;
			borderRT.localScale = Vector3.Lerp(endingScale, startingScale, t/0.1f);
			yield return null;
		}
		borderRT.localScale = startingScale;
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		if(isDeckViewerClone)
		{
			transform.SetSiblingIndex(oldSiblingIndex);
		}
		//mouseOver = false;
		if(!justForShow)
		{
			if(cardValueTooltipObject.activeSelf)
			{
				cardValueTooltipObject.SetActive(false);
			}
			if(cardMultiplierTooltipRT.gameObject.activeSelf)
			{
				cardMultiplierTooltipRT.gameObject.SetActive(false);
			}
		}
		if(tooltipScript != null)
		{
			tooltipScript.gameObject.SetActive(false);
			tooltipScript.transform.SetParent(this.transform);
		}
		if(!cardIsInShop && !faceDown)
		{
			StartCoroutine(Contract());
		}
	}
	
	public void ChangeCardMultiplier(float addAmount, float multiplyAmount)
	{
		cardMultiplierHasChanged = true;
		cardMultiplier += addAmount;
		cardMultiplier = cardMultiplier * multiplyAmount;
		UpdateCardMultTooltip();
		if(deckViewerClone != null)
		{
			deckViewerClone.ChangeCardMultiplier(addAmount, multiplyAmount);
		}
	}
	
	public void ChangeCardValue(float addAmount, float multiplyAmount)
	{
		cardValueHasBeenChanged = true;
		cardValue += addAmount;
		cardValue = cardValue * multiplyAmount;
		UpdateCardValueTooltip();
		if(deckViewerClone != null)
		{
			deckViewerClone.ChangeCardValue(addAmount, multiplyAmount);
		}
	}
	
	void OnDestroy()
	{
		//print("Destroying " + GetCardNameCharSuit());
		if(deckViewerClone != null)
		{
			handScript.deckViewer.clonedCards.Remove(deckViewerClone);
			Destroy(deckViewerClone.gameObject);
		}
	}
	
	public void UpdateCardMultTooltip()
	{
		bool isActive = gameObject.activeSelf;
		gameObject.SetActive(true);
		bool multActive = cardMultiplierTooltipRT.gameObject.activeSelf;
		cardMultiplierTooltipRT.gameObject.SetActive(true);
		for(int i = 0; i < cardMultiplierTooltipTexts.Length; i++)
		{
			cardMultiplierTooltipTexts[i].text = handScript.handValues.ConvertFloatToString(cardMultiplier);
			cardMultiplierTooltipTexts[i].ForceMeshUpdate(true, true);
		}
		cardMultiplierTooltipBorder.sizeDelta = new Vector2(cardMultiplierTooltipTexts[1].textBounds.size.x + 10, cardMultiplierTooltipBorder.sizeDelta.y);
		cardMultiplierTooltipBackdrop.sizeDelta = new Vector2(cardMultiplierTooltipTexts[1].textBounds.size.x + 8, cardMultiplierTooltipBackdrop.sizeDelta.y);
		cardMultiplierTooltipRT.gameObject.SetActive(multActive);
		gameObject.SetActive(isActive);
	}
	
	public void UpdateCardValueTooltip()
	{
		bool isActive = gameObject.activeSelf;
		gameObject.SetActive(true);
		bool valActive = cardValueTooltipObject.activeSelf;
		cardValueTooltipObject.SetActive(true);
		for(int i = 0; i < cardValueTooltipTexts.Length; i++)
		{
			cardValueTooltipTexts[i].text = handScript.handValues.ConvertFloatToString(cardValue);
			cardValueTooltipTexts[i].ForceMeshUpdate(true, true);
		}
		//print("updating " + GetCardNameCharSuit());
		cardValueTooltipBorder.sizeDelta = new Vector2(cardValueTooltipTexts[1].textBounds.size.x + 10, cardValueTooltipBorder.sizeDelta.y);
		cardValueTooltipBackdrop.sizeDelta = new Vector2(cardValueTooltipTexts[1].textBounds.size.x + 8, cardValueTooltipBackdrop.sizeDelta.y);
		cardValueTooltipObject.SetActive(valActive);
		gameObject.SetActive(isActive);
	}
	
	public string GetCardNameCharSuit()
	{
		return "" + rankChar + suitChar;
	}
	
	public void OnBeginDrag(PointerEventData eventData)
    {
		if(canMove)
		{
			SoundManager.instance.PlayCardPickupSound();
			RevertToOriginalImage();
			startedDraggingFrom = rt.anchoredPosition;
			borderRT.rotation = Quaternion.identity;
			if(!cardIsInShop)
			{
				if(handScript.selectedCards.Contains(this))
				{
					handScript.selectedCards.Remove(this);
					handScript.SelectedCardsUpdated();
				}
				if(dropZonePlacedIn != null)
				{
					transform.SetParent(GameObject.FindWithTag("LooseCards").transform);
					dropZonePlacedIn.CardRemoved(true);
					handScript.PlacedCardsUpdated(false);
					dropZonePlacedIn = null;
				}
			}
			GameObject duplicate = Instantiate(this.gameObject, new Vector3(0,0,0), Quaternion.identity, handScript.underMouseCard.transform);
			duplicate.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,0,0);
			CardScript duplicateCardScript = duplicate.GetComponent<CardScript>();
			duplicateCardScript.borderRT.localScale = new Vector3(1.1f, 1.1f, 1f);
			duplicateCardScript.deckViewerClone = null;
			duplicateCardScript.borderImage.raycastTarget = false;
			duplicateCardScript.rankImage.raycastTarget = false;
			duplicateCardScript.bigSuitImage.raycastTarget = false;
			duplicateCardScript.detailImage.raycastTarget = false;
			duplicateCardScript.backgroundImage.raycastTarget = false;
			duplicateCardScript.cardBackImage.raycastTarget = false;
			
/* 				public Image borderImage;
	public Image rankImage;
	public Image bigSuitImage;
	public Image detailImage;
	public Image backgroundImage;
	public Image cardBackImage; */
			Destroy(duplicateCardScript);
			handScript.underMouseCard.tempCard = duplicate;
		}
	}
	
	public void OnDrag(PointerEventData data)
    {
		if(canMove)
		{
			//dataText.text = rt.position.ToString();;
			Vector2 mousePos = new Vector2((Input.mousePosition.x/Screen.width)*640,((Input.mousePosition.y/Screen.height))*360);
			borderRT.rotation = Quaternion.identity;
			if(tooltipScript != null && !faceDown)
			{
				tooltipScript.gameObject.SetActive(false);
				/* tooltipScript.gameObject.SetActive(true);
				tooltipScript.transform.SetParent(this.transform);
				tooltipScript.rt.anchoredPosition = new Vector2(115, 0);
				tooltipScript.transform.SetParent(handScript.tooltipParent); */
			}
			if(!cardIsInShop)
			{
				PointerEventData pointerData = new PointerEventData (EventSystem.current)
				{
					pointerId = -1,
				};
				pointerData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerData,results);
				//HandScript handScript = GameObject.FindWithTag("HandArea").GetComponent<HandScript>();
				bool overHandArea = false;
				foreach (RaycastResult result in results)
				{
					if (result.gameObject != null)
					{
						if(result.gameObject.name == "HandAreaDropZone")
						{
							overHandArea = true;
							int originalIndex = transform.GetSiblingIndex();
							if(transform.parent.name == "LooseCards")
							{
								transform.SetParent(handScript.handParent);
								/* if(handScript.handParent.transform.childCount >= 1)
								{
									if(rt.localPosition.x < handScript.transform.GetChild(0).GetComponent<RectTransform>().localPosition.x)
									{
										this.transform.SetSiblingIndex(0);
									}
								} */
								for(int i = 0; i < handScript.handParent.transform.childCount; i++)
								{
									if(rt.localPosition.x < handScript.transform.GetChild(i).GetComponent<RectTransform>().localPosition.x)
									{
										//print("setting originalIndex to " + i);
										originalIndex = i;
										break;
									}
									if(i == handScript.handParent.transform.childCount - 1)
									{
										originalIndex = i;
									}
								}
								handScript.visualCardsInHand++;
								//print("1");
								rt.anchoredPosition = new Vector2(mousePos.x - 95, mousePos.y - 5);
								handScript.cardToNotMove = this.transform.GetSiblingIndex();
								handScript.ReorganizeHand();
								//print("increasing VCIH 1");
							}
							for(int i = 0; i < handScript.handParent.transform.childCount; i++)
							{
								
								if(rt.localPosition.x < handScript.transform.GetChild(i).GetComponent<RectTransform>().localPosition.x)
								{
									int indexShift = 0;
									if(overHandArea)
									{
										if(originalIndex < i)
										{
											indexShift = -1;
										}
									}
									//print("i= "+ i+ " rt.localPosition.x= " +rt.localPosition.x+" handScript.transform.GetChild(i).GetComponent<RectTransform>().localPosition.x= "+handScript.transform.GetChild(i).GetComponent<RectTransform>().localPosition.x + " indexShift= "+indexShift);
									this.transform.SetSiblingIndex(i + indexShift);
									if(i + indexShift != originalIndex)
									{
										//print("i= " + i + " indexShift= " + indexShift + " originalIndex= "+ originalIndex);
										handScript.ChangeAlwaysSortType(0);
									}
									
									break;
								}
								/* if(i == handScript.handParent.transform.childCount - 1)
								{
									this.transform.SetSiblingIndex(handScript.handParent.transform.childCount - 1);
								} */
								if(i == handScript.handParent.transform.childCount - 1)
								{
									this.transform.SetSiblingIndex(handScript.handParent.transform.childCount - 1);
									if(handScript.handParent.transform.childCount - 1 != originalIndex)
									{
										//print("handScript.handParent.transform.childCount - 1= " + (handScript.handParent.transform.childCount -1) + " originalIndex= "+ originalIndex);
										handScript.ChangeAlwaysSortType(0);
									}
								}
							}
							//dataText.text = dataText.text + "
							handScript.cardToNotMove = this.transform.GetSiblingIndex();
							handScript.ReorganizeHand();
						}
					}
				}
				
				if(transform.parent == handScript.handParent && !overHandArea)
				{
					handScript.cardToNotMove = -1;
					transform.SetParent(GameObject.FindWithTag("LooseCards").transform);
					handScript.visualCardsInHand--;
					handScript.ReorganizeHand();
				}
				//dataText.text = dataText.text + "mousePos= " + mousePos + "\nInput.mousePosition= " + Input.mousePosition + "\noverHandArea= "+ overHandArea + "\nlastIndex= " + lastIndex + " handScript.cardToNotMove= " +handScript.cardToNotMove;;
				rt.anchoredPosition = new Vector2(mousePos.x - 95, mousePos.y - 5);// + offset;
			}
			else
			{
				rt.anchoredPosition = new Vector2(mousePos.x, mousePos.y);
			}
			
		}
    }
	
	public void RevertToOriginalImage()
	{
		if(!standardCard)
		{
			Sprite cardSprite = null;
			switch(nonStandardCardNumber)
			{
				case 0:
				cardSprite = handScript.drawPile.nonStandardCardDetails[0];
				break;
				case 1:
				cardSprite = handScript.drawPile.nonStandardCardDetails[3];
				break;
				case 2:
				cardSprite = handScript.drawPile.nonStandardCardDetails[5];
				break;
				case 3:
				cardSprite = handScript.drawPile.nonStandardCardDetails[7];
				break;
				case 4:
				cardSprite = handScript.drawPile.nonStandardCardDetails[9];
				break;
				case 5:
				cardSprite = handScript.drawPile.nonStandardCardDetails[11];
				break;
				case 6:
				cardSprite = handScript.drawPile.nonStandardCardDetails[13];
				break;
				case 7:
				cardSprite = handScript.drawPile.nonStandardCardDetails[15];
				break;
				case 8:
				cardSprite = handScript.drawPile.nonStandardCardDetails[17];
				break;
				case 9:
				cardSprite = handScript.drawPile.nonStandardCardDetails[20];
				break;
			}
			if(cardSprite != null)
			{
				detailImage.sprite = cardSprite;
				RectTransform cardDetailRT = detailImage.GetComponent<RectTransform>();
				cardDetailRT.sizeDelta = new Vector2(cardSprite.rect.width, cardSprite.rect.height);
				int xPos = Mathf.CeilToInt((45 - cardSprite.rect.width) / 2f);
				int yPos = Mathf.CeilToInt((45 - cardSprite.rect.height) / 2f);
				cardDetailRT.anchoredPosition = new Vector2(xPos, yPos);
			}
		}
	}
	
	public void UpdateToDropZoneImage()
	{
		if(!standardCard)
		{
			Sprite cardSprite = null;
			switch(nonStandardCardNumber)
			{
				case 0:
				cardSprite = handScript.drawPile.nonStandardCardDetails[1];
				break;
				case 1:
				cardSprite = handScript.drawPile.nonStandardCardDetails[4];
				break;
				case 2:
				cardSprite = handScript.drawPile.nonStandardCardDetails[6];
				break;
				case 3:
				cardSprite = handScript.drawPile.nonStandardCardDetails[8];
				break;
				case 4:
				cardSprite = handScript.drawPile.nonStandardCardDetails[10];
				break;
				case 5:
				cardSprite = handScript.drawPile.nonStandardCardDetails[12];
				break;
				case 6:
				cardSprite = handScript.drawPile.nonStandardCardDetails[14];
				break;
				case 7:
				cardSprite = handScript.drawPile.nonStandardCardDetails[16];
				break;
				case 8:
				int cardsInCorrectLocations = 0;
				if(dropZonePlacedIn.dropZoneNumber > 0)
				{
					if(dropZonePlacedIn.handZone.dropZones[dropZonePlacedIn.dropZoneNumber - 1].cardPlaced)
					{
						if(dropZonePlacedIn.handZone.dropZones[dropZonePlacedIn.dropZoneNumber - 1].placedCardScript.standardCard)
						{
							cardsInCorrectLocations++;
						}
					}
				}
				if(dropZonePlacedIn.dropZoneNumber < dropZonePlacedIn.handZone.activeDropZones - 1)
				{
					if(dropZonePlacedIn.handZone.dropZones[dropZonePlacedIn.dropZoneNumber + 1].cardPlaced)
					{
						if(dropZonePlacedIn.handZone.dropZones[dropZonePlacedIn.dropZoneNumber + 1].placedCardScript.standardCard)
						{
							cardsInCorrectLocations++;
						}
					}
				}
				if(cardsInCorrectLocations >= 2)
				{
					cardSprite = handScript.drawPile.nonStandardCardDetails[19]; // will work
				}
				else
				{
					cardSprite = handScript.drawPile.nonStandardCardDetails[18]; // won't work
				}
				break;
				case 9:
				cardSprite = handScript.drawPile.nonStandardCardDetails[20];
				break;
			}
			if(cardSprite != null)
			{
				detailImage.sprite = cardSprite;
				RectTransform cardDetailRT = detailImage.GetComponent<RectTransform>();
				cardDetailRT.sizeDelta = new Vector2(cardSprite.rect.width, cardSprite.rect.height);
				int xPos = Mathf.CeilToInt((45 - cardSprite.rect.width) / 2f);
				int yPos = Mathf.CeilToInt((45 - cardSprite.rect.height) / 2f);
				cardDetailRT.anchoredPosition = new Vector2(xPos, yPos);
			}
		}
	}
	
	public void OnEndDrag(PointerEventData eventData)
    {
		if(canMove)
		{
			//Camera.main.GetComponent<GridPoker>().PlayCardDropSound();
			SoundManager.instance.PlayCardDropSound();
			Vector2 mousePos = new Vector2((Input.mousePosition.x/Screen.width)*640,(Input.mousePosition.y/Screen.height)*360);
			if(!cardIsInShop)
			{
				PointerEventData pointerData = new PointerEventData (EventSystem.current)
				{
					pointerId = -1,
				};
				pointerData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerData,results);
				bool overHandArea = false;
				foreach (RaycastResult result in results)
				{
					if (result.gameObject != null)
					{
						if(result.gameObject.name == "HandAreaDropZone")
						{
							overHandArea = true;
						}
						DropZone dropZone;
						if(result.gameObject.transform.TryGetComponent(out dropZone))
						{
							if(dropZone.handZone.locked == false && !cannotBePlayed)
							{
								if((dropZone.cardPlaced && dropZone.placedCardScript.canMove) || !dropZone.cardPlaced)
								{
									dropZonePlacedIn = dropZone;
									//print("dropZonePlacedIn set to dropZone " + dropZonePlacedIn.dropZoneNumber);
									if(dropZone.cardPlaced)
									{
										//print("moving " + dropZone.placedCardScript.GetCardNameCharSuit());
										dropZone.placedCardScript.dropZonePlacedIn = null;
										dropZone.placedCardScript.transform.SetParent(handScript.handParent);
										dropZone.placedCardScript.transform.SetSiblingIndex(0);
										if(!dropZone.placedCardScript.standardCard)
										{
											dropZone.placedCardScript.RevertToOriginalImage();
										}
										handScript.visualCardsInHand++;
										handScript.ReorganizeHand();
									}
									transform.SetParent(dropZone.transform);
									rt.anchoredPosition = new Vector3(22.5f,22.5f,0);
									handScript.underMouseCard.DestroyTempCard();
									handScript.PlacedCardsUpdated(true);
									//print("placing " + GetCardNameCharSuit());
									dropZone.CardPlaced(this);
									//print("UpdateToDropZoneImage called");
									UpdateToDropZoneImage();
									//dropZonePlacedIn = dropZone;
									return;
								}
							}
						}
					}
				}
				transform.SetParent(handScript.handParent);
				//HandScript handScript = transform.parent.GetComponent<HandScript>();
				for(int i = 0; i < handScript.handParent.transform.childCount; i++)
				{
					if(rt.localPosition.x < handScript.transform.GetChild(i).GetComponent<RectTransform>().localPosition.x)
					{
						int indexShift = 0;
						if(overHandArea)
						{
							int originalIndex = transform.GetSiblingIndex();
							if(originalIndex < i)
							{
								indexShift = -1;
							}
						}
						//print("i= "+ i+ " rt.localPosition.x= " +rt.localPosition.x+" handScript.transform.GetChild(i).GetComponent<RectTransform>().localPosition.x= "+handScript.transform.GetChild(i).GetComponent<RectTransform>().localPosition.x + " indexShift= "+indexShift);
						this.transform.SetSiblingIndex(i + indexShift);
						break;
					}
					if(i == handScript.handParent.transform.childCount - 1)
					{
						this.transform.SetSiblingIndex(handScript.handParent.transform.childCount - 1);
					}
				}
				if(!overHandArea)
				{
					handScript.visualCardsInHand++;
					//print("increasing VCIH 2");
				}
				handScript.cardToNotMove = -1;
				handScript.ReorganizeHand();
			}
			else
			{
				rt.anchoredPosition = startedDraggingFrom;
			}
			handScript.underMouseCard.DestroyTempCard();
		}
    }

	public void StartMove(float moveTime, Vector2 destination, Vector3 destinationRotation)
	{
		if(moving)
		{
			//print("Stopping old movement coroutine on " + rankChar + suitChar);
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = MoveCard(moveTime, destination, destinationRotation);
		StartCoroutine(moveCoroutine);
	}
	public void UpdateGraphics()
	{
		switch(rankInt)
		{
			case 0:
			rankChar = (char)((rankInt+2) + '0');
			//cardValue = 2 + rankInt;
			rankString = "Deuce";
			break;
			case 1:
			rankChar = (char)((rankInt+2) + '0');
			//cardValue = 2 + rankInt;
			rankString = "Three";
			break;
			case 2:
			rankChar = (char)((rankInt+2) + '0');
			//cardValue = 2 + rankInt;
			rankString = "Four";
			break;
			case 3:
			rankChar = (char)((rankInt+2) + '0');
			//cardValue = 2 + rankInt;
			rankString = "Five";
			break;
			case 4:
			rankChar = (char)((rankInt+2) + '0');
			//cardValue = 2 + rankInt;
			rankString = "Six";
			break;
			case 5:
			rankChar = (char)((rankInt+2) + '0');
			//cardValue = 2 + rankInt;
			rankString = "Seven";
			break;
			case 6:
			rankChar = (char)((rankInt+2) + '0');
			//cardValue = 2 + rankInt;
			rankString = "Eight";
			break;
			case 7:
			rankChar = (char)((rankInt+2) + '0');
			//cardValue = 2 + rankInt;
			rankString = "Nine";
			break;
			case 8:
			rankChar = 'T';
			//cardValue = 10;
			rankString = "Ten";
			break;
			case 9:
			rankChar = 'J';
			//cardValue = 10;
			rankString = "Jack";
			break;
			case 10:
			rankChar = 'Q';
			//cardValue = 10;
			rankString = "Queen";
			break;
			case 11:
			rankChar = 'K';
			//cardValue = 10;
			rankString = "King";
			break;
			case 12:
			rankChar = 'A';
			//cardValue = 15;
			rankString = "Ace";
			break;
			default:
			print("Mixup on CardScript");
			rankChar = (char)((rankInt+2) + '0');
			//cardValue = 2 + rankInt;
			break;
		}
		if(suitInt < 4)
		{
			rankImage.sprite = handScript.drawPile.rankSprites[rankInt];
			rankImage.GetComponent<RectTransform>().sizeDelta = new Vector2(handScript.drawPile.rankSprites[rankInt].rect.width, 13);
			rankImage.color = handScript.drawPile.suitColors[suitInt];
			bigSuitImage.color = handScript.drawPile.suitColors[suitInt];
			rankImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 29);
			bigSuitImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(4, 15);
		}
		else
		{
			rankImage.sprite = handScript.drawPile.rankSprites[rankInt+13];
			rankImage.GetComponent<RectTransform>().sizeDelta = new Vector2(handScript.drawPile.rankSprites[rankInt+13].rect.width, 15);
			rankImage.color = Color.white;
			bigSuitImage.color = Color.white;
			rankImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 27);
			bigSuitImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(4, 14);
		}
		bigSuitImage.sprite = handScript.drawPile.bigSuitSprites[suitInt];
		bigSuitImage.GetComponent<RectTransform>().sizeDelta = new Vector2(handScript.drawPile.bigSuitSprites[suitInt].rect.width, handScript.drawPile.bigSuitSprites[suitInt].rect.height);
		
		int spriteInt = suitInt * 13 + rankInt;
		detailImage.sprite = handScript.drawPile.cardDetails[spriteInt];
		detailImage.GetComponent<RectTransform>().sizeDelta = new Vector2(handScript.drawPile.cardDetails[spriteInt].rect.width, handScript.drawPile.cardDetails[spriteInt].rect.height);
		if((rankInt <= 8 || rankInt == 12) && suitInt < 4)
		{
			detailImage.color = handScript.drawPile.suitColors[suitInt];
		}
		else
		{
			detailImage.color = Color.white;
		}
		int xPos = 17 + Mathf.CeilToInt((24 - handScript.drawPile.cardDetails[spriteInt].rect.width) / 2f);
		int yPos = 5 + Mathf.CeilToInt((35 - handScript.drawPile.cardDetails[spriteInt].rect.height) / 2f);
		detailImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
		if(rankInt < 8)
		{
			name = (rankInt + 2).ToString() + suitChar;
		}
		else
		{
			char nameChar = 'X';
			switch(rankInt)
			{
				case 8:
				nameChar = 'T';
				break;
				case 9:
				nameChar = 'J';
				break;
				case 10:
				nameChar = 'Q';
				break;
				case 11:
				nameChar = 'K';
				break;
				case 12:
				nameChar = 'A';
				break;
			}
			name = nameChar.ToString() + suitChar;
		}
	}
	public void ChangeSuit(int newSuit)
	{
		switch(newSuit)
		{
			case 0:
			ChangeToHeart();
			break;
			case 1:
			ChangeToDiamond();
			break;
			case 2:
			ChangeToClub();
			break;
			case 3:
			ChangeToSpade();
			break;
			case 4:
			ChangeToRainbow();
			break;
		}
	}
	
	public void ChangeToHeart()
	{
		suitChar = 'h';
		suitSymbol =  '♥';
		suitInt = 0;
		UpdateGraphics();
		if(deckViewerClone != null)
		{
			deckViewerClone.ChangeToHeart();
		}
	}
	public void ChangeToDiamond()
	{
		suitChar = 'd';
		suitSymbol =  '♦';
		suitInt = 1;
		UpdateGraphics();
		if(deckViewerClone != null)
		{
			deckViewerClone.ChangeToDiamond();
		}
	}
	public void ChangeToClub()
	{
		suitChar = 'c';
		suitSymbol =  '♣';
		suitInt = 2;
		UpdateGraphics();
		if(deckViewerClone != null)
		{
			deckViewerClone.ChangeToClub();
		}
	}
	public void ChangeToSpade()
	{
		suitChar = 's';
		suitSymbol =  '♠';
		suitInt = 3;
		UpdateGraphics();
		if(deckViewerClone != null)
		{
			deckViewerClone.ChangeToSpade();
		}
	}
	public void ChangeRank(int rank)
	{
		float rankValueChange = DeckScript.instance.GetBaseValueBasedOnRank(rank) - DeckScript.instance.GetBaseValueBasedOnRank(rankInt);
		cardValue += rankValueChange;
		rankInt = rank;
		UpdateGraphics();
		if(deckViewerClone != null)
		{
			deckViewerClone.ChangeRank(rank);
		}
	}
	public void ChangeToRainbow()
	{
		suitChar = 'r';
		suitSymbol =  '☼';
		suitInt = 4;
		UpdateGraphics();
		if(deckViewerClone != null)
		{
			deckViewerClone.ChangeToRainbow();
		}
	}
	
	
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		if(handScript.handValues.gameOptions.cheatsOn)
		{
			if(Input.GetKey(KeyCode.H))
			{
				ChangeToHeart();
				return;
			}
			if(Input.GetKey(KeyCode.D))
			{
				ChangeToDiamond();
				return;
			}
			if(Input.GetKey(KeyCode.C))
			{
				ChangeToClub();
				return;
			}
			if(Input.GetKey(KeyCode.S))
			{
				ChangeToSpade();
				return;
			}
			if(Input.GetKey(KeyCode.Alpha2))
			{
				ChangeRank(0);
				return;
			}
			if(Input.GetKey(KeyCode.Alpha3))
			{
				ChangeRank(1);
				return;
			}
			if(Input.GetKey(KeyCode.Alpha4))
			{
				ChangeRank(2);
				return;
			}
			if(Input.GetKey(KeyCode.Alpha5))
			{
				ChangeRank(3);
				return;
			}
			if(Input.GetKey(KeyCode.Alpha6))
			{
				ChangeRank(4);
				return;
			}
			if(Input.GetKey(KeyCode.Alpha7))
			{
				ChangeRank(5);
				return;
			}
			if(Input.GetKey(KeyCode.Alpha8))
			{
				ChangeRank(6);
				return;
			}
			if(Input.GetKey(KeyCode.Alpha9))
			{
				ChangeRank(7);
				return;
			}
			if(Input.GetKey(KeyCode.Alpha0))
			{
				ChangeRank(8);
				return;
			}
			if(Input.GetKey(KeyCode.J))
			{
				ChangeRank(9);
				return;
			}
			if(Input.GetKey(KeyCode.Q))
			{
				ChangeRank(10);
				return;
			}
			if(Input.GetKey(KeyCode.K))
			{
				ChangeRank(11);
				return;
			}
			if(Input.GetKey(KeyCode.A))
			{
				ChangeRank(12);
				return;
			}
			if(Input.GetKey(KeyCode.R))
			{
				ChangeToRainbow();
				return;
			}
			if(Input.GetKey(KeyCode.I))
			{
				print("SiblingIndex= " + transform.GetSiblingIndex());
				return;
			}
		}
		if(!handScript.selectedCards.Contains(this) && handScript.selectedCards.Count < Mathf.Max(handScript.maxCardsDiscardedAtOnce, handScript.maxCardsSelectedAtOnce) && transform.parent == handScript.handParent)
		{
			handScript.selectedCards.Add(this);
			handScript.SelectedCardsUpdated();
			rt.anchoredPosition = rt.anchoredPosition + Vector2.up * 20;
			SoundManager.instance.PlayCardPickupSound();
		}
		else
		{
			if(handScript.selectedCards.Contains(this))
			{
				handScript.selectedCards.Remove(this);
				handScript.SelectedCardsUpdated();
				rt.anchoredPosition = rt.anchoredPosition - Vector2.up * 20;
				SoundManager.instance.PlayCardDropSound();
			}
		}
	}
	
	public void UpdateFacing()
	{
		if(faceDown)
		{
			cardBackImage.gameObject.SetActive(true);
			rankImage.gameObject.SetActive(false);
			bigSuitImage.gameObject.SetActive(false);
			detailImage.gameObject.SetActive(false);
			faceDown = true;
		}
		else
		{
			cardBackImage.gameObject.SetActive(false);
			rankImage.gameObject.SetActive(true);
			bigSuitImage.gameObject.SetActive(true);
			detailImage.gameObject.SetActive(true);
			faceDown = false;
		}
	}
	
	public IEnumerator MoveCard(float moveTime, Vector2 destination, Vector3 destinationRotation)
	{
		moving = true;
		if(handScript.selectedCards.Contains(this))
		{
			handScript.selectedCards.Remove(this);
			handScript.SelectedCardsUpdated();
		}
		//Vector3 originalRotation = rt.eulerAngles;
		Quaternion originalRotationQ = borderRT.localRotation;
		Quaternion destinationRotationQ = Quaternion.Euler(destinationRotation);
		Vector2 originalPosition = rt.anchoredPosition;
		float t = 0;
		while(t < moveTime)
		{
			t+=Time.deltaTime * handScript.handValues.gameOptions.gameSpeedFactor;
			//rt.eulerAngles = Vector3.Lerp(originalRotation, destinationRotation, t/moveTime);
			borderRT.localRotation = Quaternion.Lerp(originalRotationQ, destinationRotationQ, t/moveTime);
			rt.anchoredPosition = Vector2.Lerp(originalPosition, destination, t/moveTime);
			if(tooltipScript != null && !faceDown)
			{
				tooltipScript.transform.SetParent(this.transform);
				tooltipScript.rt.anchoredPosition = new Vector2(115, 0);
				tooltipScript.transform.SetParent(handScript.tooltipParent);
			}
			yield return null;
		}
		borderRT.localRotation = destinationRotationQ;
		rt.anchoredPosition = destination;
		moving = false;
		//print("Setting " + rankChar + suitChar + " to " + destinationRotationQ.eulerAngles);
	}
	
	public void StartFlip(float flipTime)
	{
		borderRT.localScale = new Vector3(1f,1f,1f);
		StartCoroutine(Flip(flipTime));
	}
	
	public void ChangeFacing(bool setFaceDown)
	{
		faceDown = setFaceDown;
		UpdateFacing();
	}
	
	public IEnumerator Flip(float flipTime)
	{
		Vector3 originalScale = borderRT.localScale;
		Vector3 destinationScale = borderRT.localScale;
		destinationScale.x = 0;
		
		float t = 0;
		while (t < flipTime)
		{
			t += Time.deltaTime * handScript.handValues.gameOptions.gameSpeedFactor;
			borderRT.localScale = Vector3.Lerp(originalScale, destinationScale, t / flipTime);
			yield return null;
		}
		
		faceDown = !faceDown;
		UpdateFacing();
		//SoundManager.instance.PlayCardDropSound();
		t = 0;
		while (t < flipTime)
		{
			borderRT.localScale = Vector3.Lerp(destinationScale, originalScale, t / flipTime);
			t += Time.deltaTime * handScript.handValues.gameOptions.gameSpeedFactor;
			yield return null;
		}
		borderRT.localScale = new Vector3(1f,1f,1f);
		//borderRT.localScale = originalScale;
	}
	
    void Start()
    {	
        //dataText = GameObject.FindWithTag("DataText").GetComponent<TMP_Text>(); //disable for release
		//handScript = GameObject.FindWithTag("HandArea").GetComponent<HandScript>();
    }
}
