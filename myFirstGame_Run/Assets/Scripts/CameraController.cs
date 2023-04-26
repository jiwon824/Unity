using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;

    // 카메라 초기 위치 설정용 변수 
    float CameraZ = 6.0f; // 플레이어와 카메라 사이의 Z거리 (일정 거리 뒤에서 따라옴)
    float CameraY = 3.5f; // 플레이어와 카메라 사이의 Y거리 (일정한 높이 위에서 따라옴)

    float lerpTime = 0.6f;
    float currentTime = 0;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        // 카메라의 초기 위치 설정 
        // 카메라의 시작 좌표를 플레이어의 x값, 플레이어와 일정 높이를 유지, 플레이어의 일정 거리 뒤(-PlayerBtwCamera)로 
        transform.position = new Vector3(target.position.x, target.position.y+CameraY, target.position.z - CameraZ);

        // 초기에 설정한 플레이어와 카메라의 거리를 유지하면서 따라가야 하
        offset = transform.position - target.position;
        
    }

    void LateUpdate()
    {
        // player가 obstacle에 맞아서 파괴가 되면 target은 null이 된다.
        if (target != null)
        {
            // currentTime은 시간 흐름에 따라 증가함
            currentTime += Time.deltaTime;
            // 이 코드를 넣어주면 0부터 증가하다가 최대 0.5(lerpTime)까지만 증가함
            if (currentTime >= lerpTime) currentTime = lerpTime;
            float t = currentTime / lerpTime; 

            // 플레이어 이동에 따라 변하는 카메라 위치
            Vector3 newPosition = new Vector3(target.position.x, target.position.y + offset.y, target.position.z+offset.z);
                // (현재 카메라의 위치, 이동할 위치)의 보간값으로 설정 
            transform.position = Vector3.Lerp(transform.position, newPosition, t);
        }
    }

}