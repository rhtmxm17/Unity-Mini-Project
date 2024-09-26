using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class HumanoidMonster : MonoBehaviour
{
    private enum State { Idle, Chase, Attack }

    // TODO: 몬스터 생성시 공유 데이터 참조만 복사
    private class Shared
    {
        public readonly YieldInstruction detectPeriod = new WaitForSeconds(1f);
        public readonly YieldInstruction checkAttackPeriod = new WaitForSeconds(1f);
        public readonly LayerMask playerLayerMask = LayerMask.GetMask("Player");
        public readonly LayerMask hitableLayerMask = LayerMask.GetMask("Player", "Default");
        public readonly int playerLayerIndex = LayerMask.NameToLayer("Player");
    }
    private Shared shared;

    [SerializeField] SampleSkill sampleProjectile;
    [SerializeField] float detectRange = 10f;
    [SerializeField] float sqrChaseRange = 12f * 12f;
    [SerializeField] float attackRange = 5f;

    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField] // 확인용
    private State currentState = State.Idle;

    private Coroutine currentRoutine;
    private Collider[] detected = new Collider[1];

    private void Awake()
    {
        shared = new();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentRoutine = StartCoroutine(DetectPlayerRoutine());
    }

    private void OnDrawGizmosSelected()
    {
        switch(currentState)
        {
            case State.Idle:
                Gizmos.DrawWireSphere(transform.position, detectRange);
                break;
        }
    }

    private IEnumerator DetectPlayerRoutine()
    {
        while (true)
        {
            if (0 < Physics.OverlapSphereNonAlloc(transform.position, detectRange, detected, shared.playerLayerMask))
            {
                currentState = State.Chase;
                animator.SetTrigger("Walk");
                StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(ChasePlayerRoutine());
                agent.destination = detected[0].transform.position;
            }

            yield return shared.detectPeriod;
        }
    }

    private IEnumerator ChasePlayerRoutine()
    {
        while (true)
        {
            Vector3 distanceVector = detected[0].transform.position - transform.position;
            Ray attackRay = new(transform.position, distanceVector);

            // 추적 한계 거리
            if (distanceVector.sqrMagnitude > sqrChaseRange)
            {
                currentState = State.Idle;
                animator.SetTrigger("Idle");
                StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(DetectPlayerRoutine());
                agent.destination = transform.position;

                yield return null;
            }

            // 공격 발사 가능여부 판단
            bool hitted = Physics.Raycast(attackRay, out RaycastHit info, attackRange, shared.hitableLayerMask);
            Debug.DrawRay(attackRay.origin, attackRay.direction * attackRange, Color.yellow, 0.5f);

            if (hitted && info.collider.gameObject.layer == shared.playerLayerIndex)
            {
                currentState = State.Attack;
                animator.SetTrigger("Attack");
                StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(AttackRoutine());
                agent.destination = transform.position;
            }
            else
            {
                agent.destination = detected[0].transform.position;
            }

            yield return shared.checkAttackPeriod;
        }
    }

    private IEnumerator AttackRoutine()
    {
        // 몬스터도 스킬 형태로 부착하기?

        yield return new WaitForSeconds(1f);

        var projectile = Instantiate(sampleProjectile, transform.position, transform.rotation);
        projectile.Init();

        currentState = State.Chase;
        animator.SetTrigger("Walk");
        StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ChasePlayerRoutine());
    }
}
