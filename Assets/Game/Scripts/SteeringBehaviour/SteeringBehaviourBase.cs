using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviourBase : MonoBehaviour
{
	public float mWeight = 1.0f;
	public float mPriority = 1.0f;
	protected Vector3 mTarget;
	public abstract Vector3 CalculateForce();
	[HideInInspector] public SteeringAgent mSteeringAgent;

	public virtual void CalculateNewPath(Vector3 pPathTarget)
	{
		mTarget = pPathTarget;
	}
}