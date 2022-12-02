using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickBlock : MonoBehaviour
{
    public float length = 0.0f; // 자동 낙하 탐지 거리
    public bool isDelete = false; // 낙하 후 제거할지 여부

    bool isFell = false; // 낙하 플래그
    float fadeTime = 0.5f; // 페이드 아웃 시간

    // Start is called before the first frame update
    void Start()
    {
        //Rigidbody2D 물리 시뮬레이션 정지
        Rigidbody2D rbody = GetComponent<Rigidbody2D>();
        rbody.bodyType = RigidbodyType2D.Static;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 플레이어 찾기
        if (player != null)
        {
            //플레이어와의 거리 계산
            float d = Vector2.Distance(transform.position, player.transform.position);
            if (length >= d)
            {
                Rigidbody2D rbody = GetComponent<Rigidbody2D>();
                if (rbody.bodyType == RigidbodyType2D.Static)
                {
                    // Rigidbody2D 물리 시뮬레이션 시작
                    rbody.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }
        if (isFell)
        {
            // 낙하
            // 투명도를 변경해 페이드아웃 효과
            fadeTime -= Time.deltaTime; // 이전 프레임과의 차이만큼 시간 차감
            Color col = GetComponent<SpriteRenderer>().color; // 컬러 값 가져오기
            col.a = fadeTime; // 투명도 변경
            GetComponent<SpriteRenderer>().color = col; // 컬러값 재설정
            if (fadeTime <= 0.0f)
            {
                //0보다 작으면 (투명) 제거
                Destroy(gameObject);
            }

        }
    }
    // 접촉 시작
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDelete)
        {
            isFell = true; // 낙하 플래그 true
        }
    }
}
