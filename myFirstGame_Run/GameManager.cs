using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    // +++ 변수 선언 +++
    public GameObject gameEndPanel;
    public Text gameStateText;
	public GameObject restartButton;
	public GameObject nextButton;

	// 플레이어를 스크립트로 가져오기 
	public Player player;
	

	// RESTART 버튼이 눌리면 
	public void GameStart()
	{
		//gameEnd UI가 안 보이게 설정
		gameEndPanel.SetActive(false);
		// 현재 씬(=현재 스테이지) 다시 시작

	}

	public void GameOver()
    {
		// 텍스트가 기본적으로 GameOver로 설정되어 있다.
		// gameStateText.text = "GameOver!";

		// next 버튼 비활성화 Restart 버튼 활성
		nextButton.SetActive(false);
		restartButton.SetActive(true);

		gameEndPanel.SetActive(true);
	}

	public void GameClear()
	{
		gameStateText.text = "GameClear!";

		// next 버튼 활성화 Restart 버튼 비활성화
		restartButton.SetActive(false);
		nextButton.SetActive(true);

		gameEndPanel.SetActive(true);
		
	}

}
