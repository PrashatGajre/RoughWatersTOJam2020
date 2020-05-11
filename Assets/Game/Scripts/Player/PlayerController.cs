using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float mReverseNegateMultiplier = 0.35f;
    Vector2 mMoveVector;
    [HideInInspector] public Raft mCurrentRaft;

    void FixedUpdate()
    {
        if (mCurrentRaft == null)
        {
            return; 
        }
        Vector2 aForceApplied = mMoveVector *
            mCurrentRaft.mSpeed * (mMoveVector.x < 0 ? mReverseNegateMultiplier : 1.0f)
            * Time.deltaTime;
        mCurrentRaft.photonView.RPC("ApplyRelativeForce", PhotonNetwork.MasterClient, aForceApplied);
    }

    public void OnMoveStick(InputAction.CallbackContext pCallbackContext)
    {
        if (DataHandler.Instance.mGameStarted)
        {
            mMoveVector = (Vector2)pCallbackContext.ReadValueAsObject();
        }
    }

    public void OnChangeRaft(InputAction.CallbackContext pCallbackContext)
    {
        if (DataHandler.Instance.mGameStarted)
        {
            if(pCallbackContext.ReadValueAsButton())
            {
                NetworkManager.Instance.RaiseEvent(
                    NetworkManager.EVNT_CHANGERAFT,
                    new object[] { PhotonNetwork.IsMasterClient },
                    new Photon.Realtime.RaiseEventOptions()
                    {
                        Receivers = Photon.Realtime.ReceiverGroup.MasterClient
                    },
                    new ExitGames.Client.Photon.SendOptions()
                    {
                        Reliability = true
                    });
            }
        }
    }
}
