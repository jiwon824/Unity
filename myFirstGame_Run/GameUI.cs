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

    // Update is called once per frame
    void Update()
    {

    }

    // UI Input
    // RESTART 버튼을 눌렀을 때 현재 씬 다시 시작
    public void OnClick_Restart()
    {
        SceneManager.LoadScene(sceneName);
    }

    // QUIT버튼을 누르면 스테이지 선택창으로 돌아가도록 만들기
    public void OnClick_Quit()
    {
        // 스테이지 선택 씬을 아직 만들지 않았으므로 주석처리
        // SceneManager.LoadScene("Menu");
    }

    // 게임을 클리어하면 Restart 버튼이 비활성화 되고
    // 다음 스테이지로 갈 수 있는 Next 버튼이 활성화 된다.
    public void OnClick_Next()
    {
        SceneManager.LoadScene(nextScene);
    }

}
