using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class GenHelpers
{
    public static void Shuffle<T>(ref List<T> pList)
    {
        int aN = pList.Count;
        while (aN > 1)
        {
            aN--;
            int aK = UnityEngine.Random.Range(0,aN + 1);
            T aValue = pList[aK];
            pList[aK] = pList[aN];
            pList[aN] = aValue;
        }
    }
    public static Vector2 GetClosestPoint(Vector2 pOrigin, Vector2 pEnd, Vector2 pPoint)
    {
        Vector2 aDirection = pEnd - pOrigin;
        float aMaxMag = aDirection.magnitude;
        aDirection.Normalize();
        Vector2 aDistFromOrigin = pPoint - pOrigin;
        float aDotProduct = Vector2.Dot(aDistFromOrigin, aDirection);
        aDotProduct = Mathf.Clamp(aDotProduct, 0f, aMaxMag);
        return pOrigin + aDirection * aDotProduct;
    }
    public static byte[] SerializeData<T>(T pData)
    {
        int aSize = Marshal.SizeOf(pData);
        byte[] aSerializedArray = new byte[aSize];
        IntPtr aPointer = Marshal.AllocHGlobal(aSize);
        Marshal.StructureToPtr(pData, aPointer, true);
        Marshal.Copy(aPointer, aSerializedArray, 0, aSize);
        Marshal.FreeHGlobal(aPointer);
        return aSerializedArray;
    }
    public static void DeSerializeData<T>(byte[] pData, ref T pObject)
    {
        int aSize = Marshal.SizeOf(pObject);
        IntPtr aPointer = Marshal.AllocHGlobal(aSize);
        Marshal.Copy(pData, 0, aPointer, aSize);
        pObject = Marshal.PtrToStructure<T>(aPointer);
        Marshal.FreeHGlobal(aPointer);
    }
}

/// <summary>
/// Used for pooled objects that need to set some default functionality while they are inactive 
/// </summary>
public interface IInitable
{
    void Init();
}

[System.Serializable]
public struct Score
{
    public float mScore;
    public float mScoreMultiplier;
}