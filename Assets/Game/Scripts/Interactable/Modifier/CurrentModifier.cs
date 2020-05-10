using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentModifier : MonoBehaviour
{
    [Range(0.0f,10f)]
    [SerializeField] float mGravityMultiplier;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        Raft aRaft = collision.GetComponent<Raft>();
        if(aRaft != null)
        {
            aRaft.mGravityScaleMultiplier *= mGravityMultiplier;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        Raft aRaft = collision.GetComponent<Raft>();
        if(aRaft != null)
        {
            aRaft.mGravityScaleMultiplier /= mGravityMultiplier;
        }
    }


}
