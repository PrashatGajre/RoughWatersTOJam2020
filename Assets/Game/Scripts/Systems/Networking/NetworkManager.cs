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

    string mPlayerNickName;

    Player mCurrentPlayer = null;

    [SerializeField] public NetworkCallbacks mNetworkCallbacks;

    [SerializeField] MenuClassifier mStartMenu;
    [SerializeField] MenuClassifier mRoomMenu;

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
        NetworkManager.Instance.mNetworkCallbacks.OnCreateRoomDelegate += CreateRoomSuccess;
    }

    private void OnDisable()
    {
        NetworkManager.Instance.mNetworkCallbacks.OnConnectedToMasterDelegate -= ConnectedToServer;
        NetworkManager.Instance.mNetworkCallbacks.OnJoinedLobbyDelegate -= JoinedLobby;
        NetworkManager.Instance.mNetworkCallbacks.OnPlayerLeftRoomDelegate -= PlayerLeftRoom;
        NetworkManager.Instance.mNetworkCallbacks.OnJoinRandomFailedDelegate -= JoinRandomRoomFailed;
        NetworkManager.Instance.mNetworkCallbacks.OnJoinedRoomDelegate -= CreateRoomSuccess;
        NetworkManager.Instance.mNetworkCallbacks.OnCreateRoomFailedDelegate -= CreateRoomFailed;
        NetworkManager.Instance.mNetworkCallbacks.OnCreateRoomDelegate -= CreateRoomSuccess;
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

        ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        if (!customProperties.ContainsKey("selectedRafts"))
        {
            customProperties.Add("selectedRafts", new bool[] { false, false, false });
            Score aScore = new Score();
            aScore.mScore = 0.0f;
            aScore.mScoreMultiplier = 1.0f;
            customProperties.Add("scoreStruct", GenHelpers.SerializeData(aScore));
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        }
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
        StartCoroutine(UnloadScenes());        
    }

    public void PlayerLeftRoom(Photon.Realtime.Player aPlayer)
    {
        if (aPlayer == mCurrentPlayer)
        {
            mCurrentPlayer = null;
        }
        StartCoroutine(UnloadScenes());
    }

    IEnumerator UnloadScenes()
    {
        for (int i = UnityEngine.SceneManagement.SceneManager.sceneCount-1; i > 1; i--) //so that the scenes at index 1 and index 0, UI and Core, are not unloaded
        {
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
        }

        MenuManager.Instance.ShowMenu(mRoomMenu);
        MenuManager.Instance.HideLoad();
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

}
