using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPoint : MonoBehaviour
{
    [SerializeField] CrocodileObstacle mCrocodileObstacle;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        Raft aRaft = collision.gameObject.GetComponent<Raft>();
        if(aRaft != null)
        {
            mCrocodileObstacle.SetFollowMode();
        }
    }


}
