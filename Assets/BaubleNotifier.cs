using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaubleNotifier : MonoBehaviour
{
	public RectTransform rt;
	private float timeAlive;
	public float moveTime;
	public float waitTime;
	public Image baubleImage;
	
	public Vector2 startPosition;
	public Vector2 endPosition;
	public Vector3 endRotation;
	private bool alreadyAnimated = false;
	public int baubleNumber;
	public RectTransform numberRT;
	public GameObject numberObject;
	public TMP_Text[] numberTexts;
	public int rollType = 0; // 0 = min roll, 1 = normal roll, 2 = maxRoll
	public int chipsToSpawn = 0;
	
	public IEnumerator Animate()
	{
		switch(baubleNumber)
		{
			case 20: // senor pail
			SoundManager.instance.PlayClatterSound();
			break;
			case 32:
			SoundManager.instance.PlayVampireSound();
			break;
			case 34: // piggy bank
			SoundManager.instance.PlayClatterSound();
			break;
			case 36:
			SoundManager.instance.PlayMonarchSound();
			break;
			case 40:
			SoundManager.instance.PlayWoodenKSound();
			break;
			case 56:
			if(rollType == 2)
			{
				SoundManager.instance.PlayMaxRollSound();
			}
			else if(rollType == 1)
			{
				SoundManager.instance.PlayDieSound();
			}
			else if(rollType == 0)
			{
				SoundManager.instance.PlayMinRollSound();
			}
			break;
			case 60:
			SoundManager.instance.PlayMagnetSound();
			break;
		}
		if(chipsToSpawn > 0)
		{
			HandValues.instance.menuButton.ChangeDisabled(true);
			for(int i = 0; i < chipsToSpawn; i++)
			{
				ScoreVial.instance.SpawnPokerChip(new Vector2(endPosition.x + UnityEngine.Random.Range(-chipsToSpawn * 2, chipsToSpawn * 2), endPosition.y + UnityEngine.Random.Range(-chipsToSpawn * 2, chipsToSpawn * 2) - 140f), ScoreVial.instance.movingChipParent);
			}
		}
		float t = 0;
		Quaternion originalRotationQ = rt.localRotation;
		Quaternion destinationRotationQ = Quaternion.Euler(endRotation);
		while(t < 0.1)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(originalRotationQ, destinationRotationQ, t / 0.1f);
			yield return null;
		}
		t = 0;
		while(t < 0.15)
		{
			t += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
			rt.localRotation = Quaternion.Lerp(destinationRotationQ, originalRotationQ, t / 0.15f);
			yield return null;
		}
	}
	
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		if(baubleNumber == 56)
		{
			transform.SetSiblingIndex(transform.parent.childCount - 1);
		}
        timeAlive += Time.deltaTime * GameOptions.instance.gameSpeedFactor;
		if(timeAlive <= moveTime)
		{
			rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, timeAlive / moveTime);
		}
		if(timeAlive > moveTime + waitTime && !alreadyAnimated)
		{
			alreadyAnimated = true;
			StartCoroutine(Animate());
		}
		if(timeAlive > moveTime + waitTime*2)
		{
			float moveBack = (timeAlive - moveTime - waitTime*2) / moveTime;
			//print("timeAlive= " + timeAlive.ToString() + " moveTime= " + moveTime.ToString() + " waitTime= " + waitTime.ToString() + " moveBack= " + moveBack.ToString());
			
			rt.anchoredPosition = Vector2.Lerp(endPosition, startPosition, moveBack);
		}
		if(timeAlive > moveTime*2 + waitTime*2)
		{
			Destroy(this.gameObject);
		}
    }
}
