using System.Collections.Generic;
using UnityEngine;

public struct UnitMovementInput
{
    public Vector2 movementAxes;
    public Vector2 lookAxes;
    public bool requestJump;
}

[RequireComponent(typeof(Unit))]
public class UnitMovement : MonoBehaviour
{
    [Inject]
    public ConfigData config { get; set; }

    [System.Serializable]
    public class UnitMovementData
    {
        public float moveSpeed = 5.0f;
        public float mouseLookScalar = 5.0f;
        public float jumpForce = 5.0f;
    }


    [SerializeField] private UnitMovementData data = null;

    private IUnit unit;

    private CharacterController characterController;

    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity { get { return velocity; } }

    public UnitMovementInput MovementInput { get; set; }

    public UnitMovementData Data { get { return data; } }

    void Awake()
    {
        Systems.Inject(this);
        unit = GetComponent<IUnit>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Rotations
        unit.LookHeading += MovementInput.lookAxes.x * data.mouseLookScalar * Time.deltaTime;

        float pitch = unit.LookPitch;
        pitch -= MovementInput.lookAxes.y * data.mouseLookScalar * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -89f, 89f);
        unit.LookPitch = pitch;

        transform.localRotation = Quaternion.AngleAxis(unit.LookHeading, Vector3.up);

        // Translations
        Vector3 inputMovement = new Vector3(MovementInput.movementAxes.x, 0.0f, MovementInput.movementAxes.y);
        inputMovement = transform.TransformDirection(inputMovement);
        Vector3 movement = inputMovement * data.moveSpeed;
        velocity.x = movement.x;
        velocity.z = movement.z;

        velocity.y = Mathf.Max(velocity.y - config.gravity * Time.deltaTime, -100.0f);

        if (MovementInput.requestJump && characterController.isGrounded)
        {
            velocity.y = data.jumpForce;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
}
