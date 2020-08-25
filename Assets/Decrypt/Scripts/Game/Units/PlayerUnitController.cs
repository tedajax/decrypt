using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(UnitMovement))]
public class PlayerUnitController : MonoBehaviour
{
    [SerializeField]
    private InputAction moveInputAction;

    [SerializeField]
    private InputAction lookInputAction;

    [SerializeField]
    private InputAction jumpInputAction;

    private UnitMovement movement;


    public bool IsMouseLocked
    {
        get { return Cursor.lockState == CursorLockMode.Locked; }
        set
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    void Awake()
    {
        IsMouseLocked = true;
        movement = GetComponent<UnitMovement>();
    }

    void OnEnable()
    {
        moveInputAction.Enable();
        lookInputAction.Enable();
        jumpInputAction.Enable();
    }

    void OnDisable()
    {
        moveInputAction.Disable();
        lookInputAction.Disable();
        jumpInputAction.Disable();
    }

    void Update()
    {
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            IsMouseLocked = false;
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            IsMouseLocked = true;
        }

        UnitMovementInput input = new UnitMovementInput
        {
            movementAxes = moveInputAction.ReadValue<Vector2>(),
            lookAxes = lookInputAction.ReadValue<Vector2>(),
            requestJump = jumpInputAction.triggered,
        };

        movement.MovementInput = input;
    }
}
