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

    // Viewer
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

    public void TriggerAttack()
    {
        animator.SetTrigger(Hash_Attack);
    }

    // Event
    public event UnityAction OnMovingChanged;
    public event UnityAction OnMoveSpeedChanged;
    public event UnityAction OnLocalVelocityChanged;
    public event UnityAction OnSkillIndexChanged;

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
}
