using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenu : Menu
{
    [SerializeField] SceneReference mGameScene;

    [SerializeField] MenuClassifier mSceneLoadingMenu;

    [SerializeField] Button mStartSinglePlayerButton;

    protected override void OnVisible()
    {
        mStartSinglePlayerButton.Select();
        base.OnVisible();
    }

    private void OnEnable()
    {
        if (NetworkManager.Instance.IsConnected())
        {
            if ((Photon.Pun.PhotonNetwork.CurrentRoom != null) && (Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount == Photon.Pun.PhotonNetwork.CurrentRoom.MaxPlayers))
            {
                StartGameplay();
            }
        }
        else
        {
            NetworkManager.Instance.mNetworkCallbacks.OnPlayerEnteredRoomDelegate += PlayerJoined;
        }

        Photon.Pun.PhotonNetwork.NetworkingClient.EventReceived += OnNetworkGameScenesLoaded;

        MultiSceneManager.Instance.mOnSceneLoad.AddListener(GameSceneLoaded);
    }

    private void OnDisable()
    {
        NetworkManager.Instance.mNetworkCallbacks.OnPlayerEnteredRoomDelegate -= PlayerJoined;
        Photon.Pun.PhotonNetwork.NetworkingClient.EventReceived -= OnNetworkGameScenesLoaded;
        MultiSceneManager.Instance.mOnSceneLoad.RemoveListener(GameSceneLoaded);
    }

    public void PlayerJoined(Photon.Realtime.Player newPlayer)
    {
        if (Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount == Photon.Pun.PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartGameplay();
        }
    }

    public void StartGameplay()
    {
        Debug.Log("Loading Game Scene..");
        MenuManager.Instance.ShowMenu(mSceneLoadingMenu);

        Photon.Pun.PhotonNetwork.CurrentRoom.IsOpen = false;
        object[] content = new object[] { mGameScene.SceneName };
        Debug.Log("RAISING EVENT EVNT_LOADGAMESCENE + " + NetworkManager.EVNT_GAMESCENELOADED.ToString());
        NetworkManager.Instance.RaiseEvent(NetworkManager.EVNT_LOADGAMESCENE, content,
            new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All },
            new ExitGames.Client.Photon.SendOptions { Reliability = true });
        //MultiSceneManager.Instance.LoadScene(mGameScene.SceneName);
    }

    public void GameSceneLoaded(List<string> scenes)
    {
        Debug.Log("SceneLoaded Event Called");
        object[] content= {1};
        string scenesLoaded = "";
        foreach (string s in scenes)
        {
            scenesLoaded += s + ",";

            if (s == mGameScene.SceneName)
            {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(s));
            }
        }

        Debug.Log("RAISING EVENT EVNT_GAMESCENELOADED + " + NetworkManager.EVNT_GAMESCENELOADED.ToString());
        NetworkManager.Instance.RaiseEvent(NetworkManager.EVNT_GAMESCENELOADED, content,
            new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All },
            new ExitGames.Client.Photon.SendOptions { Reliability = true });
    }

    int totalScenesLoaded = 0;
    public void OnNetworkGameScenesLoaded(ExitGames.Client.Photon.EventData photonEvent)
    {

        Debug.Log("PHOTON EVENT RECEIVED : " + photonEvent.Code.ToString());

        byte eventCode = photonEvent.Code;
        bool allPlayerScenesLoaded = false;

        if (eventCode == NetworkManager.EVNT_LOADGAMESCENE)
        {
            object[] data = (object[])photonEvent.CustomData;
            string sceneToLoad = (string)data[0];

            Debug.Log("LOADING : " + sceneToLoad);
            MultiSceneManager.Instance.LoadScene(sceneToLoad);
        }

        else if (eventCode == NetworkManager.EVNT_GAMESCENELOADED)
        {
            object[] data = (object[])photonEvent.CustomData;
            totalScenesLoaded += (int)data[0];
            if (totalScenesLoaded == Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount)
            {
                allPlayerScenesLoaded = true;
            }
        }


        if (allPlayerScenesLoaded)
        {
            LevelManager.Instance.Init();
            MenuManager.Instance.HideMenu(mMenuClassifier);
        }
    }

}
