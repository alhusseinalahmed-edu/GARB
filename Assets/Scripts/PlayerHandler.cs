using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHandler : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    GameObject controller;
    int kills = 0;

    public float countdownDuration = 3f;
    public TMP_Text countdownText;
    private float currentTime;
    private bool is_counting;

    public float ragdollDestroyTimer = 10f;

    public GameObject ragdoll;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
        currentTime = countdownDuration;
    }

    void Update()
    {
        // Countdown
        if (is_counting)
        {
            countdownText.gameObject.SetActive(true);
            currentTime -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);

            countdownText.text = "Respawning in => " + string.Format("{0:0}:{1:00}", minutes, seconds);

            if (currentTime <= 0f)
            {
                currentTime = countdownDuration;
                // Do something when the countdown reaches zero
                CreateController();
                is_counting = false;
                countdownText.gameObject.SetActive(false);
            }

        }
    }

    void CreateController()
    {
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint(); ;
        controller = PhotonNetwork.Instantiate(Path.Combine("Photon", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { PV.ViewID });
        controller.GetComponent<WeaponHandler>().Equip(kills);
    }

    public void Die()
    {
        if (is_counting) return;
        PhotonNetwork.Destroy(controller);
        is_counting = true;
    }

    [PunRPC]
    void RagdollCorpse()
    {        
        if(PV.IsMine)
        {
            Camera.main.transform.position = ragdoll.transform.position + new Vector3(0, 15f, -2f);
            Camera.main.transform.LookAt(ragdoll.transform.position);
        }
        Destroy(ragdoll, ragdollDestroyTimer);
    }   

    public void GetKill()
    {
        PV.RPC("RPC_GetKill", PV.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;
        GameManager.Instance.CheckKills(kills, PhotonNetwork.LocalPlayer.NickName);
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        controller.GetComponent<WeaponHandler>().Equip(kills);
    }

    public static PlayerHandler Find(Player player)
    {
        return FindObjectsOfType<PlayerHandler>().SingleOrDefault(x => x.PV.Owner == player);
    }


}
