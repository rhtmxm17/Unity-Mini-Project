using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] RoomData roomData;

    private GameObject floor;
    private GameObject elements;
    private GameObject exit;

    private void Awake()
    {
        floor = Instantiate(roomData.floorShapePrefab, this.transform);

        exit = new GameObject("Exit");
        exit.transform.parent = this.transform;
        exit.transform.SetLocalPositionAndRotation(roomData.exitPosition, roomData.exitRotation);

        elements = new GameObject("Elements");
        elements.transform.parent = this.transform;
        foreach (var element in roomData.elements)
        {
            Instantiate(element.prefab, element.position, element.rotation, this.elements.transform);
        }
    }

#if UNITY_EDITOR // 씬 뷰를 통한 ScriptableObject 편집용
    [ContextMenu("Room Data에 저장")]
    private void ApplyToRoomData()
    {
        Debug.LogError("Not Implement");
    }
#endif
}
