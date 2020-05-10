using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Raft : MonoBehaviourPun, IPunObservable
{
    public enum RaftType
    {
        Purple,
        Red,
        Yellow
    }

    [HideInInspector] public bool mSelected = false;
    [HideInInspector] public RaftType mRaftIndex;
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
            if(photonView.IsMine)
            {
                mFatigue += mFatigueIncreaseRate * Time.deltaTime;
                if (mFatigue >= mMaxFatigue)
                {
                    mFatigue = mMaxFatigue;
                }
            }
        }
        else if(!mSelectedSprite.activeInHierarchy)
        {
            mSelectedSprite.SetActive(true);
        }
    }

    public void OnPhotonSerializeView(PhotonStream pStream, PhotonMessageInfo pInfo)
    {
        pStream.Serialize(ref mHealth);
        pStream.Serialize(ref mFatigue);
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            mRigidbody.gravityScale = mGravityScaleMultiplier * (mSelected ? mSelectedGravityScale : mUnSelectedGravityScale);
        }
    }

    [PunRPC]
    public void ApplyRelativeForce(Vector2 pForce, PhotonMessageInfo pInfo)
    {
        if (pForce.sqrMagnitude > 0.0f)
        {
            mFatigue -= mFatigueDecreaseRate * Time.deltaTime;
            if (mFatigue <= 0.0f)
            {
                mFatigue = 0;
                return;
            }
        }
        else
        {
            mFatigue += mFatigueIncreaseRate * Time.deltaTime;
            if (mFatigue >= mMaxFatigue)
            {
                mFatigue = mMaxFatigue;
            }
        }
        mRigidbody.AddRelativeForce(pForce, ForceMode2D.Impulse);
    }

}
