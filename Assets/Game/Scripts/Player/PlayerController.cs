using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 mMoveVector;
    float mChangeDelta;

    Raft mCurrentRaft;

    public void OnMoveStick(InputAction.CallbackContext pCallbackContext)
    {
        mMoveVector = (Vector2) pCallbackContext.ReadValueAsObject();
    }

    public void OnChangeRaft(InputAction.CallbackContext pCallbackContext)
    {
        mChangeDelta = (float)pCallbackContext.ReadValueAsObject();
    }


}
