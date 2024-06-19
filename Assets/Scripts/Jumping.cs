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
    Coroutine myCoroutine;

    [SerializeField, Range(0, 5)]
    int maxJumpPhases = 2;

    private int jumpPhase = 0;

    [SerializeField, Range(0f, 10f)]
    float jumpForce = 2f;

    bool jumpPressed;
    bool jumpValid = false;
    bool onGround;
    Vector3 velocity;

    #region Basic Functions
    void Awake()
    {
        actionMap = new PlayerInput();
        rb = GetComponent<Rigidbody>();

        actionMap.Actions.Jump.started += OnJump;
        actionMap.Actions.Jump.canceled += OnJump;
    }


    void FixedUpdate()
    {
        CheckJump();
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
        velocity = rb.velocity;
        velocity.y += jumpForce;
        rb.velocity = velocity;

        jumpPhase++;
        jumpValid = false;
    }

    void CheckJump()
    {
        Debug.Log($"jumpPhase = {jumpPhase}");
        if (jumpValid)
        {
            if (jumpPressed)
            {
                if (onGround)
                {
                    PerformJump();
                }
                else if (!onGround &&
                    (jumpPhase < maxJumpPhases))
                {
                    PerformJump();
                }
            }
        }

        if (!jumpValid && !jumpPressed)
        {
            jumpValid = true;
        }
    }

    //void CheckJump()
    //{
    //    Debug.Log($"jumpPhase = {jumpPhase}");
    //    if (onGround && jumpPressed)
    //    {
    //        PerformJump();
    //    }
    //    else if (!onGround && jumpPressed && (jumpPhase <= maxJumpPhases))
    //    {
    //        PerformJump();
    //    }
    //    //Debug.Log($"onGround = {onGround}");
    //}



    #region Collision Functions
    void OnCollisionEnter(Collision col)
    {
        onGround = true;
        jumpPhase = 0;
        //EvaluateCollision(col);
    }

    void OnCollisionStay(Collision col)
    {
        onGround = true;
        //EvaluateCollision(col);
    }

    void OnCollisionExit(Collision col)
    {
        onGround = false;
    }

    void EvaluateCollision(Collision col)
    {
        //Debug.Log($"{col.gameObject.tag}");
    }
    #endregion
}
