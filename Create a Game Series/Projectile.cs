using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask; //� ������Ʈ, � ���̾ �߻�ü�� �浹���� ����
    float speed = 10;
    float damage = 1;

    float lifeTime = 2;
    float skinWidth = 0.1f;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        // �߻�ü�� �����ִ� ��� �浹ü���� �迭
        Collider[] initialcollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask); // (��ġ, ũ��, ���̾��ũ)
        // �Ѿ��� �������� �� � �浹ü ������Ʈ�� �̹� ��ģ(�浹��) ������ ��
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
        RaycastHit hit; //�浹 ������Ʈ�� ���ؼ� ��ȯ�� ����

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
        GameObject.Destroy(gameObject); // ������Ʈ�� �浹���� ��, �߻�ü�� ����   
    }
    void OnHitObject(Collider c)
    {
        IDamaneable damaneableObject = c.GetComponent<IDamaneable>();
        if (damaneableObject != null)
        {
            damaneableObject.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject); // ������Ʈ�� �浹���� ��, �߻�ü�� ����   
    }
}
