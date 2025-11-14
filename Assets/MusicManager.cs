using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
	
	public AudioSource musicSource;
	
	public AudioClip mainMenuMusic;
	public AudioClip[] gameplayMusic;
	private int[] songOrder;
	private int curSongIndex;
	
	private void ManageMusic()
	{
		if(!musicSource.isPlaying && GameOptions.instance.musicOn)
		{
			if(MainMenu.instance.mainMenuCanvas.activeSelf)
			{
				musicSource.clip = mainMenuMusic;
			}
			else
			{
				musicSource.clip = gameplayMusic[songOrder[curSongIndex]];
				curSongIndex++;
				if(curSongIndex >= songOrder.Length)
				{
					curSongIndex = 0;
				}
				//songTitleText.text = songNames[songOrder[curSongIndex]];
				//songArtistText.text = songArtists[songOrder[curSongIndex]];
			}
			musicSource.Play();
		}
	}
	
	void Update()
	{
		ManageMusic();
	}
	
	private void ShuffleSongOrder()
	{
		songOrder = new int[gameplayMusic.Length];
		for(int i = 0; i < gameplayMusic.Length; i++)
		{
			songOrder[i] = i;
		}
		for(int j = 0; j <gameplayMusic.Length; j++)
		{
			int k = UnityEngine.Random.Range(0, j+1);
			int temp = songOrder[j];
			songOrder[j] = songOrder[k];
			songOrder[k] = temp;
		}
	}
	
	void Start()
	{
		ShuffleSongOrder();
	}
	
	public void MusicOptionsUpdated()
	{
		musicSource.volume = GameOptions.instance.musicVolume;
		if(GameOptions.instance.musicOn)
		{
			/* if(!musicSource.isPlaying)
			{
				musicSource.Play();
			} */
		}
		else
		{
			musicSource.Stop();
		}
	}
	
	
	void Awake()
	{
		instance = this;
	}
}
