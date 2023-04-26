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

	AudioSource sfx2DSource; // 2D로 재생할 효과음 
	AudioSource backgorundMusic;
	//int activeMusicSourceIndex;

	public static AudioManager instance;

	Transform audioListener;
	Transform playerT;

	SoundLibrary library;

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

	// 뮤트 토글이랑 연결해야 함
	// 아닌가 ... setVolume(float 0, AudioChannel channel) 뭐 이런 식으로 해야하나
	/*
	public void MuteVolume(bool isMute, AudioChannel channel)
    {
		switch (channel)
		{
			case AudioChannel.Master:
				// Mute 면 AudioListener 0, 아니면 1
				if(isMute) AudioListener.volume = 0;
				else AudioListener.volume = 1;
				break;
			case AudioChannel.Sfx:
				// SoundLibrary에 함수 만들어서 효과음 안 들리게 설정하기 
				break;
			case AudioChannel.Music:
				//MusicManager에 함수 만들어서 배경음악만 안 들릴 수 있게 설정하기 
				break;
		}
	}
	*/

	// 배경음악 재생
	//public void PlayMusic(AudioClip clip, float fadeDuration = 1)
	public void PlayMusic(AudioClip clip)
	{
		//activeMusicSourceIndex = 1 - activeMusicSourceIndex;
		backgorundMusic.clip = clip;
		backgorundMusic.Play();

		//StartCoroutine(AnimateMusicCrossfade(fadeDuration));
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

	public void StopMusic()
	{
		backgorundMusic.Stop();
	}

	// AnimateMusicCrossfade 기능이 정확히 어떤 건지 모르겠어서 일단 주석처리
	/*
	IEnumerator AnimateMusicCrossfade(float duration)
	{
		float percent = 0;

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / duration;
			musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
			musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
			yield return null;
		}
	}
	*/
}
