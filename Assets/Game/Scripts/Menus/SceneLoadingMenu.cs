using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadingMenu : Menu
{
    [SerializeField] Text mLoadingMessageText;
    [SerializeField] string[] mLoadingMessages;

    public override void ShowMenu(string pOptions)
    {
        ShowMessage();
        base.ShowMenu(pOptions);
    }

    public void ShowMessage()
    {
        if (mLoadingMessages.Length > 0)
        {
            mLoadingMessageText.text = mLoadingMessages[Random.Range(0, mLoadingMessages.Length)];
        }
        else
        {
            mLoadingMessageText.text = "Loading Resources";
        }
    }
}
