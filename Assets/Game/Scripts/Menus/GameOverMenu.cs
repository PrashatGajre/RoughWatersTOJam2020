using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : Menu
{
    [SerializeField] Text mGameOverTitleText;
    [SerializeField] Text mScoreText;
    [SerializeField] Button mReturnToMainButton;

    protected override void OnVisible()
    {
        mReturnToMainButton.Select();
        base.OnVisible();
    }

    public void ShowGameOver(string message, string score)
    {
        mGameOverTitleText.text = message;
        mScoreText.text = score;
    }

    public void LeaveRoom()
    {
        MenuManager.Instance.ShowLoad();
        MenuManager.Instance.HideMenu(mMenuClassifier);

        Photon.Pun.PhotonNetwork.LeaveRoom();
        NetworkManager.Instance.LeaveRoom();
    }
}
