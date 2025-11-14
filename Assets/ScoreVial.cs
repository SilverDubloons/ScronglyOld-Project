using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreVial : MonoBehaviour
{
	public static ScoreVial instance;
	public RectTransform fillRT;
	public AnimationCurve fillCurve;
	public float fillTime;
	public HandValues handValues;
	public TMP_Text[] currentScoreTexts;
	public RectTransform slimeParentRT;
	public Transform slimeParent;
	public Sprite[] slimeSprites;
	public GameObject slimePrefab;
	public SlimeScript newestSlimeScript;
	public RectTransform currentScoreBorder;
	public RectTransform currentScoreBackdrop;
	public GameObject scoreThresholdPrefab;
	public Transform scoreThresholdParent;
	public List<ScoreThreshold> scoreThresholds;
	private const float epsilon = 0.0001f;
	public RectTransform movingChipParent;
	public TMP_Text[] moneyTexts;
	public int currentMoney;
	public Transform particleParent;
	public float currentScore;
	public float currentScoreDecimal;
	public float timeBetweenBubbles = 100;
	public GameObject bubblePrefab;
	public Transform bubbleParent;
	private float yDifference;
	public float scoreTarget;
	public ShopScript shopScript;
	public HandScript handScript;
	public TMP_Text[] anteNumberTexts;
	public TMP_Text[] anteScoreTexts;
	public GameObject pokerChipPrefab;
	//public Statistics statistics;
	
	void Awake()
	{
		instance = this;
	}
	
	public IEnumerator CheckIfMenuShouldUnlock()
	{
		yield return null;
		if(handValues.menuButton.disabled)
		{
			if(movingChipParent.childCount <= 0)
			{
				if(!handValues.AnyHandZonesAreLocked() || shopScript.shopActive)
				{
					handValues.menuButton.ChangeDisabled(false);
				}
			}
		}
		yield return null;
		if(handValues.menuButton.disabled)
		{
			if(movingChipParent.childCount <= 0)
			{
				if(!handValues.AnyHandZonesAreLocked() || shopScript.shopActive)
				{
					handValues.menuButton.ChangeDisabled(false);
				}
			}
		}
	}
	
	public void MoneyChanged(int change)
	{
		currentMoney += change;
		if(change > 0)
		{
			Statistics.instance.currentRun.chipsEarned += change;
			if(change == 1)
			{
				SoundManager.instance.PlayChipSound();
			}
		}
		else
		{
			
		}
		if(currentMoney > Statistics.instance.currentRun.mostMoneyHeldAtOnce)
		{
			Statistics.instance.currentRun.mostMoneyHeldAtOnce = currentMoney;
		}
		for(int i = 0; i < moneyTexts.Length; i++)
		{
			moneyTexts[i].text = "" + currentMoney;
		}
		shopScript.CheckAffordability();
	}
	
	public void UpdateScoreThresholds()
	{
		for(int i = 0; i < scoreThresholds.Count; i++)
		{
			scoreThresholds[i].handsRemaining--;
			if(scoreThresholds[i].handsRemaining > 1)
			{
				for(int j = 0; j < scoreThresholds[i].scoreTexts.Length; j++)
				{
					scoreThresholds[i].scoreTexts[j].text = scoreThresholds[i].handsRemaining + " Hands\n" + handValues.ConvertFloatToString(scoreThresholds[i].score);
					scoreThresholds[i].scoreTexts[j].ForceMeshUpdate(true, true);
					ResizeBackdropForScore(scoreThresholds[i].border, scoreThresholds[i].backdrop, scoreThresholds[i].scoreTexts[0]);
				}
			}
			else if(scoreThresholds[i].handsRemaining == 1)
			{
				for(int j = 0; j < scoreThresholds[i].scoreTexts.Length; j++)
				{
					scoreThresholds[i].scoreTexts[j].text = scoreThresholds[i].handsRemaining + " Hand\n" + handValues.ConvertFloatToString(scoreThresholds[i].score);
					scoreThresholds[i].scoreTexts[j].ForceMeshUpdate(true, true);
					ResizeBackdropForScore(scoreThresholds[i].border, scoreThresholds[i].backdrop, scoreThresholds[i].scoreTexts[0]);
					scoreThresholds[i].stayDecompressed = true;
					scoreThresholds[i].decompress = true;
				}
				scoreThresholds[i].backdropImage.color = Color.red;
			}
			else if(scoreThresholds[i].handsRemaining == 0)
			{
				if(scoreThresholds.Count > 1)
				{
					
					float lastScore = scoreThresholds[0].score;
					for(int  j = 1; j < scoreThresholds.Count; j++)
					{
						float tempScore = scoreThresholds[j].score;
						RectTransform strt = scoreThresholds[j].GetComponent<RectTransform>();
						Vector2 startPosition = strt.anchoredPosition;
						scoreThresholds[j].score = lastScore;
						Vector2 endPosition = new Vector2(strt.anchoredPosition.x, strt.anchoredPosition.y - yDifference);
						if(currentScore / scoreTarget * 250f + 6 > endPosition.y)
						{
							endPosition.y = currentScore / scoreTarget * 250f + 6f;
						}
						StartCoroutine(MoveOverTime(strt, startPosition, endPosition, 1, 1, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
						lastScore = tempScore;
					}
					i--;
				}
				//Destroy(scoreThresholds[0].gameObject);
				SoundManager.instance.PlayScoreThresholdDissolveSound();
				scoreThresholds[0].dissolveScript.StartDissolving(1);
				scoreThresholds.RemoveAt(0);
			}
		}
	}
	
	public IEnumerator MoveOverTime(RectTransform rt, Vector2 startPosition, Vector2 endPosition, float moveTime, float delayTime, Transform newParent, Vector2 finalPosition, Vector2 finalAnchorMin, Vector2 finalAnchorMax, bool destroyAfterMove = false)
	{
		if(!rt)
		{
			yield break;
		}
		delayTime = delayTime / handValues.gameOptions.gameSpeedFactor;
		moveTime = moveTime / handValues.gameOptions.gameSpeedFactor;
		float t = 0;
		while(t < delayTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		t = 0;
		while(t < moveTime)
		{
			if(!rt)
			{
				yield break;
			}
			t += Time.deltaTime;
			rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, fillCurve.Evaluate(t / moveTime));
			yield return null;
		}
		if(!rt)
		{
			yield break;
		}
		rt.anchoredPosition = endPosition;
		if(newParent != null)
		{
			rt.transform.SetParent(newParent);
			rt.anchorMin = finalAnchorMin;
			rt.anchorMax = finalAnchorMax;
			rt.anchoredPosition = finalPosition;
		}
		if(destroyAfterMove)
		{
			Destroy(rt.gameObject);
		}
	}
	
	public int numberOfThresholds;
	
	public void SetupNewBlind(float firstThreshold, float newScoreTarget, int currentAnte)
	{	// firstThreshold is between 0 and 1. Defaults to 0.2
		
		for(int i = 0; i < anteNumberTexts.Length; i++)
		{
			anteNumberTexts[i].text = "" + (currentAnte + 1);
		}
		Statistics.instance.currentRun.anteReached = currentAnte + 1;
		Statistics.instance.currentRun.scoreInFinalAnte = 0;
		for(int i = 0; i < anteScoreTexts.Length; i++)
		{
			anteScoreTexts[i].text = "" + handValues.ConvertFloatToString(newScoreTarget);
		}
		scoreTarget = newScoreTarget;
		for(int i = 0; i < scoreThresholds.Count; i++)
		{
			Destroy(scoreThresholds[i].gameObject);
		}
		scoreThresholds.Clear();
		for(int i = 0; i < numberOfThresholds; i++)
		{
			GameObject newThreshold = Instantiate(scoreThresholdPrefab, new Vector3(0,0,0), Quaternion.identity, scoreThresholdParent);
			float thresholdScore = (scoreTarget * firstThreshold) + (scoreTarget * (1 - firstThreshold)) / (numberOfThresholds - 1) * i;
			ScoreThreshold scoreThreshold = newThreshold.GetComponent<ScoreThreshold>();
			for(int j = 0; j < scoreThreshold.scoreTexts.Length; j++)
			{
				scoreThreshold.scoreTexts[j].text = "" + (i + 2) + " Hands\n" + handValues.ConvertFloatToString(thresholdScore);
				scoreThreshold.scoreTexts[j].ForceMeshUpdate(true, true);
			}
			scoreThreshold.handsRemaining = i + 2;
			ResizeBackdropForScore(scoreThreshold.border, scoreThreshold.backdrop, scoreThreshold.scoreTexts[0]);
			scoreThreshold.decompressedWidth = scoreThreshold.border.sizeDelta.x;
			float yPos = thresholdScore / scoreTarget * 250;
			scoreThreshold.rt.anchoredPosition = new Vector2(0, yPos - 9.5f);
			if(i == 0)
			{
				scoreThreshold.compressionState = 1;
				scoreThreshold.stayDecompressed = true;
			}
			else
			{
				scoreThreshold.compressionState = 0;
				scoreThreshold.stayDecompressed = false;
			}
			scoreThreshold.score = thresholdScore;
			scoreThresholds.Add(scoreThreshold);
		}
		if(scoreThresholds.Count >= 2)
		{
			yDifference = scoreThresholds[1].GetComponent<RectTransform>().anchoredPosition.y - scoreThresholds[0].GetComponent<RectTransform>().anchoredPosition.y;
		}
		BossAntes.instance.UpdateAnteDisplay(currentAnte, currentAnte >= 29 ? true : false);
	}
	
	public void ResizeBackdropForScore(RectTransform border, RectTransform backdrop, TMP_Text text)
	{
		border.sizeDelta = new Vector2(text.textBounds.size.x + 10, border.sizeDelta.y);
		backdrop.sizeDelta = new Vector2(text.textBounds.size.x + 8, backdrop.sizeDelta.y);
	}
	
	public void SpawnNextSlime(RectTransform lastSlime, float lastSlimeSpeed)
	{
		GameObject newSlime = Instantiate(slimePrefab, new Vector3(0,0,0), Quaternion.identity, slimeParent);
		RectTransform newSlimeRT = newSlime.GetComponent<RectTransform>();
		newSlimeRT.anchoredPosition = new Vector2(lastSlime.anchoredPosition.x - lastSlime.sizeDelta.x, -1);
		SlimeScript newSlimeScript = newSlime.GetComponent<SlimeScript>();
		newSlimeScript.scoreVial = this;
		newSlimeScript.speed = lastSlimeSpeed;
		newestSlimeScript = newSlimeScript;
	}
	
    void Start()
    {
       // MoneyChanged(5);
    }
	
	public void ScoreUpdated(float pointsGained, bool updateScoreThresholds, bool resetting)
	{
		float oldScore = currentScore;
		currentScore = oldScore + pointsGained;
		float newScore = currentScore;
		currentScoreDecimal = currentScore / scoreTarget;
		timeBetweenBubbles = Mathf.Clamp((3f - 2.75f * currentScoreDecimal), 0.1f, 5f);
		timeBetweenBubblesRandomized = timeBetweenBubbles;
		newScore = Mathf.Clamp(newScore, 0, scoreTarget);
		StartCoroutine(AdjustFill(oldScore, newScore, updateScoreThresholds, resetting));
		//print("updating stats. handValues.currentAnte= " + handValues.currentAnte);
		Statistics.instance.UpdateRunStats();
		if(currentScore >= scoreTarget)
		{
			StartCoroutine(AnteCompleted());
		}
	}
	
	public IEnumerator AnteCompleted(bool onToEndless = false, bool startingInShop = false)
	{
		if(handValues.currentAnte == 29 && onToEndless == false && !startingInShop)
		{
			Statistics.instance.currentRun.anteReached = handValues.currentAnte + 1;
			handValues.StartCoroutine(handValues.ShowGameStats(false, true, false));
			SoundManager.instance.PlayVictorySound();
			yield break;
		}
		if(handValues.currentAnte == 49 && !startingInShop)
		{
			Statistics.instance.currentRun.anteReached = handValues.currentAnte + 1;
			handValues.StartCoroutine(handValues.ShowGameStats(false, false, true));
			SoundManager.instance.PlayVictorySound();
			yield break;
		}
		handScript.DiscardCardsInHand();
		if(!startingInShop)
		{
			yield return new WaitForSeconds(3 / handValues.gameOptions.gameSpeedFactor);
			SoundManager.instance.PlayAnteClearedSound();
			handValues.currentAnte++;
		}
		//statistics.AnteCleared();
		handScript.visualCardsInHand = 0;
		shopScript.shopActive = true;
		shopScript.SetupShop();
		StartCoroutine(MoveOverTime(handValues.rt, handValues.rt.anchoredPosition, new Vector3(0, 360, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		StartCoroutine(MoveOverTime(handScript.baseRT, handScript.baseRT.anchoredPosition, new Vector3(95, -205, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		StartCoroutine(MoveOverTime(shopScript.rt, shopScript.rt.anchoredPosition, new Vector3(0, 0, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		ScoreUpdated(-currentScore, false, true);
		handScript.ResetDiscards();
		handValues.UpdateHandsUntilFatigue(0, true);
		yield return new WaitForSeconds(1f / handValues.gameOptions.gameSpeedFactor);
		handScript.StartCoroutine(handScript.AddAllCardsInDiscardPileToDeck());
		shopScript.shuffleAndContinueButton.ChangeDisabled(false);
		SetupNewBlind(0.2f, handValues.antes[handValues.currentAnte], handValues.currentAnte);
		HandsInformation.instance.RecolorHandLabels(new bool[18]);
		StartCoroutine(CheckIfMenuShouldUnlock());
	}
	
	public IEnumerator StartNextAnte()
	{
		StartCoroutine(MoveOverTime(handValues.rt, handValues.rt.anchoredPosition, new Vector3(0, 0, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		StartCoroutine(MoveOverTime(handScript.baseRT, handScript.baseRT.anchoredPosition, new Vector3(95, 5, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		StartCoroutine(MoveOverTime(shopScript.rt, shopScript.rt.anchoredPosition, new Vector3(0, 360, 0), 1, 0, null, new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)));
		yield return new WaitForSeconds(1 / handValues.gameOptions.gameSpeedFactor);
		handScript.drawPile.ShuffleDeck();
		handScript.DrawToFullHand();
	}
	
	public PokerChip SpawnPokerChip(Vector2 spawnPosition, Transform parent)
	{
		GameObject newPokerChip = Instantiate(pokerChipPrefab, new Vector3(0,0,0), Quaternion.identity, parent);
		PokerChip pokerChip = newPokerChip.GetComponent<PokerChip>();
		pokerChip.enabled = true;
		pokerChip.scoreVial = this;
		pokerChip.startPosition = spawnPosition;
		pokerChip.movingChipParent = movingChipParent;
		pokerChip.endPosition = new Vector2(movingChipParent.GetComponent<RectTransform>().anchoredPosition.x + 11, movingChipParent.GetComponent<RectTransform>().anchoredPosition.y + 11);
		pokerChip.chipImage.color = Color.white;
		pokerChip.particleParent = particleParent;
		return pokerChip;
	}
	
	public int allThresholdsInARow;
	
	public IEnumerator AdjustFill(float oldScore, float newScore, bool updateScoreThresholds, bool resetting)
	{
		float fillDesiredHeight = newScore / scoreTarget * 250f;
		float t = 0;
		float startingHeight = fillRT.sizeDelta.y;
		int chipsEarned = 0;
		while(t < fillTime)
		{
			t += Time.deltaTime * handValues.gameOptions.gameSpeedFactor;
			//float scoreLabel = Mathf.Lerp(oldScore, newScore, fillCurve.Evaluate(t/fillTime));
			float scoreLabel = Mathf.Lerp(oldScore, currentScore, fillCurve.Evaluate(t/fillTime));
			for(int i = 0; i < currentScoreTexts.Length; i++)
			{
				currentScoreTexts[i].text = handValues.ConvertFloatToString(scoreLabel);
			}
			ResizeBackdropForScore(currentScoreBorder, currentScoreBackdrop, currentScoreTexts[0]);
			float newHeight = Mathf.Lerp(startingHeight, fillDesiredHeight, fillCurve.Evaluate(t/fillTime));
			slimeParentRT.anchoredPosition = new Vector2(slimeParentRT.anchoredPosition.x, newHeight);
			fillRT.sizeDelta = new Vector2(fillRT.sizeDelta.x, newHeight);
			newestSlimeScript.speed = newHeight / 10;
			if(scoreThresholds.Count > 0)
			{
				for(int i = 0; i < scoreThresholds.Count; i++)
				{
					if(newHeight + 6 > scoreThresholds[i].rt.anchoredPosition.y)
					{
						scoreThresholds[i].rt.anchoredPosition = new Vector3(scoreThresholds[i].rt.anchoredPosition.x, newHeight + 6);
						if(scoreThresholds[i].pointer)
						{
							Destroy(scoreThresholds[i].pointer.gameObject);
						}
					}
					if(scoreLabel >= scoreThresholds[i].score - epsilon)
					{
						SoundManager.instance.PlayXylophoneSound();
						/* if(timeSinceLastScoreThresholdCompleted > 2f)
						{
							xylophoneTone = 0;
							
							xylophoneTone++;
							timeSinceLastScoreThresholdCompleted = 0;
						}
						else
						{
							SoundManager.instance.PlayXylophoneSound(xylophoneTone);
							xylophoneTone++;
						} */
						chipsEarned++;
						scoreThresholds[i].chipScript.enabled = true;
						scoreThresholds[i].chipScript.transform.SetParent(movingChipParent);
						scoreThresholds[i].chipScript.scoreVial = this;
						scoreThresholds[i].chipScript.startPosition = scoreThresholds[i].chipScript.GetComponent<RectTransform>().anchoredPosition;
						scoreThresholds[i].chipScript.endPosition = new Vector2(movingChipParent.GetComponent<RectTransform>().anchoredPosition.x + 11, movingChipParent.GetComponent<RectTransform>().anchoredPosition.y + 11);
						scoreThresholds[i].chipImage.color = Color.white;
						scoreThresholds[i].chipScript.particleParent = particleParent;
						if(scoreThresholds.Count - 1 > i)
						{
							scoreThresholds[i + 1].stayDecompressed = true;
							scoreThresholds[i + 1].decompress = true;
						}
						Destroy(scoreThresholds[i].gameObject);
						scoreThresholds.Remove(scoreThresholds[i]);
					}
				}
			}
			yield return null;
		}
		for(int i = 0; i < currentScoreTexts.Length; i++)
		{
			currentScoreTexts[i].text = handValues.ConvertFloatToString(currentScore);
		}
		ResizeBackdropForScore(currentScoreBorder, currentScoreBackdrop, currentScoreTexts[0]);
		if(updateScoreThresholds)
		{
			UpdateScoreThresholds();
		}
		if(!resetting)
		{
			if(chipsEarned > 0)
			{
				handValues.menuButton.ChangeDisabled(true);
				if(chipsEarned > 1)
				{
					if(handValues.baubleScript.baubles[60].quantityOwned > 0)
					{
						int chipsToSpawn = chipsEarned - 1;
						if(chipsEarned == numberOfThresholds)
						{
							chipsToSpawn += 2;
						}
						
						BaubleNotifications.instance.Notify(60, chipsToSpawn);
					}
				}
			}
			//print("chipsEarned= " + chipsEarned + " numberOfThresholds= " + numberOfThresholds + " allThresholdsInARow= " + allThresholdsInARow);
			if(chipsEarned == numberOfThresholds && handValues.baubleScript.baubles[60].quantityOwned > 0)
			{
				allThresholdsInARow++;
				if(allThresholdsInARow >= 2)
				{
					if(!Decks.instance.decks[6].unlocked && !Statistics.instance.currentRun.runIsDailyGame && !Statistics.instance.currentRun.runIsSeededGame && DeckViewer.instance.variantInUse == "")
					{
						Decks.instance.decks[6].unlocked = true;
						Decks.instance.UpdateDecksFile();
						Decks.instance.DeckKnobs[6].knobImage.sprite = Decks.instance.unlockedKnob;
						Decks.instance.DeckKnobs[6].rt.sizeDelta = new Vector2(10,10);
						UnlockNotifications.instance.CreateNewUnlockNotifier(0,6);
					}
				}
			}
			else
			{
				allThresholdsInARow = 0;
			}
		}
		StartCoroutine(CheckIfMenuShouldUnlock());
	}
	
	private Dictionary<KeyCode, (float, float)> scoreMappings = new Dictionary<KeyCode, (float, float)>()
    {
		{ KeyCode.Alpha0, (0f, 500f) },
        { KeyCode.Alpha1, (50f, 500f) },
        { KeyCode.Alpha2, (99f, 500f) },
        { KeyCode.Alpha3, (125f, 500f) },
        { KeyCode.Alpha4, (200f, 500f) },
        { KeyCode.Alpha5, (250f, 500f) },
        { KeyCode.Alpha6, (300f, 500f) },
        { KeyCode.Alpha7, (350f, 500f) },
        { KeyCode.Alpha8, (400f, 500f) },
        { KeyCode.Alpha9, (450f, 500f) },
		{ KeyCode.P, (100000f, 500f) }
    };
	
	private float timeSinceLastBubble = 0;
	private float timeBetweenBubblesRandomized;
	//public float timeSinceLastScoreThresholdCompleted;
	//public int xylophoneTone = 0;

    void Update()
    {
        /* foreach (var kvp in scoreMappings)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                ScoreUpdated(oldScoreTest, kvp.Value.Item1, kvp.Value.Item2, true);
				oldScoreTest = kvp.Value.Item1;
                break;
            }
        } */
		//timeSinceLastScoreThresholdCompleted += Time.deltaTime;
		if(handValues.gameOptions.cheatsOn)
		{
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				MoneyChanged(1);
			}
			if(Input.GetKeyDown(KeyCode.Backspace))
			{
				MoneyChanged(-1);
			}
			if(Input.GetKeyDown(KeyCode.X))
			{
				handValues.baubleScript.baubles[0].quantityOwned += 5000;
				handValues.baubleScript.BaublePurchased(0);
			}
			if(Input.GetKeyDown(KeyCode.B))
			{
				handValues.baubleScript.baubles[8].quantityOwned += 999999999;
				handValues.baubleScript.BaublePurchased(8);
			}
			if(Input.GetKeyDown(KeyCode.V))
			{
				MoneyChanged(1000);
			}
			if(Input.GetKeyDown(KeyCode.M))
			{
				for(int i = 9; i < 18; i++)
				{
					handValues.baubleScript.HighTierHandPlayed(i);
				}
			}
		}
		if(currentScore > 0)
		{
			timeSinceLastBubble += Time.deltaTime;
		}
		if(timeSinceLastBubble >= timeBetweenBubblesRandomized)
		{
			timeSinceLastBubble -= timeBetweenBubblesRandomized;
			timeBetweenBubblesRandomized = timeBetweenBubbles * Random.Range(0f, 2f);
			GameObject newBubble = Instantiate(bubblePrefab, new Vector3(0,0,0), Quaternion.identity, bubbleParent);
			RectTransform newBubbleRT = newBubble.GetComponent<RectTransform>();
			newBubbleRT.anchoredPosition = new Vector2(Random.Range(-2, 8), -4);
			BubbleScript bubbleScript = newBubble.GetComponent<BubbleScript>();
			bubbleScript.verticalSpeed = (5f - timeBetweenBubbles) * 20f;
			bubbleScript.fillRT = fillRT;
		}
    }
}
