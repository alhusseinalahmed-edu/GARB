using UnityEngine;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text killCountText;

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    public void SetKillCount(int killCount)
    {
        killCountText.text = killCount.ToString() + " kills";
    }
}
