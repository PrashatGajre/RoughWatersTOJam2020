﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float mChangeRaftThreshold = 0.95f;
    [SerializeField] float mDelayBetweenShifting = 0.5f;
    [SerializeField] float mReverseNegateMultiplier = 0.35f;
    Vector2 mMoveVector;
    float mChangeDelta;
    [Tooltip("Temporary Assignment")]
    public Raft mCurrentRaft;
    float mCurrentShiftDelay = -1.0f;
    void Start()
    {
        mCurrentRaft.mSelected = true;
    }

    void Update()
    {
        if(mCurrentShiftDelay < 0.0f)
        {
            return;
        }
        mCurrentShiftDelay -= Time.deltaTime;
        if(mCurrentShiftDelay <= 0.0f)
        {
            mCurrentShiftDelay = -1.0f;
            Raft aRaft = null;
            if (mChangeDelta <= -mChangeRaftThreshold && mCurrentRaft.mRaftIndex != 0)
            {
                aRaft = DataHandler.Instance.mActiveRafts[mCurrentRaft.mRaftIndex - 1];
                if (aRaft.mSelected)
                {
                    while (aRaft.mRaftIndex != 0)
                    {
                        aRaft = DataHandler.Instance.mActiveRafts[aRaft.mRaftIndex - 1];
                        if (!aRaft.mSelected)
                        {
                            break;
                        }
                    }
                    if (aRaft.mSelected)
                    {
                        return;
                    }
                }
                
            }
            else if (mChangeDelta >= mChangeRaftThreshold &&
                mCurrentRaft.mRaftIndex != (DataHandler.Instance.mActiveRafts.Length - 1))
            {
                aRaft = DataHandler.Instance.mActiveRafts[mCurrentRaft.mRaftIndex + 1];
                if (aRaft.mSelected)
                {
                    while (aRaft.mRaftIndex != (DataHandler.Instance.mActiveRafts.Length - 1))
                    {
                        aRaft = DataHandler.Instance.mActiveRafts[aRaft.mRaftIndex + 1];
                        if (!aRaft.mSelected)
                        {
                            break;
                        }
                    }
                    if (aRaft.mSelected)
                    {
                        return;
                    }
                }
                
            }
            if(aRaft == null)
            {
                return;
            }
            mCurrentRaft.mSelected = false;
            aRaft.mSelected = true;
            mCurrentRaft = aRaft;
            mCurrentShiftDelay = mDelayBetweenShifting;
        }
    }

    void FixedUpdate()
    {
        
        mCurrentRaft.mRigidbody.AddRelativeForce(mMoveVector * 
            mCurrentRaft.mSpeed * (mMoveVector.x < 0 ? mReverseNegateMultiplier : 1.0f)
            * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void OnMoveStick(InputAction.CallbackContext pCallbackContext)
    {
        mMoveVector = (Vector2) pCallbackContext.ReadValueAsObject();
    }

    public void OnChangeRaft(InputAction.CallbackContext pCallbackContext)
    {
        mChangeDelta = (float)pCallbackContext.ReadValueAsObject();
        if ((mChangeDelta <= -mChangeRaftThreshold && mCurrentRaft.mRaftIndex != 0)
            ||(mChangeDelta >= mChangeRaftThreshold &&
            mCurrentRaft.mRaftIndex != (DataHandler.Instance.mActiveRafts.Length - 1)))
        {
            if (mCurrentShiftDelay == -1)
            {
                mCurrentShiftDelay = 0.0f;
            }
        }
    }


}
