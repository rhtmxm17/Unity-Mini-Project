using UnityEngine;
using UnityEngine.AI;

public class NavMeshSourceTag : MonoBehaviour
{
    [SerializeField] private NavMeshBuildData sharedData;
    [SerializeField, Tooltip("shape가 Mesh일 경우 사용")] private MeshFilter meshFilter;

    private void Awake()
    {
        if (meshFilter == null && sharedData.Shape == NavMeshBuildSourceShape.Mesh)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
    }

    public NavMeshBuildSource GetBuildSource()
    {
        var source = sharedData.GetBuildSource(transform);
        source.component = meshFilter;
        source.sourceObject = meshFilter != null ? meshFilter.sharedMesh : null;

        if (source.shape == NavMeshBuildSourceShape.Mesh
                && source.component == null
                && source.sourceObject == null)
            Debug.LogWarning("NavMeshBuild 정보가 잘못되었습니다.");

        return source;
    }
}
