using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        // 카메라위치-플레이어의 위치
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // 카메라의 x, y는 처음 위치 그대로 두고, 
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, offset.z + target.position.z);
        transform.position = newPosition;
    }
}