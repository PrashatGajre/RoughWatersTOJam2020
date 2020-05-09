using System.Collections;
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
                aRaft = DataHandler.Instance.mActiveRafts[(int)mCurrentRaft.mRaftIndex - 1];
                if (aRaft.mSelected)
                {
                    while (aRaft.mRaftIndex != 0)
                    {
                        aRaft = DataHandler.Instance.mActiveRafts[(int)aRaft.mRaftIndex - 1];
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
                (int)mCurrentRaft.mRaftIndex != (DataHandler.Instance.mActiveRafts.Length - 1))
            {
                aRaft = DataHandler.Instance.mActiveRafts[(int)mCurrentRaft.mRaftIndex + 1];
                if (aRaft.mSelected)
                {
                    while ((int)aRaft.mRaftIndex != (DataHandler.Instance.mActiveRafts.Length - 1))
                    {
                        aRaft = DataHandler.Instance.mActiveRafts[(int)aRaft.mRaftIndex + 1];
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
        Vector2 aForceApplied = mMoveVector *
            mCurrentRaft.mSpeed * (mMoveVector.x < 0 ? mReverseNegateMultiplier : 1.0f)
            * Time.deltaTime;
        if(aForceApplied.sqrMagnitude > 0.0f)
        {
            mCurrentRaft.mFatigue -= mCurrentRaft.mFatigueDecreaseRate * Time.deltaTime;
            if(mCurrentRaft.mFatigue <= 0.0f)
            {
                mCurrentRaft.mFatigue = 0;
                return;
            }
        }
        else
        {
            mCurrentRaft.mFatigue += mCurrentRaft.mFatigueIncreaseRate * Time.deltaTime;
            if(mCurrentRaft.mFatigue >= mCurrentRaft.mMaxFatigue)
            {
                mCurrentRaft.mFatigue = mCurrentRaft.mMaxFatigue;
            }
        }
        mCurrentRaft.mRigidbody.AddRelativeForce(aForceApplied, ForceMode2D.Impulse);
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
            (int)mCurrentRaft.mRaftIndex != (DataHandler.Instance.mActiveRafts.Length - 1)))
        {
            if (mCurrentShiftDelay == -1)
            {
                mCurrentShiftDelay = 0.0f;
            }
        }
    }


}
