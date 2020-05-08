using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Vector2 mMoveVector;
    [HideInInspector] public float mChangeDelta;
    [HideInInspector] public float mChainLengthDelta;

    public void OnMoveStick(InputAction.CallbackContext pCallbackContext)
    {
        mMoveVector = (Vector2) pCallbackContext.ReadValueAsObject();
    }

    public void OnChangeRaft(InputAction.CallbackContext pCallbackContext)
    {
        mChangeDelta = (float)pCallbackContext.ReadValueAsObject();
    }

    public void OnChainLengthChange(InputAction.CallbackContext pCallbackContext)
    {
        mChainLengthDelta = (float)pCallbackContext.ReadValueAsObject();
    }

}
