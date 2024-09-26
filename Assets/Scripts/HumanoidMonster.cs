using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidMonster : MonoBehaviour
{
    private enum State { Idle, Chase, Attack }

    // TODO: 몬스터 생성시 공유 데이터 참조만 복사
    private class Shared
    {
        public readonly YieldInstruction detectPeriod = new WaitForSeconds(1f);
        public readonly LayerMask playerLayer = LayerMask.GetMask("Player");
    }
    private Shared shared;

    [SerializeField] float detectRange = 10f;

    [SerializeField] // 확인용
    private State currentState = State.Idle;

    private Coroutine currentRoutine;
    private Collider[] detected = new Collider[4];

    private void Awake()
    {
        shared = new();
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
            if (0 < Physics.OverlapSphereNonAlloc(transform.position, detectRange, detected, shared.playerLayer))
            {
                Debug.Log("감지됨");
            }
            yield return shared.detectPeriod;
        }
    }
}
