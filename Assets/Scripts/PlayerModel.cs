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

    // Event
    public event UnityAction OnMovingChanged;
    public event UnityAction OnMoveSpeedChanged;
    public event UnityAction OnLocalVelocityChanged;

    // Hash
    private static readonly int Hash_VelocityZ = Animator.StringToHash("VelocityZ");
    private static readonly int Hash_IsMoving = Animator.StringToHash("IsMoving");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        OnMovingChanged = PresentMoving;
        OnMoveSpeedChanged = PresentMoveSpeed;
        OnLocalVelocityChanged = PresentLocalVelocity;
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
}
