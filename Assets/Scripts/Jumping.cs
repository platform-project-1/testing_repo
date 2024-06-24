using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jumping : MonoBehaviour
{
    PlayerInput actionMap;
    Rigidbody rb;

    [SerializeField, Range(0, 5)]
    int maxJumpPhases = 2;

    private int jumpPhase = 0;

    [SerializeField, Range(0f, 100f)]
    float fallMultiplier = 2f;

    [SerializeField, Range(0f, 5f)]
    float maxJumpTime = 1f, maxJumpHeight = 5f;

    //[SerializeField, Range(-100f, 0f)]
    //float gravity = -9.81f;

    float groundedGravity = -0.05f;

    bool jumpPressed;
    bool jumpValid = false;
    bool isGrounded;
    bool isFalling;
    float jumpGravity;
    float jumpVelocity;
    Vector3 velocity;

    #region Basic Functions
    void Awake()
    {
        actionMap = new PlayerInput();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;

        actionMap.Actions.Jump.started += OnJump;
        actionMap.Actions.Jump.canceled += OnJump;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        ApplyGravity();
        IsFallingCheck();
        CheckForJump();
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

    void OnJump(InputAction.CallbackContext context)
    {
        jumpPressed = context.ReadValueAsButton();
    }
    #endregion

    void PerformJumpCalc()
    {
        
        float timeToApex = maxJumpTime / 2;
        jumpGravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void PerformJump()
    {
        PerformJumpCalc();
        velocity = rb.velocity;
        velocity.y = jumpVelocity;
        rb.velocity = velocity;
        jumpPhase++;
    }

    void CheckForJump()
    {
        //Debug.Log($"jumpPhase = {jumpPhase}");
        if (jumpValid && jumpPressed)
        {
            if (isGrounded)
            {
                PerformJump();
            }
            else if (!isGrounded &&
                (jumpPhase < maxJumpPhases))
            {
                PerformJump();
            }
        }

        if (!jumpValid && !jumpPressed)
        {
            jumpValid = true;
        }
    }

    void ApplyGravity()
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
        isFalling = !isGrounded && !jumpPressed;
    }

    #region Collision Functions
    void OnCollisionEnter(Collision col)
    {
        isGrounded = true;
        jumpValid = true;
    }

    void OnCollisionStay(Collision col)
    {
        isGrounded = true;
        jumpValid = true;
    }

    void OnCollisionExit(Collision col)
    {
        isGrounded = false;
    }
    #endregion
}
