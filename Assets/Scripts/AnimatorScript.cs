using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    Animator animator;
    Climbing climbing;
    CustomGravity gravity;
    GroundMovement groundMovement;
    Jumping jumping;
    StateChecker stateChecker;

    int moveMagHash;
    int currentAnimation;

    readonly int IDLE_WALK_RUN = Animator.StringToHash("Idle/Walk/Run");
    readonly int JUMPING_UP = Animator.StringToHash("Jumping Up");
    readonly int FALLING_IDLE = Animator.StringToHash("Falling Idle");
    readonly int CLIMBING = Animator.StringToHash("Climbing");

    void Awake()
    {
        animator = GetComponent<Animator>();
        climbing = GetComponent<Climbing>();
        gravity = GetComponent<CustomGravity>();
        groundMovement = GetComponent<GroundMovement>();
        jumping = GetComponent<Jumping>();
        stateChecker = GetComponent<StateChecker>();

        currentAnimation = IDLE_WALK_RUN;
        animator.CrossFade(currentAnimation, 0.2f);
        moveMagHash = Animator.StringToHash("MoveMag");
    }

    void Update()
    {
        CheckAnimation();
    }

    void CheckAnimation()
    {
        if (stateChecker.state == StateChecker.PlayerState.GROUNDED)
        {
            float input = groundMovement.movementInput.magnitude;
            animator.SetFloat(moveMagHash, Mathf.Abs(input));
            if (currentAnimation == CLIMBING) ChangeAnimation(IDLE_WALK_RUN);
            
            if (currentAnimation == JUMPING_UP)
            {
                if (gravity.isFalling) ChangeAnimation(FALLING_IDLE);
                else if (gravity.isGrounded) ChangeAnimation(IDLE_WALK_RUN);
                else return;
            }
            else if (currentAnimation == FALLING_IDLE)
            {
                if (gravity.isGrounded) ChangeAnimation(IDLE_WALK_RUN);
                else if (jumping.jumpPressed) ChangeAnimation(JUMPING_UP);
                else return;
            }
            else if (currentAnimation == IDLE_WALK_RUN)
            {
                if (jumping.jumpPressed) ChangeAnimation(JUMPING_UP);
                else if (gravity.isFalling) ChangeAnimation (FALLING_IDLE);
            }
        }

        if (stateChecker.state == StateChecker.PlayerState.CLIMBING)
        {
            float input = climbing.movementInput.magnitude;
            animator.SetFloat(moveMagHash, Mathf.Abs(input));
            if (currentAnimation != CLIMBING) ChangeAnimation(CLIMBING);
            else return;
        }
    }

    public void ChangeAnimation(int animation, float crossfade = 0.2f)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
        }
    }
}
