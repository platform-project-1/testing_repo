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

    [SerializeField, Range(0f, 100f)]
    float maxJumpTime = 10f, maxJumpHeight = 10f;

    [SerializeField, Range(-100f, 0f)]
    float gravity = -9.81f;

    float groundedGravity = -0.05f;

    bool jumpPressed;
    bool jumpValid = false;
    bool isGrounded;
    bool isFalling;
    //float gravity;
    Vector3 velocity;

    #region Basic Functions
    void Awake()
    {
        actionMap = new PlayerInput();
        rb = GetComponent<Rigidbody>();

        actionMap.Actions.Jump.started += OnJump;
        actionMap.Actions.Jump.canceled += OnJump;
    }

    void Update()
    {
        Debug.Log($"rb.velocity.y = {rb.velocity.y}");
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

    void PerformJump()
    {
        //velocity = rb.velocity;

        //float timeToApex = (-2 * maxJumpTime) / 2;
        //gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        //float jumpVelocity = (2 * maxJumpHeight) / timeToApex;
        //velocity.y = jumpVelocity;

        //rb.velocity = velocity;

        //jumpPhase++;
        //jumpValid = false;
    }

    void CheckForJump()
    {
        //Debug.Log($"jumpPhase = {jumpPhase}");
        //if (jumpValid && jumpPressed)
        //{
        //    if (isGrounded)
        //    {
        //        PerformJump();
        //    }
        //    else if (!isGrounded &&
        //        (jumpPhase < maxJumpPhases))
        //    {
        //        PerformJump();
        //    }
        //}

        //if (!jumpValid && !jumpPressed)
        //{
        //    jumpValid = true;
        //}
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
            previousYVelocity = previousYVelocity + (gravity * fallMultiplier * Time.deltaTime);
            newYVelocity = Mathf.Max((previousYVelocity + velocity.y) * 0.5f, gravity);
        }
        else
        {
            float previousYVelocity = velocity.y;
            previousYVelocity = previousYVelocity + (gravity * Time.deltaTime);
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
    }

    void OnCollisionStay(Collision col)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision col)
    {
        isGrounded = false;
    }
    #endregion
}
