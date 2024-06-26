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

    private int jumpPhase = 1;

    [SerializeField, Range(0f, 100f)]
    float fallMultiplier = 2f;

    [SerializeField, Range(0f, 5f)]
    float maxJumpTime = 0.5f, maxJumpHeight = 1.75f;

    float groundedGravity = -0.05f;

    bool jumpPressed = false;
    bool initialJumpPerformed = false;
    bool subsequentJumpsValid = false;
    bool breakEarly = false;
    bool isJumping = false;
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
        PerformJumpCalc();
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
        if (jumpPressed)
        {
            jumpPhase++;
        }
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
    IEnumerator PerformInitialJump()
    {
        // Logic to add velocity to initial jump.
        velocity = rb.velocity;
        velocity.y = jumpVelocity;
        rb.velocity = velocity;
        
        for (float timer = maxJumpTime; timer >= 0; timer -= Time.deltaTime)
        {
            isJumping = true;
            if (breakEarly)
            {
                isJumping = false;
                yield break;
            }
        }
        yield return null;
        initialJumpPerformed = true;
        isJumping = false;
    }
    
    IEnumerator PerformSubsequentJump()
    {
        // Logic to add velocity to subsequent jumps.
        velocity = rb.velocity;
        velocity.y = jumpVelocity;
        rb.velocity = velocity;

        isJumping = true;
        subsequentJumpsValid = false;
        for (float timer = maxJumpTime; timer >= 0; timer -= Time.deltaTime)
        {
            if (breakEarly)
            {
                isJumping = false;
                yield break;
            }
        }
        yield return null;
    }

    void CheckForJump()
    {
        if (jumpPressed)
        {
            if (isGrounded) 
            {
                StartCoroutine(PerformInitialJump());
            }
            
            if (initialJumpPerformed)
            {
                if (subsequentJumpsValid && (jumpPhase <= maxJumpPhases))
                {
                    StartCoroutine(PerformSubsequentJump());
                }
            }
        }

        if (initialJumpPerformed && !jumpPressed)
        {
            subsequentJumpsValid = true;
        }

        if (isJumping && !jumpPressed)
        {
            breakEarly = true;
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
        isFalling = (!isGrounded && !jumpPressed) || rb.velocity.y <= 0.0f;
    }

    #region Collision Functions
    void OnCollisionEnter(Collision col)
    {
        isGrounded = true;
        breakEarly = false;
        initialJumpPerformed = false;
        subsequentJumpsValid = false;
        jumpPhase = 0;
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
