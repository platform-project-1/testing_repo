using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestClimbing : MonoBehaviour
{
    public enum PlayerState
    {
        WALKING,
        FALLING,
        CLIMBING
    }

    PlayerInput actionMap;
    Rigidbody rb;

    [SerializeField]
    public PlayerState state = PlayerState.CLIMBING;

    [SerializeField]
    float walkSpeed = 5f;

    [SerializeField]
    float climbSpeed = 5f;

    float h = 0f;
    float v = 0f;
    bool jumpDown = false;
    Transform cam;
    Vector2 input;
    Vector2 stickInput;
    Vector3 moveDirection;

    #region Basic Functions
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        actionMap = new PlayerInput();

        rb.useGravity = false;

        actionMap.Movement.Move.started += OnMove;
        actionMap.Movement.Move.performed += OnMove;
        actionMap.Movement.Move.canceled += OnMove;

        actionMap.Actions.Jump.started += OnJumpDown;
        actionMap.Actions.Jump.canceled += OnJumpDown;
    }

    void Update()
    {
        // Input happends per-frame not in the Physics Loop
        h = stickInput.x;
        v = stickInput.y;
    }

    void FixedUpdate()
    {
        input = SquareToCircle(new Vector2(h, v));
        cam = Camera.main.transform;
        moveDirection = Quaternion.FromToRotation(cam.up, Vector3.up)
            * cam.TransformDirection(new Vector3(input.x, 0f, input.y));

        switch (state)
        {
            case PlayerState.WALKING: { HandleWalking(moveDirection); } break;
            case PlayerState.FALLING: { HandleFalling(); } break;
            case PlayerState.CLIMBING: { HandleClimbing(input); } break;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.02f))
            state = PlayerState.WALKING;
        else if (state == PlayerState.WALKING)
            state = PlayerState.FALLING;

        rb.useGravity = state != PlayerState.CLIMBING;

        // Reset input
        jumpDown = false;
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
        stickInput = context.ReadValue<Vector2>();
    }

    void OnJumpDown(InputAction.CallbackContext context)
    {
        jumpDown = context.ReadValueAsButton();
    }
    #endregion

    void HandleWalking(Vector3 moveDirection)
    {
        Vector3 oldVelo = rb.velocity;
        Vector3 newVelo = moveDirection * walkSpeed;
        newVelo.y = oldVelo.y;

        if (jumpDown)
        {
            newVelo.y = 5f;
            state = PlayerState.FALLING;
        }

        rb.velocity = newVelo;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            transform.forward = Vector3.Lerp(transform.forward,
                moveDirection, 10f * Time.fixedDeltaTime);
        }
    }

    void HandleFalling()
    {
        if (jumpDown && Physics.Raycast(transform.position, transform.forward * 0.4f))
        {
            state = PlayerState.CLIMBING;
        }
    }

    void HandleClimbing(Vector2 input)
    {
        // Check walls in a cross pattern
        Vector3 offset = transform.TransformDirection(Vector2.one * 0.5f);
        Vector3 checkDirection = Vector3.zero;
        int k = 0;

        for (int i = 0; i < 4; i++)
        {
            RaycastHit checkHit;
            if (Physics.Raycast(transform.position + offset,
                transform.forward, out checkHit))
            {
                Debug.DrawRay(transform.position + offset, transform.forward, Color.red);
                checkDirection += checkHit.normal;
                k++;
            }
            // Rotate Offset by 90 degrees
            offset = Quaternion.AngleAxis(90f, transform.forward) * offset;
        }
        checkDirection /= k;

        // Check wall directly in front
        RaycastHit hit;
        if (Physics.Raycast(transform.position, checkDirection, out hit))
        {
            //Debug.DrawRay(transform.position, -checkDirection, Color.red);
            float dot = Vector3.Dot(transform.forward, -hit.normal);

            rb.position = Vector3.Lerp(rb.position,
                hit.point + hit.normal * 0.05f,
                5f * Time.fixedDeltaTime);

            transform.forward = Vector3.Lerp(transform.forward,
                -hit.normal, 10f * Time.fixedDeltaTime);

            rb.useGravity = false;
            rb.velocity = transform.TransformDirection(input) * climbSpeed;

            if (jumpDown)
            {
                rb.velocity = Vector3.up * 5f + hit.normal * 2f;
                state = PlayerState.FALLING;
            }
        }
        else
        {
            state = PlayerState.FALLING;
        }
    }

    Vector2 SquareToCircle(Vector2 input)
    {
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
    }
}
