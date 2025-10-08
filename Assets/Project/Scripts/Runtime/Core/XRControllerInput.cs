using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class XRControllerInput : MonoBehaviour
{
    [Header("Input Actions (New in XRI 3.0)")]
    [SerializeField] private InputActionProperty moveActionReference;
    [SerializeField] private InputActionProperty jumpActionReference;
    [SerializeField] private InputActionProperty descendActionReference;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float verticalSpeed = 2f;

    private CharacterController characterController;
    private Transform xrCamera;
    private Vector3 movementVelocity;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction descendAction;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        xrCamera = GetComponentInChildren<Camera>()?.transform;

        // �������� InputActions �� ������ (����� ������ � XRI 3.0)
        moveAction = moveActionReference.action;
        jumpAction = jumpActionReference.action;
        descendAction = descendActionReference.action;

        EnableInputActions();
    }

    void OnEnable()
    {
        EnableInputActions();
    }

    void OnDisable()
    {
        DisableInputActions();
    }

    void Update()
    {
        UpdateMovement();
        ApplyMovement();
    }

    private void EnableInputActions()
    {
        // �������� ��� Input Actions (����������� � ����� �����������)
        moveAction?.Enable();
        jumpAction?.Enable();
        descendAction?.Enable();
    }

    private void DisableInputActions()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        descendAction?.Disable();
    }

    private void UpdateMovement()
    {
        // ���������� velocity ������ ���� ��� ����������� ������/���������
        movementVelocity = Vector3.zero;

        // ��������� ��������������� ����������� � ���������
        HandleHorizontalMovement();

        // ��������� ������������� ����������� (������/�����)
        HandleVerticalMovement();
    }

    private void HandleHorizontalMovement()
    {
        Vector2 moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;

        if (moveInput.magnitude > 0.1f && xrCamera != null)
        {
            // �������� ����������� ������������ ������� ������
            Vector3 forward = xrCamera.forward;
            Vector3 right = xrCamera.right;

            // ���������� ������������ ���������� ��� ��������������� ��������
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // ������������ ����������� �������� �� ������ ����� ���������
            Vector3 horizontalMovement = (forward * moveInput.y + right * moveInput.x) * movementSpeed;
            movementVelocity.x = horizontalMovement.x;
            movementVelocity.z = horizontalMovement.z;
        }
    }

    private void HandleVerticalMovement()
    {
        // ������ �������� ������ (����� ������ � XRI 3.0)
        float jumpValue = jumpAction?.ReadValue<float>() ?? 0f;
        float descendValue = descendAction?.ReadValue<float>() ?? 0f;

        bool isJumpPressed = jumpValue > 0.1f;
        bool isDescendPressed = descendValue > 0.1f;

        // �������� �����������: �������� ������ ��� ������� �������
        if (isJumpPressed && !isDescendPressed)
        {
            // �������� ����� ��� ����������
            movementVelocity.y = verticalSpeed;
        }
        else if (isDescendPressed && !isJumpPressed)
        {
            // �������� ���� ��� ����������
            movementVelocity.y = -verticalSpeed;
        }
        // ���� �� ���� ������ �� ������ - ������������ �������� �����������

        // ���� ������ ��� ������ - ��������� � �������� ����
        if (isJumpPressed && isDescendPressed)
        {
            movementVelocity.y = -verticalSpeed;
        }
    }

    private void ApplyMovement()
    {
        // ��������� ������������ ��������
        characterController.Move(movementVelocity * Time.deltaTime);
    }
}