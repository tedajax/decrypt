using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    private CharacterController characterController;

    public float moveSpeed = 5.0f;
    public float gravity = 9.8f;
    public float mouseLookScalar = 5.0f;


    private Vector3 velocity = Vector3.zero;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        input.Normalize();

        float mouseLookX = Input.GetAxis("Mouse X");

        yaw += mouseLookX * mouseLookScalar;
        transform.rotation = Quaternion.AngleAxis(yaw, Vector3.up);

        input = transform.TransformDirection(input);
        Vector3 inputVelocity = input * moveSpeed;
        velocity.x = inputVelocity.x;
        velocity.z = inputVelocity.z;

        if (characterController.isGrounded)
        {
            velocity.y = 0.0f;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
}
