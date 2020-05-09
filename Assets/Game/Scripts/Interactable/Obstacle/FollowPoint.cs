using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPoint : MonoBehaviour
{
    [SerializeField] CrocodileObstacle mCrocodileObstacle;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(mCrocodileObstacle.mState == CrocodileObstacle.State.Follow)
        {
            return;
        }
        Raft aRaft = collision.gameObject.GetComponent<Raft>();
        if(aRaft != null)
        {
            mCrocodileObstacle.mState = CrocodileObstacle.State.Follow;
        }
    }


}
