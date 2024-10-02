using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PlayerModel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] bool isMoving;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 localVelocity;
    [SerializeField] int skillIndex;
    [SerializeField] float maxHP;
    [SerializeField] float curHP;

    [Header("Viewer")]
    [SerializeField] UnitUI unitUI;
    private NavMeshAgent agent;
    private Animator animator;

    // Field
    public bool IsMoving
    {
        get => isMoving;
        set { isMoving = value; OnMovingChanged.Invoke(); }
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set { moveSpeed = value; OnMoveSpeedChanged.Invoke(); }
    }

    public Vector3 LocalVelocity
    {
        get => localVelocity;
        set { localVelocity = value; OnLocalVelocityChanged.Invoke(); }
    }

    public int SkillIndex
    {
        get => skillIndex;
        set { skillIndex = value; OnSkillIndexChanged.Invoke(); }
    }

    public float MaxHP
    {
        get => maxHP;
        set { maxHP = value; OnMaxHP_Changed.Invoke(); }
    }

    public float CurHP
    {
        get => curHP;
        set { curHP = value; OnCurHP_Changed.Invoke(); }
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(Hash_Attack);
    }

    // Event
    public event UnityAction OnMovingChanged;
    public event UnityAction OnMoveSpeedChanged;
    public event UnityAction OnLocalVelocityChanged;
    public event UnityAction OnSkillIndexChanged;
    public event UnityAction OnMaxHP_Changed;
    public event UnityAction OnCurHP_Changed;

    // Hash
    private static readonly int Hash_VelocityZ = Animator.StringToHash("VelocityZ");
    private static readonly int Hash_IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Hash_Attack = Animator.StringToHash("Attack");
    private static readonly int Hash_Switch = Animator.StringToHash("Switch");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        OnMovingChanged = PresentMoving;
        OnMoveSpeedChanged = PresentMoveSpeed;
        OnLocalVelocityChanged = PresentLocalVelocity;
        OnSkillIndexChanged = PresentSkillIndex;
        OnMaxHP_Changed = PresentMaxHP;
        OnCurHP_Changed = PresentCurHP;
    }

    private void Start()
    {
        // 초기값 전달
        OnMovingChanged.Invoke();
        OnMoveSpeedChanged.Invoke();
        OnMoveSpeedChanged.Invoke();
        OnSkillIndexChanged.Invoke();
        OnMaxHP_Changed.Invoke();
        OnCurHP_Changed.Invoke();

        unitUI.HP_SliderColor = Color.green;
    }

    private void PresentMoving()
    {
        animator.SetBool(Hash_IsMoving, isMoving);
    }

    private void PresentMoveSpeed()
    {
        agent.speed = moveSpeed;
    }

    private void PresentLocalVelocity()
    {
        animator.SetFloat(Hash_VelocityZ, localVelocity.z);
    }

    private void PresentSkillIndex()
    {
        animator.SetInteger(Hash_Switch, skillIndex);
    }

    private void PresentMaxHP()
    {
        unitUI.MaxHP = maxHP;
    }

    private void PresentCurHP()
    {
        unitUI.CurHP = curHP;
    }
}
