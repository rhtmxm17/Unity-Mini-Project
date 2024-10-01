using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Room Data")]
public class RoomData : ScriptableObject
{
    [System.Serializable]
    public struct SingleData
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
    }

    public List<SingleData> props = new(); // 방에 배치될 지형지물(NavMesh 생성에 사용되는 장애물을 포함)
    public List<SingleData> units = new(); // 방에 배치될 몬스터들, IUnit을 구현하고 있어야 처치 카운트 가능
    public Vector3 exitPosition;    //
    public Quaternion exitRotation; // 다음 방을 이어붙일 출구 좌표
}
