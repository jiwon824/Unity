using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public float moveX = 0.0f; // X 이동 거리
    public float moveY = 0.0f; // Y 이동 거리
    public float times = 0.0f; // 시간
    public float weight = 0.0f; // 정지 시간
    public bool isMoveWhenOn = false; // 올라갔을 때 움직이기

    public bool isCanMove = true; // 움직임
    float perDX; // 1프레임 당 X 이동 값
    float perDY; // 1프레임 당 Y 이동 값
    Vector3 defPos; // 초기 위치
    bool isReverse = false; // 반전 여부

    // Start is called before the first frame update
    void Start()
    {
        // 초기 위치
        defPos = transform.position;
        //1 프레임에 이동하는 시간
        float timestep = Time.fixedDeltaTime;
        // 1프레임의 x 이동값
        perDX = moveX / (1.0f / timestep * times);
        // 1프레임의 y 이동값
        perDY = moveY / (1.0f / timestep * times);

        if (isMoveWhenOn)
        {
            // 처음엔 움직이지 않고 올라가면 움직이기 시작
            isCanMove = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isCanMove)
        {
            //이동 중
            float x = transform.position.x;
            float y = transform.position.y;
            bool endX = false;
            bool endY = false;
            if (isReverse)
            {
                //반대 방향 이동
                //이동량이 양수고 이동 위치가 초기 위치보다 작거나
                //이동량이 음수고 이동 위치가 초기 위치보다 큰 경우
                if ((perDX>=0.0f && x<=defPos.x)|| (perDX < 0.0f && x >= defPos.x))
                {
                    //이동량이 +
                    endX = true; //x방향 이동 종료
                }
                if ((perDY >= 0.0f && y <= defPos.y) || (perDY < 0.0f && y >= defPos.y))
                {
                    endY = true; //y방향 이동 종료
                }
                // 블록 이동
                transform.Translate(new Vector3(-perDX, -perDY, defPos.z));
            }
            else
            {
                // 정방향 이동
                // 이동량이 양수고 이동 위치가 초기 위치보다 크거나
                // 이동량이 음수고 이동 위치가 초기 + 이동거리보다 작은 경우
                if ((perDX >= 0.0f && x >= defPos.x) || (perDX < 0.0f && x <= defPos.x + moveX))
                {
                    //이동량이 +
                    endX = true; //x방향 이동 종료
                }
                if ((perDY >= 0.0f && y >= defPos.y) || (perDY < 0.0f && y <= defPos.y + moveY))
                {
                    endY = true; //y방향 이동 종료
                }
                // 블록 이동
                Vector3 v = new Vector3(perDX, perDY, defPos.z);
                transform.Translate(v);
            }

            if (endX && endY)
            {
                // 이동 종료
                if (isReverse)
                {
                    // 위치가 어긋나는 것을 방지하고자 정면 방향 이동으로 돌아가기 전에 초기 위치로 돌리기
                    transform.position = defPos;
                }
                isReverse = !isReverse; // 값을 반전시키기
                isCanMove = false; // 이동 가능 값을 false
                
                if(isMoveWhenOn == false)
                {
                    //올라갔을 때 움직이는 값이 꺼진 경우
                    Invoke("Move", weight); // weight 만큼 지연 후 다시 이동
                }
            }
        }
    }
    // 이동하게 만들기
    public void Move()
    {
        isCanMove = true;
    }
    // 이동하지 못하게 만들기
    public void Stop()
    {
        isCanMove = false;
    }

    // 접촉 시작
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //접촉한 것이 플레이어라면 이동블록의 자식으로 만들기
            collision.transform.SetParent(transform);
            if (isMoveWhenOn)
            {
                //올라갔을 때 움직이는 경우라면
                isCanMove = true; // 이동하게 만들기
            }
        }
    }
    // 접촉 종료
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player"){
            // 접촉한 것이 플레이어라면 이동 블록의 자식에서 제외시키기
            collision.transform.SetParent(null);
        }
    }
}
