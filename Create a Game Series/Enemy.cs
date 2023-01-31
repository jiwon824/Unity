using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{

    public enum State
    {
        // �ƹ��͵� ���� �ʴ� ������ Idle, �÷��̾ �߰��ϴ� ������ Chasing, �׸��� �����ϴ� �������� ��Ÿ���� Attacking.
        Idle, Chasing, Attacking
    };
    State currentState; // ���� ���¸� �������ִ�

    NavMeshAgent pathFinder; // ��ã�⸦ ����
    Transform target;
    LivingEntity targetEntity; // �÷��̾ �׾����� Ȯ���ϱ� ���� �̿�
    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1; // ���� ���̿� Ÿ�̸Ӱ� �ʿ�
    float damage = 1;

    float nextAttackTime; // ���� ���� ���� �ð�
    float myCollisionRadius; // �� ĳ������ �浹����
    float targetCollisionRadius; // �÷��̾� ĳ������ �浹����

    bool hasTarget; // �Ѿư� Ÿ���� �ִ���

    protected override void Start()
    {
        base.Start(); // �θ��� LivingEntity�� start�� ȣ��
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        // ��ǥ�� ������, �÷��̾� �±׸� ���� ���� ������Ʈ�� ���� Ȯ��
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true; // ��ǥ ����

            target = GameObject.FindGameObjectWithTag("Player").transform; // �÷��̾� �±׸� ���� ������Ʈ�� ��ǥ�� ����
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
    }

    void OnTargetDeath()
    {
        // ��ǥ�� ���� ���� �Ѿư� Ÿ���� ������
        hasTarget = false;
        currentState = State.Idle; // + ���� �� ���� ������
    }

    void Update()
    {
        // Ÿ���� ���� ���� �����ϵ��� ����
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; // ��ǥ������ �����Ÿ�
                // �Ÿ��� ���� ������ ��ŭ ����� ��
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    // ������ ���� �ڷ�ƾ
    IEnumerator Attack()
    {
        // ���� ���·� ����+���� ����
        currentState = State.Attacking;
        pathFinder.enabled = false;

        Vector3 originalPosition = transform.position; // ���� ��ġ ����
        Vector3 dirToTarget = (target.position - transform.position).normalized; // ��ǥ���� ���� ����
        Vector3 attackPositon = target.position - dirToTarget * (myCollisionRadius); // ���� ��ǥ ����

        float attackSpeed = 3; // ���� �������� ���� �ִϸ��̼��� ������
        float percent = 0; // ��� �ִϸ��̼��� ��ŭ �ָ� �� ������ ����. 0~1�� ���� ����

        skinMaterial.color = Color.red; // ������ �� ���������� ���ϵ��� ����
        bool hasAppliedDamage = false; // �÷��̾�� �������� �־��°�

        while (percent <= 1)
        {
            if (percent >=0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true; // �������� ���ϴ� ������ ����
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; // ���� ��, ��Ī�Լ��� �̿���
            transform.position = Vector3.Lerp(originalPosition, attackPositon, interpolation);

            yield return null;
        }

        // ������ ������ �ٽ� ���� ���·� �ٲ� + ���� �簳 + ���� ������ �ǵ�����
        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }

    // �÷��̾ �����ϴ� �ڷ�ƾ
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f; // ���� ã�� ���� �ֱ� 1/4�ʷ� ����

        while (hasTarget) // Ÿ���� �����ϴ� ���� �ݺ�
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized; // ��ǥ���� ���� ����
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
