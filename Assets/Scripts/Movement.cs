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
    float groundSpeed = 10f, acceleration = 5f;

    [SerializeField]
    Transform relativeCamera = default;

    bool sprintPressed;

    Vector2 movementInput;
    Vector3 velocity, desiredVelocity;

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

    void Update()
    {
        if (relativeCamera)
        {
            // NOT SURE WHICH IS BETTER
            //desiredVelocity = relativeCamera.TransformDirection(
            //    movementInput.x, 0f, movementInput.y
            //    ) * groundSpeed;

            Vector3 forward = relativeCamera.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = relativeCamera.right;
            right.y = 0f;
            right.Normalize();
            desiredVelocity = 
                (forward * movementInput.y + right * movementInput.x) * groundSpeed;
        }
        else
        {
            desiredVelocity =
                new Vector3(movementInput.x, 0f, movementInput.y) * groundSpeed;
        }
    }

    void FixedUpdate()
    {
        Moving();
        // FIGURE OUT HOW PREVENT MAGNITUDE FROM DECREASING DURING TURNS
        //Debug.Log($"rb.velocity.magnitude = {rb.velocity.magnitude}");
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
    }

    void OnSprint(InputAction.CallbackContext context)
    {
        sprintPressed = context.ReadValueAsButton();
    }
    #endregion

    void Moving()
    {
        velocity = rb.velocity;
        float speedChange =
            sprintPressed ?
            2 * acceleration * Time.deltaTime :
            acceleration * Time.deltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, speedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, desiredVelocity.z, speedChange);
        rb.velocity = velocity;
    }
}
