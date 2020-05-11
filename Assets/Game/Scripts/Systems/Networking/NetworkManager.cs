using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof(NetworkCallbacks))]
public class NetworkManager : Singleton<NetworkManager>
{
    public static string LOBBY_NAME = "CustomLobby";
    
    //Event Codes
    public static byte EVNT_LOADGAMESCENE = 1;
    public static byte EVNT_GAMESCENELOADED = 2;
    public static byte EVNT_GAMESCENEREADY = 3;
    public static byte EVNT_GAMEWON = 4;
    public static byte EVNT_GAMELOST = 5;
    //public static byte EVNT_GAMEOVER = 6;
    public static byte EVNT_EFFECTS = 7;
    string mPlayerNickName;

    Player mCurrentPlayer = null;

    [SerializeField] public NetworkCallbacks mNetworkCallbacks;
    public MenuClassifier mSceneLoadingMenu;

    [SerializeField] public MenuClassifier mStartMenu;
    [SerializeField] MenuClassifier mRoomMenu;
    [SerializeField] MenuClassifier mGameOverMenu;

    public SceneReference mGameScene;

    public bool IsConnected() { return PhotonNetwork.IsConnected; }
    public bool IsMasterClient() { return PhotonNetwork.IsMasterClient; }

    private void OnEnable()
    {
        NetworkManager.Instance.mNetworkCallbacks.OnConnectedToMasterDelegate += ConnectedToServer;
        NetworkManager.Instance.mNetworkCallbacks.OnJoinedLobbyDelegate += JoinedLobby;
        NetworkManager.Instance.mNetworkCallbacks.OnPlayerLeftRoomDelegate += PlayerLeftRoom;
        NetworkManager.Instance.mNetworkCallbacks.OnJoinRandomFailedDelegate += JoinRandomRoomFailed;
        NetworkManager.Instance.mNetworkCallbacks.OnJoinedRoomDelegate += CreateRoomSuccess;
        NetworkManager.Instance.mNetworkCallbacks.OnCreateRoomFailedDelegate += CreateRoomFailed;
        //NetworkManager.Instance.mNetworkCallbacks.OnCreateRoomDelegate += CreateRoomSuccess;
        Photon.Pun.PhotonNetwork.NetworkingClient.EventReceived += OnPhotonEvents;
        MultiSceneManager.Instance.mOnSceneUnload.AddListener(OnSceneUnload);
    }

    private void OnDisable()
    {
        NetworkManager.Instance.mNetworkCallbacks.OnConnectedToMasterDelegate -= ConnectedToServer;
        NetworkManager.Instance.mNetworkCallbacks.OnJoinedLobbyDelegate -= JoinedLobby;
        NetworkManager.Instance.mNetworkCallbacks.OnPlayerLeftRoomDelegate -= PlayerLeftRoom;
        NetworkManager.Instance.mNetworkCallbacks.OnJoinRandomFailedDelegate -= JoinRandomRoomFailed;
        NetworkManager.Instance.mNetworkCallbacks.OnJoinedRoomDelegate -= CreateRoomSuccess;
        NetworkManager.Instance.mNetworkCallbacks.OnCreateRoomFailedDelegate -= CreateRoomFailed;
        //NetworkManager.Instance.mNetworkCallbacks.OnCreateRoomDelegate -= CreateRoomSuccess;
        PhotonNetwork.NetworkingClient.EventReceived -= OnPhotonEvents;
        MultiSceneManager.Instance.mOnSceneUnload.RemoveListener(OnSceneUnload);
    }

    void Awake()
    {
        LeanTween.init(1000);
    }

    private void Start()
    {
        if (mNetworkCallbacks == null)
        {
            mNetworkCallbacks = GetComponent<NetworkCallbacks>();
        }
        mPlayerNickName = SystemInfo.deviceUniqueIdentifier;
        ConnectToServer(mPlayerNickName);
    }

    public void ConnectToServer(string aPlayerName)
    {
        if (IsConnected())
        {
            return;
        }
        PhotonNetwork.NickName = aPlayerName;
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.ConnectUsingSettings();

        MenuManager.Instance.ShowLoad();
    }

    public void ConnectedToServer()
    {
        PhotonNetwork.JoinLobby(new TypedLobby(LOBBY_NAME, LobbyType.Default));
        //MenuManager.Instance.ShowMenu(mStartMenu);
    }

    public void JoinedLobby()
    {
        MenuManager.Instance.HideLoad();
    }

    public void JoinRandomRoom()
    {
        int maxPlayers = 2;

        PhotonNetwork.JoinRandomRoom(null, System.Convert.ToByte(maxPlayers), Photon.Realtime.MatchmakingMode.FillRoom, new TypedLobby(LOBBY_NAME, LobbyType.Default), null);
    }

    public void JoinRandomRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join random room failed: " + message + "\nCreating new room...");
        MenuManager.Instance.ShowLoad();
        CreateRoom();
    }
    
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
        //PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString(), roomOptions);
        PhotonNetwork.JoinOrCreateRoom(System.Guid.NewGuid().ToString(), new RoomOptions { MaxPlayers = 2 }, new TypedLobby(LOBBY_NAME, LobbyType.Default));
    }

    public void CreateRoomSuccess()
    {
        Debug.Log("Room joined taking to the room menu");
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == mPlayerNickName)
            {
                mCurrentPlayer = player;
            }
        }

        MenuManager.Instance.ShowMenu(mRoomMenu);
    }

    public void CreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room: " + message);
    }

    public void LeaveRoom()
    {
        MenuManager.Instance.ShowMenu(mSceneLoadingMenu);
        MultiSceneManager.Instance.UnloadScene(mGameScene.SceneName);
    }

    public void PlayerLeftRoom(Photon.Realtime.Player aPlayer)
    {
        PhotonNetwork.LeaveRoom();
        if (aPlayer == mCurrentPlayer)
        {
            mCurrentPlayer = null;
        }
        LeaveRoom();
    }

    void OnSceneUnload(List<string> pScenes)
    {
        if(pScenes.Contains(mGameScene.SceneName))
        {
            MenuManager.Instance.HideMenu(mGameOverMenu);
            MenuManager.Instance.ShowMenu(mStartMenu);
            MenuManager.Instance.HideMenu(mSceneLoadingMenu);
        }
    }

    public void RaiseEvent(byte aEventCode, object[] aContent, RaiseEventOptions aRaiseEventOptions, SendOptions aSendOptions)
    {
        PhotonNetwork.RaiseEvent(aEventCode, aContent, aRaiseEventOptions, aSendOptions);
    }

    public void AddCallbackTarget(object aTarget)
    {
        PhotonNetwork.AddCallbackTarget(aTarget);
    }

    public void RemoveCallbackTarget(object aTarget)
    {
        PhotonNetwork.RemoveCallbackTarget(aTarget);
    }

    public void OnPhotonEvents(ExitGames.Client.Photon.EventData photonEvent) 
    {
        //Debug.Log("PHOTON EVENT RECEIVED : " + photonEvent.Code.ToString());

        byte eventCode = photonEvent.Code;

        if (eventCode == NetworkManager.EVNT_GAMELOST)
        {
            object[] data = (object[])photonEvent.CustomData;
            string message = (string)data[0];

            MenuManager.Instance.ShowMenu(mGameOverMenu);
            GameOverMenu gameover = GameObject.FindObjectOfType<GameOverMenu>();

            gameover.ShowGameOver(message, ((int)DataHandler.Instance.mScore.mScore).ToString());
        }
        if (eventCode == NetworkManager.EVNT_GAMEWON)
        {
            object[] data = (object[])photonEvent.CustomData;
            string message = (string)data[0];

            MenuManager.Instance.ShowMenu(mGameOverMenu);
            GameOverMenu gameover = GameObject.FindObjectOfType<GameOverMenu>();

            gameover.ShowGameOver(message, ((int)DataHandler.Instance.mScore.mScore).ToString());
        }
        //if (eventCode == NetworkManager.EVNT_GAMEOVER)
        //{
        //    object[] data = (object[])photonEvent.CustomData;
        //    string message = (string)data[0];

        //    MenuManager.Instance.ShowLoad();
        //    UnloadScenes();
        //    MenuManager.Instance.HideMenu(mGameOverMenu);
        //}
    }

}
