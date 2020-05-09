using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class CrocodileObstacle : Obstacle
{
    public enum State
    {
        Patrol,
        Follow
    }
    [SerializeField] float mFollowTime = 1.0f;
    [SerializeField] Transform mWaypointParent;
    [SerializeField] ArriveSteeringBehaviour mArriveBehaviour;
    List<Transform> mWaypoints;
    int mCurrentWaypointIndex = 0;
    [HideInInspector] public State mState = State.Patrol;
    float mBackToPatrolTimer = 0.0f;
    void Start()
    {
        mWaypoints = new List<Transform>();
        for(int aI = 0;aI < mWaypointParent.childCount; aI ++)
        {
            mWaypoints.Add(mWaypointParent.GetChild(aI));
        }
        mCurrentWaypointIndex = 0;
        mArriveBehaviour.CalculateNewPath(mWaypoints[mCurrentWaypointIndex].position);
    }


    protected override void Update()
    {
        base.Update();
        switch(mState)
        {
            case State.Patrol:
                {
                    PatrolUpdate();
                    break;
                }
            case State.Follow:
                {
                    FollowUpdate();
                    break;
                }
        }
    }

    void PatrolUpdate()
    {
        if(mArriveBehaviour.mPathComplete)
        {
            mCurrentWaypointIndex = (mCurrentWaypointIndex + 1) % mWaypoints.Count;
            mArriveBehaviour.CalculateNewPath(mWaypoints[mCurrentWaypointIndex].position);
        }
    }

    void FollowUpdate()
    {
        mBackToPatrolTimer += Time.deltaTime;
        if(mBackToPatrolTimer >= mFollowTime)
        {
            mBackToPatrolTimer = 0.0f;
            mArriveBehaviour.CalculateNewPath(mWaypoints[mCurrentWaypointIndex].position);
            mState = State.Patrol;
            return;
        }
        float aMinDistance = float.MaxValue;
        Vector3 aTargetPos = Vector3.zero;
        for(int aI = 0; aI < DataHandler.Instance.mActiveRafts.Length; aI ++)
        {
            Vector3 aCurrRaftPos = DataHandler.Instance.mActiveRafts[aI].transform.position;
            float aDist = Vector3.Distance(transform.position, aCurrRaftPos);
            if(aDist < aMinDistance)
            {
                aTargetPos = aCurrRaftPos;
                aMinDistance = aDist;
            }
        }
        mArriveBehaviour.CalculateNewPath(aTargetPos);
    }

    void OnDrawGizmos()
    {
        for(int aI = 0; aI < mWaypointParent.childCount; aI ++)
        {
            DebugExtension.DrawPoint(mWaypointParent.GetChild(aI).position,Color.yellow);
        }
    }


}
