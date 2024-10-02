using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    public RoomData roomData;
    [SerializeField] GameObject floor;
    [SerializeField] GameObject units;
    [SerializeField] GameObject exit;

    public Transform ExitTransform => exit.transform;
    public event UnityAction OnRoomClear;

    [SerializeField] // 확인용
    private int countUnits;

    [ContextMenu("CreateRomm 호출 테스트")]
    public void CreateRoom()
    {
        // 지형 정보 불러오기
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

        // NavMesh 갱신
        NavManager.Instance.BakeNavMesh();

        // 유닛 정보 불러오기
        foreach (var element in roomData.units)
        {
            var clone = Instantiate(element.prefab, this.units.transform);
            clone.transform.SetLocalPositionAndRotation(element.position, element.rotation);
            if (clone.TryGetComponent(out IUnit unit))
            {
                countUnits++;
                unit.OnDie += () =>
                {
                    countUnits--;
                    if (countUnits == 0)
                        OnRoomClear?.Invoke();
                };
            }
        }

        // 출구 위치 불러오기
        exit.transform.SetLocalPositionAndRotation(roomData.exitPosition, roomData.exitRotation);

        // 유닛이 없는 방일 경우 OnDie를 트리거 하지 않으므로 예외 처리
        if (countUnits == 0)
            OnRoomClear?.Invoke();
    }

#if UNITY_EDITOR // 씬 뷰를 통한 ScriptableObject 편집용
    [ContextMenu("Room Data에 저장")]
    private void CreateRoomData()
    {
        RoomData newRoom = ScriptableObject.CreateInstance<RoomData>();

        // 지형 정보 저장
        for (int i = 0; i < floor.transform.childCount; i++)
        {
            Transform floorElement = floor.transform.GetChild(i);

            newRoom.props.Add(new RoomData.SingleData()
            {
                // 오브젝트로부터 프리펩 참조 가져오기
                prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(floorElement.gameObject),
                position = floorElement.localPosition,
                rotation = floorElement.localRotation,
            });
        }

        // 지형 정보 저장
        for (int i = 0; i < units.transform.childCount; i++)
        {
            Transform unitElement = units.transform.GetChild(i);

            newRoom.units.Add(new RoomData.SingleData()
            {
                prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(unitElement.gameObject),
                position = unitElement.localPosition,
                rotation = unitElement.localRotation,
            });
        }

        // 출구 위치 저장
        newRoom.exitPosition = exit.transform.localPosition;
        newRoom.exitRotation = exit.transform.localRotation;

        AssetDatabase.CreateAsset(newRoom, "Assets/SavedRoom.asset");
        Debug.Log("Assets/SavedRoom.asset 으로 저장됨");
    }
#endif
}
