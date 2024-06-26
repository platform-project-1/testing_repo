using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Debugging : MonoBehaviour
{
    PlayerInput actionMap;
    Rigidbody rb;

    [SerializeField]
    Transform resetPosition;

    #region Basic Functions
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        actionMap = new PlayerInput();

        actionMap.Debug.Quit.started += QuitEditor;
        actionMap.Debug.Reset.started += ResetPosition;
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

    void QuitEditor(InputAction.CallbackContext context)
    {
        EditorApplication.isPlaying = false;
    }

    void ResetPosition(InputAction.CallbackContext context)
    {
        this.transform.position = resetPosition.position;
        transform.localRotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
    }
    #endregion

    #region Gizmos
    [Header("Gizmos")]
    [SerializeField, Range(0f, 10f)]
    float distance = 1f;

    [SerializeField]
    Gizmos color;

    [SerializeField]
    bool drawLineOn = false;
    
    [SerializeField]
    bool drawRayOn = false;
    void OnDrawGizmos()
    {
        if (drawLineOn)
        {
            Vector3 position = transform.position;
            Vector3 direction = transform.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(position, direction * distance);
        }

        if (drawRayOn)
        {
            // Draws a 5 unit long red line in front of the object
            Gizmos.color = Color.red;
            Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
            Gizmos.DrawRay(transform.position, direction);
        }
    }
    #endregion
}
