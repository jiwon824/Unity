using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
	public AudioClip gameMusic; // gameMusic
	public AudioClip titleMusic; // titleMusic

	string sceneName;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	// void OnLevelWasLoaded() 대신 sceneLoaded()를 사용하라는 경고가 떴다.(앞의 함수는 구버전에서 사용되었던 함수)
	// SceneManager.sceneLoaded
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		string newSceneName = SceneManager.GetActiveScene().name;

		// 씬 이름이 바뀌었으면 현재 씬 이름을 바뀐 씬 이름으로 변경하고 PlayMusic() 실행
		if (newSceneName != sceneName)
		{
			sceneName = newSceneName;
			// Invoke(string methodName, float time);
			// time 초 뒤에 methodName 함수를 호출하는 함수.
			// 성능이 좋지 않기 때문에 코루틴으로 변경하는 것이 좋다.
			Invoke("PlayMusic", .2f);
		}
		else PlayMusic();
	}

	// called when the game is terminated
	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void PlayMusic()
	{
		// Play 해야하는 클립을 설정하는 조건/
		AudioClip clipToPlay = null;
		// 메인 화면에서는 titleMusic 실행
		// ++++++ 나중에 스테이지 선택 창을 넣을 거라면 여기 조건문에 스테이지 선택창을 넣을 것 ++++++
		if (sceneName == "Title")
		{
			clipToPlay = titleMusic;
		}
		// 그게 아니라면(지금은 타이틀 하니면 Stage 씬 밖에 없으니까) 게임 화면에서는 gameMusic 실행 
		else
		{
			clipToPlay = gameMusic;
		}

		// 플레이 해야 하는 클립이 있으면 AudioManager의 PlayMusic() 실행 
		if (clipToPlay != null)
		{
			AudioManager.instance.PlayMusic(clipToPlay);
			Invoke("PlayMusic", clipToPlay.length);
		}

	}

}
