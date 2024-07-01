using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class StateChecker : MonoBehaviour
{
    public enum PlayerState
    {
        GROUNDED,
        CLIMBING
    }

    public PlayerState state;
    PlayerState currentState;

    Animation anim;
    BoxCollider boxCollider;
    CustomGravity applyGravity;
    Climbing climbing;
    CapsuleCollider capsuleCollider;
    GroundMovement groundMovement;
    Jumping jumping;
    Rigidbody rb;

    [HideInInspector]
    public WallData hitData = new WallData();
    Vector3 rayDirection;

    [SerializeField, Range(0f, 30f)]
    float rayLength = 1f;

    Vector3 rayOffset = new Vector3(0f, 1f, 0f);
    public LayerMask wallLayer;

    void Awake()
    {
        anim = GetComponent<Animation>();
        applyGravity = GetComponent<CustomGravity>();
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        climbing = GetComponent<Climbing>();
        groundMovement = GetComponent<GroundMovement>();
        jumping = GetComponent<Jumping>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;

        // Initialize player in grounded state
        state = PlayerState.GROUNDED;
        currentState = state;
        SetGroundedState();
    }

    void Update()
    {
        //Debug.Log($"TransformDirection = {transform.TransformDirection(Vector2.one * 0.5f)}");
        CheckState();
    }

    void CheckState()
    {
        //Debug.Log($"state check1 = {state}");
        if (currentState == PlayerState.GROUNDED)
        {
            if (CheckForWall()) 
            {
                //Debug.Log($"state check2 = {state}");
                state = PlayerState.CLIMBING;
                ChangeState(state);
            }
        }
        else if (currentState == PlayerState.CLIMBING) 
        {
            if (!CheckForWall())
            {
                //Debug.Log($"state check3 = {state}");
                state = PlayerState.GROUNDED;
                ChangeState(state);
            }
        }
    }

    void ChangeState(PlayerState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            if (newState == PlayerState.GROUNDED) SetGroundedState();
            else if (newState == PlayerState.CLIMBING) SetClimbingState();
            StartCoroutine(WaitForChange());
        }
        else return;
    }

    IEnumerator WaitForChange()
    {
        yield return new WaitForSeconds(0.5f);
    }

    void SetGroundedState()
    {
        // Enable/Disable Components
        climbing.enabled = false;
        applyGravity.enabled = true;
        boxCollider.enabled = true;
        groundMovement.enabled = true;
        jumping.enabled = true;

        // Rotate GameObject
        Quaternion currentRotations = transform.rotation;
        Quaternion target = Quaternion.Euler(0f, currentRotations.y, currentRotations.z);
        transform.localRotation = target;
        //transform.localRotation = Quaternion.Slerp(transform.rotation, target, 0f);

        // Rotate Collider
        //The value can be 0, 1 or 2 corresponding to the X, Y and Z axes, respectively.
        capsuleCollider.direction = 1;
    }

    void SetClimbingState()
    {
        // Enable/Disable Components
        state = PlayerState.CLIMBING;
        rb.velocity = Vector3.zero;
        applyGravity.enabled = false;
        boxCollider.enabled = false;
        groundMovement.enabled = false;
        jumping.enabled = false;
        //climbing.enabled = true;

        // Rotate GameObject
        //Quaternion currentRotations = transform.rotation;
        //Quaternion target = Quaternion.Euler(-90f, currentRotations.y, currentRotations.z);
        //transform.localRotation = target;
        //transform.localRotation = Quaternion.Slerp(transform.rotation, target, 0f);

        // Rotate Collider
        //The value can be 0, 1 or 2 corresponding to the X, Y and Z axes, respectively.
        capsuleCollider.direction = 2;
    }

    bool CheckForWall()
    {
        rayDirection = transform.forward;
        //var hitData = new WallData();
        var rayOrigin = transform.position + rayOffset;

        if (state == PlayerState.GROUNDED) rayDirection = transform.forward;
        else if (state == PlayerState.CLIMBING) rayDirection = -transform.up;

        hitData.hitFound = Physics.Raycast(rayOrigin, rayDirection, out hitData.hitInfo, rayLength, wallLayer);

        Debug.DrawRay(rayOrigin, transform.forward * rayLength, (hitData.hitFound) ? Color.red : Color.green);

        if (hitData.hitFound)
        {
            transform.up = hitData.hitInfo.normal;
            transform.position = hitData.hitInfo.point;
            return true;
            // COMMENTED CODE BELOW WILL BE USED FOR DETECTING CLIMBABLE WALLS BASED ON HEIGHT
            //hitData.targetHeight = hitData.hitInfo.transform.gameObject.GetComponent<Collider>().bounds.size;
            //Debug.Log($"Target Height = {hitData.targetHeight}");
        }
        return false;
    }
}

public struct WallData
{
    public bool hitFound;
    public RaycastHit hitInfo;
    public Vector3 targetHeight;
}
