using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float mReverseNegateMultiplier = 0.35f;
    Vector2 mMoveVector;
    [HideInInspector] public Raft mCurrentRaft;

    private void Start()
    {
        if (photonView.IsMine)
        {
            var actionEventArray = GameObject.FindObjectOfType<PlayerInput>().actionEvents;

            foreach (var actionEvent in actionEventArray)
            {
                if (actionEvent.actionName.Contains("MoveRaft"))
                {
                    actionEvent.AddListener(OnMoveStick);
                }
                else if (actionEvent.actionName.Contains("ChangeRaft"))
                {
                    actionEvent.AddListener(OnChangeRaft);
                }
            }
            mCurrentRaft = DataHandler.Instance.mActiveRafts[(PhotonNetwork.IsMasterClient ? 0 : 2)];
            mCurrentRaft.mSelected = true;
        }
    }

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
                if(!photonView.IsMine)
                {
                    return;
                }
                int aIx = (int)mCurrentRaft.mRaftIndex;
                aIx = (aIx + 1) % DataHandler.Instance.mActiveRafts.Length;
                while (aIx != (int) mCurrentRaft.mRaftIndex)
                {
                    if(DataHandler.Instance.mActiveRafts[aIx].mSelected)
                    {
                        aIx = (aIx + 1) % DataHandler.Instance.mActiveRafts.Length;
                    }
                    else
                    {
                        Raft aRaft = DataHandler.Instance.mActiveRafts[aIx];
                        mCurrentRaft.mSelected = false;
                        aRaft.mSelected = true;
                        ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                        bool[] selectedRafts = (bool[])customProperties["selectedRafts"];
                        customProperties = new ExitGames.Client.Photon.Hashtable();
                        selectedRafts[(int)mCurrentRaft.mRaftIndex] = false;
                        selectedRafts[(int)aRaft.mRaftIndex] = true;
                        //customProperties["selectedRafts"] = selectedRafts;
                        customProperties.Add("selectedRafts",selectedRafts);
                        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
                        mCurrentRaft = aRaft;
                    }
                }

            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
