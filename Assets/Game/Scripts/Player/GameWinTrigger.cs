using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinTrigger : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D collision)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
			return;
        }
        Raft aRaft = collision.GetComponent<Raft>();
        if(aRaft != null)
        {
            //GAME WON
            object[] content = new object[] { "WON" };
            NetworkManager.Instance.RaiseEvent(NetworkManager.EVNT_GAMEWON, content,
                new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All },
                new ExitGames.Client.Photon.SendOptions { Reliability = true });
        }
    }
}
