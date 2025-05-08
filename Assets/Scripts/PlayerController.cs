using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;

    private float speed = 5;
    private float jumpForce = 5f;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        jumpAction = playerInput.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpAction.IsPressed())
        {
            JumpPlayer();
        }
        Physics.gravity = new Vector3(0, -1f, 0);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        transform.position += new Vector3(direction.x, 0, direction.y) * Time.deltaTime * speed;
    }

    void JumpPlayer()
    {
        transform.position += new Vector3(0, 5f, 0) * Time.deltaTime;
    }
}
