using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] string mLevelDataPrefabName;
    [SerializeField] string mPlayerDataPrefabName;
    [SerializeField] MenuClassifier mSceneLoadingMenuClassifier;
    [SerializeField] MenuClassifier mHUDClassifier;
    [SerializeField] Cinemachine.CinemachineVirtualCamera mLevelCam;

    Vector2 gravity;

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
        gravity = Physics2D.gravity;
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
                
        foreach (PlayerController pc in playerControllers)
        {
            //Debug.Log("PLAYER CONTROLLER ITERATION");
            Photon.Pun.PhotonView pcView = pc.gameObject.GetComponent<Photon.Pun.PhotonView>();
            if (pcView != null)
            {
                if (pcView.IsMine)
                {
                    //Debug.Log("PHOTONVIEW IS MINE");
                    foreach (Raft activeRaft in DataHandler.Instance.mActiveRafts) 
                    {
                        //Debug.Log("ITERATING ACTIVE RAFTS");
                        if (!activeRaft.mSelected)
                        {
                            //Debug.Log("ACTIVE RAFT FOUND : " + activeRaft.gameObject.name);
                            activeRaft.mSelected = true;
                            ExitGames.Client.Photon.Hashtable customProperties = Photon.Pun.PhotonNetwork.CurrentRoom.CustomProperties;
                            bool[] selectedRafts = (bool[])customProperties["selectedRafts"];

                            selectedRafts[(int)activeRaft.mRaftIndex] = true;

                            customProperties["selectedRafts"] = selectedRafts;
                            Photon.Pun.PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
                            pc.mCurrentRaft = activeRaft;
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.3f);
     

        OnSceneReadyEvent(new object[]{1});
    }

    public void OnSceneReadyEvent(object[] aContent)
    {
        //Debug.Log("RAISING EVENT EVNT_GAMESCENEREADY");
        NetworkManager.Instance.RaiseEvent(NetworkManager.EVNT_GAMESCENEREADY, aContent, 
            new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All }, 
            new ExitGames.Client.Photon.SendOptions { Reliability = true });
    }

    public void IsSceneReady(ExitGames.Client.Photon.EventData aPhotonEvent)
    {
        byte eventCode = aPhotonEvent.Code;

        if (eventCode == NetworkManager.EVNT_GAMESCENEREADY)
        {
            MenuManager.Instance.HideMenu(mSceneLoadingMenuClassifier);
            Physics2D.gravity = gravity;
            DataHandler.Instance.mGameStarted = true;
            MenuManager.Instance.ShowMenu(mHUDClassifier);
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
