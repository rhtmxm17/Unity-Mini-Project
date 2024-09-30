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

    [SerializeField] SkillData[] skillDatas;

    private enum State { Idle, Move, Attack, _COUNT }

    private NavMeshAgent agent;
    private PlayerModel model;

    private PlayerInput input;
    private InputAction cursorAction;
    private InputAction fireAction;
    private InputAction moveAction;

    private LayerMask groundMask;

    private bool isLookCursor;

    private State currentState = State.Idle;
    private StateBase[] states = new StateBase[(int)State._COUNT];
    private Coroutine stateRoutine;

    private Skill[] skills;
    private Skill CurrentSkill { get => skills[model.SkillIndex]; }

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

        skills = new Skill[skillDatas.Length];
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i] = skillDatas[i].BakeSkill(IDamageable.Flag.Monster, this);
        }
    }

    private void Start()
    {
        if (input.camera == null)
            input.camera = Camera.main;
        cursorAction = input.actions["Point"];

        moveAction = input.actions["Move"];
        fireAction = input.actions["Fire"];

        input.actions["Select"].started += SelectSkill;

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

    private void SelectSkill(InputAction.CallbackContext context)
    {
        // 스킬을 사용중이라면 무효
        if (currentState == State.Attack)
            return;

        float axisInput = context.ReadValue<float>();
        int next = model.SkillIndex;
        if (axisInput > 0f)
        {
            next++;
            if (next >= skills.Length)
                next = 0;
        }
        else
        {
            next--;
            if (next < 0)
                next = skills.Length - 1;
        }
        model.SkillIndex = next;
        Debug.Log($"스킬{next} 선택됨");
    }

    private void LookAtMouse()
    {
        Vector2 cursorPoint = cursorAction.ReadValue<Vector2>(); // 커서 위치 받아오기
        Ray clickRay = Camera.main.ScreenPointToRay(cursorPoint); // 카메라를 통해 Ray로 변환

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
        private readonly YieldInstruction waitRotationLock = new WaitForSeconds(0.1f);

        public AttackState(PlayerControl self)
        {
            this.self = self;
        }

        public override void Enter()
        {
            self.isLookCursor = true;
            self.StartCoroutine(RotationLock());
            self.model.TriggerAttack();

            self.CurrentSkill.OnSkillComplete += CheckSkillLoop; // 스킬 시전 완료시 반복할지 확인
            self.StartCoroutine(self.CurrentSkill.CastSkill(self.transform));
        }

        public override void Exit()
        {
            self.CurrentSkill.OnSkillComplete -= CheckSkillLoop;
            // self.StopCoroutine(self.stateRoutine);
        }

        private void CheckSkillLoop()
        {
            // 공격 버튼을 누른채라면 반복
            if (self.fireAction.inProgress)
            {
                self.StartCoroutine(self.CurrentSkill.CastSkill(self.transform));
                self.model.TriggerAttack();
            }
            else
            {
                if (self.moveAction.inProgress)
                    self.ChangeState(State.Move);
                else
                    self.ChangeState(State.Idle);
            }
        }

        private IEnumerator RotationLock()
        {
            yield return waitRotationLock;
            self.isLookCursor = false;
        }
    }
}
