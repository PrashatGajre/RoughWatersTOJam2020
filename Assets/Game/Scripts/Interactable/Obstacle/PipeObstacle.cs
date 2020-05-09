using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeObstacle : Obstacle
{
    [SerializeField] Raft.RaftType mDamageImmuneRaft;
    [SerializeField] float mNoCollisionTime = 0.2f;
    bool mImmuneCollision = false;
    float mLastParticleCollision = 0.0f;
    void OnParticleCollision(GameObject pOther)
    {
        if (mCollidedTime > 0.0f)
        {
            return;
        }
        Raft aRaft = pOther.GetComponent<Raft>();
        if (aRaft == null)
        {
            return;
        }
        mLastParticleCollision = 0.0f;
        if(!mImmuneCollision)
        {
            mImmuneCollision = aRaft.mRaftIndex == mDamageImmuneRaft;
            if(!mImmuneCollision)
            {
                ObstacleBehaviour(aRaft);
            }
        }
    }

    void LateUpdate()
    {
        if(!mImmuneCollision)
        {
            return;
        }
        mLastParticleCollision += Time.deltaTime;
        if(mLastParticleCollision > mNoCollisionTime)
        {
            mImmuneCollision = false;
        }
    }
}
