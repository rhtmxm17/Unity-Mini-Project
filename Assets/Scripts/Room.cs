using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    [SerializeField] RoomData roomData;

    private GameObject floor;
    private GameObject elements;
    private GameObject exit;

    [ContextMenu("CreateRomm 호출 테스트")]
    public void CreateRomm()
    {
        floor = new GameObject("Floor");
        floor.transform.parent = this.transform;
        floor.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        foreach (var element in roomData.props)
        {
            var clone = Instantiate(element.prefab, this.floor.transform);
            clone.transform.SetLocalPositionAndRotation(element.position, element.rotation);

            // 태그를 달고있다면 빌드 정보 등록
            if (clone.TryGetComponent(out NavMeshSourceTag tag))
            {
                NavManager.Instance.AddBuildSource(tag);
            }
        }

        NavManager.Instance.BakeNavMesh();

        exit = new GameObject("Exit");
        exit.transform.parent = this.transform;
        exit.transform.SetLocalPositionAndRotation(roomData.exitPosition, roomData.exitRotation);

        elements = new GameObject("Elements");
        elements.transform.parent = this.transform;
        elements.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        foreach (var element in roomData.units)
        {
            var clone = Instantiate(element.prefab, this.elements.transform);
            clone.transform.SetLocalPositionAndRotation(element.position, element.rotation);
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
