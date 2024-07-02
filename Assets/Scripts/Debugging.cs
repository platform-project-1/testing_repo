using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Debugging : MonoBehaviour
{
    CapsuleCollider capsuleCollider;
    PlayerInput actionMap;
    Rigidbody rb;
    StateManager stateChecker;

    [SerializeField]
    Transform resetPosition;

    //[SerializeField]
    //bool testRotations = false;

    bool pausePressed = false;

    #region Basic Functions
    void Awake()
    {
        actionMap = new PlayerInput();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        stateChecker = GetComponent<StateManager>();

        actionMap.Debug.Quit.started += QuitEditor;
        actionMap.Debug.Reset.started += ResetPosition;
        actionMap.Debug.Pause.started += OnPause;
    }

    void Update()
    {
        //if (testRotations)
        //{
        //    TestRotations();
        //}
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

    void OnPause(InputAction.CallbackContext context)
    {
        pausePressed = context.ReadValueAsButton();
        if (pausePressed) EditorApplication.isPaused = true;
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

    [SerializeField]
    bool drawCrossPattern = false;

    [SerializeField]
    bool drawSphere = false;

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
            Gizmos.color = Color.cyan;
            Vector3 direction = transform.TransformDirection(Vector3.up) * 5;
            Gizmos.DrawRay(stateChecker.hitData.hitInfo.point, stateChecker.hitData.hitInfo.normal * distance);
        }

        if (drawCrossPattern)
        {
            // Check walls in a cross pattern
            Vector3 offset = transform.TransformDirection(Vector2.one * 0.5f);
            Vector3 checkDirection = Vector3.zero;
            int k = 0;
            for (int i = 0; i < 4; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(transform.position + offset, transform.forward);
                k++;
                //RaycastHit checkHit;
                //if (Physics.Raycast(transform.position + offset,
                //                    transform.forward,
                //                    out checkHit))
                //{
                //    checkDirection += checkHit.normal;
                //    k++;
                //}
                // Rotate Offset by 90 degrees
                offset = Quaternion.AngleAxis(90f, transform.forward) * offset;
            }
            //checkDirection /= k;
        }

        if (drawSphere)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(stateChecker.hitData.hitInfo.point, 0.5f);
        }
    }
    #endregion

    #region Rotation Testing
    //[SerializeField, Range(0f, 10f)]
    //float rotationSpeed = 5f;

    //void TestRotations()
    //{
    //    //if (Gamepad.current.buttonEast.isPressed)
    //    //{
    //    //    // Rotate GameObject
    //    //    Quaternion currentRotations = transform.rotation;
    //    //    Quaternion target = Quaternion.Euler(-90f, currentRotations.y, currentRotations.z);
    //    //    transform.localRotation = target;
    //    //    //transform.localRotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed* Time.deltaTime);

    //    //    // Rotate Collider
    //    //    //The value can be 0, 1 or 2 corresponding to the X, Y and Z axes, respectively.
    //    //    capsuleCollider.direction = 2;
    //    //}
    //    //if (Gamepad.current.buttonEast.wasReleasedThisFrame)
    //    //{
    //    //    Debug.Log("check2");
    //    //    // Rotate GameObject
    //    //    Quaternion currentRotations = transform.rotation;
    //    //    Quaternion target = Quaternion.Euler(0f, currentRotations.y, currentRotations.z);
    //    //    transform.localRotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);

    //    //    // Rotate Collider
    //    //    //The value can be 0, 1 or 2 corresponding to the X, Y and Z axes, respectively.
    //    //    capsuleCollider.direction = 1;
    //    //}
    //}
    #endregion
}
