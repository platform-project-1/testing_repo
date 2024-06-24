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

    bool sprintPressed;

    Vector2 movementInput;
    Vector3 velocity;



    #region Basic Functions
    void Awake()
    {
        actionMap = new PlayerInput();
        rb = GetComponent<Rigidbody>();

        //rb.useGravity = false;

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
        HandleRotation();
        //HandleFalling();
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

    #region Movement Functions
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
        velocity = rb.velocity;
        if (velocity.x > 0.01f || velocity.z > 0.01f) 
        {
            Vector3 rotate = new Vector3(velocity.x * Time.deltaTime, 0f, velocity.z * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(rotate);
        }
    }

    void HandleFalling()
    {
        //// FIGURE HOW TO INCREASE FALL SPEED WITHOUT IT BEING TOO MUCH
        //// LOOK INTO VERLET INTEGRATION
        //velocity = rb.velocity;
        //if (velocity.y >= 0)
        //{
        //    velocity.y = normalGravity * Time.deltaTime;
        //}
        //else
        //{
        //    velocity.y = fallingGravity * Time.deltaTime;
        //}
        ////velocity.y += Time.deltaTime;
        //rb.velocity = velocity;
    }
    #endregion


}
