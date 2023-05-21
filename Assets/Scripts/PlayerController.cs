using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
{
    private PlayerHandler playerHandler;
    [Header("Components")]
    [SerializeField] CharacterController characterController;
    [SerializeField] WeaponHandler weaponHandler;
    [SerializeField] InputHandler inputHandler;
    [SerializeField] AnimatorHandler animatorHandler;
    [SerializeField] RagdollHandler ragdollHandler;
    [SerializeField] PhotonView PV;
    [SerializeField] AudioSource playerAudioSource;

    [Header("References")]
    [SerializeField] Transform model;
    [SerializeField] GameObject UI;
    [SerializeField] AudioClip hitClip;
    [SerializeField] GameObject hitmarker;
    [SerializeField] TMP_Text healthText;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    [HideInInspector] public bool isDead = false;

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
            healthText.text = currentHealth.ToString();
        }
        else
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            GetComponent<RecoilHandler>().enabled = false;
            Destroy(UI);
        }
        ragdollHandler.Setup(false);
        GetComponent<Collider>().enabled = true;
    }
    
    public void Heal()
    {
        PV.RPC("RPC_Heal", RpcTarget.All);
    }
    [PunRPC]
    void RPC_Heal()
    {
        currentHealth = maxHealth;

        if (PV.IsMine)
        {
            healthText.text = currentHealth.ToString();
        }

    }
    public void PlayHitSound()
    {
        playerAudioSource.PlayOneShot(hitClip);

        // Hitmarker Effect
        hitmarker.SetActive(true);
        Invoke("HideHitMarker", 0.1f);
    }
    void HideHitMarker()
    {
        hitmarker.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }
    [PunRPC]
    void RPC_TakeDamage(int damage, PhotonMessageInfo info)
    {
        if (!PV.IsMine) return;
        currentHealth -= damage;
        healthText.text = currentHealth.ToString();
        if (currentHealth <= 0)
        {
            Die();
            PlayerHandler.Find(info.Sender).GetKill();
        }

    }
    public void Die()
    {
        ragdollHandler.Die();
        playerHandler.Die();
    }

    public void LeaveRoom()
    {
        GameManager.Instance.LeaveRoom();
    }
}
