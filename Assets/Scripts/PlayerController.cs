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

    private PlayerHandler playerHandler;

    [Header("Settings")]
    [SerializeField] float sprintSpeed, smoothTime, jumpForce;

    private Rigidbody rb;
    private PhotonView PV;
    private Animator anim;


    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    public int doubleJump = 1;

    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    public bool isDead = false;
    public bool Grounded;

    #region Unitys
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
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
            Destroy(rb);
            Destroy(UI);
        }
        GetComponentInChildren<SetupRagdoll>().Setup(true);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
    }
    private void Update()
    {
        if (!PV.IsMine) return;
        Move();

    }
    private void FixedUpdate()
    {
        if (!PV.IsMine) return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    #endregion

    #region Functions


    private void Move()
    {
        Vector3 moveDir = new Vector3(inputHandler.horz, 0, inputHandler.vert).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir *  sprintSpeed, ref smoothMoveVelocity, smoothTime);

    }
    private void Die()
    {
        GetComponentInChildren<SetupRagdoll>().Setup(true);
        playerHandler.Die();
    }

    public void Jump()
    {
        if (Grounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animatorHandler.CrossFadeInFixedTime("Jump", 0.05f);
        }
        else if(!Grounded && doubleJump != 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animatorHandler.CrossFadeInFixedTime("Jump", 0.05f);
            doubleJump = 0;
        }
        else if (!Grounded)
        {
            return;
        }
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
