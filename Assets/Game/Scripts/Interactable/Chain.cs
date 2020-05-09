using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public Rigidbody2D mRigidbody;
    public HingeJoint2D mHingeJoint;
    public BoxCollider2D mChainCollider;
    public Chain mConnectedTo;
    public Chain mConnectedFrom;
}
