using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class GroundMovement : MonoBehaviour
{
    PlayerInput actionMap;
    Jumping jumping;
    Rigidbody rb;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 20f;

    bool boostPressed;

    [HideInInspector]
    public Vector2 movementInput;
    bool moving;

    Vector3 velocity;

    #region Basic Functions
    void Awake()
    {
        actionMap = new PlayerInput();
        jumping = GetComponent<Jumping>();
        rb = GetComponent<Rigidbody>();

        actionMap.Movement.Move.started += OnMove;
        actionMap.Movement.Move.performed += OnMove;
        actionMap.Movement.Move.canceled += OnMove;
        actionMap.Actions.Boost.started += OnBoost;
        actionMap.Actions.Boost.performed += OnBoost;
        actionMap.Actions.Boost.canceled += OnBoost;
    }

    void FixedUpdate()
    {
        HandleMoving();
        HandleRotation();
    }

    void Update()
    {
        
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
        moving = movementInput.x != 0 || movementInput.y != 0;
    }

    void OnBoost(InputAction.CallbackContext context)
    {
        boostPressed = context.ReadValueAsButton();
    }
    #endregion

    #region Movement Functions
    void HandleMoving()
    {
        // Determines if sprint speed is applied or not
        float sprint =
            boostPressed ? 2f : 1f;

        // Getting camera info
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;

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
        // Getting camera info
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 positionToLookAt = new Vector3 (movementInput.x, 0f, movementInput.y);
        positionToLookAt = forward * positionToLookAt.z + right * positionToLookAt.x;
        positionToLookAt.Normalize();
        if (moving)
        {
            transform.rotation = Quaternion.LookRotation(positionToLookAt);
        }
    }
    #endregion

    #region Collision Functions
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "wall")
        {
            jumping.enabled = false;
        }
        else if (col.gameObject.tag == "ground")
        {
            jumping.enabled = true;
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "wall")
        {
            jumping.enabled = false;
        }
        else if (col.gameObject.tag == "ground")
        {
            jumping.enabled = true;
        }
    }

    void OnCollisionExit(Collision col)
    {
        
    }
    #endregion
}
