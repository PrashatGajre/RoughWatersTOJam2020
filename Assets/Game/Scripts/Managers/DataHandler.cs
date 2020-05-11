using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DataHandler : Singleton<DataHandler>
{
    public Raft[] mActiveRafts;
    public PlayerController mPlayerController;
    public Transform mRaftTargetGroup;
    [SerializeField] float mLevelTraversalScore = 10.0f;
    [HideInInspector] public Score mScore;
    int mLastUpdatedScore = 0;
    int mLastUpdatedMultiplier = 1;
    float mCurrentMaxDistance = 0.0f;
    Vector3 mStartPosition = Vector3.zero;

    public bool mGameStarted = false;

    Raft.RaftType mMasterRaft = Raft.RaftType.Purple;
    Raft.RaftType mClientRaft = Raft.RaftType.Yellow;
    bool mSinglePlayer = false;
    private void OnEnable()
    {
        NetworkManager.Instance.mNetworkCallbacks.OnRoomPropertiesUpdateDelegate += Propertiesupdated;
        PhotonNetwork.NetworkingClient.EventReceived += OnChangeRaftEvent;
    }

    private void OnDisable()
    {
        NetworkManager.Instance.mNetworkCallbacks.OnRoomPropertiesUpdateDelegate -= Propertiesupdated;
        PhotonNetwork.NetworkingClient.EventReceived -= OnChangeRaftEvent;

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

    void SetActiveRaftSelected()
    {
        foreach(Raft aRaft in mActiveRafts)
        {
            if (mSinglePlayer)
            {
                aRaft.mSelected = aRaft.mRaftIndex == mMasterRaft;
            }
            else
            {
                aRaft.mSelected = aRaft.mRaftIndex == mMasterRaft || aRaft.mRaftIndex == mClientRaft;
            }
        }
    }

    public void Propertiesupdated(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.ContainsKey("MasterPlayer"))
        {
            object aMasterRaft = propertiesThatChanged["MasterPlayer"];
            mMasterRaft = (Raft.RaftType)((int)aMasterRaft);
            if(PhotonNetwork.IsMasterClient)
            {
                mPlayerController.mCurrentRaft = mActiveRafts[(int)mMasterRaft];
            }
            SetActiveRaftSelected();
        }
        if(propertiesThatChanged.ContainsKey("ClientPlayer"))
        {
            object aClientRaft = propertiesThatChanged["ClientPlayer"];
            int aRaft = (int)aClientRaft;
            if(aRaft == -1)
            {
                mSinglePlayer = true;
                SetActiveRaftSelected();
                return;
            }
            mClientRaft = (Raft.RaftType)((int)aClientRaft);
            if(!PhotonNetwork.IsMasterClient)
            {
                mPlayerController.mCurrentRaft = mActiveRafts[(int)mClientRaft];
            }
            SetActiveRaftSelected();
        }
        if (propertiesThatChanged.ContainsKey("scoreStruct"))
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


    public void OnChangeRaftEvent(EventData pData)
    {
        if(pData.Code != NetworkManager.EVNT_CHANGERAFT)
        {
            return;
        }
        object[] aData = (object[])pData.CustomData;
        bool aMaster = (bool)aData[0];

        int aCurIx = aMaster ? (int)mMasterRaft : (int)mClientRaft;
        aCurIx = (aCurIx + 1) % mActiveRafts.Length;
        while(aCurIx != (aMaster ? (int)mMasterRaft : (int) mClientRaft ))
        {
            if (!mActiveRafts[aCurIx].mSelected)
            {
                ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
                if(aMaster)
                {
                    customProperties.Add("MasterPlayer", aCurIx);
                }
                else
                {
                    customProperties.Add("ClientPlayer", aCurIx);
                }
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
                break;
            }
            aCurIx = (aCurIx + 1) % mActiveRafts.Length;
        }
    }


    public void Init()
    {
        Raft[] allRafts = FindObjectsOfType<Raft>();

        foreach (Raft aRaft in allRafts)
        {
            mActiveRafts[(int)aRaft.mRaftIndex] = aRaft;
        }

        Cinemachine.CinemachineTargetGroup target = GameObject.FindObjectOfType<Cinemachine.CinemachineTargetGroup>();

        mRaftTargetGroup = target.gameObject.transform;
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            customProperties.Add("MasterPlayer", 0);
            customProperties.Add("ClientPlayer", -1);
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                customProperties["ClientPlayer"] = 2;
            }
            mScore = new Score();
            mScore.mScore = 0.0f;
            mScore.mScoreMultiplier = 1.0f;
            byte[] aByteArray = GenHelpers.SerializeData(mScore);
            customProperties.Add("scoreStruct", aByteArray);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        }

    }


}
