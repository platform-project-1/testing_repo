using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jumping : MonoBehaviour
{
    ApplyGravity gravityScript;
    PlayerInput actionMap;
    Rigidbody rb;

    [SerializeField, Range(0, 5)]
    int maxJumpPhases = 2;

    private int jumpPhase = 1;

    bool jumpPressed = false;
    bool initialJumpPerformed = false;
    bool subsequentJumpsValid = false;
    bool breakEarly = false;
    bool isJumping = false;
    Vector3 velocity;

    #region Basic Functions
    void Awake()
    {
        gravityScript = GetComponent<ApplyGravity>();
        actionMap = new PlayerInput();

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        actionMap.Actions.Jump.started += OnJump;
        actionMap.Actions.Jump.canceled += OnJump;
    }

    void FixedUpdate()
    {
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

    IEnumerator PerformInitialJump()
    {
        // Logic to add velocity to initial jump.
        velocity = rb.velocity;
        velocity.y = gravityScript.jumpVelocity;
        rb.velocity = velocity;

        for (float timer = gravityScript.maxJumpTime; timer >= 0; timer -= Time.deltaTime)
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
        velocity.y = gravityScript.jumpVelocity;
        rb.velocity = velocity;

        isJumping = true;
        subsequentJumpsValid = false;
        for (float timer = gravityScript.maxJumpTime; timer >= 0; timer -= Time.deltaTime)
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
            if (gravityScript.isGrounded) 
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

    #region Collision Functions
    void OnCollisionEnter(Collision col)
    {
        breakEarly = false;
        initialJumpPerformed = false;
        subsequentJumpsValid = false;
        jumpPhase = 0;
    }
    #endregion
}
