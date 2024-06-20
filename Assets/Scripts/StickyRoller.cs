using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StickyRoller : MonoBehaviour
{
    PlayerInput actionMap;
    Rigidbody rb;

    [SerializeField, Range(0f, 100f)]
    float groundSpeed = 10f, acceleration = 5f;

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
    }

    void Update()
    {
        desiredVelocity =
            new Vector3(movementInput.x, 0f, movementInput.y) * groundSpeed;
    }

    void FixedUpdate()
    {
        Moving();
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
    #endregion

    void Moving()
    {
        velocity = rb.velocity;
        float speedChange = acceleration * Time.deltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, speedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, desiredVelocity.z, speedChange);
        rb.velocity = velocity;
    }
}
