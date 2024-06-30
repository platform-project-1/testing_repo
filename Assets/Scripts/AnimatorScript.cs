using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    Animator animator;
    CustomGravity gravity;
    GroundMovement groundMovement;
    Jumping jumping;

    int isFallingHash;
    int isGroundedHash;
    int jumpPressedHash;
    int moveMagHash;

    void Awake()
    {
        animator = GetComponent<Animator>();
        gravity = GetComponent<CustomGravity>();
        groundMovement = GetComponent<GroundMovement>();
        jumping = GetComponent<Jumping>();

        isGroundedHash = Animator.StringToHash("isGrounded");
        isFallingHash = Animator.StringToHash("isFalling");
        jumpPressedHash = Animator.StringToHash("jumpPressed");
        moveMagHash = Animator.StringToHash("MoveMag");
    }

    void Update()
    {
        HandleAnimation();
    }

    void HandleAnimation()
    {
        float input = groundMovement.movementInput.magnitude;

        animator.SetFloat(moveMagHash, Mathf.Abs(input));
        animator.SetBool(isFallingHash, gravity.isFalling);
        animator.SetBool(isGroundedHash, gravity.isGrounded);
        animator.SetBool(jumpPressedHash, jumping.jumpPressed);
    }
}
