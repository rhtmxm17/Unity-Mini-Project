using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent), typeof(PlayerInput))]
public class PlayerControl : MonoBehaviour
{
    private NavMeshAgent agent;
    private PlayerInput input;
    private InputAction cursorAction;
    private LayerMask groundMask;

    private Vector2 moveInput;
    [SerializeField] Transform lookAtMarker;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
            lookAtMarker.position = hitInfo.point;
            Vector3 LookDirection = hitInfo.point - transform.position;
            LookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(LookDirection); // 커서 위치를 향해 회전
        }
    }

    private void ReadMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Movement()
    {
        Vector3 moveAxisX = input.camera.transform.right;
        Vector3 moveAxisY = input.camera.transform.up;

        moveAxisX.y = 0f;
        moveAxisY.y = 0f;

        moveAxisX.Normalize();
        moveAxisY.Normalize();

        agent.Move(Time.deltaTime * agent.speed * (moveAxisX * moveInput.x + moveAxisY * moveInput.y));
    }
}
