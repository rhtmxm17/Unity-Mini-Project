using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent), typeof(PlayerInput))]
public class PlayerControl : MonoBehaviour, IDamageable, IUnit
{
    public event UnityAction OnDie;
    public float Hp { get { Debug.LogWarning("Not Implimented"); return 0; } }

    [SerializeField] Transform cursorMarker;
    [SerializeField] SampleSkill sampleProjectile;

    [SerializeField] SkillData skillData;

    private enum State { Idle, Move, Attack, _COUNT }

    private NavMeshAgent agent;
    private PlayerModel model;

    private PlayerInput input;
    private InputAction cursorAction;
    private InputAction fireAction;
    private InputAction moveAction;

    private LayerMask groundMask;

    private bool isLookCursor;

    [SerializeField] // 검사용
    private State currentState = State.Idle;
    private StateBase[] states = new StateBase[(int)State._COUNT];
    private Coroutine stateRoutine;

    private Skill currentSkill;

    #region IDamageable
    public IDamageable.Flag HitFlag => IDamageable.Flag.Player;


    public void TakeDamage(float damage, IUnit source = null)
    {
        Debug.Log($"피격 데미지: {damage}");
    }
    #endregion IDamageable

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        model = GetComponent<PlayerModel>();
        input = GetComponent<PlayerInput>();
        groundMask = LayerMask.GetMask("Ground");

        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Move] = new MoveState(this);
        states[(int)State.Attack] = new AttackState(this);

        currentSkill = skillData.BakeSkill(IDamageable.Flag.Monster, this);
    }

    private void Start()
    {
        if (input.camera == null)
            input.camera = Camera.main;
        cursorAction = input.actions["Point"];

        moveAction = input.actions["Move"];
        fireAction = input.actions["Fire"];

        states[(int)currentState].Enter();
    }

    private void ChangeState(State state)
    {
        states[(int)currentState].Exit();
        currentState = state;
        states[(int)currentState].Enter();
    }

    private void Update()
    {
        // 상태와 무관하게 항상 작동하는 작업
        LookAtMouse();
    }

    private void LookAtMouse()
    {
        Vector2 cursorPoint = cursorAction.ReadValue<Vector2>(); // 커서 위치 받아오기
        Ray clickRay = input.camera.ScreenPointToRay(cursorPoint); // 카메라를 통해 Ray로 변환

        if (Physics.Raycast(clickRay, out RaycastHit hitInfo, 50f, groundMask))
        {
            cursorMarker.position = hitInfo.point;
            if (isLookCursor)
            {
                Vector3 LookDirection = hitInfo.point - transform.position;
                LookDirection.y = 0;
                transform.rotation = Quaternion.LookRotation(LookDirection); // 커서 위치를 향해 회전
            }
        }
    }

    private void CastEnter(InputAction.CallbackContext context)
    {
        ChangeState(State.Attack);

        StartCoroutine(CastSkill());
        model.TriggerAttack();
    }

    private IEnumerator CastSkill()
    {
        yield return new WaitForSeconds(0.5f);
        
        fireAction.started += CastEnter;
        var projectile = Instantiate(sampleProjectile, transform.position, transform.rotation);
        projectile.Init();
        projectile.hitMask = IDamageable.Flag.Wall | IDamageable.Flag.Monster;
        projectile.source = this;

        isLookCursor = false;
    }

    private class IdleState : StateBase
    {
        private readonly PlayerControl self;

        public IdleState(PlayerControl self)
        {
            this.self = self;
        }

        public override void Enter()
        {
            self.model.LocalVelocity = Vector3.zero;
            self.isLookCursor = true;

            self.moveAction.started += ReadMoveInput; // Idle -> Move
            self.fireAction.started += self.CastEnter; // Idle -> Attack
        }

        public override void Exit()
        {
            self.moveAction.started -= ReadMoveInput;
            self.fireAction.started -= self.CastEnter;
        }

        private void ReadMoveInput(InputAction.CallbackContext context)
        {
            self.model.IsMoving = true;
            self.ChangeState(State.Move);
        }
    }

    private class MoveState : StateBase
    {
        private readonly PlayerControl self;
        private Vector2 moveInput;

        public MoveState(PlayerControl self)
        {
            this.self = self;
        }

        public override void Enter()
        {
            moveInput = self.moveAction.ReadValue<Vector2>();
            self.moveAction.performed += ReadMoveValue;
            self.isLookCursor = false;
            self.stateRoutine = self.StartCoroutine(MovementRoutine());

            self.moveAction.canceled += ReadMoveInputCancled; // Move -> Idle
            self.fireAction.started += self.CastEnter; // Move -> Attack
        }

        public override void Exit()
        {
            self.moveAction.performed -= ReadMoveValue;
            self.moveAction.canceled -= ReadMoveInputCancled;
            self.fireAction.started -= self.CastEnter;
            self.StopCoroutine(self.stateRoutine);
        }

        private void ReadMoveValue(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void ReadMoveInputCancled(InputAction.CallbackContext context)
        {
            self.model.IsMoving = false;
            self.ChangeState(State.Idle);
        }

        private IEnumerator MovementRoutine()
        {
            while (true)
            {
                // 카메라의 up, right를 xz평면에서의 이동 방향으로 변환
                Vector3 moveAxisX = Camera.main.transform.right;
                Vector3 moveAxisY = Camera.main.transform.up;

                moveAxisX.y = 0f;
                moveAxisY.y = 0f;

                moveAxisX.Normalize();
                moveAxisY.Normalize();

                // 입력 방향과 속도 능력치를 적용
                Vector3 velocity = self.model.MoveSpeed * (moveAxisX * moveInput.x + moveAxisY * moveInput.y);
                self.agent.Move(Time.deltaTime * velocity);

                self.transform.rotation = Quaternion.LookRotation(velocity); // 이동 방향을 향해 회전

                self.model.LocalVelocity = self.transform.worldToLocalMatrix * velocity;

                yield return null;
            }
        }
    }

    private class AttackState : StateBase
    {
        private readonly PlayerControl self;

        public AttackState(PlayerControl self)
        {
            this.self = self;
        }

        public override void Enter()
        {
            self.isLookCursor = false;
            self.model.TriggerAttack();
            self.stateRoutine = self.StartCoroutine(CastSkill());
            return;

            self.currentSkill.OnSkillComplete += CheckSkillLoop;
            self.StartCoroutine(self.currentSkill.CastSkill());
        }

        public override void Exit()
        {
            self.StopCoroutine(self.stateRoutine);
        }

        private void CheckSkillLoop()
        {
            // 공격 버튼을 누른채라면 반복
            if (self.fireAction.inProgress)
            {
                self.StartCoroutine(self.currentSkill.CastSkill());
            }
            else
            {
                self.ChangeState(State.Idle);
            }
        }

        private IEnumerator CastSkill()
        {
            yield return new WaitForSeconds(0.1f);

            self.isLookCursor = false;

            yield return new WaitForSeconds(0.4f);

            // TODO: skill.Perform();
            {
                var projectile = Instantiate(self.sampleProjectile, self.transform.position, self.transform.rotation);
                projectile.Init();
                projectile.hitMask = IDamageable.Flag.Wall | IDamageable.Flag.Monster;
                projectile.source = self;
            }

            // 공격 버튼을 누른채라면 반복
            if (self.fireAction.inProgress)
            {
                Enter();
            }
            else
            {
                self.ChangeState(State.Idle);
            }
        }
    }
}
