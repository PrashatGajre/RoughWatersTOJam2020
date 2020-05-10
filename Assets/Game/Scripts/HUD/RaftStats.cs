using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaftStats : MonoBehaviour
{
    [SerializeField] Image mHealthBar;
    [SerializeField] Image mFatigueBar;
    [SerializeField] Image mRaftImage;
    [SerializeField] Raft.RaftType mRaftType;
    [SerializeField] Color mRaftColor;
    public Raft mReferenceRaft;

    void Start()
    {
        mRaftImage.color = mRaftColor;
    }

    private void OnEnable()
    {
        if (DataHandler.IsValidSingleton())
        {
            for (int aI = 0; aI < DataHandler.Instance.mActiveRafts.Length; aI++)
            {
                if (DataHandler.Instance.mActiveRafts[aI].mRaftIndex == mRaftType)
                {
                    mReferenceRaft = DataHandler.Instance.mActiveRafts[aI];
                }
            }
        }
    }

    void Update()
    {
        mHealthBar.fillAmount = mReferenceRaft.mHealth / mReferenceRaft.mMaxHealth;
        mFatigueBar.fillAmount = mReferenceRaft.mFatigue / mReferenceRaft.mMaxFatigue;
    }
}
