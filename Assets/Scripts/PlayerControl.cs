using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent), typeof(PlayerInput))]
public class PlayerControl : MonoBehaviour, IDamageable
{
    [SerializeField] Transform cursorMarker;
    [SerializeField] SampleSkill sampleProjectile;

    private NavMeshAgent agent;
    private PlayerModel model;
    private PlayerInput input;

    private InputAction cursorAction;
    private InputAction fireAction;
    private LayerMask groundMask;

    private Vector2 moveInput;
    private bool isLookCursor;

    #region IDamageable
    public IDamageable.Flag HitFlag => IDamageable.Flag.Player;
    public void TakeDamage(float damage)
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
    }

    private void Start()
    {
        if (input.camera == null)
            input.camera = Camera.main;
        cursorAction = input.actions["Point"];

        InputAction moveAction = input.actions["Move"];
        moveAction.performed += ReadMoveInput;
        moveAction.canceled += ReadMoveInput;

        fireAction = input.actions["Fire"];
        fireAction.started += CastEnter;
    }


    private void Update()
    {
        LookAtMouse();
        Movement();
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

    private void ReadMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        model.IsMoving = context.performed;
    }

    private void Movement()
    {
        // 카메라의 up, right를 xz평면에서의 이동 방향으로 변환
        Vector3 moveAxisX = input.camera.transform.right;
        Vector3 moveAxisY = input.camera.transform.up;

        moveAxisX.y = 0f;
        moveAxisY.y = 0f;

        moveAxisX.Normalize();
        moveAxisY.Normalize();

        Vector3 velocity = model.MoveSpeed * (moveAxisX * moveInput.x + moveAxisY * moveInput.y);
        agent.Move(Time.deltaTime * velocity);

        if ((!isLookCursor) && model.IsMoving)
        {
            transform.rotation = Quaternion.LookRotation(velocity); // 이동 방향을 향해 회전
        }

        model.LocalVelocity = transform.worldToLocalMatrix * velocity;
    }

    private void CastEnter(InputAction.CallbackContext context)
    {
        // 공격 입력 비활성화
        fireAction.started -= CastEnter;

        isLookCursor = true;
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

        isLookCursor = false;
    }

}
