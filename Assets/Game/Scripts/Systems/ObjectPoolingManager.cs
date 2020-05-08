using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{
    readonly Dictionary<string, ObjectPool> mCurrentPools = new Dictionary<string, ObjectPool>();

    readonly Dictionary<string, List<GameObject>> mAvailablePooledObjects = new Dictionary<string, List<GameObject>>();
    readonly Dictionary<string, List<GameObject>> mUnAvailablePooledObjects = new Dictionary<string, List<GameObject>>();

    public void AddObjectPool(ObjectPool pPool)
    {
        if(pPool == null)
        {
            return;
        }
        if(string.IsNullOrEmpty(pPool.mPoolID))
        {
            return;
        }
        if (!mCurrentPools.ContainsKey(pPool.mPoolID))
        {
            mCurrentPools.Add(pPool.mPoolID, pPool);
            CreatePools(ref pPool.mPoolID);
        }
    }
    public void RemoveObjectPool(string pPoolId)
    {
        if(string.IsNullOrEmpty(pPoolId))
        {
            return;
        }
        if (mCurrentPools.ContainsKey(pPoolId))
        {
            mCurrentPools.Remove(pPoolId);
            DestroyPools(ref pPoolId);
        }
    }
    public GameObject GetPooledObject(string pPoolId)
    {
        if(string.IsNullOrEmpty(pPoolId))
        {
            return null;
        }
        List<GameObject> aAvailableGObjs = null;
        if(mAvailablePooledObjects.TryGetValue(pPoolId, out aAvailableGObjs))
        {
            if (aAvailableGObjs.Count > 0)
            {
                GameObject aPooledObj = aAvailableGObjs[0];
                aAvailableGObjs.RemoveAt(0);
                mUnAvailablePooledObjects[pPoolId].Add(aPooledObj);
                return aPooledObj;
            }
            return IncreasePools(ref pPoolId);
        }
        return null;
    }
    public void ReturnPooledObject(string pPoolId, GameObject pObject)
    {
        if(string.IsNullOrEmpty(pPoolId) || pObject == null)
        {
            return;
        }

        List<GameObject> aUnAvailableGObjs = null;
        if(mUnAvailablePooledObjects.TryGetValue(pPoolId, out aUnAvailableGObjs))
        {
            aUnAvailableGObjs.Remove(pObject);
            mAvailablePooledObjects[pPoolId].Add(pObject);
        }
    }

    void CreatePools(ref string pPoolId)
    {
        ObjectPool aPool = null;
        if(mCurrentPools.TryGetValue(pPoolId, out aPool))
        {
            mAvailablePooledObjects.Add(pPoolId, new List<GameObject>(aPool.mPoolCapacity));
            mUnAvailablePooledObjects.Add(pPoolId, new List<GameObject>(aPool.mPoolCapacity));
            for(int aI = 0; aI < aPool.mPoolCapacity; aI ++)
            {
                AddNewGameObject(ref aPool);
            }
        }
    }

    void DestroyPools(ref string pPoolId)
    {
        List<GameObject> aPooledObjects = null;
        if(mAvailablePooledObjects.TryGetValue(pPoolId, out aPooledObjects))
        {
            RemoveListObjects(ref aPooledObjects);
            mAvailablePooledObjects.Remove(pPoolId);
        }
        aPooledObjects = null;
        if(mUnAvailablePooledObjects.TryGetValue(pPoolId, out aPooledObjects))
        {
            RemoveListObjects(ref aPooledObjects);
            mUnAvailablePooledObjects.Remove(pPoolId);
        }
    }

    void RemoveListObjects(ref List<GameObject> pPooledObjects)
    {
        foreach (GameObject aObj in pPooledObjects)
        {
            if (aObj != null)
            {
                Destroy(aObj);
            }
        }
    }

    GameObject IncreasePools(ref string pPoolId)
    {
        ObjectPool aPool = null;
        if (mCurrentPools.TryGetValue(pPoolId, out aPool))
        {
            if(aPool.mPoolIncreaseAmount <= 0)
            {
                return null;
            }
            for(int aI = 0; aI < aPool.mPoolIncreaseAmount; aI ++)
            {
                AddNewGameObject(ref aPool);
            }
            return GetPooledObject(pPoolId);
        }
        return null;
    }

    void AddNewGameObject(ref ObjectPool pObjectPoolInst)
    {
        List<GameObject> aAvailObjs = null;
        if(mAvailablePooledObjects.TryGetValue(pObjectPoolInst.mPoolID, out aAvailObjs))
        {
            GameObject aNewPooledObj = Instantiate(pObjectPoolInst.mPoolObject, pObjectPoolInst.transform, false);
            aNewPooledObj.name = string.Format("{0} - {1}", pObjectPoolInst.mPoolID,
                aAvailObjs.Count + mUnAvailablePooledObjects[pObjectPoolInst.mPoolID].Count);
            IInitable aInitableComp = aNewPooledObj.GetComponent<IInitable>();
            if(aInitableComp != null)
            {
                aInitableComp.Init();
            }
            aNewPooledObj.SetActive(false);
            aAvailObjs.Add(aNewPooledObj);
        }
    }
}
