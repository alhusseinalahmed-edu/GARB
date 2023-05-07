using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    public GameObject scorePanelPrefab;
    public Transform scorePanelParent;

    private Dictionary<Player, ScorePanel> scorePanels = new Dictionary<Player, ScorePanel>();

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("kills"))
        {
            UpdateScorePanel(targetPlayer);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (scorePanels.ContainsKey(otherPlayer))
        {
            Destroy(scorePanels[otherPlayer].gameObject);
            scorePanels.Remove(otherPlayer);
        }
    }

    private void UpdateScorePanel(Player player)
    {
        if (scorePanels.ContainsKey(player))
        {
            scorePanels[player].SetKillCount((int)player.CustomProperties["kills"]);
        }
        else
        {
            GameObject scorePanelObject = Instantiate(scorePanelPrefab, scorePanelParent);
            ScorePanel scorePanel = scorePanelObject.GetComponent<ScorePanel>();
            scorePanel.SetPlayerName(player.NickName);
            scorePanel.SetKillCount((int)player.CustomProperties["kills"]);
            scorePanels.Add(player, scorePanel);
        }
    }

    public void UpdateScoreboard()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            UpdateScorePanel(player);
        }
    }
}
