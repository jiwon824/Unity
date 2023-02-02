using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;

    Camera viewCamera;
    // moveVelocity를 다른 스크립트인 PlayerController로 전달해서 물리적인 부분들을 처리할 수 있도록 할 것
    // 그래서 PlayerController에 대한 레퍼런스를 가져와야합니다.
    PlayerController controller;
    GunController gunController;

    protected override void Start()
    {
        base.Start(); // 부모인 LivingEntity의 start를 호출

        // GetComponent<PlayerController>()는 PlayerController와 Player 스크립트가 같은 오브젝트에 붙어 있는걸 전제로 한다.  
        // [RequireComponent(typeof(PlayerController))] 코드를 위에 적어줘서 이 부분이 에러를 내지 않게 됨.
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        // +++ 플레이어 이동을 입력받는 코드 +++
        // 플레이어의 움직임을 입력 받는 것
        // 수평(Horizontal) 과 수직(Vertical) 방향에 대한 입력을 받고 싶으니
        // Input. 을 적고 GetAxis()를 호출해서 "Horizontal"을 넣습니다.
        // y값은 내버려 둬도 되며(0), 이어서 Input.GetAxis("Vertical")을 호출
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        // 입력 받은 값을 방향으로 변환하고 움직임 속도를 곱할 겁니다.
        // 입력의 방향을 얻기 위해 moveInput를 정규화(nomalized)하여 가져옵니다 (nomarlized는 방향을 가리키는 단위벡터로 만드는 연산)
        // 거기에 moveSpeed를 곱해줍니다
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        // +++ 플레이어가 바라보는 방향을 입력받는 코드 +++
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        // ray가 바닥 plane과 교차하면 true
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
        }

        // +++ 무기 입력을 받는 코드 +++
        if (Input.GetMouseButton(0)) // GetMouseButton(0)은 좌클릭을 의미한다
        {
            gunController.Shoot();
        }
    }
}
