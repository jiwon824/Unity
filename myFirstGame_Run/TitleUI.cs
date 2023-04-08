using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    // +++ 타이틀 / 옵션 패널 오브젝트 변수 +++
    public GameObject title;
    public GameObject options;

    // +++ 해상도 관련 변수 +++
    // FullScreenMode 열거형
    FullScreenMode screenMode; 
    // 창모드 여부
    bool isWindow;

    public Toggle windowModeToggle; // 창모드 토글 
    // options에 값을 넣어주려면 Dropdown 변수를 선언해줘야 한다.
    public Dropdown resolutionDropdown;
    // DropboxOptionChange(int x)에서 x
    int resolutionNum;
    // 지원하는 해상도를 넣어줄 리스트 생성 
    List<Resolution> resolutions = new List<Resolution>();


    // 게임 스타트를 누르면 스테이지 선택 씬으로 넘어감
    // 스테이지 선택씬의 이름을 담는 변수 
    public string selectStage;


    void Awake()
    {
        // PlayerPrefs.SetInt("screen rex index", resolutionNum);
        resolutionNum = PlayerPrefs.GetInt("screen res index");
        // PlayerPrefs.SetInt("WindowMode", ((isWindow) ? 1 : 0));
        isWindow = (PlayerPrefs.GetInt("WindowMode") == 1) ? true : false;

        windowModeToggle.isOn = isWindow;
    }

    void Start()
    {
        // 옵션 패널숨기기
        options.SetActive(false);
        InitResolutionDropdown();
    }

    // 해상도 드롭다운의 값을 모니터 지원 해상도 값 목록으로 초기화해주는 함/
    void InitResolutionDropdown()
    {

        // Dropdown에 있는 기존의 옵션을 전부 제거
        // options는 리스트이기 때문에 리스트의 Clear()함수를 사용하여 전부 제거할 수 있다.
        resolutionDropdown.options.Clear();

        // 모니터가 지원하는 해상도는 Screen.resolutions라는 배열에 들어있다.
            // AddRange함수를 이용해서 지원하는 함수를 전부 리스트에 넣을 수 있다.
            // resolutions.AddRange(Screen.resolutions);
        // 화면재생빈도가 60인 것만 리스트에 넣어주기
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRate == 60)
                resolutions.Add(Screen.resolutions[i]);
        }
        
        int optionNum = 0;

        // 위에서 지원하는 해상도를 resolutions 리스트에 넣었다 
        // 리스트의 길이 만큼 반복하면서 option 추가
        foreach (Resolution item in resolutions)
        {
            // options list의 형식은 OptionData 클래스로 되어 있다 -> OptionData 객체를 생성
            Dropdown.OptionData optionData = new Dropdown.OptionData();
            // OptionData의 text 변수에 해상도 값을 넣어준다.
            optionData.text = item.width + "x" + item.height + " " + item.refreshRate + "hz";
            // options list에 추가
            resolutionDropdown.options.Add(optionData);

            // 처음 실행하면 Dropdown에 선택된 값이 초기화되어 있지 않으니
            // 현재 해상도의 값과 해상도 목록을 비교해서 Dropdown에 value값을 변경
            if (item.width ==Screen.width && item.height == Screen.height)
            {
                resolutionDropdown.value = optionNum;
            }
            optionNum++;

        }
        // Dropdown의 options가 변경되었으니 새로고침 함수를 불러준다.
        resolutionDropdown.RefreshShownValue();

        // 현재 전체화면인지 아닌지 확인 후 토글 버튼의 체크 버튼을 초기화
        // 창모드라면 창모드 토글에 체크
        // windowModeToggle.isOn = Screen.fullScreenMode.Equals(FullScreenMode.Windowed) ? true : false;
    }

    // Dropbox의 value값을 이용하여 현재 선택된 해상도를 적용할 수 있게 하는 메서드
    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }



    // TitleUI Input
    // START 버튼을 누르면 stage 선택창으로 이동 
    public void OnClick_Start()
    {
        SceneManager.LoadScene(selectStage);
    }

    public void OnClick_Options()
    {
        // Options 패널 활성화, title 패널 끄기 
        options.SetActive(true);
        title.SetActive(false);
    }

    // Title에서 QUIT버튼을 누르면 게임 종료 
    public void OnClick_Quit()
    {
        Application.Quit();
    }



    //OptionUI Input
    // 창모드 토글 버튼 
    public void WindowModeBtn(bool isWindowOn)
    {
        // 버튼이 클릭되면 isWindow 변수를 변경
        isWindow = isWindowOn;

        // 참이면 창모드 / 거짓이면 전체화면
        screenMode = isWindow ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;

    }

    // 오른쪽 아래 확인 버튼
    public void OnClick_Agree()
    {
        // Screen.SetResolution(너비, 높이, 전체화면, 화면재생빈도)
        // 전체화면모드는 참 거짓만 넘겨줘도 되지만, 여기서는 FullScreenMode 열거형을 사용했다.
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height,
            screenMode);

        // 해상도랑 창모드를 PlayerPrefs에 저장 
        PlayerPrefs.SetInt("screen rex index", resolutionNum);
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("WindowMode", ((isWindow) ? 1 : 0));
        PlayerPrefs.Save();

        // 옵션 패널 끄고 타이틀 패널 열기 
        options.SetActive(false);
        title.SetActive(true);
    }


}
