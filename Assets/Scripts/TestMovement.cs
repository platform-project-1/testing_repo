using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMovement : MonoBehaviour
{
    PlayerInput actionMap;
    Rigidbody rb;

    [SerializeField]
    Transform relativeCamera;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 15f;

    bool sprintPressed;
    Vector2 movementInput;
    Vector3 velocity;

    void Awake()
    {
        actionMap = new PlayerInput();
        rb = GetComponent<Rigidbody>();

        actionMap.Movement.Move.started += OnMove;
        actionMap.Movement.Move.performed += OnMove;
        actionMap.Movement.Move.canceled += OnMove;
        actionMap.Actions.Sprint.started += OnSprint;
        actionMap.Actions.Sprint.performed += OnSprint;
        actionMap.Actions.Sprint.canceled += OnSprint;
    }

    void Update()
    {
        if (relativeCamera)
        {
            Vector3 camera  = relativeCamera.TransformDirection(
                movementInput.x, 0f, movementInput.y);
        }
    }

    void FixedUpdate()
    {
        float sprint =
            sprintPressed ? 2f : 1f;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        Vector3 move = new Vector3(movementInput.x, 0f, movementInput.y);
        move = forward * move.z + right * move.x;

        velocity = rb.velocity;
        move.Normalize();
        velocity.x += move.x * maxAcceleration * sprint * Time.deltaTime;
        velocity.z += move.z * maxAcceleration * sprint * Time.deltaTime;

        rb.velocity = velocity;
    }

    #region Input Functions
    void OnEnable()
    {
        actionMap.Enable();
    }

    void OnDisable()
    {
        actionMap.Disable();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    void OnSprint(InputAction.CallbackContext context)
    {
        sprintPressed = context.ReadValueAsButton();
    }
    #endregion
}
