using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
	public static Tutorial instance;
    public TooltipScript[] tutorialTooltips;
	public GameObject tutorialInteractionBlocker;
	public GameObject tutorialGameObject;
	public int tutorialStage = 0;
	public AnimationCurve moveCurve;
	public TMP_Text[] skipTutorialButtonTexts;
	public GameObject areYouSureMenuObject;
	
	public delegate void CallbackFunction();
	
	public void SkipTutorialClicked()
	{
		GameOptions.instance.resetTutorialButton.ChangeDisabled(false);
		tutorialGameObject.SetActive(false);
		GameOptions.instance.tutorialDone = true;
		GameOptions.instance.UpdateOptionsFile();
	}
	
	public IEnumerator MoveOverTime(RectTransform rt, Vector2 startPosition, Vector2 endPosition, float moveTime, float delayTime, CallbackFunction endFunction = null)
	{
		float t = 0;
		while(t < delayTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		t = 0;
		while(t < moveTime)
		{
			t += Time.deltaTime;
			rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, moveCurve.Evaluate(t / moveTime));
			yield return null;
		}
		rt.anchoredPosition = endPosition;
		if(endFunction != null)
		{
			endFunction();
		}
	}
	
	public void TutorialGotItClicked(int tutorialNumber)
	{
		AdvanceTutorial();
		/* switch(tutorialNumber)
		{
			case 0:
			
			break;
		} */
	}
	
	public void AdvanceTutorial()
	{
		print("tutorialStage= " + tutorialStage);
		switch(tutorialStage)
		{
			case 0:
			tutorialTooltips[0].SetupTooltip("To play a hand, select the cards you want to play by clicking them, then click on the red play area to place them. Try now!", "Playing Hands", 6);
			tutorialTooltips[0].tutorialNumber++;
			tutorialTooltips[0].gotItButton.ChangeDisabled(true);
			tutorialInteractionBlocker.gameObject.SetActive(false);
			StartCoroutine(MoveOverTime(tutorialTooltips[0].rt, tutorialTooltips[0].rt.anchoredPosition, new Vector2(560f, 180f), 1, 0));
			break;
			case 1:
			tutorialTooltips[0].SetupTooltip("You can also move cards by dragging them. If you want to return placed cards to your hand, click the \"Recall\" button", "Nice Job!", 6);
			tutorialTooltips[0].gotItButton.ChangeDisabled(false);
			break;
			case 2:
			tutorialTooltips[0].SetupTooltip("Once you're satisfied with your hand, click on the lock button on the left of the play area to lock in your hand, and get your hard earned points!", "Locking it up", 6);
			tutorialTooltips[0].gotItButton.ChangeDisabled(true);
			break;
			case 3:
			StartCoroutine(MoveOverTime(tutorialTooltips[0].rt, tutorialTooltips[0].rt.anchoredPosition, new Vector2(80f, 200f), 1, 0));
			tutorialTooltips[0].SetupTooltip("If your hand isn't perfect and you'd like to draw different cards, select some cards you don't want and click the \"Discard\" button. Try now!", "Sweet!", 6);
			tutorialTooltips[0].gotItButton.ChangeDisabled(true);
			break;
			case 4:
			StartCoroutine(MoveOverTime(tutorialTooltips[0].rt, tutorialTooltips[0].rt.anchoredPosition, new Vector2(145f, 200f), 1, 0));
			tutorialTooltips[0].SetupTooltip("Beware, you only have a limited number of discards per ante. Similarly, you have a limited number of hands you can play until you become \"Fatigued\" Check the left side of the screen to see how many of each you have left", "You got this!", 6);
			tutorialTooltips[0].gotItButton.ChangeDisabled(false);
			break;
			case 5:
			StartCoroutine(MoveOverTime(tutorialTooltips[0].rt, tutorialTooltips[0].rt.anchoredPosition, new Vector2(470f, 320f), 1, 0));
			tutorialTooltips[0].SetupTooltip("Now, keep playing until you reach 300 points!", "Keep going!", 6);
			tutorialTooltips[0].gotItButton.ChangeDisabled(true);
			break;
			case 6:
			StartCoroutine(MoveOverTime(tutorialTooltips[0].rt, tutorialTooltips[0].rt.anchoredPosition, new Vector2(560f, 180f), 1, 0));
			tutorialTooltips[0].SetupTooltip("Well done! Now you can spend chips you earned during the ante on new cards, zodiacs that improve hands, and baubles that give permanent bonuses to your run! Mouse over zodiacs, baubles, and non-standard cards to see a description of what they do", "The Shop", 6);
			tutorialTooltips[0].gotItButton.ChangeDisabled(false);
			break;
			case 7:
			StartCoroutine(MoveOverTime(tutorialTooltips[0].rt, tutorialTooltips[0].rt.anchoredPosition, new Vector2(560f, 180f), 1, 0));
			tutorialTooltips[0].SetupTooltip("Once you're done shopping, click the \"Shuffle and Continue\" button to move on to the next ante. The stakes are always rising, so make sure you're prepared!", "Shopping spree", 6);
			tutorialTooltips[0].gotItButton.ChangeDisabled(true);
			break;
			case 8:
			StartCoroutine(MoveOverTime(tutorialTooltips[0].rt, tutorialTooltips[0].rt.anchoredPosition, new Vector2(560f, 180f), 1, 0));
			tutorialTooltips[0].SetupTooltip("That's all you need to know to become a Scrongly master! Keep clicking \"Got it!\" to go over what you've learned, and to get some advanced tips, or click \"End Tutorial\" at the top of the screen to shoo this box away", "Let's do This!", 6);
			tutorialTooltips[0].gotItButton.ChangeDisabled(false);
			for(int i = 0; i < skipTutorialButtonTexts.Length; i++)
			{
				skipTutorialButtonTexts[i].text = "End Tutorial";
			}
			break;
			case 9:
			//StartCoroutine(MoveOverTime(tutorialTooltips[0].rt, tutorialTooltips[0].rt.anchoredPosition, new Vector2(560f, 180f), 1, 0));
			tutorialTooltips[0].SetupTooltip("Select cards by clicking on them. With cards selected, click on the red play zone to move them there. Press the recall button to return placed cards to your hand. You can also drag cards.", "Moving Cards", 6);
			break;
			case 10:
			tutorialTooltips[0].SetupTooltip("When you're satisfied with your hand, click the lock button on the left of the play zone to lock it in and get your points", "Locking in Hands", 6);
			break;
			case 11:
			tutorialTooltips[0].SetupTooltip("If you don't have just what you want, select cards and click the \"Discard\" button below your hand to get some new ones. You have a limited number of discards per ante", "Discarding Cards", 6);
			break;
			case 12:
			tutorialTooltips[0].SetupTooltip("You may only play a certain number of hands before you become \"Fatigued\". Once fatigued, you will discard your remaining hand after locking in your played hand. You lose the game when you have no more cards to play", "Fatigue", 6);
			break;
			case 13:
			tutorialTooltips[0].SetupTooltip("Keep playing hands until you reach the antes score threshold to get to the shop. You can see what ante you are on as well as the score needed to pass it on the left side of the screen", "Antes", 6);
			break;
			case 14:
			tutorialTooltips[0].SetupTooltip("Click on the \"Rank\" and \"Suit\" buttons below your hand to sort your hand. Double click them to retain that sorting. You can also drag cards around to rearrange them", "Sorting Your Hand", 6);
			break;
			case 15:
			tutorialTooltips[0].SetupTooltip("In the shop, you can purchase new cards. Some are typical and have standard ranks and suits. Some are rainbow, which cost extra but count as any suit. Some are non-standard. Non-standard cards do not take up space in your hand once drawn", "Buying Cards", 6);
			break;
			case 16:
			tutorialTooltips[0].SetupTooltip("The shop also sells Zodiacs, which improve the value and multiplier of specific hands. You can mouse over the tag that says \"Hands\" on the left side of the screen to see the current values of hands you play", "Buying Zodiacs", 6);
			break;
			case 17:
			tutorialTooltips[0].SetupTooltip("The most fun thing to buy in the shop are Baubles. Baubles improve your run in various ways, from giving new ways to earn chips to decreasing the number of cards needed to make a flush. Try them out!", "Buying Baubles", 6);
			break;
			case 18:
			tutorialTooltips[0].SetupTooltip("You can mouse over your draw pile to see a quick overview of what cards are remaining. If you click on it, you can see a larger, more detailed view", "Checking out your Deck", 6);
			break;
			case 19:
			tutorialTooltips[0].SetupTooltip("Numbered cards are worth their number, face cards are worth 10, and aces are worth 15. All played cards that are part of the scored hand have their base value added before the hand multiplier takes effect.", "Card Values", 6);
			break;
			case 20:
			tutorialTooltips[0].SetupTooltip("On the right side of the screen, you can see your current score, and all score thresholds for that ante. Mouse over compacted ones to see exactly the score you need to reach. Each of them has a certain number of hands you may play before they disappear.", "Score Thresholds", 6);
			break;
			case 21:
			tutorialTooltips[0].SetupTooltip("Click on \"Menu\" to abandon your current run or change options such as the game speed", "Options", 6);
			break;
			case 22:
			tutorialTooltips[0].SetupTooltip("That's all I've got for you! Thank you so much for playing Scrongly. Clicking \"Got it!\" will cycle back through these tips. Click \"End Tutorial\" when you're finished", "That's All, Folks!", 6);
			tutorialStage = 8;
			break;
		}
		tutorialStage++;
	}
	
	public void RevealTutorial()
	{
		tutorialGameObject.SetActive(true);
		tutorialInteractionBlocker.SetActive(true);
	}
	
	public void SetupTutorial()
	{
		tutorialTooltips[0].rt.anchoredPosition = new Vector2(320,180);
		tutorialTooltips[0].gotItButton.ChangeDisabled(false);
		tutorialStage = 0;
		areYouSureMenuObject.SetActive(false);
		tutorialGameObject.SetActive(false);
		tutorialInteractionBlocker.SetActive(false);
		tutorialTooltips[0].SetupTooltip("The objective of the game is to play strong poker hands. Stronger hands net you more points, and each round, or ante, has a point value you need to reach to continue the game", "Welcome to Scrongly!", 6);
	}
	
	void Start()
	{
		for(int i = 0; i < tutorialTooltips.Length; i++)
		{
			tutorialTooltips[i].tutorialNumber = i;
		}
	}
	
	void Awake()
	{
		instance = this;
	}
}
