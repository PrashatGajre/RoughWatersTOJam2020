using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Raft : MonoBehaviour/*Pun,IPunObservable*/
{
    [HideInInspector] public bool mSelected;
    [HideInInspector] public int mRaftIndex;
    public float mSpeed = 50.0f;
    public float mFatigueDecreaseRate = 10.0f;
    public float mFatigueIncreaseRate = 10.0f;
    public Rigidbody2D mRigidbody;
    public float mMaxHealth = 100.0f;
    public float mMaxFatigue = 100.0f;
    [HideInInspector] public float mHealth;
    [HideInInspector] public float mFatigue;
    public GameObject mSelectedSprite;
    [HideInInspector] public float mGravityScaleMultiplier = 1.0f;
    [SerializeField] float mSelectedGravityScale = 0.65f;
    [SerializeField] float mUnSelectedGravityScale = 1.0f;

    protected virtual void Start()
    {
        mHealth = mMaxHealth;
        mFatigue = mMaxFatigue;
    }


    protected virtual void Update()
    {
        if(!mSelected)
        {
            if(mSelectedSprite.activeInHierarchy)
            {
                mSelectedSprite.SetActive(false);
            }
            mFatigue += mFatigueIncreaseRate * Time.deltaTime;
            if (mFatigue >= mMaxFatigue)
            {
                mFatigue = mMaxFatigue;
            }
        }
        else if(!mSelectedSprite.activeInHierarchy)
        {
            mSelectedSprite.SetActive(true);
        }
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
