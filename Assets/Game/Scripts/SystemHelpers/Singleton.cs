using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T mInstance;

	public static T Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = (T)FindObjectOfType(typeof(T));
				if (mInstance == null)
				{
                    Debug.LogAssertionFormat("Singleton of type : {0} not in scene", typeof(T).Name);
				}
			}
			return mInstance;
		}
	}

	public static bool IsValidSingleton()
	{
		return (mInstance != null);
	}

	public static void Destroy()
	{
        if(mInstance != null && mInstance.gameObject != null)
        {
            Destroy(mInstance.gameObject);
            mInstance = null;
        }
    }
}
