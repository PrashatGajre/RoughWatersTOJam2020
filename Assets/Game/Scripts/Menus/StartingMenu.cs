using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingMenu : Menu
{
    [SerializeField] Button mStartButton;

    protected override void OnVisible()
    {
        mStartButton.Select();
        base.OnVisible();
    }

    public void StartGame()
    {
        NetworkManager.Instance.JoinRandomRoom();
        MenuManager.Instance.ShowLoad();
        MenuManager.Instance.HideMenu(mMenuClassifier);
    }
}
