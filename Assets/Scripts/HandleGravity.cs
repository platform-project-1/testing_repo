using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleGravity : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField, Range(-100f, 0f)]
    float gravity = -9.8f;

    [SerializeField, Range(1f, 10f)]
    float fallMultiplier = 2f;

    public bool isGrounded;
    public bool isFalling = false;
    Vector3 velocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        //CheckFalling();
        //ApplyGravity();
    }

    void CheckFalling()
    {
        if (rb.velocity.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }
    void ApplyGravity()
    {
        velocity = rb.velocity;
        float previousYVelocity = velocity.y;
        float newYVelocity = 0f;
        if (isFalling) 
        {
            previousYVelocity = previousYVelocity + (gravity * fallMultiplier * Time.deltaTime);
            newYVelocity = Mathf.Max((previousYVelocity + velocity.y) * 0.5f, -20f);
        }
        else
        {
            previousYVelocity = previousYVelocity + (gravity * Time.deltaTime);
            newYVelocity = Mathf.Max((previousYVelocity + velocity.y) * 0.5f, -20f);
        }
        velocity.y = newYVelocity;
        rb.velocity = velocity;
    }


}
