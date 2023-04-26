using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI를 사용할 때 필요
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject GameEnd;
    public GameObject GameState;// 게임 상태를 표시하는 오브젝트
    private Text text; // GameOver!/GameClear!를 표시
    public GameObject RestartButton;
    public GameObject QuitButton;

    // 현재 씬 이름 (RESTRAT를 위/)
    public string sceneName;
    public string nextScene;

    // Start is called before the first frame update
    void Start()
    {
        text = GameState.GetComponent<Text>();
        GameEnd.SetActive(false); // 패널숨기기

    }

    // UI Input
    // RESTART 버튼을 눌렀을 때 현재 씬 다시 시작
    public void OnClick_Restart()
    {
        AudioManager.instance.PlaySound2D("Button Click");
        //AudioManager.instance.PlayMusic();
        SceneManager.LoadScene(sceneName);
    }

    // QUIT버튼을 누르면 스테이지 선택창으로 돌아가도록 만들기
    public void OnClick_Quit()
    {
        AudioManager.instance.PlaySound2D("Button Click");
        // 스테이지 선택 씬이 생기면 씬 이름 변경해주기
        SceneManager.LoadScene("Title");
    }

    // 게임을 클리어하면 Restart 버튼이 비활성화 되고
    // 다음 스테이지로 갈 수 있는 Next 버튼이 활성화 된다.
    public void OnClick_Next()
    {
        AudioManager.instance.PlaySound2D("Button Click");
        SceneManager.LoadScene(nextScene);
    }

}
