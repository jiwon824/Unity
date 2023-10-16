using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [field:Header("References")]
    [field: SerializeField] public PlayerSO data { get; private set; }

    [field : Header("Collisions")]
    [field: SerializeField] public CapsuleColliderUtility colliderUtility { get; private set; }
    [field: SerializeField] public PlayerLayerData layerData { get; private set; }

    public Rigidbody rb { get; private set; }
    public PlayerInput input { get; private set; }
    private PlayerMovementStateMachine movementStateMachine;

    public Transform mainCameraTransform { get; private set; }

    private void Awake()
    {
        movementStateMachine = new PlayerMovementStateMachine(this);
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        colliderUtility.Initialize(gameObject);
        colliderUtility.CalculateCapsuleColliderDimensions();

        // 시네머신 카메라가 아닌 메인 카메라를 가져오는 이유는 시네머신에서 메인 카메라를 제어하므로 거기서 데이터를 가져와도 안전하기 때문입니다.
        mainCameraTransform = Camera.main.transform;
    }
    void OnValidate()
    {
        colliderUtility.Initialize(gameObject);
        colliderUtility.CalculateCapsuleColliderDimensions();
    }

    private void Start()
    {
        movementStateMachine.ChangeState(movementStateMachine.idlingState);   
    }

    private void Update()
    {
        movementStateMachine.HandleInput();
        movementStateMachine.Update();
	}
    private void FixedUpdate()
    {
        movementStateMachine.PhysicsUpdate();
    }

}
