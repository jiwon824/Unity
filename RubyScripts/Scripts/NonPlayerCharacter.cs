using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    //��ȭ������ ǥ�� �ð�(�� ����)�� ����
    public float displayTime = 4.0f;
    //GameObject ������ dialogBox ������ Canvas ���� ������Ʈ�� �����ϹǷ�, �̸� ��ũ��Ʈ���� Ȱ��ȭ �� ��Ȱ��ȭ�� �� �ִ�.
    public GameObject dialogBox;
    //timerDisplay ������ ��ȭ���ڰ� �� �� ���� ǥ�õǾ����� ����.
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
            //���� �ð��� 0���� �۾����� ��ȭ���� �����
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
