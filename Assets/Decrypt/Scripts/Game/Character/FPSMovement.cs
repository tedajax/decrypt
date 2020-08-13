﻿using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    public Transform pivotTransform;
    private CharacterController characterController;

    public float moveSpeed = 5.0f;
    public float mouseLookScalar = 5.0f;
    public float jumpForce = 5.0f;


    private Vector3 velocity = Vector3.zero;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        IsMouseLocked = true;
    }

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

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        input.Normalize();

        float mouseLookX = Input.GetAxis("Mouse X");
        float mouseLookY = Input.GetAxis("Mouse Y");

        yaw += mouseLookX * mouseLookScalar * Time.deltaTime;
        transform.localRotation = Quaternion.AngleAxis(yaw, Vector3.up);

        pitch = Mathf.Clamp(pitch - mouseLookY * mouseLookScalar * Time.deltaTime, -89.9f, 89.9f);
        pivotTransform.localRotation = Quaternion.AngleAxis(pitch, Vector3.right);

        input = transform.TransformDirection(input);
        Vector3 inputVelocity = input * moveSpeed;
        velocity.x = inputVelocity.x;
        velocity.z = inputVelocity.z;

        velocity.y = Mathf.Max(velocity.y - Systems.Instance.Config.gravity * Time.deltaTime, -100.0f);

        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            velocity.y = jumpForce;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
}