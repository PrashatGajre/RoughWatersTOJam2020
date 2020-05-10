using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item : MonoBehaviour
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
                        PhotonNetwork.CurrentRoom.SetCustomProperties(
                        new ExitGames.Client.Photon.Hashtable()
                        {
                            { "scoreStruct", GenHelpers.SerializeData(DataHandler.Instance.mScore)}
                        });
                        break;
                    }
                case Type.Score:
                    {
                        DataHandler.Instance.mScore.mScore += DataHandler.Instance.mScore.mScoreMultiplier * mModifierAmount;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(
                        new ExitGames.Client.Photon.Hashtable()
                        {
                            { "scoreStruct", GenHelpers.SerializeData(DataHandler.Instance.mScore)}
                        });
                        break;
                    }
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }


}
