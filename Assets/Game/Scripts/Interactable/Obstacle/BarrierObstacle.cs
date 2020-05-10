using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierObstacle : Obstacle
{
    [SerializeField] Raft.RaftType mImmuneRaft;
    bool mActive = true;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!mActive)
        {
            return;
        }
        Raft aRaft = collision.GetComponent<Raft>();
        if(aRaft == null)
        {
            return;
        }
        if(aRaft.mRaftIndex == mImmuneRaft)
        {
            mActive = false;
            return;
        }
        ObstacleBehaviour(aRaft);
    }
}
