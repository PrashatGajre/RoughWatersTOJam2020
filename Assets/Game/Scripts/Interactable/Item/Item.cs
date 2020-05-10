using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Raft aRaft = collision.GetComponent<Raft>();
        if(aRaft != null)
        {
            switch(mType)
            {
                case Type.Fatigue:
                    {
                        aRaft.mFatigue += mModifierAmount;
                        if(aRaft.mFatigue > aRaft.mMaxFatigue)
                        {
                            aRaft.mFatigue = aRaft.mMaxFatigue;
                        }
                        break;
                    }
                case Type.Health:
                    {
                        aRaft.mHealth += mModifierAmount;
                        if(aRaft.mHealth > aRaft.mMaxHealth)
                        {
                            aRaft.mHealth = aRaft.mMaxHealth;
                        }
                        break;
                    }
                case Type.Multiplier:
                    {
                        DataHandler.Instance.mScoreMultiplier *= mModifierAmount;
                        break;
                    }
                case Type.Score:
                    {
                        DataHandler.Instance.mCurrentScore += DataHandler.Instance.mScoreMultiplier * mModifierAmount;
                        break;
                    }
            }
            Destroy(gameObject);
        }
    }


}
