using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{

    public enum State
    {
        // 아무것도 하지 않는 상태의 Idle, 플레이어를 추격하는 상태인 Chasing, 그리고 공격하는 도중임을 나타내는 Attacking.
        Idle, Chasing, Attacking
    };
    State currentState; // 현재 상태를 지정해주는

    NavMeshAgent pathFinder; // 길찾기를 관리
    Transform target;
    LivingEntity targetEntity; // 플레이어가 죽었는지 확인하기 위해 이용
    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1; // 공격 사이에 타이머가 필요
    float damage = 1;

    float nextAttackTime; // 다음 공격 가능 시간
    float myCollisionRadius; // 적 캐릭터의 충돌범위
    float targetCollisionRadius; // 플레이어 캐릭터의 충돌범위

    bool hasTarget; // 쫓아갈 타겟이 있는지

    protected override void Start()
    {
        base.Start(); // 부모인 LivingEntity의 start를 호출
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        // 목표로 설정할, 플레이어 태그를 가진 게임 오브젝트의 존재 확인
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true; // 목표 있음

            target = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어 태그를 가진 오브젝트를 목표로 설정
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
    }

    void OnTargetDeath()
    {
        // 목표가 죽은 순간 쫓아갈 타겟이 없어짐
        hasTarget = false;
        currentState = State.Idle; // + 적이 할 일이 없어짐
    }

    void Update()
    {
        // 타겟이 있을 때만 공격하도록 변경
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; // 목표까지의 제곱거리
                // 거리가 공격 가능할 만큼 가까울 때
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    // 공격을 위한 코루틴
    IEnumerator Attack()
    {
        // 공격 상태로 변경+추적 중지
        currentState = State.Attacking;
        pathFinder.enabled = false;

        Vector3 originalPosition = transform.position; // 현재 위치 지정
        Vector3 dirToTarget = (target.position - transform.position).normalized; // 목표로의 방향 벡터
        Vector3 attackPositon = target.position - dirToTarget * (myCollisionRadius); // 공격 목표 지점

        float attackSpeed = 3; // 값이 높을수록 공격 애니메이션이 빨라짐
        float percent = 0; // 찌르기 애니메이션이 얼만큼 멀리 갈 것인지 지정. 0~1의 값을 가짐

        skinMaterial.color = Color.red; // 공격할 때 빨간색으로 변하도록 설정
        bool hasAppliedDamage = false; // 플레이어에게 데미지를 주었는가

        while (percent <= 1)
        {
            if (percent >=0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true; // 데미지를 가하는 중으로 변경
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; // 보간 값, 대칭함수를 이용함
            transform.position = Vector3.Lerp(originalPosition, attackPositon, interpolation);

            yield return null;
        }

        // 공격이 끝나면 다시 추적 상태로 바꿈 + 추적 재개 + 원래 색으로 되돌리기
        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }

    // 플레이어를 추적하는 코루틴
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f; // 길을 찾는 갱신 주기 1/4초로 설정

        while (hasTarget) // 타겟이 존재하는 동안 반복
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized; // 목표로의 방향 벡터
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
