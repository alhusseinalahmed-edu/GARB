using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class PlayerHandler : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;
    int kills = 0;

    public float countdownDuration = 3f;
    public TMP_Text countdownText;
    private float currentTime;
    private bool is_counting;

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
        if (Input.GetKeyDown(KeyCode.V)) { Die(); }
    }

    void CreateController()
    {
        kills = 0;
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint(); ;
        controller = PhotonNetwork.Instantiate(Path.Combine("Photon", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { PV.ViewID });
    }
    public void Die()
    {
        if (is_counting) return;
        PV.RPC("RagdollCorpse", RpcTarget.All);
        PhotonNetwork.Destroy(controller);
        is_counting = true;
    }
    [PunRPC]
    void RagdollCorpse( )
    {
        Transform model = controller.transform.Find("Model");
        model.SetParent(null);
        model.gameObject.SetActive(true);
        Camera.main.transform.position = model.transform.position + new Vector3(0, 10f, -2f);
        Camera.main.transform.LookAt(model);
    }
    public void GetKill()
    {
        PV.RPC("RPC_GetKill", PV.Owner);
    }
    [PunRPC]
    void RPC_GetKill()
    {
        kills++;
        GameManager.instance.CheckKills(kills, PhotonNetwork.LocalPlayer.NickName);
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
