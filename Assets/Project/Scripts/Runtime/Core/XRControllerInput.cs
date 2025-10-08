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

        // Получаем InputActions из ссылок (новый способ в XRI 3.0)
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
        // Включаем все Input Actions (обязательно в новой архитектуре)
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
        // Сбрасываем velocity каждый кадр для мгновенного старта/остановки
        movementVelocity = Vector3.zero;

        // Обработка горизонтального перемещения с джойстика
        HandleHorizontalMovement();

        // Обработка вертикального перемещения (прыжок/спуск)
        HandleVerticalMovement();
    }

    private void HandleHorizontalMovement()
    {
        Vector2 moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;

        if (moveInput.magnitude > 0.1f && xrCamera != null)
        {
            // Получаем направление относительно взгляда камеры
            Vector3 forward = xrCamera.forward;
            Vector3 right = xrCamera.right;

            // Игнорируем вертикальную компоненту для горизонтального движения
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Рассчитываем направление движения на основе ввода джойстика
            Vector3 horizontalMovement = (forward * moveInput.y + right * moveInput.x) * movementSpeed;
            movementVelocity.x = horizontalMovement.x;
            movementVelocity.z = horizontalMovement.z;
        }
    }

    private void HandleVerticalMovement()
    {
        // Читаем значения кнопок (новый способ в XRI 3.0)
        float jumpValue = jumpAction?.ReadValue<float>() ?? 0f;
        float descendValue = descendAction?.ReadValue<float>() ?? 0f;

        bool isJumpPressed = jumpValue > 0.1f;
        bool isDescendPressed = descendValue > 0.1f;

        // Механика невесомости: движение только при зажатых кнопках
        if (isJumpPressed && !isDescendPressed)
        {
            // Движение вверх без гравитации
            movementVelocity.y = verticalSpeed;
        }
        else if (isDescendPressed && !isJumpPressed)
        {
            // Движение вниз без гравитации
            movementVelocity.y = -verticalSpeed;
        }
        // Если ни одна кнопка не нажата - вертикальное движение отсутствует

        // Если нажаты обе кнопки - приоритет у движения вниз
        if (isJumpPressed && isDescendPressed)
        {
            movementVelocity.y = -verticalSpeed;
        }
    }

    private void ApplyMovement()
    {
        // Применяем рассчитанную скорость
        characterController.Move(movementVelocity * Time.deltaTime);
    }
}