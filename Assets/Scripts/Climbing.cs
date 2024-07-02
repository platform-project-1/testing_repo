using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Climbing : MonoBehaviour
{
    CapsuleCollider capsuleCollider;
    Rigidbody rb;
    StateManager stateChecker;

    //[SerializeField, Range(0f, 10f)]
    //float climbingSpeed = 5f;

    //[SerializeField, Range(0f, 10f)]
    //float speedBoost = 2f;

    [SerializeField]
    AnimationCurve animCurve;

    bool boostActive;
    PlayerInput actionMap;
    [HideInInspector]
    public Vector2 movementInput;
    Vector2 input;

    RaycastHit newHit;
    

    #region Basic Functions
    void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        stateChecker = GetComponent<StateManager>();

        newHit = stateChecker.hitData.hitInfo;

        actionMap = new PlayerInput();
        actionMap.Movement.Move.started += OnMove;
        actionMap.Movement.Move.performed += OnMove;
        actionMap.Movement.Move.canceled += OnMove;

        // Currently, boostActive only shows true when button is held.
        // Unsure how to make value true on press.
        //actionMap.Actions.Boost.started += OnBoost;
        actionMap.Actions.Boost.performed += OnBoost;
        actionMap.Actions.Boost.canceled += OnBoost;
    }

    void Update()
    {
        input = SquareToCircle(movementInput);
    }

    void FixedUpdate()
    {
        SquareToCircle(movementInput);
        HandleClimbing();
    }
    #endregion

    void HandleClimbing()
    {
        RaycastHit hit;
        //Quaternion RotationRef = Quaternion.Euler(0f, 0f, 0f);
        //Debug.DrawRay(transform.position, -transform.up);

        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            Debug.DrawRay(transform.position, -hit.normal, Color.magenta);
            transform.rotation = Quaternion.Euler(hit.normal.x, hit.normal.y, hit.normal.z);
            //Debug.Log($"{hit.normal}");
            Vector3 velocity = Vector3.zero;
            velocity.y = -hit.normal.y * 0.05f;
            rb.velocity = velocity;
            //RotationRef = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, info.normal), 
            //    animCurve.Evaluate(Time.time));
            //transform.rotation = Quaternion.Euler(RotationRef.eulerAngles.x, transform.eulerAngles.y, RotationRef.eulerAngles.z);
        }

        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward, out hit))
        //{
        //    transform.forward = -hit.normal;
        //    //transform.up = hit.normal;
        //    rb.position = Vector3.Lerp(rb.position,
        //        hit.point + hit.normal * 0.51f,
        //        10f * Time.fixedDeltaTime);
        //}

        //rb.velocity = boostActive ?
        //    transform.TransformDirection(input) * climbingSpeed * speedBoost : 
        //    transform.TransformDirection(input) * climbingSpeed;
    }

    #region Input Functions
    void OnEnable()
    {
        actionMap.Enable();
    }

    void OnDisable()
    {
        actionMap.Disable();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    void OnBoost(InputAction.CallbackContext context)
    {
        boostActive = context.ReadValueAsButton();
        //Debug.Log($"boostActive = {boostActive}");
    }
    #endregion

    Vector2 SquareToCircle(Vector2 input)
    {
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
    }
}

