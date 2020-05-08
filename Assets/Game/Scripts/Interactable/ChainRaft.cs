using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChainRaft : Raft
{
    float mChainLengthDelta;
    public void OnChainLengthChange(InputAction.CallbackContext pCallbackContext)
    {
        mChainLengthDelta = (float)pCallbackContext.ReadValueAsObject();
    }

}
