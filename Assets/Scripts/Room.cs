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

    private void Awake()
    {
        // NavMesh 생성용 정보
        List<NavMeshBuildSource> buildSources = new();

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
                buildSources.Add(tag.GetBuildSource());
            }
        }

        // 기존 Agents 정보 가져오기
        NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(0);

        // 데이터 생성
        // 사이즈는 임시로 지정했음
        Debug.LogWarning("임시로 지정한 NavMeshBuild 사이즈 사용중");
        NavMeshData data = NavMeshBuilder.BuildNavMeshData(settings, buildSources, new Bounds(Vector3.forward * 5f, Vector3.one * 10f), transform.position, transform.rotation);
        
        NavMesh.AddNavMeshData(data);


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
