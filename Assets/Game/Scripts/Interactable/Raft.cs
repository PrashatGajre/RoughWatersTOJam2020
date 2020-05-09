using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Raft : MonoBehaviour/*Pun,IPunObservable*/
{
    [HideInInspector] public bool mSelected;
    [HideInInspector] public int mRaftIndex;
    public float mSpeed = 50.0f;
    public Rigidbody2D mRigidbody;
    public float mHealth = 100;
    public float mFatigue = 100;
    public GameObject mSelectedSprite;
    [HideInInspector] public float mGravityScaleMultiplier = 1.0f;
    [SerializeField] float mSelectedGravityScale = 0.65f;
    [SerializeField] float mUnSelectedGravityScale = 1.0f;

    protected virtual void Update()
    {
        mSelectedSprite.SetActive(mSelected);
    }

    //public void OnPhotonSerializeView(PhotonStream pStream, PhotonMessageInfo pInfo)
    //{
    //    pStream.Serialize(ref mSelected);
    //    pStream.Serialize(ref mHealth);
    //    pStream.Serialize(ref mFatigue);
    //}

    void FixedUpdate()
    {
        //if(photonView.IsMine)
        //{
        mRigidbody.gravityScale = mGravityScaleMultiplier * (mSelected ? mSelectedGravityScale : mUnSelectedGravityScale);
        //}
    }
}
