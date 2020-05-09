using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidBehaviour : SteeringBehaviourBase
{
    [System.Serializable]
    public struct Feeler
    {
        public float mDistance;
        public Vector3 mOffset;
    }


    [SerializeField] List<Feeler> mFeelers;

    [SerializeField] LayerMask mObstacleLayers;

    public override Vector3 CalculateForce()
    {
        RaycastHit2D aHit;
        Vector3 aFinalForce = Vector3.zero;
        foreach (Feeler aFeeler in mFeelers)
        {
            Vector3 aFPos = mSteeringAgent.transform.rotation * aFeeler.mOffset + mSteeringAgent.transform.position;
            aHit = Physics2D.Raycast(aFPos, mSteeringAgent.transform.up, aFeeler.mDistance, mObstacleLayers);
            if (aHit.collider != null)
            {
                Vector3 aColliderPosition = aHit.collider.transform.position;
                Vector3 aCollisionPoint = Vector3.Project(aColliderPosition - mSteeringAgent.transform.position, mSteeringAgent.transform.up) + mSteeringAgent.transform.position;
                float aAvoidanceStrength = 1.0f + (aCollisionPoint.magnitude - aFeeler.mDistance) / aFeeler.mDistance;
                aFinalForce += (aCollisionPoint - aColliderPosition).normalized * aAvoidanceStrength;
            }
        }
        return aFinalForce;
    }


    void OnDrawGizmos()
    {
        foreach (Feeler aFeeler in mFeelers)
        {
            Vector3 aFPos = transform.rotation * aFeeler.mOffset + transform.position;
            Debug.DrawLine(aFPos, aFPos + transform.up * aFeeler.mDistance, Color.blue);
        }
    }
}