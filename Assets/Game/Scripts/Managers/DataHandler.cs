using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : Singleton<DataHandler>
{
    public Raft[] mActiveRafts;
    public Transform mRaftTargetGroup;
    [SerializeField] float mLevelTraversalScore = 10.0f;
    [HideInInspector] public float mCurrentScore = 0;
    [HideInInspector] public float mScoreMultiplier = 1.0f;
    float mCurrentMaxDistance = 0.0f;
    Vector3 mStartPosition = Vector3.zero;

    public bool mGameStarted = false;

    private void OnEnable()
    {
        NetworkManager.Instance.mNetworkCallbacks.OnRoomPropertiesUpdateDelegate += Propertiesupdated;
    }

    private void OnDisable()
    {
        NetworkManager.Instance.mNetworkCallbacks.OnRoomPropertiesUpdateDelegate -= Propertiesupdated;
        
    }

    void Start()
    {
        for(int aI = 0; aI < mActiveRafts.Length;aI ++)
        {
            mActiveRafts[aI].mRaftIndex = (Raft.RaftType)aI;
        }
        mStartPosition = mRaftTargetGroup.position;
    }

    void Update()
    {
        float aDistance = Vector3.Distance(mRaftTargetGroup.position, mStartPosition);
        if(mCurrentMaxDistance < aDistance)
        {
            mCurrentScore += mScoreMultiplier * mLevelTraversalScore * (aDistance - mCurrentMaxDistance);
            mCurrentMaxDistance = aDistance;
        }
    }

    public void Propertiesupdated(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("selectedRafts"))
        {
            bool[] selectedRafts = (bool[])propertiesThatChanged["selectedRafts"];

            for (int i = 0; i < selectedRafts.Length; i++)
            {
                mActiveRafts[i].mSelected = selectedRafts[i];
            }
        }
    }

}
