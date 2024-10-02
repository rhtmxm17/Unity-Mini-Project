using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Stage : MonoBehaviour
{
    [SerializeField] Room roomPrefab;
    [SerializeField] RoomData[] randomRoomTable;
    [SerializeField] RoomData bossRoom;
    [SerializeField, Tooltip("보스방을 제외한 방 개수")] int numRooms;

    public UnityEvent OnStageClear;

#if UNITY_EDITOR
    [ContextMenu("CreateStage 호출 (Play Mode)", true)]
    private bool IsPlayMode()
    {
        return EditorApplication.isPlaying;
    }

    [ContextMenu("CreateStage 호출 (Play Mode)")]
#endif
    public void CreateStage()
    {
        if (false == EditorApplication.isPlaying)
        {

        }

        Vector3 roomPosition = transform.position;
        Quaternion roomRotation = transform.rotation;

        Room firstRoom = null;
        Room previousRoom = null;

        for (int i = 0; i < numRooms; i++)
        {
            Room nextRoom = Instantiate(roomPrefab, roomPosition, roomRotation); // 이전 방의 Exit을 진입 위치로
            nextRoom.name = $"Room {i}";
            nextRoom.roomData = randomRoomTable[Random.Range(0, randomRoomTable.Length)];

            roomPosition = nextRoom.transform.TransformPoint(nextRoom.roomData.exitPosition);
            roomRotation = nextRoom.transform.rotation * nextRoom.roomData.exitRotation;

            // 이전 방이 클리어되면 생성되도록 설정
            if (previousRoom != null)
                previousRoom.OnRoomClear += nextRoom.CreateRoom;
            else
                firstRoom = nextRoom;
            previousRoom = nextRoom;
        }

        // 보스룸 생성
        {
            Room nextRoom = Instantiate(roomPrefab, roomPosition, roomRotation);
            nextRoom.name = $"Boss Room";
            nextRoom.roomData = bossRoom;

            roomPosition = nextRoom.transform.TransformPoint(nextRoom.roomData.exitPosition);
            roomRotation = nextRoom.transform.rotation * nextRoom.roomData.exitRotation;

            // 이전 방이 클리어되면 생성되도록 설정
            if (previousRoom != null)
                previousRoom.OnRoomClear += nextRoom.CreateRoom;
            else
                nextRoom.CreateRoom();
            previousRoom = nextRoom;
        }

        // 보스룸 클리어 이벤트
        previousRoom.OnRoomClear += OnStageClear.Invoke;

        // 첫번째 방 생성
        firstRoom.CreateRoom();
    }
}
