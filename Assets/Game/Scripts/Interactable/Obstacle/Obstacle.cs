using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType
    {
        None,
        Damage,
        Push
    }

    [SerializeField] ObstacleType mObstacleType = ObstacleType.Damage;
    [SerializeField] float mPushMagnitude = 10.0f;
    [SerializeField] float mDamageAmount = 10.0f;
    [SerializeField] float mRedamageAfterTime = 0.5f;
    protected float mCollidedTime = 0.0f;

    protected virtual void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (mCollidedTime > 0.0f)
        {
            mCollidedTime -= Time.deltaTime;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D pCollision)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (mCollidedTime > 0.0f)
        {
            return;
        }
        Raft aRaft = pCollision.collider.gameObject.GetComponent<Raft>();
        if(aRaft == null)
        {
            return;
        }
        ObstacleBehaviour(aRaft);
    }

    protected void ObstacleBehaviour(Raft pRaft)
    {
        if(!DataHandler.Instance.mGameStarted)
        {
            return;
        }
        mCollidedTime = mRedamageAfterTime;
        switch (mObstacleType)
        {
            case ObstacleType.None:
                break;
            case ObstacleType.Damage:
                {
                    pRaft.mHealth -= mDamageAmount;
                    EffectsManager.Instance.DamageEffect();
                    if (pRaft.mHealth <= 0.0f)
                    {
                        pRaft.mHealth = 0.0f;
                        //GAME LOST
                        object[] content = new object[] { "LOST" };
                        NetworkManager.Instance.RaiseEvent(NetworkManager.EVNT_GAMELOST, content,
                            new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All },
                            new ExitGames.Client.Photon.SendOptions { Reliability = true });
                    }
                    break;
                }
            case ObstacleType.Push:
                {
                    Vector3 aDirection = (pRaft.transform.position - transform.position).normalized;
                    pRaft.mRigidbody.AddRelativeForce(aDirection * mPushMagnitude, ForceMode2D.Impulse);
                    break;
                }
        }
    }



}
