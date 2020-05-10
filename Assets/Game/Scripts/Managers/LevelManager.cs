using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] string mLevelData;
    [SerializeField] MenuClassifier mSceneLoadingMenuClassifier;

    private void OnEnable()
    {
        Photon.Pun.PhotonNetwork.NetworkingClient.EventReceived += IsSceneReady;
    }

    private void OnDisable()
    {
        Photon.Pun.PhotonNetwork.NetworkingClient.EventReceived -= IsSceneReady;
    }

    private void Init()
    {
        MenuManager.Instance.ShowLoad();
        StartCoroutine(LoadLevelData());
    }

    IEnumerator LoadLevelData()
    {
        if (NetworkManager.Instance.IsMasterClient())
        {
            //GameObject levelSetup = Photon.Pun.PhotonNetwork.Instantiate(mLevelData, Vector3.zero, Quaternion.identity);

            /*
             * 
             **DO SOMETHING WITH levelSetup HERE** 
             *
            */
        }
        yield return new WaitForSeconds(3.0f);
        SpawnPlayer();

        yield return new WaitForSeconds(2.0f);
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
        }
        yield return new WaitForSeconds(0.5f);
     
        /*
         
         
        *****SETUP PLAYER HERE******
         
         
         */

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
            MenuManager.Instance.HideMenu(mSceneLoadingMenuClassifier);
        }
    }

    public void SpawnPlayer()
    {
        /*
         
         
        *****SPAWN PLAYER HERE******
         
         
         */
    }
}
