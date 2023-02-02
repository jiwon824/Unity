using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;

    Camera viewCamera;
    // moveVelocity�� �ٸ� ��ũ��Ʈ�� PlayerController�� �����ؼ� �������� �κе��� ó���� �� �ֵ��� �� ��
    // �׷��� PlayerController�� ���� ���۷����� �����;��մϴ�.
    PlayerController controller;
    GunController gunController;

    protected override void Start()
    {
        base.Start(); // �θ��� LivingEntity�� start�� ȣ��

        // GetComponent<PlayerController>()�� PlayerController�� Player ��ũ��Ʈ�� ���� ������Ʈ�� �پ� �ִ°� ������ �Ѵ�.  
        // [RequireComponent(typeof(PlayerController))] �ڵ带 ���� �����༭ �� �κ��� ������ ���� �ʰ� ��.
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        // +++ �÷��̾� �̵��� �Է¹޴� �ڵ� +++
        // �÷��̾��� �������� �Է� �޴� ��
        // ����(Horizontal) �� ����(Vertical) ���⿡ ���� �Է��� �ް� ������
        // Input. �� ���� GetAxis()�� ȣ���ؼ� "Horizontal"�� �ֽ��ϴ�.
        // y���� ������ �ֵ� �Ǹ�(0), �̾ Input.GetAxis("Vertical")�� ȣ��
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        // �Է� ���� ���� �������� ��ȯ�ϰ� ������ �ӵ��� ���� �̴ϴ�.
        // �Է��� ������ ��� ���� moveInput�� ����ȭ(nomalized)�Ͽ� �����ɴϴ� (nomarlized�� ������ ����Ű�� �������ͷ� ����� ����)
        // �ű⿡ moveSpeed�� �����ݴϴ�
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        // +++ �÷��̾ �ٶ󺸴� ������ �Է¹޴� �ڵ� +++
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        // ray�� �ٴ� plane�� �����ϸ� true
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
        }

        // +++ ���� �Է��� �޴� �ڵ� +++
        if (Input.GetMouseButton(0)) // GetMouseButton(0)�� ��Ŭ���� �ǹ��Ѵ�
        {
            gunController.Shoot();
        }
    }
}
