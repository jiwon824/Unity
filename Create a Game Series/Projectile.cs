using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask; //어떤 오브젝트, 어떤 레이어가 발사체와 충돌할지 결정
    float speed = 10;
    float damage = 1;

    float lifeTime = 2;
    float skinWidth = 0.1f;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        // 발사체와 겹쳐있는 모든 충돌체들의 배열
        Collider[] initialcollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask); // (위치, 크기, 레이어마스크)
        // 총알이 생성됐을 때 어떤 충돌체 오브젝트와 이미 겹친(충돌한) 상태일 때
        if (initialcollisions.Length > 0)
        {
            OnHitObject(initialcollisions[0]);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit; //충돌 오브젝트에 대해서 반환한 정보

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        IDamaneable damaneableObject = hit.collider.GetComponent<IDamaneable>();
        if (damaneableObject != null)
        {
            damaneableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject); // 오브젝트에 충돌했을 때, 발사체를 제거   
    }
    void OnHitObject(Collider c)
    {
        IDamaneable damaneableObject = c.GetComponent<IDamaneable>();
        if (damaneableObject != null)
        {
            damaneableObject.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject); // 오브젝트에 충돌했을 때, 발사체를 제거   
    }
}
