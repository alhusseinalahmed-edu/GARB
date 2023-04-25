using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PerformanceDisplay : MonoBehaviourPunCallbacks
{
    public TMP_Text _text;
    private float deltaTime;

    private void Update()
    {
        int ping = PhotonNetwork.GetPing();

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        _text.text = string.Format("{0:0.} FPS", fps) + " " + ping + " ms";
    }
}
