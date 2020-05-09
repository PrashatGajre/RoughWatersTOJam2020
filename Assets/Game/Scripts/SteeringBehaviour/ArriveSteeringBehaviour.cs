using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveSteeringBehaviour : SeekSteeringBehaviour
{
    [SerializeField] float mSlowDownDistance = 5.0f;
    [SerializeField] float mDeceleration = 2.5f;
    [SerializeField] float mStoppingDistance = 0.5f;
    [HideInInspector] public bool mPathComplete = true;
    public override void CalculateNewPath(Vector3 pPathTarget)
    {
        mPathComplete = false;
        base.CalculateNewPath(pPathTarget);
    }

    public override Vector3 CalculateForce()
    {
        if(mPathComplete)
        {
            return Vector3.zero;
        }
        Vector3 aDistanceVector = mTarget - mSteeringAgent.transform.position;

        float aMag = aDistanceVector.magnitude;

        if (aMag > mSlowDownDistance)
        {
            return base.CalculateForce();
        }
        else if (aMag < mStoppingDistance)
        {
            mPathComplete = true;
            return -mSteeringAgent.mVelocity;
        }
        
        float aSpeed = aMag / mDeceleration;
        if (aSpeed > mSteeringAgent.mMaxSpeed)
        {
            aSpeed = mSteeringAgent.mMaxSpeed;
        }

        aSpeed /= mSlowDownDistance;

        Vector3 aDesiredVelocity = aDistanceVector.normalized * aSpeed;

        return aDesiredVelocity - mSteeringAgent.mVelocity;
    }

}