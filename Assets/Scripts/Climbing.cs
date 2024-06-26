using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Climbing : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;



    PlayerInput actionMap;
    Vector2 movementInput;

    Rigidbody rb;

    #region Basic Functions
    void Awake()
    {
        actionMap = new PlayerInput();

        actionMap.Movement.Move.started += OnMove;
        actionMap.Movement.Move.performed += OnMove;
        actionMap.Movement.Move.canceled += OnMove;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
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

    void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }
    #endregion


}

