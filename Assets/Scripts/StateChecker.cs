using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateChecker : MonoBehaviour
{
    public enum PlayerState
    {
        GROUNDED,
        CLIMBING
    }

    public PlayerState state;

    BoxCollider boxCollider;
    CustomGravity applyGravity;
    Climbing climbing;
    GroundMovement groundMovement;
    Jumping jumping;

    [SerializeField, Range(0f, 30f)]
    float rayLength = 1f;

    Vector3 rayOffset = new Vector3(0f, 0.5f, 0f);
    public LayerMask wallLayer;

    private bool initialRotations = false;

    void Awake()
    {
        applyGravity = GetComponent<CustomGravity>();
        boxCollider = GetComponent<BoxCollider>();
        climbing = GetComponent<Climbing>();
        groundMovement = GetComponent<GroundMovement>();
        jumping = GetComponent<Jumping>();
    }

    void Update()
    {
        if (CheckForWall())
        {
            state = PlayerState.CLIMBING;
            applyGravity.enabled = false;
            boxCollider.enabled = false;
            climbing.enabled = true;
            groundMovement.enabled = false;
            jumping.enabled = false;
        }
        else
        {
            state = PlayerState.GROUNDED;
            applyGravity.enabled = true;
            boxCollider.enabled = true;
            climbing.enabled = false;
            groundMovement.enabled = true;
            jumping.enabled = true;
        }
    }

    void SetClimbingRotation()
    {
        Quaternion target = Quaternion.Euler(-90f, 0f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime);
    }

    void SetStandardRotation() 
    {
        Quaternion target = Quaternion.Euler(0f, 0f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime);
    }

    bool CheckForWall()
    {
        var hitData = new WallData();

        var rayOrigin = transform.position + rayOffset;
        hitData.hitFound = Physics.Raycast(rayOrigin, transform.forward, out hitData.hitInfo, rayLength, wallLayer);

        Debug.DrawRay(rayOrigin, transform.forward * rayLength, (hitData.hitFound) ? Color.red : Color.green);

        if (hitData.hitFound)
        {
            //SetClimbingRotation();
            return true;
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
