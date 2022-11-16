using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    //대화상자의 표시 시간(초 단위)을 저장
    public float displayTime = 4.0f;
    //GameObject 유형의 dialogBox 변수는 Canvas 게임 오브젝트를 저장하므로, 이를 스크립트에서 활성화 및 비활성화할 수 있다.
    public GameObject dialogBox;
    //timerDisplay 변수는 대화상자가 몇 초 동안 표시되었는지 저장.
    float timerDisplay;

    void Start()
    {
        dialogBox.SetActive(false);
        timerDisplay = -1.0f;
    }

    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            //남은 시간이 0보다 작아지면 대화상자 숨기기
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
            }
        }
    }

    public void DisplayDialog()
    {
        timerDisplay = displayTime;
        dialogBox.SetActive(true);
    }

}
