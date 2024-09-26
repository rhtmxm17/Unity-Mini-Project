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

    [SerializeField] float detectRange = 10f;
    [SerializeField] float attackRange = 5f;

    private NavMeshAgent agent;

    [SerializeField] // 확인용
    private State currentState = State.Idle;

    private Coroutine currentRoutine;
    private Collider[] detected = new Collider[4];

    private void Awake()
    {
        shared = new();
        agent = GetComponent<NavMeshAgent>();
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
            // 공격 발사 가능여부 판단
            bool hitted = Physics.Raycast(transform.position, detected[0].transform.position - transform.position, out RaycastHit info, attackRange, shared.hitableLayerMask);

            if (hitted && info.collider.gameObject.layer == shared.playerLayerIndex)
            {
                Debug.Log("공격");
            }
            else
            {
                agent.destination = detected[0].transform.position;
            }

            yield return shared.checkAttackPeriod;
        }
    }
}
