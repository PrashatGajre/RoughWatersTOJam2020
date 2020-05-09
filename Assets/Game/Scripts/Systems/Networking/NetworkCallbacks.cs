using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkCallbacks : MonoBehaviourPunCallbacks
{
    public delegate void ConnectedToServer();
    public ConnectedToServer OnConnectedToServerDelegate;
    public override void OnConnected()
    {
        OnConnectedToServerDelegate?.Invoke();
    }

    public delegate void ConnectedToMaster();
    public ConnectedToMaster OnConnectedToMasterDelegate;
    public override void OnConnectedToMaster()
    {
        OnConnectedToMasterDelegate?.Invoke();
    }

    public delegate void CreateRoom();
    public CreateRoom OnCreateRoomDelegate;
    public override void OnCreatedRoom()
    {
        OnCreateRoomDelegate?.Invoke();
    }

    public delegate void CreateRoomFailed(short returncode, string message);
    public CreateRoomFailed OnCreateRoomFailedDelegate;
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        OnCreateRoomFailedDelegate?.Invoke(returnCode, message);
    }
    //Remove
    public delegate void CustomAuthenticationFailed(string debugMessage);
    public CustomAuthenticationFailed OnCustomAuthenticationFailedDelegate;
    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        OnCustomAuthenticationFailedDelegate?.Invoke(debugMessage);
    }
    //Remove
    public delegate void CustomAuthenticationResponse(Dictionary<string, object> data);
    public CustomAuthenticationResponse OnCustomAuthenticationResponseDelegate;
    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        OnCustomAuthenticationResponseDelegate?.Invoke(data);
    }

    public delegate void Disconnected(DisconnectCause cause);
    public Disconnected OnDisconnectedDelegate;
    public override void OnDisconnected(DisconnectCause cause)
    {
        OnDisconnectedDelegate?.Invoke(cause);
    }
    //Remove
    public delegate void FriendListUpdate(List<FriendInfo> friendList);
    public FriendListUpdate OnFriendListUpdateDelegate;
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        OnFriendListUpdateDelegate?.Invoke(friendList);
    }

    public delegate void JoinedLobby();
    public JoinedLobby OnJoinedLobbyDelegate;
    public override void OnJoinedLobby()
    {
        OnJoinedLobbyDelegate?.Invoke();
    }

    public delegate void JoinedRoom();
    public JoinedRoom OnJoinedRoomDelegate;
    public override void OnJoinedRoom()
    {
        OnJoinedRoomDelegate?.Invoke();
    }
    //Remove
    public delegate void JoinRandomFailed(short returnCode, string message);
    public JoinRandomFailed OnJoinRandomFailedDelegate;
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        OnJoinRandomFailedDelegate?.Invoke(returnCode, message);
    }

    public delegate void JoinRoomFailed(short returnCode, string message);
    public JoinRoomFailed OnJoinRoomFailedDelegate;
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        OnJoinRoomFailedDelegate?.Invoke(returnCode, message);
    }

    public delegate void LeftLobby();
    public LeftLobby OnLeftLobbyDelegate;
    public override void OnLeftLobby()
    {
        OnLeftLobbyDelegate?.Invoke();
    }

    public delegate void LeftRoom();
    public LeftRoom OnLeftRoomDelegate;
    public override void OnLeftRoom()
    {
        OnLeftRoomDelegate?.Invoke();
    }
    //Remove
    public delegate void LobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics);
    public LobbyStatisticsUpdate OnLobbyStatisticsUpdateDelegate;
    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        OnLobbyStatisticsUpdateDelegate?.Invoke(lobbyStatistics);
    }
    //Remove
    public delegate void MasterClientSwitched(Player newMasterClient);
    public MasterClientSwitched OnMasterClientSwitchedDelegate;
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        OnMasterClientSwitchedDelegate?.Invoke(newMasterClient);
    }

    public delegate void PlayerEnteredRoom(Player newPlayer);
    public PlayerEnteredRoom OnPlayerEnteredRoomDelegate;
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnPlayerEnteredRoomDelegate?.Invoke(newPlayer);
    }

    public delegate void PlayerLeftRoom(Player otherPlayer);
    public PlayerLeftRoom OnPlayerLeftRoomDelegate;
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerLeftRoomDelegate?.Invoke(otherPlayer);
    }

    public delegate void PlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps);
    public PlayerPropertiesUpdate OnPlayerPropertiesUpdateDelegate;
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        OnPlayerPropertiesUpdateDelegate?.Invoke(targetPlayer, changedProps);
    }
    //Remove
    public delegate void RegionListReceived(RegionHandler regionHandler);
    public RegionListReceived OnRegionListReceivedDelegate;
    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        OnRegionListReceivedDelegate?.Invoke(regionHandler);
    }

    public delegate void RoomListUpdate(List<RoomInfo> roomList);
    public RoomListUpdate OnRoomListUpdateDelegate;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        OnRoomListUpdateDelegate?.Invoke(roomList);
    }

    public delegate void RoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged);
    public RoomPropertiesUpdate OnRoomPropertiesUpdateDelegate;
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        OnRoomPropertiesUpdateDelegate?.Invoke(propertiesThatChanged);
    }
    //Remove
    public delegate void WebRpcResponse(OperationResponse response);
    public WebRpcResponse OnWebRpcResponseDelegate;
    public override void OnWebRpcResponse(OperationResponse response)
    {
        OnWebRpcResponseDelegate?.Invoke(response);
    }
}
