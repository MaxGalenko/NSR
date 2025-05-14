using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    public Transform groundCheck;
    public LayerMask groundMask;

    [Header("Movement Settings")]
    private float moveSpeed = 7f;
    private float airControlMultiplier = 0.6f;
    private float acceleration = 10f;

    [Header("Jump Settings")]
    private float jumpForce = 6f;
    private float jumpCooldown = 0.2f;
    private bool readyToJump = true;

    [Header("Look Settings")]
    private float mouseSensitivity = 50f;

    [Header("Ground Check")]
    private float groundCheckDistance = 0.3f;

    // Internal state
    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookAction;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isGrounded;
    private float xRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        lookAction = playerInput.actions["Look"];
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        moveInput = moveAction.ReadValue<Vector2>();
        if (jumpAction.WasPerformedThisFrame()) AttemptJump();
    }

    private void FixedUpdate()
    {
        CheckGround();
        HandleMovement();
    }

    private void HandleLook()
    {
        lookInput = lookAction.ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;

        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x);
    }

    private void HandleMovement()
    {
        Vector3 inputDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        inputDirection.Normalize();

        float multiplier = isGrounded ? 1f : airControlMultiplier;
        Vector3 targetVelocity = inputDirection * moveSpeed * multiplier;

        Vector3 velocityChange = (targetVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z)) * acceleration;
        velocityChange.y = 0f;

        rb.AddForce(velocityChange, ForceMode.Acceleration);
    }

    private void AttemptJump()
    {
        if (!readyToJump || !isGrounded) return;

        readyToJump = false;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f; // reset vertical velocity
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundMask);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}