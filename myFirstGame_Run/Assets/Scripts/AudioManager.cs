using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{

	public enum AudioChannel { Master, Sfx, Music };

	public float masterVolumePercent { get; private set; }
	public float sfxVolumePercent { get; private set; }
	public float musicVolumePercent { get; private set; }

	public bool isMuteMaster;
	public bool isMuteMusic;
	public bool isMuteSfx;

	AudioSource sfx2DSource; // 2D로 재생할 효과음
	AudioSource backgorundMusic;

	public static AudioManager instance;

	Transform audioListener;
	Transform playerT;

	SoundLibrary library;
	//TitleUI title;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{

			instance = this;
			DontDestroyOnLoad(gameObject);

			library = GetComponent<SoundLibrary>();
			//title = GetComponent<TitleUI>();
			// 배경음악 GameObject
			GameObject newbackgorundMusic = new GameObject("backgorund source");
			backgorundMusic = newbackgorundMusic.AddComponent<AudioSource>();
			newbackgorundMusic.transform.parent = transform;
			// 효과음 GameObject
			GameObject newSfx2Dsource = new GameObject("2D sfx source");
			sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
			newSfx2Dsource.transform.parent = transform;

			audioListener = FindObjectOfType<AudioListener>().transform;
			if (FindObjectOfType<Player>() != null)
			{
				playerT = FindObjectOfType<Player>().transform;
			}

			masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
			sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
			musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);

			// mute 설정 불러오기
			isMuteMaster = (PlayerPrefs.GetInt("master mute") == 1) ? true : false;
			isMuteMusic = (PlayerPrefs.GetInt("master mute") == 1) ? true : false;
			isMuteSfx = (PlayerPrefs.GetInt("master mute") == 1) ? true : false;

		}
	}

	void Update()
	{
		if (playerT != null)
		{
			audioListener.position = playerT.position;
		}
	}

	public void SetVolume(float volumePercent, AudioChannel channel)
	{
		switch (channel)
		{
			case AudioChannel.Master:
				masterVolumePercent = volumePercent;
				break;
			case AudioChannel.Sfx:
				sfxVolumePercent = volumePercent;
				break;
			case AudioChannel.Music:
				musicVolumePercent = volumePercent;
				break;
		}

		backgorundMusic.volume = musicVolumePercent * masterVolumePercent;

		PlayerPrefs.SetFloat("master vol", masterVolumePercent);
		PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
		PlayerPrefs.SetFloat("music vol", musicVolumePercent);
		PlayerPrefs.Save();
	}

	public void SetMute (bool isMute, AudioChannel channel)
	{
		switch (channel)
		{
			case AudioChannel.Master:
				isMuteMaster = isMute;
				if (isMuteMaster) masterVolumePercent = 0;
				//else masterVolumePercent = title.volumeSliders[0].value;
				break;
			case AudioChannel.Sfx:
				isMuteSfx = isMute;
				if (isMuteSfx) sfxVolumePercent = 0;
				//else sfxVolumePercent = title.volumeSliders[2].value;
				break;
			case AudioChannel.Music:
				isMuteMusic = isMute;
				if (isMuteMusic) musicVolumePercent = 0;
				//else musicVolumePercent = title.volumeSliders[1].value;
				break;
		}

		PlayerPrefs.SetInt("master mute", ((isMuteMaster) ? 1 : 0));
		PlayerPrefs.SetInt("sfx mute", ((isMuteSfx) ? 1 : 0));
		PlayerPrefs.SetInt("music mute", ((isMuteMusic) ? 1 : 0));
		PlayerPrefs.Save();
	}

	// 배경음악 재생
	public void PlayMusic(AudioClip clip)
	{
		backgorundMusic.clip = clip;
		backgorundMusic.Play();
	}

	// AudioClip을 매개변수로 PlaySound()
	public void PlaySound(AudioClip clip, Vector3 pos)
	{
		if (clip != null)
		{
			AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
		}
	}

	// sound의 이름(=string)을 매개변수로 PlaySound()
	public void PlaySound(string soundName, Vector3 pos)
	{
		PlaySound(library.GetClipFromName(soundName), pos);
	}

	// 위의 함수는 3D 사운드로 재생되지만, level completion 사운드와 같은 몇몇 사운드들은 사실 2d 사운드가 더 낫다.
	// sfx2DSource.PlayOneShot() 함수 사용. clip을 라이브러리에서 가져오고, 음량도 매개변수로 함께 전달.
	public void PlaySound2D(string soundName)
	{
		sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
	}

	// 게임이 종료 되었을 때 (게임 클리어/게임오버) 음악을 멈추는 함수 
	public void StopMusic()
	{
		backgorundMusic.Stop();
	}

}
