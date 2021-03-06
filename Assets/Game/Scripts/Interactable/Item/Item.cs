﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item : MonoBehaviourPun
{
    public enum Type
    {
        Health,
        Fatigue,
        Score,
        Multiplier
    }

    [SerializeField] Type mType;
    [SerializeField] float mModifierAmount = 1.0f;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (!photonView.IsMine)
        {
            return;
        }
        Raft aRaft = collision.GetComponent<Raft>();
        if (aRaft != null)
        {
            EffectsManager.Instance.ItemCollectedEffect(mType);
            switch (mType)
            {
                case Type.Fatigue:
                    {
                        aRaft.mFatigue += mModifierAmount;
                        if (aRaft.mFatigue > aRaft.mMaxFatigue)
                        {
                            aRaft.mFatigue = aRaft.mMaxFatigue;
                        }
                        break;
                    }
                case Type.Health:
                    {
                        aRaft.mHealth += mModifierAmount;
                        if (aRaft.mHealth > aRaft.mMaxHealth)
                        {
                            aRaft.mHealth = aRaft.mMaxHealth;
                        }
                        break;
                    }
                case Type.Multiplier:
                    {
                        DataHandler.Instance.mScore.mScoreMultiplier *= mModifierAmount;
                        break;
                    }
                case Type.Score:
                    {
                        DataHandler.Instance.mScore.mScore += DataHandler.Instance.mScore.mScoreMultiplier * mModifierAmount;
                        break;
                    }
            }
            photonView.RPC("Kill", RpcTarget.All, 1);
        }
    }

    [PunRPC]
    void Kill(int pPlaceHolder, PhotonMessageInfo pInfo)
    {
        Destroy(gameObject);
    }
}
