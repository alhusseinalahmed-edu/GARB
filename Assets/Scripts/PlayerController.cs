using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
{
    [Header("References")]
    [SerializeField] GameObject UI;
    [SerializeField] Image healthBarImage;
    [SerializeField] AudioSource mainSource;
    [SerializeField] Transform model;
    [SerializeField] WeaponHandler weaponHandler;
    [SerializeField] InputHandler inputHandler;
    [SerializeField] AnimatorHandler animatorHandler;
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioSource footstepsSource;
    private PlayerHandler playerHandler;

    [Header("Settings")]
    public float sprintSpeed;
    private CharacterController characterController;
    private PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    public bool isDead = false;
    public bool isMoving = false;

    #region Unitys
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
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
        Vector3 moveDir = new Vector3(inputHandler.horz, 0, inputHandler.vert);
        moveDir = transform.TransformDirection(moveDir);
        characterController.SimpleMove(moveDir * sprintSpeed);


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
