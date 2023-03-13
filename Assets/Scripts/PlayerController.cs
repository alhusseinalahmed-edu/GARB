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

    #region Unitys
    private void Awake()
    {
        playerHandler = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerHandler>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
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

    }
    #endregion

    #region Functions


    private void Move()
    {
        // Simple Movement
        Vector3 moveDir = new Vector3(inputHandler.horz, 0, inputHandler.vert);
        moveDir = transform.TransformDirection(moveDir);
        characterController.SimpleMove(moveDir * sprintSpeed);

        // Footsteps
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
