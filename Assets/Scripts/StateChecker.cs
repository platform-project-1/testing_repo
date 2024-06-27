using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateChecker : MonoBehaviour
{
    ApplyGravity applyGravity;
    Climbing climbing;
    GroundMovement groundMovement;
    Jumping jumping;

    [SerializeField, Range(0f, 30f)]
    float rayLength = 1f;

    Vector3 rayOffset = new Vector3(0f, 0.5f, 0f);
    public LayerMask wallLayer;

    void Awake()
    {
        applyGravity = GetComponent<ApplyGravity>();
        climbing = GetComponent<Climbing>();
        groundMovement = GetComponent<GroundMovement>();
        jumping = GetComponent<Jumping>();
    }

    void Update()
    {
        if (CheckForWall())
        {
            applyGravity.enabled = false;
            groundMovement.enabled = false;
            jumping.enabled = false;
            climbing.enabled = true;
        }
    }

    bool CheckForWall()
    {
        var hitData = new WallData();

        var rayOrigin = transform.position + rayOffset;
        hitData.hitFound = Physics.Raycast(rayOrigin, transform.forward, out hitData.hitInfo, rayLength, wallLayer);

        Debug.DrawRay(rayOrigin, transform.forward * rayLength, (hitData.hitFound) ? Color.red : Color.green);

        if (hitData.hitFound)
        {
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
