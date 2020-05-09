using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenu : Menu
{
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
    }

    private void OnDisable()
    {
        NetworkManager.Instance.mNetworkCallbacks.OnPlayerEnteredRoomDelegate -= PlayerJoined;        
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
        MenuManager.Instance.ShowLoad();
        MenuManager.Instance.HideMenu(mMenuClassifier);
        Photon.Pun.PhotonNetwork.CurrentRoom.IsOpen = false;
    }
}
