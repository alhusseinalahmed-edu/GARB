using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class MovementHandler : MonoBehaviour
{
    public float moveSpeed = 4f;
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [SerializeField] PhotonView PV;
    [SerializeField] InputHandler inputHandler;
    [SerializeField] CharacterController controller;

    public bool isMoving;
    private Vector3 velocity;
    private bool isGrounded;

    private void Update()
    {
        if (!PV.IsMine) return;
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = inputHandler.horz;
        float z = inputHandler.vert;

        Vector3 moveDir = transform.right * x + transform.forward * z;
        moveDir.Normalize();

        if (inputHandler.isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        if (inputHandler.isRunning)
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        controller.Move(moveDir * moveSpeed * Time.deltaTime);
        // Apply gravity to the character controller
        controller.Move(velocity * Time.deltaTime);

        velocity = controller.velocity;
    }
}
