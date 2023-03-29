using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;

    float lerpTime = 0.6f;
    float currentTime = 0;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
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
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, offset.z + target.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, t);
        }
    }

}