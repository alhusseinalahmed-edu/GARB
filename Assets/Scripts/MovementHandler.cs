using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class MovementHandler : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float maxVelocity = 5f;
    public float jumpVelocity = 5f;

    [SerializeField] PhotonView PV;
    [SerializeField] InputHandler inputHandler;
    [SerializeField] AnimatorHandler animatorHandler;

    public CharacterController controller;

    public bool isMoving;
    private float verticalVelocity;


    public void OnWeaponEquip(float speedMultiplier)
    {
        maxVelocity = baseSpeed * speedMultiplier;
    }
    private void Update()
    {
        if (!PV.IsMine) return;
        Move();
    }
    private void Move()
    {
        isMoving = controller.velocity.magnitude > 0;
        verticalVelocity -= 9 * Time.deltaTime;
        if (controller.isGrounded)
        {
            animatorHandler.SetBool("isGrounded", true);
            verticalVelocity = -5f;
            if (inputHandler.isJumping)
            {
                animatorHandler.SetBool("isGrounded", false);
                verticalVelocity = jumpVelocity;
            }
        }
        else if (controller.velocity.y == 0)
        {
            verticalVelocity = controller.velocity.y - 9 * Time.deltaTime;
        }
        Vector3 localVelocity = controller.velocity;
        Vector2 velocity = PlaneVelocity(new Vector2(localVelocity.x, localVelocity.z));
        Vector3 worldVelocity = new Vector3(velocity.x, verticalVelocity, velocity.y);
        controller.Move(worldVelocity * Time.deltaTime);
    }

    private Vector2 PlaneVelocity(Vector2 velocity)
    {
        Vector3 localDirection = transform.rotation * new Vector3(inputHandler.horz, 0, inputHandler.vert).normalized;
        Vector2 wishDirection = new Vector2(localDirection.x, localDirection.z);
        float currentMaxVelocity = .65f;
        if (controller.isGrounded)
        {
            currentMaxVelocity = maxVelocity;
            velocity = Friction(velocity);
        }

        float currentVelocity = Vector2.Dot(velocity, wishDirection);
        float acceleration = Mathf.Clamp(currentMaxVelocity - currentVelocity, 0, maxVelocity * 10 * Time.deltaTime);

        return velocity + acceleration * wishDirection;
    }

    private Vector2 Friction(Vector2 velocity)
    {
        float friction = velocity.magnitude * 5f;
        float actualFriction = friction < 15 ? 15 : friction;
        Vector2 newVelocity = velocity - (velocity.normalized * actualFriction * Time.deltaTime);
        if (Vector2.Dot(newVelocity, velocity) < 0)
        {
            return Vector2.zero;
        }
        else
        {
            return newVelocity;
        }
    }

}
