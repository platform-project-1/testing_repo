using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    PlayerInput actionMap;
    Rigidbody rb;

    [SerializeField, Range(0f, 100f)]
    float groundSpeed = 10f, acceleration = 5f;

    [SerializeField]
    Transform resetPosition;

    Vector2 movementInput;
    Vector3 velocity, desiredVelocity;


    #region Unity Functions

    void Awake()
    {
        actionMap = new PlayerInput();
        rb = GetComponent<Rigidbody>();

        actionMap.Movement.Move.started += OnMove;
        actionMap.Movement.Move.performed += OnMove;
        actionMap.Movement.Move.canceled += OnMove;
        actionMap.Debug.Quit.started += DebugQuitEditor;
        actionMap.Debug.Reset.started += DebugResetPosition;
    }

    void Update()
    {
        desiredVelocity = 
            new Vector3(movementInput.x, 0f, movementInput.y) * groundSpeed;
    }

    void FixedUpdate()
    {
        velocity = rb.velocity;
        float speedChange = acceleration * Time.deltaTime;
        velocity.x = 
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, speedChange);
        velocity.z = 
            Mathf.MoveTowards(velocity.z, desiredVelocity.z, speedChange);
        rb.velocity = velocity;
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

    void DebugQuitEditor(InputAction.CallbackContext context)
    {
        EditorApplication.isPlaying = false;
    }

    void DebugResetPosition(InputAction.CallbackContext context)
    {
        this.transform.position = resetPosition.position;
        rb.velocity = Vector3.zero;
    }
    #endregion
}
