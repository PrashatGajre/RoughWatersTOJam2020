using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : Singleton<DataHandler>
{
    public Raft[] mActiveRafts;
    void Start()
    {
        for(int aI = 0; aI < mActiveRafts.Length;aI ++)
        {
            mActiveRafts[aI].mRaftIndex = (Raft.RaftType)aI;
        }
    }
}
