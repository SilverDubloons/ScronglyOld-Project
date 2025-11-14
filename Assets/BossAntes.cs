using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAntes : MonoBehaviour
{
    public static BossAntes instance;
	public int bossAnteFrequency; // 3 would be every 3 levels, 4 every 4 etc
	public bool alwaysAnte30;
	public bool alwaysAnte50;
	public TextPrefab anteText;
	public RectTransform anteDisplayRT;
	public RectTransform anteDisplayContentRT;
	
	void Awake()
	{
		instance = this;
	}
	
	void ChangeRandomBossAntes()
	{
		int ranSuit = RandomNumbers.instance.Range(0, 4);
		string suitName = HandValues.instance.suitNames[ranSuit] + "s";
		bossAntes[1].bossName = string.Format(bossAntes[1].bossName, suitName);
		bossAntes[1].affectInt = ranSuit;
	}
	
	[System.Serializable]
	public class BossAnte
	{
		public string bossName;
		public string bossDescription;
		public Sprite bossImage;
		public int bossMinimumAnte;
		public int bossMaximumAnte;	// 0 = infinite
		public int bossAnteIndex;
		public int timesUsedInRun;
		public int affectInt;
	}
	
	public BossAnte[] bossAntes;
	
	void Start()
	{
		AssignBossAnteIndices();
	}
	
	public void AssignBossAnteIndices()
	{
		for(int i = 0; i < bossAntes.Length; i++)
		{
			bossAntes[i].bossAnteIndex = i;
		}
	}
	
	public class CurrentRunBossAnte
	{
		public int anteNumber;
		public BossAnte bossAnte;
		
		public CurrentRunBossAnte(int anteNumber, BossAnte bossAnte)
		{
			this.anteNumber = anteNumber;
			this.bossAnte = bossAnte;
		}
	}
	
	public List<CurrentRunBossAnte> currentRunBossAntes = new List<CurrentRunBossAnte>();
	
	public void ResetTimesUsedInRun()
	{
		for(int i = 0; i < bossAntes.Length; i++)
		{
			bossAntes[i].timesUsedInRun = 0;
		}
	}
	
	public int GetBossAnteIndex(int ante)
	{
		for(int i = 0; i < currentRunBossAntes.Count; i++)
		{
			if(currentRunBossAntes[i].anteNumber == ante)
			{
				return currentRunBossAntes[i].bossAnte.bossAnteIndex;
			}
		}
		return -1;
	}
	
	public void SetupBossAntes()
	{
		ResetTimesUsedInRun();
		currentRunBossAntes = new List<CurrentRunBossAnte>();
		if(alwaysAnte30)
		{
			currentRunBossAntes.Add(new CurrentRunBossAnte(29, GetBossAnteForAnteNumber(29)));
		}
		if(alwaysAnte50)
		{
			currentRunBossAntes.Add(new CurrentRunBossAnte(49, GetBossAnteForAnteNumber(49)));
		}
		for(int i = bossAnteFrequency - 1; i < 50; i += bossAnteFrequency)
		{
			if((i == 29 && !alwaysAnte30) || (i == 49 && !alwaysAnte50) || (i != 29 && i != 49))
			{
				currentRunBossAntes.Add(new CurrentRunBossAnte(i, GetBossAnteForAnteNumber(49)));
			}
		}
		currentRunBossAntes.Sort(new CurrentRunBossAntesComparer());
		ChangeRandomBossAntes();
	}
	
	public BossAnte GetBossAnteForAnteNumber(int ante)
	{
		List<BossAnte> possibleAntes = new List<BossAnte>();
		for(int i = 0; i < bossAntes.Length; i++)
		{
			if((ante >= bossAntes[i].bossMinimumAnte || bossAntes[i].bossMinimumAnte == 0) && (ante <= bossAntes[i].bossMaximumAnte || bossAntes[i].bossMinimumAnte == 0) && bossAntes[i].timesUsedInRun <= 0)
			{
				possibleAntes.Add(bossAntes[i]);
			}
		}
		BossAnte bossAnteToReturn = possibleAntes[RandomNumbers.instance.Range(0, possibleAntes.Count)];
		// bossAnteToReturn.timesUsedInRun++; // turn this back on when we have enough (17 for base use case)
		return bossAnteToReturn;
	}
	
	public class CurrentRunBossAntesComparer : IComparer<CurrentRunBossAnte>
	{
		public int Compare(CurrentRunBossAnte crba1, CurrentRunBossAnte crba2)
		{
			return crba1.anteNumber.CompareTo(crba2.anteNumber);
		}
	}
	
	public void UpdateAnteDisplay(int curAnte, bool displayEndless)
	{
		bool showBossAntes = false;
		int maxAnte = 29;
		if(displayEndless)
		{
			maxAnte = 49;
		}
		string anteString = "";
		string anteStringShadow = "";
		int lastBossLevel = 0;
		for(int i = curAnte; i <= maxAnte; i++)
		{
			anteString += (i < 9 ? " " : "") + "<color=red>" + (i + 1) + "</color> " + HandValues.instance.ConvertFloatToString(HandValues.instance.antes[i]) + "\n";
			anteStringShadow += (i < 9 ? " " : "") + (i + 1) + " " + HandValues.instance.ConvertFloatToString(HandValues.instance.antes[i]) + "\n";
			if(currentRunBossAntes[lastBossLevel].anteNumber == i && showBossAntes)
			{
				anteString += currentRunBossAntes[lastBossLevel].bossAnte.bossName + "\n";
				anteStringShadow += currentRunBossAntes[lastBossLevel].bossAnte.bossName + "\n";
				lastBossLevel++;
			}
		}
		anteText.ChangeShadowText(anteStringShadow);
		anteText.ChangeOpaqueText(anteString);
		anteText.ChangeAlignmentToLeft();
		anteText.ChangeFontSizeMax(12);
		anteText.rt.sizeDelta = new Vector2(anteText.rt.sizeDelta.x, anteText.GetDesiredHeight());
		anteDisplayRT.sizeDelta = new Vector2(anteDisplayRT.sizeDelta.x, Mathf.Min((anteText.rt.sizeDelta.y + 12), 360));
		anteDisplayContentRT.sizeDelta = new Vector2(anteDisplayContentRT.sizeDelta.x, anteText.rt.sizeDelta.y + 6);
	}
}
