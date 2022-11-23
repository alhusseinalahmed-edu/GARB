using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerHandler : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;

    int kills = 0;
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
    }
    void Update()
    {
        
    }

    void CreateController()
    {
        kills = 0;
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint(); ;
        controller = PhotonNetwork.Instantiate(Path.Combine("Photon", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] {PV.ViewID});
    }
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
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
