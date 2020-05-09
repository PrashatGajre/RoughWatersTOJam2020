using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChainRaft : Raft
{
    [SerializeField] int mMinimumChainLength = 8;
    [SerializeField] int mMaximumChainLength = 30;
    [SerializeField] int mGameStartChainLength = 10;
    [SerializeField] float mTriggerThreshold = 0.75f;
    [SerializeField] GameObject mChainObject;
    [SerializeField] Raft mLeftRaft;
    [SerializeField] Chain mLeftChainJoint;
    [SerializeField] Chain mLeftEndChain;
    [SerializeField] Raft mRightRaft;
    [SerializeField] Chain mRightChainJoint;
    [SerializeField] Chain mRightEndChain;
    [SerializeField] Transform mChainParent;
    [SerializeField] float mChainIncreaseDelay = 0.3f;
    int mCurrentChainLength = 0;
    bool mAddChainJoint;
    bool mRemoveChainJoint;
    float mChainTimer = 0.0f;

    protected override void Start()
    {
        base.Start();
        ConnectionSetup(mRightChainJoint, mRightRaft, -1);
        ConnectionSetup(mLeftChainJoint, mLeftRaft, 1);
        mCurrentChainLength++;
        for(int aI = 1; aI < mGameStartChainLength; aI ++)
        {
            AddNewChainConnection();
        }
    }

    protected override void Update()
    {
        base.Update();
        mChainTimer += Time.deltaTime;
        if(mChainTimer >= mChainIncreaseDelay)
        {
            mChainTimer = 0.0f;
            if(mAddChainJoint)
            {
                AddNewChainConnection();
            }
            if(mRemoveChainJoint)
            {
                RemoveChainConnection();
            }
        }
    }

    void AddNewChainConnection()
    {
        mAddChainJoint = false;
        if(mCurrentChainLength == mMaximumChainLength)
        {
            return;
        }
        mCurrentChainLength++;
        ConnectionSetup(mRightEndChain, mRightRaft, -1, true);
        ConnectionSetup(mLeftEndChain, mLeftRaft, 1, true);

    }
    void RemoveChainConnection()
    {
        mRemoveChainJoint = false;
        if(mCurrentChainLength == mMinimumChainLength)
        {
            return;
        }
        mCurrentChainLength--;
        RemoveConnectionSetup(mLeftEndChain, mLeftRaft, -1);
        RemoveConnectionSetup(mRightEndChain, mRightRaft, 1);
    }

    void ConnectionSetup(Chain pMainChainJoint, Raft pMovingRaft, float pYOffset, bool pEnd = false)
    {
        GameObject aChainObject = Instantiate(mChainObject, mChainParent, false);
        Chain aChain = aChainObject.GetComponent<Chain>();
        Chain aCurrentConnectedChain = pMainChainJoint.mHingeJoint.connectedBody.gameObject.GetComponent<Chain>();
        if(pEnd)
        {
            aCurrentConnectedChain.mConnectedTo = aChain;
            aChain.mConnectedFrom = aCurrentConnectedChain;
            aChain.mConnectedTo = pMainChainJoint;
            pMainChainJoint.mConnectedFrom = aChain;
            aCurrentConnectedChain.mHingeJoint.connectedBody = aChain.mRigidbody;
            aChain.mHingeJoint.connectedBody = pMainChainJoint.mRigidbody;
            pMainChainJoint.mHingeJoint.connectedBody = aChain.mRigidbody;
            aChainObject.transform.position = pMainChainJoint.transform.position;
            aChainObject.transform.rotation = pMainChainJoint.transform.rotation;
            pMainChainJoint.transform.position += pYOffset * Vector3.up * aChain.mChainCollider.size.y;
        }
        else
        {
            pMainChainJoint.mConnectedFrom = null;
            pMainChainJoint.mConnectedTo = aChain;
            aChain.mConnectedFrom = pMainChainJoint;
            aChain.mConnectedTo = aCurrentConnectedChain;
            aCurrentConnectedChain.mConnectedFrom = aChain;
            aCurrentConnectedChain.mConnectedTo = null;
            aCurrentConnectedChain.mHingeJoint.connectedBody = aChain.mRigidbody;
            aChain.mHingeJoint.connectedBody = aCurrentConnectedChain.mRigidbody;
            pMainChainJoint.mHingeJoint.connectedBody = aChain.mRigidbody;
            aChainObject.transform.position = aCurrentConnectedChain.transform.position;
            aChainObject.transform.rotation = aCurrentConnectedChain.transform.rotation;
            aCurrentConnectedChain.transform.position += pYOffset * Vector3.up * aChain.mChainCollider.size.y;
        }
        pMovingRaft.transform.position += pYOffset * Vector3.up * aChain.mChainCollider.size.y;
    }

    void RemoveConnectionSetup(Chain pMainChainJoint, Raft pMovingRaft, float pYOffset)
    {
        Chain aChain = pMainChainJoint.mHingeJoint.connectedBody.gameObject.GetComponent<Chain>();
        Chain aConnectedFrom = aChain.mConnectedFrom;
        pMainChainJoint.mConnectedFrom = aConnectedFrom;
        aConnectedFrom.mConnectedTo = pMainChainJoint;
        aConnectedFrom.mHingeJoint.connectedBody = pMainChainJoint.mRigidbody;
        pMainChainJoint.mHingeJoint.connectedBody = aConnectedFrom.mRigidbody;
        pMainChainJoint.transform.position += pYOffset * Vector3.up * aChain.mChainCollider.size.y;
        pMovingRaft.transform.position += pYOffset * Vector3.up * aChain.mChainCollider.size.y;
        Destroy(aChain.gameObject);
    }

    public void OnChainLengthChange(InputAction.CallbackContext pCallbackContext)
    {
        float aChainLengthDelta = (float)pCallbackContext.ReadValueAsObject();
        if(aChainLengthDelta <= -mTriggerThreshold)
        {
            mRemoveChainJoint = true;
        }
        else if(aChainLengthDelta >= mTriggerThreshold)
        {
            mAddChainJoint = true;
        }
    }

}
