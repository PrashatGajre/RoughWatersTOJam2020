using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : Singleton<DataHandler>
{
    public Raft[] mActiveRafts;
    public Transform mRaftTargetGroup;
    [SerializeField] float mLevelTraversalScore = 10.0f;
    [HideInInspector] public Score mScore;
    int mLastUpdatedScore = 0;
    int mLastUpdatedMultiplier = 1;
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

    void Update()
    {
        if (!mGameStarted)
        {
            if(mRaftTargetGroup != null)
            {
                mStartPosition = mRaftTargetGroup.position;
            }
            return;
        }
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        float aDistance = Vector3.Distance(mRaftTargetGroup.position, mStartPosition);
        if(mCurrentMaxDistance < aDistance)
        {
            mScore.mScore += mScore.mScoreMultiplier * mLevelTraversalScore * (aDistance - mCurrentMaxDistance);
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable()
                {
                    { "scoreStruct", GenHelpers.SerializeData(mScore)}
                });
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
        if(propertiesThatChanged.ContainsKey("scoreStruct"))
        {
            object scoreStruct = propertiesThatChanged["scoreStruct"];
            GenHelpers.DeSerializeData((byte[])scoreStruct, ref mScore);
            int aCurScore = (int)mScore.mScore;
            int aCurMulti = (int)mScore.mScoreMultiplier;
            if(aCurMulti != mLastUpdatedMultiplier)
            {
                mLastUpdatedMultiplier = aCurMulti;
                EffectsManager.Instance.ScoreMultiplierAddedEffect();
            }
            if(aCurScore != mLastUpdatedScore)
            {
                mLastUpdatedScore = aCurScore;
                EffectsManager.Instance.ScoreAddedEffect();
            }
        }
    }

}
