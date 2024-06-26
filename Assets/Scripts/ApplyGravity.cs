using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApplyGravity : MonoBehaviour
{
    PlayerInput actionMap;
    Rigidbody rb;

    [SerializeField, Range(1f, 10f)]
    float fallMultiplier = 2f;

    [SerializeField, Range(0f, 5f)]
    public float maxJumpTime = 0.5f, maxJumpHeight = 1.75f;

    float groundedGravity = -0.05f;

    public bool isGrounded;
    bool isFalling = false;
    bool jumpPressed = false;
    float jumpGravity;
    public float jumpVelocity;
    Vector3 velocity;

    void Awake()
    {
        actionMap = new PlayerInput();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Start()
    {
        PerformJumpCalc();
    }

    void FixedUpdate()
    {
        HandleGravity();
        IsFallingCheck();
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

    void OnJump(InputAction.CallbackContext context)
    {
        jumpPressed = context.ReadValueAsButton();
    }
    #endregion

    void PerformJumpCalc()
    {
        /*
        Performs the calculations needed for Verlet Integration.
        Should be called in Start() or Awake() for deployment.
        Call in Update() for testing or debugging.
         */
        float timeToApex = maxJumpTime / 2;
        jumpGravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void HandleGravity()
    {
        velocity = rb.velocity;

        float newYVelocity;
        if (isGrounded)
        {
            newYVelocity = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = velocity.y;
            velocity.y = velocity.y + (jumpGravity * fallMultiplier * Time.deltaTime);
            newYVelocity = Mathf.Max((previousYVelocity + velocity.y) * 0.5f, jumpGravity);
        }
        else
        {
            float previousYVelocity = velocity.y;
            velocity.y = velocity.y + (jumpGravity * Time.deltaTime);
            newYVelocity = (previousYVelocity + velocity.y) * 0.5f;
        }
        velocity.y = newYVelocity;
        rb.velocity = velocity;
    }

    void IsFallingCheck()
    {
        isFalling = (!isGrounded && !jumpPressed) || rb.velocity.y <= 0.0f;
    }

    #region Collision Functions
    void OnCollisionEnter(Collision col)
    {
        //Debug.Log($"enter col.gameObject.tag = {col.gameObject.tag}");
        isGrounded = true;
    }

    void OnCollisionStay(Collision col)
    {
        //Debug.Log($"stay col.gameObject.tag = {col.gameObject.tag}");
        isGrounded = true;
    }

    void OnCollisionExit(Collision col)
    {
        isGrounded = false;
    }
    #endregion
}
