using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GetComponent<PlayerController>()는 PlayerController와 Player 스크립트가 같은 오브젝트에 붙어 있는걸 전제로 한다.
// [RequireComponent(typeof(PlayerController))] 코드를 적어줘서 이 부분이 에러를 내지 않게 됨.
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    protected bool dead;

    // moveVelocity를 다른 스크립트인 PlayerController로 전달해서 물리적인 부분들을 처리할 수 있도록 할 것
    // 그래서 PlayerController에 대한 레퍼런스를 가져와야합니다.
    PlayerController controller;

    void Start()
    {

    }

    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!dead)
        {
            // Movement input 플레이어의 움직임을 입력 받는 것
            // 수평(Horizontal) 과 수직(Vertical) 방향에 대한 입력을 받고 싶으니
            // Input. 을 적고 GetAxis()를 호출해서 "Horizontal"을 넣습니다.
            // y값은 내버려 둬도 되며(0), 일정한 속도로 앞으로 가도록 moveSpeed를 넣어줬다.
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed, 0, moveSpeed);
            // 입력 받은 값을 방향으로 변환하고 움직임 속도를 곱할 겁니다.
            // 입력의 방향을 얻기 위해 moveInput를 정규화(nomalized)하여 가져옵니다 (nomarlized는 방향을 가리키는 단위벡터로 만드는 연산)
            // 거기에 moveSpeed를 곱해줍니다
            Vector3 moveVelocity = moveInput.normalized * moveSpeed;
            // controller에게 velocity값을 넘겨줌
            controller.Move(moveVelocity);
        }
    }

    public void Die()
    {
        dead = true;
        Time.timeScale = 0;
    }

}