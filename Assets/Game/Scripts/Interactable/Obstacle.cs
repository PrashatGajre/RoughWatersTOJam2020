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
    float mCollidedTime = 0.0f;

    protected virtual void Update()
    {
        if(mCollidedTime > 0.0f)
        {
            mCollidedTime -= Time.deltaTime;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D pCollision)
    {
        if(mCollidedTime > 0.0f)
        {
            return;
        }
        Raft aRaft = pCollision.collider.gameObject.GetComponent<Raft>();
        if(aRaft == null)
        {
            return;
        }
        mCollidedTime = mRedamageAfterTime;
        switch(mObstacleType)
        {
            case ObstacleType.None:
                break;
            case ObstacleType.Damage:
                {
                    aRaft.mHealth -= mDamageAmount;
                    if(aRaft.mHealth <= 0.0f)
                    {
                        aRaft.mHealth = 0.0f;
                        //do gameover
                    }
                    break;
                }
            case ObstacleType.Push:
                {
                    Vector3 aDirection = (aRaft.transform.position - transform.position).normalized;
                    aRaft.mRigidbody.AddRelativeForce(aDirection * mPushMagnitude, ForceMode2D.Impulse);
                    break;
                }
        }
    }



}
