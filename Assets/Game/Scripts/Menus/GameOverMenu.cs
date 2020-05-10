using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : Menu
{
    [SerializeField] Text mGameOverTitleText;
    [SerializeField] Text mScoreText;

    public void ShowGameOver(string message, string score)
    {
        mGameOverTitleText.text = message;
        mScoreText.text = score;
    }

    public void LeaveRoom()
    {
        object[] content = new object[] { "Leave Room" };
        NetworkManager.Instance.RaiseEvent(NetworkManager.EVNT_GAMEOVER, content,
            new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All },
            new ExitGames.Client.Photon.SendOptions { Reliability = true });
    }
}
