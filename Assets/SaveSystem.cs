using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

All cards, and where they are, from deck to hand to discard pile, and their modified score and multiplier
Order of deck
Current chips
Current hands until fatigue and discards
Current score, and ante
Current variant, if any
Current chosen deck
Purchased baubles and zodiacs, preferably in order so it doesn't change the order in the baubles pull out
Seed and number of pulls from the PRNG so it couldn't be manipulated
Is player in the shop or in the middle of an ante
Save should be made automatically after each hand played to prevent abuse
Whether or not it's a daily game or seeded game
Information for the run, like the date it took place (started), number of hands played, cards discarded, number of baubles purchased, number of cards added to deck, zodiacs earned, chips earned, best hand so far (name, score and cards involved)
Which non-standard hand types have been played so far (five of a kind and up)

*/

public class SaveSystem : MonoBehaviour
{
	public int chips;
	public int handsUntilFatigue;
	public int discards;
	// public float score;
	// public int ante;
	// public int deck;
	// public int seed;
	public int PRNGPulls;
	// public bool daily;
	// public bool seeded;
	public bool inShop;
	// public string curVariant;
	public string runInformationString;	// contains variant, ante, score in final ante, deck, seed, and everything needed for the stats screen
	
	public class SaveInformation
	{
		
		
	}
	
	public void UpdateSaveInformation()
	{
		
	}
	
	void Start()
    {
        
    }

    void Update()
    {
        
    }
}
