using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [Header("Refs")]
    [SerializeField] GameObject InGameMenu;
    [SerializeField] GameObject GameOverMenu;
    [SerializeField] PhotonView PV;

    
    [HideInInspector] public int MostKills;

    [Header("Settings")]
    public int killsToWin = 5;

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected) SceneManager.LoadScene("MainMenu");
        instance = this;
    }

    public void OpenInGameMenu(InputHandler inputHandler)
    {
        if (!InGameMenu.activeSelf)
        {
            InGameMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            inputHandler.isPaused = true; 
        }
        else
        {
            InGameMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inputHandler.isPaused = false;
        }
    }
    public void CheckKills(int kills, string playerName)
    {
        if(kills > MostKills)
        {
            MostKills = kills;
        }
        if(kills == killsToWin)
        {
            EndGame(playerName);
        }
    }
    public void EndGame(string playerName)
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PV.RPC("RPC_DestroyAll", RpcTarget.All);
        PV.RPC("RPC_GameOverMenu", RpcTarget.All, playerName);
    }
    [PunRPC]
    private void RPC_DestroyAll()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.DestroyAll();
    }
    [PunRPC]
    void RPC_GameOverMenu(string playerName)
    {
        GameOverMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameOverMenu.transform.Find("Winner").GetComponent<TMP_Text>().text = "The winner is " + playerName;
    }
    public void LeaveRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        Destroy(RoomManager.Instance.gameObject);
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
