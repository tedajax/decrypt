using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMovement))]
public class PlayerUnitController : MonoBehaviour
{
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.LeftShift))
        {
            IsMouseLocked = false;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            IsMouseLocked = true;
        }

        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementInput.Normalize();

        Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        bool requestJump = Input.GetButtonDown("Jump");

        UnitMovementInput input = new UnitMovementInput
        {
            movementAxes = movementInput,
            lookAxes = lookInput,
            requestJump = requestJump
        };

        movement.MovementInput = input;
    }
}
