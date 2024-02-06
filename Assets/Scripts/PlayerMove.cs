using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementState
{
    Walking,
    Running
}


public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 250f;
    [SerializeField] private float maxWalkSpeed = 400;
    [SerializeField] private float bonusSpeed = 100f;
    [SerializeField] private float minSpeed;
    private Vector2 moveInput;
    private Rigidbody rb;
    private MovementState movementState = MovementState.Walking;


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        minSpeed = walkSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        Run();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            movementState = MovementState.Running;

        else if (Input.GetKeyUp(KeyCode.LeftShift)) movementState = MovementState.Walking;

        switch (movementState)
        {
            case MovementState.Running:
                walkSpeed += bonusSpeed;
                break;
            case MovementState.Walking:
                walkSpeed = minSpeed;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (walkSpeed >= maxWalkSpeed) walkSpeed = maxWalkSpeed;
    }

    private void Run()
    {
        var playerVelocity = new Vector3(moveInput.x * walkSpeed, rb.velocity.y, moveInput.y * walkSpeed);
        rb.velocity = transform.TransformDirection(playerVelocity);
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}