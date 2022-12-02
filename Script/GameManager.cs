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

    Image titleImage; //�̹����� ǥ���ϴ� Image ������Ʈ

    // +++ �ð� ���� �߰� +++
    public GameObject timeBar;
    public GameObject timeText;
    TimeController timeCnt;

    // +++ ���� �߰� +++
    public GameObject scoreText; //���� �ؽ�Ʈ
    public static int totalScore; // ���� ����
    public int stageScore = 0; // �������� ����

    // Start is called before the first frame update
    void Start()
    {
        // �̹��� �����
        Invoke("InactiveImage", 1.0f);
        // ��ư(�г�) �����
        Panel.SetActive(false);
        // +++ �ð� ���� �߰� +++
        //TimeController ������
        timeCnt = GetComponent<TimeController>();
        if (timeCnt != null)
        {
            if (timeCnt.gameTime == 0.0f)
            {
                timeBar.SetActive(false); // �ð� ������ ������ ����
            }
        }
        // +++ ���� �߰� +++
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        // ���� Ŭ����
        if (PlayerController.gameState == "gameclear")
        {
            mainImage.SetActive(true);
            Panel.SetActive(true);
            //RESET ��ư ��ȿȭ
            Button bt = restartButton.GetComponent<Button>();
            bt.interactable = false;
            mainImage.GetComponent<Image>().sprite = gameClearSpr;
            PlayerController.gameState = "gameend";
            // +++ �ð� ���� �߰� +++
            if (timeCnt != null)
            {
                timeCnt.isTimeOver = true; // �ð� ī��Ʈ ����
                // +++ ���� �߰� +++
                int time = (int)timeCnt.displayTime; //���� �Ҵ� �Ҽ��� ������
                totalScore += time * 10; //���� �ð��� ������ ���Ѵ�.
            }
            // +++ ���� �߰� +++
            totalScore += stageScore;
            stageScore = 0;
            UpdateScore(); // ���� ����
        }

        // ���� ����
        else if (PlayerController.gameState == "gameover")
        {
            mainImage.SetActive(true);
            Panel.SetActive(true);
            //NEXT ��ư ��ȿȭ
            Button bt = nextButton.GetComponent<Button>();
            bt.interactable = false;
            mainImage.GetComponent<Image>().sprite = gameOverSpr;
            PlayerController.gameState = "gameend";
            // +++ �ð� ���� �߰� +++
            if (timeCnt != null)
            {
                timeCnt.isTimeOver = true; // �ð� ī��Ʈ ����
            }

        }
        else if (PlayerController.gameState == "playing")
        {
            // ���� ��
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            //PlayerController ��������
            PlayerController playerCnt = player.GetComponent<PlayerController>();
            // +++ �ð� ���� �߰� +++
            // �ð� ����
            if (timeCnt != null)
            {
                if (timeCnt.gameTime > 0.0f)
                {
                    // ������ �Ҵ��Ͽ� �Ҽ��� ���ϸ� ����
                    int time = (int)timeCnt.displayTime;
                    // �ð� ����
                    timeText.GetComponent<Text>().text = time.ToString();
                    // Ÿ�� ����
                    if (time == 0)
                    {
                        playerCnt.GameOver(); //���� ����
                    }
                }
            }
            // +++ ���� �߰� +++
            if (playerCnt.score != 0)
            {
                stageScore += playerCnt.score;
                playerCnt.score = 0;
                UpdateScore();
            }
        }
    }


    // �̹��� �����
    void InactiveImage()
    {
        mainImage.SetActive(false);
    }
    // +++ ���� �߰� +++
    void UpdateScore()
    {
        int score = stageScore + totalScore;
        scoreText.GetComponent<Text>().text = score.ToString();
    }
}
