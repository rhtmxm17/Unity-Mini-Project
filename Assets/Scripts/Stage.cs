using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Stage : MonoBehaviour
{
    [SerializeField] RoomData[] randomRoomTable;
    [SerializeField] RoomData bossRoom;
    [SerializeField, Tooltip("보스방을 제외한 방 개수")] int numRooms;

    public event UnityAction OnStageClear;

    public void CreateStage()
    {
        Vector3 roomPosition = transform.position;
        Quaternion roomRotation = transform.rotation;

        Room previousRoom = null;

        for (int i = 0; i < numRooms; i++)
        {
            Room nextRoom = new GameObject($"Room{i}").AddComponent<Room>();
            nextRoom.transform.SetPositionAndRotation(roomPosition, roomRotation); // 이전 방의 Exit을 진입 위치로
            nextRoom.roomData = randomRoomTable[Random.Range(0, randomRoomTable.Length)];

            roomPosition = nextRoom.roomData.exitPosition;
            roomRotation = nextRoom.roomData.exitRotation;

            // 이전 방이 클리어되면 생성되도록 설정
            if (previousRoom != null)
                previousRoom.OnRoomClear += nextRoom.CreateRoom;
            else
                nextRoom.CreateRoom();
            previousRoom = nextRoom;
        }

        // 보스룸 생성
        {
            Room nextRoom = new GameObject($"Boss Room").AddComponent<Room>();
            nextRoom.transform.SetPositionAndRotation(roomPosition, roomRotation); // 이전 방의 Exit을 진입 위치로
            nextRoom.roomData = bossRoom;

            roomPosition = nextRoom.roomData.exitPosition;
            roomRotation = nextRoom.roomData.exitRotation;

            // 이전 방이 클리어되면 생성되도록 설정
            if (previousRoom != null)
                previousRoom.OnRoomClear += nextRoom.CreateRoom;
            else
                nextRoom.CreateRoom();
            previousRoom = nextRoom;
        }

        // 보스룸 클리어 이벤트
        previousRoom.OnRoomClear += OnStageClear;
    }
}
