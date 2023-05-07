using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
{
    private PlayerHandler playerHandler;
    [Header("References")]
    [SerializeField] CharacterController characterController;
    [SerializeField] WeaponHandler weaponHandler;
    [SerializeField] InputHandler inputHandler;
    [SerializeField] AnimatorHandler animatorHandler;
    [SerializeField] PhotonView PV;
    [SerializeField] Transform model;
    [SerializeField] GameObject UI;
    [SerializeField] Image healthBarImage;

    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioSource mainSource;
    [SerializeField] AudioSource footstepsSource;

    public GameObject hitmarker;
    [Header("Settings")]
    public float sprintSpeed;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isMoving = false;
    private float verticalVelocity;
    public float maxVelocity = 5f;
    public float defaultVelocity = 5f;

    #region Unitys
    private void Awake()
    {
        playerHandler = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerHandler>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        Application.targetFrameRate = 144;
        if (PV.IsMine)
        {
            model.gameObject.SetActive(false);
        }
        else
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            GetComponent<RecoilHandler>().enabled = false;
            Destroy(UI);
        }
        GetComponentInChildren<SetupRagdoll>().Setup(true);
        GetComponent<Collider>().enabled = true;
    }
    private void Update()
    {
        if (!PV.IsMine) return;
        Move();
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Die();
        }

    }
    #endregion

    #region Functions


    private void Move()
    {
        verticalVelocity -= 9 * Time.deltaTime;
        if (characterController.isGrounded)
        {
            verticalVelocity = -5f;
            if (Input.GetKey(KeyCode.Space))
            {
                verticalVelocity = 5;
            }
        }
        else if (characterController.velocity.y == 0)
        {
            verticalVelocity = characterController.velocity.y - 9 * Time.deltaTime;
        }
        Vector3 localVelocity = characterController.velocity;
        Vector2 velocity = PlaneVelocity(new Vector2(localVelocity.x, localVelocity.z));
        Vector3 worldVelocity = new Vector3(velocity.x, verticalVelocity, velocity.y);
        characterController.Move(Time.deltaTime * worldVelocity);

        if (characterController.velocity.magnitude > 1f)
        {
            isMoving = true;
            PV.RPC("FootSteps_RPC", RpcTarget.All, isMoving);
        }
        else
        {
            isMoving = false;
            PV.RPC("FootSteps_RPC", RpcTarget.All, isMoving);
        }
    }

    private Vector2 PlaneVelocity(Vector2 velocity)
    {
        Vector3 localDirection = transform.rotation * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector2 wishDirection = new Vector2(localDirection.x, localDirection.z);
        float currentMaxVelocity = .65f;
        if (characterController.isGrounded)
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

    [PunRPC]
    private void FootSteps_RPC(bool isMoving)
    {
        if (isMoving)
        {
            footstepsSource.enabled = true;
        }
        else
        {
            footstepsSource.enabled = false;
        }
    }
    private void Die()
    {
        GetComponentInChildren<SetupRagdoll>().Setup(true);
        playerHandler.Die();
    }
    public void PlayHitSound()
    {
        mainSource.PlayOneShot(hitClip);

        // Hitmarker Effect
        hitmarker.SetActive(true);
        Invoke("HideHitMarker", 0.1f);
    }
    void HideHitMarker()
    {
        hitmarker.SetActive(false);
    }
    #endregion

    #region RPCs/Photon

    public void TakeDamage(int damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }
    [PunRPC]
    void RPC_TakeDamage(int damage, PhotonMessageInfo info)
    {
        if (!PV.IsMine) return;
        currentHealth -= damage;
        healthBarImage.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            Die();
            PlayerHandler.Find(info.Sender).GetKill();
        }

    }

    #endregion
}
