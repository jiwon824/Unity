using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI를 사용할 때 필p

public class GameManager : MonoBehaviour
{
    public GameObject GameState;// 게임 상태를 표시하는 오브젝트
    private Text text; // GameOver!/GameClear!를 표시
    public GameObject panel; // 패널
    public GameObject RestartButton;
    public GameObject QuitButton;

    // Start is called before the first frame update
    void Start()
    {
        text = GameState.GetComponent<Text>();
        panel.SetActive(false); // 패널숨기기
        GameState.SetActive(false); // 텍스트 숨기ㅑ
        
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 클리어시 텍스트 바꾸고 패널, 텍스트 보이기
        // text.text = "Game Clear!";
        // panel.SetActive(true);
        // GameState.SetActive(true);
    }
}
