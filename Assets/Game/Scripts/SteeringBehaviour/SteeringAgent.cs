using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    public enum SummingMethod
    {
        WeightedAverage,
        Prioritized,
        Dithered
    };
    [SerializeField] SummingMethod mSummingMethod = SummingMethod.WeightedAverage;
    [SerializeField] float mMass = 1.0f;
    public float mMaxSpeed = 1.0f;
    [SerializeField] float mMaxForce = 10.0f;
    [SerializeField] float mDeadZone = 5.0f;
    [SerializeField] float mAngularDampeningTime = 5.0f;
    [HideInInspector] public Vector3 mVelocity = Vector3.zero;
    List<SteeringBehaviourBase> mSteeringBehaviours = new List<SteeringBehaviourBase>();

    private Animator mAnimator;

    void Start()
    {
        mSteeringBehaviours.AddRange(GetComponentsInChildren<SteeringBehaviourBase>());
        foreach (SteeringBehaviourBase aBehaviour in mSteeringBehaviours)
        {
            aBehaviour.mSteeringAgent = this;
        }
        mAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 aSteeringForce = CalculateSteeringForce();
        aSteeringForce.z = 0.0f;

        Vector3 aAcceleration = aSteeringForce * (1.0f / mMass);
        mVelocity = mVelocity + (aAcceleration * Time.deltaTime);
        mVelocity = Vector3.ClampMagnitude(mVelocity, mMaxSpeed);

        float aSpeed = mVelocity.magnitude;

        if (aSpeed > 0.0f)
        {
            transform.position += transform.up * aSpeed * Time.deltaTime;
            float aAngle = Vector3.Angle(transform.up, mVelocity);
            if (Mathf.Abs(aAngle) <= mDeadZone)
            {

            }
            else
            {
                transform.rotation = transform.rotation * Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0, 0, aAngle), mAngularDampeningTime * Time.deltaTime);
            }
        }
    }


    Vector3 CalculateSteeringForce()
    {
        Vector3 aTotalForce = Vector3.zero;
        switch (mSummingMethod)
        {
            case SummingMethod.Prioritized:
                mSteeringBehaviours.Sort((aFirst, aSecond) => { return aSecond.mPriority.CompareTo(aFirst.mPriority); });
                break;
            case SummingMethod.Dithered:
                GenHelpers.Shuffle(ref mSteeringBehaviours);
                break;
        }
        foreach (SteeringBehaviourBase aBehaviour in mSteeringBehaviours)
        {
            if (!aBehaviour.enabled)
            {
                continue;
            }
            switch (mSummingMethod)
            {
                case SummingMethod.Prioritized:
                case SummingMethod.Dithered:
                    aTotalForce += aBehaviour.CalculateForce();
                    Vector3.ClampMagnitude(aTotalForce, mMaxForce);
                    if (aTotalForce.sqrMagnitude == mMaxForce * mMaxForce)
                    {
                        return aTotalForce;
                    }
                    break;
                case SummingMethod.WeightedAverage:
                    aTotalForce += aBehaviour.CalculateForce() * aBehaviour.mWeight;
                    Vector3.ClampMagnitude(aTotalForce, mMaxForce);
                    break;
            }
        }
        return aTotalForce;
    }

}
