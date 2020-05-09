using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomObject : MonoBehaviour
{
    public RoomInfo mRoomInfo { get; private set; }
    [SerializeField] Toggle mToggleRoom;
    [SerializeField] Text mRoomName;

    public void SetRoomInfo(RoomInfo aRoomInfo, ToggleGroup aToggleGroup)
    {
        if (mToggleRoom == null)
        {
            mToggleRoom = GetComponent<Toggle>();
        }

        this.mRoomInfo = aRoomInfo;
        gameObject.name = aRoomInfo.Name;
        mRoomName.text = aRoomInfo.Name;
        mToggleRoom.group = aToggleGroup;
    }
}
