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

    private enum State { Idle, Chase, Attack, Die, _COUNT }

    // TODO: 몬스터 생성시 공유 데이터 참조만 복사
    private class Shared
    {
        public readonly LayerMask playerLayerMask = LayerMask.GetMask("Player");
        public readonly LayerMask hitableLayerMask = LayerMask.GetMask("Player", "Default"); // 스킬 구현시 스킬마다 달라져야 할듯
        public readonly int playerLayerIndex = LayerMask.NameToLayer("Player");
    }
    private Shared shared;

    [SerializeField] float attackRange = 5f;
    [SerializeField] float hp = 10f;

    [SerializeField] float detectRange = 10f;
    [SerializeField] float sqrChaseRange = 12f * 12f;
    [SerializeField] SkillData skillData;

    private NavMeshAgent agent;
    private Animator animator;

    private State currentState = State.Idle;
    private StateBase[] states = new StateBase[(int)State._COUNT];
    private Skill skill;

    private Coroutine currentRoutine;
    private Transform target;

    #region IDamageable
    public IDamageable.Flag HitFlag => IDamageable.Flag.Monster;
    public void TakeDamage(float damage, IUnit source = null)
    {
        Hp -= damage;
        if (currentState == State.Idle && source != null)
        {
            target = source.gameObject.transform;
            ChangeState(State.Chase);
        }
    }
    #endregion IDamageable

    private void Awake()
    {
        shared = new();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        skill = skillData.BakeSkill(IDamageable.Flag.Player, this);

        OnDie = () => { ChangeState(State.Die); };

        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Chase] = new ChaseState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Die] = new DieState(this);

    }

    private void Start()
    {
        currentState = State.Idle;
        states[(int)State.Idle].Enter();
    }

    private void ChangeState(State state)
    {
        states[(int)currentState].Exit();
        currentState = state;
        states[(int)currentState].Enter();
    }

    private bool AttackCheckAndLook()
    {
        Vector3 distanceVector = target.transform.position - transform.position;
        Ray attackRay = new(transform.position, distanceVector);

        // 공격 발사 가능여부 판단
        bool hitted = Physics.Raycast(attackRay, out RaycastHit info, attackRange, shared.hitableLayerMask);
        Debug.DrawRay(attackRay.origin, attackRay.direction * attackRange, Color.yellow, 0.5f);

        bool result = (hitted && info.collider.gameObject.layer == shared.playerLayerIndex);

        transform.LookAt(target);

        return result;
    }

    private class IdleState : StateBase
    {
        private readonly HumanoidMonster self;
        public YieldInstruction detectPeriod = new WaitForSeconds(1f);
        private Collider[] detected = new Collider[1];


        public IdleState(HumanoidMonster self)
        {
            this.self = self;
        }

        public override void Enter()
        {
            self.animator.SetTrigger("Idle");
            self.currentRoutine = self.StartCoroutine(DetectPlayerRoutine());
            self.agent.isStopped = true;
        }

        public override void Exit()
        {
            self.StopCoroutine(self.currentRoutine);
        }

        private IEnumerator DetectPlayerRoutine()
        {
            yield return null;
            while (true)
            {
                if (0 < Physics.OverlapSphereNonAlloc(self.transform.position, self.detectRange, detected, self.shared.playerLayerMask))
                {
                    self.target = detected[0].transform;
                    self.ChangeState(State.Chase);
                }

                yield return detectPeriod;
            }
        }
    }

    private class ChaseState : StateBase
    {
        private readonly HumanoidMonster self;
        public YieldInstruction checkAttackPeriod = new WaitForSeconds(1f);


        public ChaseState(HumanoidMonster self)
        {
            this.self = self;
        }

        public override void Enter()
        {
            self.animator.SetTrigger("Walk");
            self.currentRoutine = self.StartCoroutine(ChasePlayerRoutine());
            self.agent.isStopped = false;
        }

        public override void Exit()
        {
            self.StopCoroutine(self.currentRoutine);
        }

        private IEnumerator ChasePlayerRoutine()
        {
            yield return null;
            while (true)
            {
                Vector3 distanceVector = self.target.transform.position - self.transform.position;

                // 추적 한계 거리
                if (distanceVector.sqrMagnitude > self.sqrChaseRange)
                {
                    self.ChangeState(State.Idle);

                    yield break;
                }


                if (self.AttackCheckAndLook())
                {
                    self.ChangeState(State.Attack);
                }
                else
                {
                    self.agent.destination = self.target.transform.position;
                }

                yield return checkAttackPeriod;
            }
        }
    }

    private class AttackState : StateBase
    {
        private readonly HumanoidMonster self;

        public AttackState(HumanoidMonster self)
        {
            this.self = self;
        }

        public override void Enter()
        {
            self.agent.isStopped = true;
            Attack();
            self.skill.OnSkillComplete += WhenSkillComplete;
        }

        public override void Exit()
        {
            self.StopCoroutine(self.currentRoutine);
            self.skill.OnSkillComplete -= WhenSkillComplete;
        }

        private void Attack()
        {
            self.currentRoutine = self.StartCoroutine(self.skill.CastSkill(self.transform));
            self.animator.SetTrigger("Attack");
        }

        private void WhenSkillComplete()
        {
            if (self.AttackCheckAndLook())
                Attack();
            else
                self.ChangeState(State.Chase);
        }
    }

    private class DieState : StateBase
    {
        private readonly HumanoidMonster self;

        public DieState(HumanoidMonster self)
        {
            this.self = self;
        }

        public override void Enter()
        {
            self.animator.SetTrigger("Die");
            self.GetComponent<Collider>().enabled = false;
            self.agent.isStopped = true;
            Destroy(self.gameObject, 4f);
        }

        public override void Exit()
        {
            Debug.LogError("몬스터가 Die 상태를 벗어남");
        }
    }
}
