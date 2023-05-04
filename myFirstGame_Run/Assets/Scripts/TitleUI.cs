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
    // FullScreenMode screenMode; 
    // 창모드 여부
    bool isWindow;

    public Toggle windowModeToggle; // 창모드 토글 
    // options에 값을 넣어주려면 Dropdown 변수를 선언해줘야 한다.
    public Dropdown resolutionDropdown;
    // SetScreenResolution(int x)에서 x
    int resolutionNum;
    // 지원하는 해상도를 넣어줄 리스트 생성 
    List<Resolution> resolutions = new List<Resolution>();

    // 볼륨 슬라이더 
    public Slider[] volumeSliders;
    // 음소거 토글
    public Toggle[] muteToggles;


    // 게임 스타트를 누르면 스테이지 선택 씬으로 넘어감
    // 스테이지 선택씬의 이름을 담는 변수 
    public string selectStage;


    void Awake()
    {
        // 해상도 드롭다운 옵션 초기
        InitResolutionDropdown();
        // 해상도 설정 불러오기 
        resolutionNum = PlayerPrefs.GetInt("screen res index");

        // 창모드 설정 불러오SetScreenResolution
        isWindow = (PlayerPrefs.GetInt("WindowMode") == 1) ? true : false;
        windowModeToggle.isOn = isWindow;

    }

    void Start()
    {
        // 옵션 패널숨기기
        title.SetActive(true);
        options.SetActive(false);

        // 볼륨 설정 불러오기
        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        // 뮤트 여부 불러오기 
        muteToggles[0].isOn = AudioManager.instance.isMuteMaster;
        muteToggles[1].isOn = AudioManager.instance.isMuteMusic;
        muteToggles[2].isOn = AudioManager.instance.isMuteSfx;
    }

    // 해상도 드롭다운의 값을 모니터 지원 해상도 값 목록으로 초기화해주는 함수 
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
            if (item.width == Screen.width && item.height == Screen.height)
            {
                resolutionDropdown.value = optionNum;
            }
            optionNum++;
        }

        // Dropdown의 options가 변경되었으니 새로고침 함수를 불러준다.
        resolutionDropdown.RefreshShownValue();

    }



    // TitleUI Input
    // START 버튼을 누르면 stage 선택창으로 이동 
    public void OnClick_Start()
    {
        // 버튼 클릭음
        AudioManager.instance.PlaySound2D("Button Click");

        SceneManager.LoadScene(selectStage);
    }

    public void OnClick_Options()
    {
        // 버튼 클릭음
        AudioManager.instance.PlaySound2D("Button Click");
        // Options 패널 활성화, title 패널 끄기
        options.SetActive(true);
        title.SetActive(false);
    }

    // Title에서 QUIT버튼을 누르면 게임 종료 
    public void OnClick_Quit()
    {
        // 버튼 클릭음
        AudioManager.instance.PlaySound2D("Button Click");
        Application.Quit();
    }



    //OptionUI Input
    // Dropbox의 value값을 이용하여 선택된 해상도를 적용할 수 있게 하는 메서드
    public void SetScreenResolution(int x)
    {
        // select index x at Resolutoin Dropdown
        // parm:
        
        resolutionNum = x;

        // Screen.SetResolution(너비, 높이, 전체화면, 화면재생빈도)
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height,
            false);

        // 해상도랑 창모드를 PlayerPrefs에 저장 
        PlayerPrefs.SetInt("screen rex index", resolutionNum);
        PlayerPrefs.Save();


    }

    /// <summary>
    /// if check to "창모드" Toggle change ScreenMode
    /// </summary>
    /// <param name="isWindowOn">Checked toggle means Windowmode</param>
    public void SetFullscreen(bool isWindowOn)
    {
        // 버튼이 클릭되면 isWindow 값을 변경
        isWindow = isWindowOn;

        // fullscreen이면 드롭다운으로 해상도 설정이 안 되도록 비활성화
        // isWindow(창모드)가 참이면 interactable = True(상호작용 가능)
        resolutionDropdown.interactable = isWindow;

        // 참이면 창모드 / 거짓이면 전체화면
        // screenMode = isWindow ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;

        // 창모드면 선택한 해상도+창모드로 설정
        if (isWindow)
        {
            SetScreenResolution(resolutionNum);
            // Screen.SetResolution(너비, 높이, 전체화면, 화면재생빈도)
            //Screen.SetResolution(resolutions[resolutionNum].width,
            //resolutions[resolutionNum].height,
            //false);
        }
        // 전체화면이면 제일 큰 해상도+전체화면true
        else
        {
            // Screen.SetResolution(너비, 높이, 전체화면, 화면재생빈도)
            Screen.SetResolution(resolutions[resolutions.Count - 1].width,
            resolutions[resolutions.Count - 1].height,
            true);
            // 드롭다운에 표시되는 값도 최대 해상도 값으로 변경 
            resolutionDropdown.value = resolutions.Count - 1;
        }

        // isWindow가 True면 창모드 1저장 거짓이면 0저장 
        PlayerPrefs.SetInt("WindowMode", ((isWindow) ? 1 : 0));
        PlayerPrefs.Save();
    }

    // 볼륨 관련 설
    // 볼륨 조절 슬라이더 
    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

    // 음소거 토글
    public void MuteMaster(bool isMute)
    {
        AudioManager.instance.SetMute(isMute, AudioManager.AudioChannel.Master);
        if (isMute) volumeSliders[0].value = 0;
        //else volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[0].interactable = !isMute;
    }

    public void MuteMusic (bool isMute)
    {
        AudioManager.instance.SetMute(isMute, AudioManager.AudioChannel.Music);
        if (isMute) volumeSliders[1].value = 0;
        //else volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[1].interactable = !isMute;
    }

    public void MuteSfx (bool isMute)
    {
        AudioManager.instance.SetMute(isMute, AudioManager.AudioChannel.Sfx);
        if (isMute) volumeSliders[2].value = 0;
        //else volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;
        volumeSliders[2].interactable = !isMute;
    }

    // 오른쪽 아래 확인 버튼
    public void OnClick_Agree()
    {
        // 버튼 클릭음
        AudioManager.instance.PlaySound2D("Button Click");

        // 옵션 패널 끄고 타이틀 패널 열기 
        options.SetActive(false);
        title.SetActive(true);
    }


}