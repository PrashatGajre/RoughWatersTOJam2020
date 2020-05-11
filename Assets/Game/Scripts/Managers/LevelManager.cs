using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] string mLevelDataPrefabName;
    [SerializeField] string mPlayerDataPrefabName;
    [SerializeField] float mCurrentFlow = 10.0f;
    [SerializeField] MenuClassifier mSceneLoadingMenuClassifier;
    public Cinemachine.CinemachineVirtualCamera mLevelCam;
    [SerializeField] MenuClassifier mHUDClassifier;
    int totalScenesLoaded = 0;

    private void OnEnable()
    {
        Photon.Pun.PhotonNetwork.NetworkingClient.EventReceived += IsSceneReady;
    }

    private void OnDisable()
    {
        Photon.Pun.PhotonNetwork.NetworkingClient.EventReceived -= IsSceneReady;
    }

    public void Init()
    {
        //if (NetworkManager.Instance.IsMasterClient())
        //{
        StartCoroutine(LoadLevelData());
        Physics2D.gravity = Vector2.zero;
        //}
    }

    IEnumerator LoadLevelData()
    {
        if (NetworkManager.Instance.IsMasterClient())
        {
            GameObject levelSetup = Photon.Pun.PhotonNetwork.Instantiate(mLevelDataPrefabName, Vector3.zero, Quaternion.identity);

            /*
             * 
             **DO SOMETHING WITH levelSetup HERE** 
             *
            */
        }
        yield return new WaitForSeconds(3.0f);

        SpawnPlayer();

        yield return new WaitForSeconds(2.0f);

        if (mLevelCam == null)
        {
            mLevelCam = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            Debug.Log(mLevelCam.gameObject.name);
        }
        mLevelCam.Follow = DataHandler.Instance.mActiveRafts[(int)Raft.RaftType.Red].transform;

        /******SETUP PLAYER HERE*******/
        PlayerController[] playerControllers = GameObject.FindObjectsOfType<PlayerController>();
        //Debug.Log("PLAYER CONTROLLERS FOUND : " + playerControllers.Length);
        //DataHandler dataHandler = GameObject.FindObjectOfType<DataHandler>();
        yield return new WaitForSeconds(0.3f);
     
        OnSceneReadyEvent(new object[]{1});
    }

    public void OnSceneReadyEvent(object[] aContent)
    {
        Debug.Log("RAISING EVENT EVNT_GAMESCENEREADY");
        NetworkManager.Instance.RaiseEvent(NetworkManager.EVNT_GAMESCENEREADY, aContent, 
            new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All }, 
            new ExitGames.Client.Photon.SendOptions { Reliability = true });
    }

    public void IsSceneReady(ExitGames.Client.Photon.EventData aPhotonEvent)
    {
        byte eventCode = aPhotonEvent.Code;

        if (eventCode == NetworkManager.EVNT_GAMESCENEREADY)
        {
            object[] data = (object[])aPhotonEvent.CustomData;
            totalScenesLoaded += (int)data[0];
            if (totalScenesLoaded == Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount)
            {
                MenuManager.Instance.HideMenu(mSceneLoadingMenuClassifier);
                Physics2D.gravity = new Vector2(mCurrentFlow,0);
                DataHandler.Instance.mGameStarted = true;
                MenuManager.Instance.ShowMenu(mHUDClassifier);
            }
        }
        if (eventCode == NetworkManager.EVNT_GAMELOST)
        {
            object[] data = (object[])aPhotonEvent.CustomData;
            string message = (string)data[0];

            DataHandler.Instance.mGameStarted = false;
            Physics2D.gravity = Vector2.zero;
            MenuManager.Instance.HideMenu(mHUDClassifier);
        }
        if (eventCode == NetworkManager.EVNT_GAMEWON)
        {
            object[] data = (object[])aPhotonEvent.CustomData;
            string message = (string)data[0];

            DataHandler.Instance.mGameStarted = false;
            Physics2D.gravity = Vector2.zero;
            MenuManager.Instance.HideMenu(mHUDClassifier);
        }
    }

    public void SpawnPlayer()
    {
        //Debug.Log("Creating Player");
        GameObject player = Photon.Pun.PhotonNetwork.Instantiate(mPlayerDataPrefabName, Vector3.zero, Quaternion.identity);
        //Debug.Log("Creating Created");
        player.name = Photon.Pun.PhotonNetwork.NickName;
        //Debug.Log("Creating Nickname Changed");
    }
}
