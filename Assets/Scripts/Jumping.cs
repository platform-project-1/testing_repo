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
    float maxJumpTime = 0.5f, maxJumpHeight = 1.75f;

    //[SerializeField, Range(-100f, 0f)]
    //float gravity = -9.81f;

    float groundedGravity = -0.05f;

    bool jumpPressed;
    //bool jumpValid = false;
    private bool isJumping;
    private bool jumpValid = false;
    bool isGrounded;
    bool isFalling;
    float jumpGravity;
    float jumpVelocity;
    Vector3 velocity;
    Coroutine jumpTime;

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

    void Update()
    {
        //Debug.Log($"jumpPhase = {jumpPhase}");
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
        //if (jumpPressed)
        //{
        //    jumpPhase++;
        //}
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

    //IEnumerator TestJumpButtonRelease()
    //{

    //    Debug.Log("Before yield");
    //    yield return new WaitUntil(() => jumpPressed == false);
    //    Debug.Log("After yield");
    //    jumpPhase++;
    //}
    //IEnumerator JumpPress()
    //{
        // USE TO KEEP TRACK OF BUTTON PRESS AND RELEASE FOR JUMP PHASE
    //}

    IEnumerator JumpTime()
    {
        isJumping = true;
        velocity = rb.velocity;
        velocity.y = jumpVelocity;
        rb.velocity = velocity;
        yield return new WaitForSeconds(maxJumpTime);
        isJumping = false;
        jumpPhase++;
    }

    void CheckForJump()
    {
        if (jumpPressed && jumpValid)
        {
            if (isGrounded)
            {
                jumpTime = StartCoroutine(JumpTime());
            }
            else if (!isGrounded &&
                (jumpPhase < maxJumpPhases))
            {
                jumpTime = StartCoroutine(JumpTime());
            }
        }

        if (isJumping && !jumpPressed)
        {
            StopCoroutine(jumpTime);
            jumpValid = false;
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
        isFalling = (!isGrounded && !jumpPressed) || rb.velocity.y <= 0.0f;
    }

    #region Collision Functions
    void OnCollisionEnter(Collision col)
    {
        isGrounded = true;
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
