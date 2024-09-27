using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class HumanoidMonster : MonoBehaviour, IDamageable, IUnit
{
    public event UnityAction OnDie;
    public float Hp
    {
        get => hp;
        private set
        {
            hp = value;
            if (hp <= 0f)
                OnDie?.Invoke();
        }
    }



    private enum State { Idle, Chase, Attack, Die }

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

    [SerializeField] float hp = 10f;

    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField] // 확인용
    private State currentState = State.Idle;

    private Coroutine currentRoutine;
    private Collider[] detected = new Collider[1];

    #region IDamageable
    public IDamageable.Flag HitFlag => IDamageable.Flag.Monster;
    public void TakeDamage(float damage)
    {
        Hp -= damage;
    }
    #endregion IDamageable

    private void Awake()
    {
        shared = new();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        OnDie = Die;
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


            if (AttackCheck())
            {
                currentState = State.Attack;
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
        do
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(1f);

            var projectile = Instantiate(sampleProjectile, transform.position, transform.rotation);
            projectile.Init();
            projectile.hitMask = IDamageable.Flag.Wall | IDamageable.Flag.Player;
        } while (AttackCheck());

        currentState = State.Chase;
        animator.SetTrigger("Walk");
        StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ChasePlayerRoutine());
    }

    private bool AttackCheck()
    {
        Vector3 distanceVector = detected[0].transform.position - transform.position;
        Ray attackRay = new(transform.position, distanceVector);

        // 공격 발사 가능여부 판단
        bool hitted = Physics.Raycast(attackRay, out RaycastHit info, attackRange, shared.hitableLayerMask);
        Debug.DrawRay(attackRay.origin, attackRay.direction * attackRange, Color.yellow, 0.5f);

        return (hitted && info.collider.gameObject.layer == shared.playerLayerIndex);

    }

    private void Die()
    {
        currentState = State.Die;
        animator.SetTrigger("Die");
        StopCoroutine(currentRoutine);
        agent.destination = transform.position;
        Destroy(gameObject, 2f);
    }
}
