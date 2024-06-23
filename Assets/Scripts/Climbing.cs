using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Climbing : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    PlayerInput actionMap;
    Vector2 movementInput;

    Rigidbody rb;

    void Awake()
    {
        actionMap = new PlayerInput();

        actionMap.Movement.Move.started += OnMove;
        actionMap.Movement.Move.performed += OnMove;
        actionMap.Movement.Move.canceled += OnMove;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        float h = movementInput.x;
        float v = movementInput.y;
        Vector2 input = SquareToCircle(new Vector2(h, v));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, // Position
                            transform.forward,  // Direction
                            out hit))
        {
            transform.forward = -hit.normal;
            rb.position = Vector3.Lerp(rb.position,
                hit.point + hit.normal * 0.51f,
                10f * Time.fixedDeltaTime
                );
        }
        rb.velocity = transform.TransformDirection(input) * speed;
    }

    Vector2 SquareToCircle(Vector2 input)
    {
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
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

    //void OnSprint(InputAction.CallbackContext context)
    //{
    //    sprintPressed = context.ReadValueAsButton();
    //}
    #endregion
}
