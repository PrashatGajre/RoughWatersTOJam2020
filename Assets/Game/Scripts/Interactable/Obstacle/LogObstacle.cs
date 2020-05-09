using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogObstacle : Obstacle
{
    [SerializeField] Transform mFinalPosition;
    Vector3 mLogMoveDirection;
    [SerializeField] float mFloatingSpeed = 20.0f;
    [SerializeField] float mRotationSpeed = 25.0f;
    [SerializeField] float mStoppingDistance = 0.5f;
    Vector3 mInitialPosition;
    float mDirectionInverser;
    void Start()
    {
        mLogMoveDirection = mFinalPosition.position - transform.position;
        mLogMoveDirection.Normalize();
        mInitialPosition = transform.position;
        mDirectionInverser = 1;
    }

    protected override void Update()
    {
        base.Update();
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + mRotationSpeed * Time.deltaTime);
        transform.position += mDirectionInverser * mLogMoveDirection * mFloatingSpeed * Time.deltaTime;
        if(mDirectionInverser > 0)
        {
            if(Vector3.Distance(transform.position, mFinalPosition.position) <= mStoppingDistance)
            {
                mDirectionInverser = -1;
            }
        }
        else
        {
            if(Vector3.Distance(transform.position, mInitialPosition) <= mStoppingDistance)
            {
                mDirectionInverser = 1;
            }
        }
    }
}
