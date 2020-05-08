using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Raft : MonoBehaviourPun,IPunObservable
{
    [HideInInspector] public bool mSelected;
    public Rigidbody2D mRigidbody;
    public float mHealth = 100;
    public float mFatigue = 100;
    public void OnPhotonSerializeView(PhotonStream pStream, PhotonMessageInfo pInfo)
    {
        pStream.Serialize(ref mSelected);
        pStream.Serialize(ref mHealth);
        pStream.Serialize(ref mFatigue);
    }
}
