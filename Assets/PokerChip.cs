using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PokerChip : MonoBehaviour
{
	public RectTransform rt;
	public Vector2 startPosition;
	public Vector2 endPosition;
	public float travelTime;
	public float t;
	public AnimationCurve travelCurve;
	public ScoreVial scoreVial;
	public GameObject particlePrefab;
	public float spawnInterval;
	private float lastSpawnTime;
	public Transform particleParent;
	public Image chipImage;
	public bool moving = true;
	public Transform movingChipParent;
	
    void Start()
    {
        lastSpawnTime = Time.time;
    }

    void Update()
    {
		if(moving)
		{
			if(t < travelTime)
			{
				t += Time.deltaTime * scoreVial.handValues.gameOptions.gameSpeedFactor;
				rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, travelCurve.Evaluate(t / travelTime));
			}
			else
			{
				scoreVial.MoneyChanged(1);
				scoreVial.StartCoroutine(scoreVial.CheckIfMenuShouldUnlock());
				Destroy(this.gameObject);
			}
			if(Time.time - lastSpawnTime >= spawnInterval)
			{
				SpawnParticle();
				lastSpawnTime = Time.time;
			}
		}
    }
	
	void SpawnParticle()
	{
		GameObject particle = Instantiate(particlePrefab, rt.anchoredPosition, Quaternion.identity, particleParent);
		ParticleScript particleScript = particle.GetComponent<ParticleScript>();
		particleScript.rt.anchoredPosition = rt.anchoredPosition;
		particleScript.direction = (particleScript.rt.anchoredPosition - endPosition).normalized;
		//particle.transform.SetParent(particleParent, false);
	}
}
