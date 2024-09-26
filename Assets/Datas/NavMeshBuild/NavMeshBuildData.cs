using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "ScriptableObject/NavMesh Build Data")]
public class NavMeshBuildData : ScriptableObject
{
    [SerializeField, Range(0, 31), Tooltip("네비게이션 에리어")] private int area;
    [SerializeField] private NavMeshBuildSourceShape shape;
    [SerializeField, Tooltip("shape가 기본 도형/ModifierBox일 경우 사용")] private Vector3 size = Vector3.one;

    public NavMeshBuildSource GetBuildSource(Transform sourceTransform)
    {
        return new NavMeshBuildSource()
        {
            area = area,
            shape = shape,
            transform = sourceTransform.localToWorldMatrix,
            size = size,
        };
    }
}
