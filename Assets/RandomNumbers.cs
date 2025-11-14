using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomNumbers : MonoBehaviour
{
	public static RandomNumbers instance;
    private System.Random randomGenerator;
	private System.Random dailyDeckGenerator;
	
	void Awake()
	{
		instance = this;
	}
	
	public int GetDailyDeck(int min, int max, int dailySeed)
	{
		dailyDeckGenerator = new System.Random(dailySeed);
		return dailyDeckGenerator.Next(min, max);
	}
	
	public int GetDailyVariant(int min, int max)
	{
		return dailyDeckGenerator.Next(min, max);
	}
	
	public void ChangeSeed(int seed)
	{
		randomGenerator = new System.Random(seed);
	}
	
	public int Range(int min, int max)
	{
		return randomGenerator.Next(min, max);
	}
	
	public float Range(float min, float max)
	{
		return Mathf.Lerp(min, max, (float)randomGenerator.NextDouble());
	}
}
