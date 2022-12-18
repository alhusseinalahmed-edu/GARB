using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    [SerializeField] GameObject InGameMenu;
    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint(); ;
        PhotonNetwork.Instantiate(Path.Combine("Photon", "PlayerController"), spawnPoint.position, spawnPoint.rotation);
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
