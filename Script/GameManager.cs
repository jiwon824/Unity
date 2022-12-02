using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject mainImage;
    public Sprite gameOverSpr;
    public Sprite gameClearSpr;
    public GameObject Panel;
    public GameObject restartButton;
    public GameObject nextButton;

    Image titleImage; //이미지를 표시하는 Image 컴포넌트

    // +++ 시간 제한 추가 +++
    public GameObject timeBar;
    public GameObject timeText;
    TimeController timeCnt;

    // +++ 점수 추가 +++
    public GameObject scoreText; //점수 텍스트
    public static int totalScore; // 점수 총합
    public int stageScore = 0; // 스테이지 점수

    // Start is called before the first frame update
    void Start()
    {
        // 이미지 숨기기
        Invoke("InactiveImage", 1.0f);
        // 버튼(패널) 숨기기
        Panel.SetActive(false);
        // +++ 시간 제한 추가 +++
        //TimeController 가져옴
        timeCnt = GetComponent<TimeController>();
        if (timeCnt != null)
        {
            if (timeCnt.gameTime == 0.0f)
            {
                timeBar.SetActive(false); // 시간 제한이 없으면 숨김
            }
        }
        // +++ 점수 추가 +++
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 클리어
        if (PlayerController.gameState == "gameclear")
        {
            mainImage.SetActive(true);
            Panel.SetActive(true);
            //RESET 버튼 무효화
            Button bt = restartButton.GetComponent<Button>();
            bt.interactable = false;
            mainImage.GetComponent<Image>().sprite = gameClearSpr;
            PlayerController.gameState = "gameend";
            // +++ 시간 제한 추가 +++
            if (timeCnt != null)
            {
                timeCnt.isTimeOver = true; // 시간 카운트 중지
                // +++ 점수 추가 +++
                int time = (int)timeCnt.displayTime; //정수 할당 소수점 버리기
                totalScore += time * 10; //남은 시간에 점수를 더한다.
            }
            // +++ 점수 추가 +++
            totalScore += stageScore;
            stageScore = 0;
            UpdateScore(); // 점수 갱신
        }

        // 게임 오버
        else if (PlayerController.gameState == "gameover")
        {
            mainImage.SetActive(true);
            Panel.SetActive(true);
            //NEXT 버튼 무효화
            Button bt = nextButton.GetComponent<Button>();
            bt.interactable = false;
            mainImage.GetComponent<Image>().sprite = gameOverSpr;
            PlayerController.gameState = "gameend";
            // +++ 시간 제한 추가 +++
            if (timeCnt != null)
            {
                timeCnt.isTimeOver = true; // 시간 카운트 중지
            }

        }
        else if (PlayerController.gameState == "playing")
        {
            // 게임 중
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            //PlayerController 가져오기
            PlayerController playerCnt = player.GetComponent<PlayerController>();
            // +++ 시간 제한 추가 +++
            // 시간 갱신
            if (timeCnt != null)
            {
                if (timeCnt.gameTime > 0.0f)
                {
                    // 정수에 할당하여 소수점 이하를 버림
                    int time = (int)timeCnt.displayTime;
                    // 시간 갱신
                    timeText.GetComponent<Text>().text = time.ToString();
                    // 타임 오버
                    if (time == 0)
                    {
                        playerCnt.GameOver(); //게임 오버
                    }
                }
            }
            // +++ 점수 추가 +++
            if (playerCnt.score != 0)
            {
                stageScore += playerCnt.score;
                playerCnt.score = 0;
                UpdateScore();
            }
        }
    }


    // 이미지 숨기기
    void InactiveImage()
    {
        mainImage.SetActive(false);
    }
    // +++ 점수 추가 +++
    void UpdateScore()
    {
        int score = stageScore + totalScore;
        scoreText.GetComponent<Text>().text = score.ToString();
    }
}
