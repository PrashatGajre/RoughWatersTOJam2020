using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public string mPoolID;
    public GameObject mPoolObject;
    public int mPoolCapacity;
    public int mPoolIncreaseAmount;

    void Awake()
    {
        ObjectPoolingManager.Instance.AddObjectPool(this);
    }

    void OnDestroy()
    {
        if(!ObjectPoolingManager.IsValidSingleton())
        {
            return;
        }
        ObjectPoolingManager.Instance.RemoveObjectPool(mPoolID);
    }

}
