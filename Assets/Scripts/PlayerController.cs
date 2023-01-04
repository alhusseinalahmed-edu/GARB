using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviourPunCallbacks
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
    private PlayerHandler playerHandler;
    private Manager manager;

    [Header("Settings")]
    [SerializeField] float sprintSpeed;
    private CharacterController characterController;
    private PhotonView PV;

    const float maxHealth = 100f;
    public float currentHealth = maxHealth;
    public bool isDead = false;
    public bool isMoving = false;

    #region Unitys
    private void Awake()
    {
        //playerHandler = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerHandler>();
    }
    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        characterController = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        }
        else
        {
            isMoving = false;
        }
    }
    private void Die()
    {
        //GetComponentInChildren<SetupRagdoll>().Setup(true);
        //playerHandler.Die();
        manager.SpawnPlayer();
        PhotonNetwork.Destroy(gameObject);
        
    }
    public void PlayHitSound()
    {
        mainSource.PlayOneShot(hitClip);
    }
    #endregion

    #region RPCs/Photon

    public void TakeDamage(int damage, int actor)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }
    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        if (!PV.IsMine) return;
        currentHealth -= damage;
        healthBarImage.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            Die();
            //PlayerHandler.Find(info.Sender).GetKill();
        }

    }

    #endregion
}
