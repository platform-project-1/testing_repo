using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    PlayerInput actionMap;
    Rigidbody rb;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 20f;

    [SerializeField]
    Transform target;

    [SerializeField, Range(0f, 100f)]
    float rotateSpeed = 1f;

    bool sprintPressed;

    Vector2 movementInput;

    Vector3 velocity;

    #region Basic Functions
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

    void FixedUpdate()
    {
        HandleMoving();
        //HandleRotation();
    }
    #endregion

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
        //movementInput = Vector2.ClampMagnitude(movementInput, 1f);
    }

    void OnSprint(InputAction.CallbackContext context)
    {
        sprintPressed = context.ReadValueAsButton();
    }
    #endregion

    #region Movement and Rotation Functions
    void HandleMoving()
    {
        // Determines if sprint speed is applied or not
        float sprint =
            sprintPressed ? 2f : 1f;

        // Getting camera info
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // Creates new vector to adjust movement based on camera
        Vector3 move = new Vector3(movementInput.x, 0f, movementInput.y);
        move = forward * move.z + right * move.x;

        // Get velocity, modify, and return it
        velocity = rb.velocity;
        move.Normalize();
        velocity.x += move.x * maxAcceleration * sprint * Time.deltaTime;
        velocity.z += move.z * maxAcceleration * sprint * Time.deltaTime;
        rb.velocity = velocity;
    }

    void HandleRotation()
    {
        float step = rotateSpeed * Time.deltaTime;

        Vector3 positionToLookAt;

        positionToLookAt.x = movementInput.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = movementInput.y;

        Quaternion currectRotation = transform.rotation;

        if (movementInput.x > 0.01f || movementInput.x < -0.01f ||
            movementInput.y > 0.01f || movementInput.y < -0.01f)
        {

            Quaternion targetRotation = Quaternion.LookRotation(
                positionToLookAt);

            transform.localRotation = Quaternion.Slerp(currectRotation,
                targetRotation, step);
        }
    }
    #endregion
}
