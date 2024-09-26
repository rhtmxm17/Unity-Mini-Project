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

    public List<SingleData> elements; // 방에 배치될 요소(몬스터, 지형지물)들
    public GameObject floorShapePrefab; // Vector3.zero를 입구 기준점으로 하는 바닥 형태
    public Vector3 exitPosition;    //
    public Quaternion exitRotation; // 다음 방을 이어붙일 출구 좌표
}
