using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Raft : MonoBehaviourPun,IPunObservable
{
    [HideInInspector] public bool mSelected;

    public void OnPhotonSerializeView(PhotonStream pStream, PhotonMessageInfo pInfo)
    {
        pStream.Serialize(ref mSelected);
    }
}
