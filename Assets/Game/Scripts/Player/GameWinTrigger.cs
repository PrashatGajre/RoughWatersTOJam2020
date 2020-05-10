using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinTrigger : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D collision)
    {
        // if(!PhotonNetwork.IsMasterClient)
        // {
            // return;
        // }
        Raft aRaft = collision.GetComponent<Raft>();
        if(aRaft != null)
        {
            //win game
        }
    }
}
