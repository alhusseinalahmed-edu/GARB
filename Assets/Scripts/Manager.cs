using Photon.Pun;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Manager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public GameObject leaderboardPrefab;
    public AudioMixer audioMixer;
    [SerializeField] GameObject InGameMenu;
    public Transform leaderboard;
    public TMP_Text fpsText;
    public float deltaTime;
    public int kills;
    public void UpdateScores(int playerInt)
    {
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

    }
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SpawnPlayer();
        }
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
    public void ChangeGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }
    public void SetFullScreen(bool istrue)
    {
        Screen.fullScreen = istrue;
    }


}
