using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;
	public AudioSource soundSource;
	public AudioSource tickSource;
	public AudioClip[] cardPickupSounds;
	public AudioClip[] cardDropSounds;
	public AudioClip[] cardSlideSounds;
	public AudioClip[] chipSpounds;
	public AudioClip[] xylophoneSounds;
	public AudioClip[] clatterSounds;
	public AudioClip[] shuffleSounds;
	public AudioClip anteClearedSound;
	public AudioClip cashRegisterSound;
	//public AudioClip slideSound;
	public AudioClip buttonSound;
	public AudioClip rerollSound;
	public AudioClip victorySound;
	public AudioClip[] scoreSounds;
	public AudioClip[] growthSounds;
	public AudioClip scoreMultipliedSound;
	public AudioClip[] paintSounds;
	public AudioClip bombSound;
	public AudioClip scoreThresholdDissolveSound;
	public AudioClip slideOutSound;
	public AudioClip slideOutSoundReversed;
	public AudioClip[] tickSounds;
	public AudioClip[] violinSounds;
	public AudioClip[] vampireSounds;
	public AudioClip monarchSound;
	public AudioClip gameOverSound;
	public AudioClip promotionSound;
	public AudioClip demotionSound;
	public AudioClip woodenKSound;
	public AudioClip dieSound;
	public AudioClip maxRollSound;
	public AudioClip minRollSound;
	public AudioClip markerSound;
	public AudioClip bottlePopSound;
	public AudioClip cardDissolveSound;
	public AudioClip spyglassSound;
	public AudioClip magnetSound;
	public AudioClip queenSound;
	
    void Awake()
	{
		instance = this;
	}
	
	public void PlayRandomSound()
	{
		int ran = UnityEngine.Random.Range(0,33);
		switch(ran)
		{
			case 0:
			PlayXylophoneSound();
			break;
			case 1:
			PlayChipSound();
			break;
			case 2:
			PlayCardSlideSound();
			break;
			case 3:
			PlayCardPickupSound();
			break;
			case 4:
			PlayCardDropSound();
			break;
			case 5:
			PlayClatterSound();
			break;
			case 6:
			PlayCashRegisterSound();
			break;
			case 7:
			PlayAnteClearedSound();
			break;
			case 8:
			PlayShuffleSound();
			break;
			case 9:
			PlayRerollSound();
			break;
			case 10:
			PlayVictorySound();
			break;
			case 11:
			PlayGrowScoreSound();
			break;
			case 12:
			PlayStandardScoreSound();
			break;
			case 13:
			PlayScoreMultipliedSound();
			break;
			case 14:
			PlayPaintSound();
			break;
			case 15:
			PlayBombSound();
			break;
			case 16:
			PlayScoreThresholdDissolveSound();
			break;
			case 17:
			PlaySlideOutSound();
			break;
			case 18:
			PlayViolinSound();
			break;
			case 19:
			PlayVampireSound();
			break;
			case 20:
			PlayMonarchSound();
			break;
			case 21:
			PlayGameOverSound();
			break;
			case 22:
			PlayPromotionSound();
			break;
			case 23:
			PlayDemotionSound();
			break;
			case 24:
			PlayWoodenKSound();
			break;
			case 25:
			PlayDieSound();
			break;
			case 26:
			PlayMaxRollSound();
			break;
			case 27:
			PlayMinRollSound();
			break;
			case 28:
			PlayMarkerSound();
			break;
			case 29:
			PlayBottlePopSound();
			break;
			case 30:
			PlayCardDissolveSound();
			break;
			case 31:
			PlaySpyglassSound();
			break;
			case 32:
			PlayMagnetSound();
			break;
		}
	}
	
	public void PlayQueenSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(queenSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayMagnetSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(magnetSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlaySpyglassSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(spyglassSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayCardDissolveSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(cardDissolveSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayBottlePopSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(bottlePopSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayMarkerSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(markerSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayMinRollSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(minRollSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayMaxRollSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(maxRollSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayDieSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(dieSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayWoodenKSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(woodenKSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayDemotionSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(demotionSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayPromotionSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(promotionSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayGameOverSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(gameOverSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayMonarchSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(monarchSound, GameOptions.instance.soundVolume * 0.5f);
		}
	}
	
	private float lastVampireSoundTime = 0;
	private int vampireIndex = 0;
	
	public void PlayVampireSound()
	{
		if(GameOptions.instance.soundOn)
		{
			if(Time.time - lastVampireSoundTime > 4f)
			{
				vampireIndex = 0;
			}
			if(vampireIndex >= vampireSounds.Length)
			{
				vampireIndex = vampireSounds.Length - 1;
			}
			lastVampireSoundTime = Time.time;
			//print("playing vamp sound " + vampireIndex);
			soundSource.PlayOneShot(vampireSounds[vampireIndex], GameOptions.instance.soundVolume * 0.6f);
			vampireIndex++;
		}
	}
	
	private float lastViolinSoundTime = 0;
	private int violinIndex = 0;
	
	public void PlayViolinSound()
	{
		if(GameOptions.instance.soundOn)
		{
			if(Time.time - lastViolinSoundTime > 4f)
			{
				violinIndex = 0;
			}
			if(violinIndex >= violinSounds.Length)
			{
				violinIndex = violinSounds.Length - 1;
			}
			lastViolinSoundTime = Time.time;
			soundSource.PlayOneShot(violinSounds[violinIndex], GameOptions.instance.soundVolume);
			violinIndex++;
		}
	}
	
	private float lastTickSoundTime = 0;
	private int tickSoundIndex = 0;
	
	public void PlayTickSound()
	{
		if(GameOptions.instance.soundOn)
		{
			if(Time.time - lastTickSoundTime > 0.2f)
			{
				tickSoundIndex = 0;
			}
			lastTickSoundTime = Time.time;
			tickSource.pitch = 1f + 0.05f * tickSoundIndex;
			tickSource.PlayOneShot(tickSounds[UnityEngine.Random.Range(0,tickSounds.Length)], GameOptions.instance.soundVolume * 0.5f);
			tickSoundIndex++;
		}
	}
	
	public void PlaySlideOutSound(bool reversed = false)
	{
		if(GameOptions.instance.soundOn)
		{
			if(reversed)
			{
				soundSource.PlayOneShot(slideOutSoundReversed, GameOptions.instance.soundVolume * 0.5f);
			}
			else
			{
				soundSource.PlayOneShot(slideOutSound, GameOptions.instance.soundVolume * 0.5f);
			}
		}
	}
	
	public void PlayScoreThresholdDissolveSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(scoreThresholdDissolveSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayBombSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(bombSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayPaintSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(paintSounds[UnityEngine.Random.Range(0,paintSounds.Length)], GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayScoreMultipliedSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(scoreMultipliedSound, GameOptions.instance.soundVolume);
		}
	}
	
	private float lastScoreSoundTime = 0;
	private int scoreSoundIndex = 0;
	
	public void PlayStandardScoreSound()
	{
		if(GameOptions.instance.soundOn)
		{
			if(Time.time - lastScoreSoundTime > 2f)
			{
				scoreSoundIndex = 0;
			}
			if(scoreSoundIndex >= scoreSounds.Length)
			{
				scoreSoundIndex = scoreSounds.Length - 1;
			}
			soundSource.PlayOneShot(scoreSounds[scoreSoundIndex], GameOptions.instance.soundVolume);
			scoreSoundIndex++;
			lastScoreSoundTime = Time.time;
		}
	}
	
	public void PlayGrowScoreSound()
	{
		if(GameOptions.instance.soundOn)
		{
			if(Time.time - lastScoreSoundTime > 2f)
			{
				scoreSoundIndex = 0;
			}
			if(scoreSoundIndex >= growthSounds.Length)
			{
				scoreSoundIndex = 0;
			}
			// soundSource.PlayOneShot(growthSounds[scoreSoundIndex], GameOptions.instance.soundVolume);
			soundSource.PlayOneShot(growthSounds[0], GameOptions.instance.soundVolume);
			scoreSoundIndex++;
			lastScoreSoundTime = Time.time;
		}
	}
	
	public void PlayVictorySound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(victorySound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayRerollSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(rerollSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayShuffleSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(shuffleSounds[UnityEngine.Random.Range(0,shuffleSounds.Length)], GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayAnteClearedSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(anteClearedSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayCashRegisterSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(cashRegisterSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayClatterSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(clatterSounds[UnityEngine.Random.Range(0,clatterSounds.Length)], GameOptions.instance.soundVolume);
		}
	}
	
	private float lastXylophoneTime = 0;
	private int xylophoneIndex = 0;
	
	public void PlayXylophoneSound()
	{
		if(GameOptions.instance.soundOn)
		{
			if(Time.time - lastXylophoneTime > 2f)
			{
				xylophoneIndex = 0;
			}
			if(xylophoneIndex >= xylophoneSounds.Length)
			{
			
			}
			else
			{
				soundSource.PlayOneShot(xylophoneSounds[xylophoneIndex], GameOptions.instance.soundVolume);
				xylophoneIndex++;
				lastXylophoneTime = Time.time;
			}
		}
	}
	
	public void PlayChipSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(chipSpounds[UnityEngine.Random.Range(0,chipSpounds.Length)], GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayButtonSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(buttonSound, GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayCardSlideSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(cardSlideSounds[UnityEngine.Random.Range(0,cardSlideSounds.Length)], GameOptions.instance.soundVolume);
		}
	}
	
	public void PlayCardPickupSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(cardPickupSounds[UnityEngine.Random.Range(0,cardPickupSounds.Length)], GameOptions.instance.soundVolume * 0.5f);
		}
	}
	
	public void PlayCardDropSound()
	{
		if(GameOptions.instance.soundOn)
		{
			soundSource.PlayOneShot(cardDropSounds[UnityEngine.Random.Range(0,cardDropSounds.Length)], GameOptions.instance.soundVolume * 0.5f);
		}
	}
}
