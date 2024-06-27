using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Climbing : MonoBehaviour
{
    [SerializeField]
    float climbingSpeed = 5f;

    PlayerInput actionMap;
    Vector2 movementInput;
    Vector2 input;

    Rigidbody rb;

    //Vector3 zero = Vector3.zero;

    #region Basic Functions
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        actionMap = new PlayerInput();
        actionMap.Movement.Move.started += OnMove;
        actionMap.Movement.Move.performed += OnMove;
        actionMap.Movement.Move.canceled += OnMove;
    }

    void Update()
    {
        input = SquareToCircle(movementInput);
    }

    void FixedUpdate()
    {
        SquareToCircle(movementInput);
        HandleClimbing(input);
    }
    #endregion

    void HandleClimbing(Vector2 input)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            transform.forward = -hit.normal;
            rb.position = Vector3.Lerp(rb.position, 
                hit.point + hit.normal * 0.51f,
                10f * Time.fixedDeltaTime);
        }

        rb.velocity = transform.TransformDirection(input) * climbingSpeed;
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
    #endregion


    Vector2 SquareToCircle(Vector2 input)
    {
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
    }

    #region Collision Functions
    void OnCollisionEnter(Collision col)
    {
        
    }

    void OnCollisionStay(Collision col)
    {

    }

    void OnCollisionExit(Collision col)
    {

    }
    #endregion
}

