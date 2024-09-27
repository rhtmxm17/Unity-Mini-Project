using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavManager : Singleton<NavManager>
{
    [SerializeField] List<NavMeshSourceTag> navMeshSourcesOnEditor;

    private List<NavMeshBuildSource> buildSources = new();
    private NavMeshBuildSettings defaultAgentSetting;
    private Bounds bakeBound = new Bounds(Vector3.zero, Vector3.one * 1000f);

    private NavMeshData navMeshData;

    private void Awake()
    {
        AwakeSingleton(this);

    }

    private void Start()
    {
        // 기존 Agents 정보 가져오기
        // Humanoid 사용
        defaultAgentSetting = NavMesh.GetSettingsByIndex(0);

        foreach (var tag in navMeshSourcesOnEditor)
        {
            AddBuildSource(tag);
        }

        // 데이터 생성
        navMeshData = NavMeshBuilder.BuildNavMeshData
        (
            defaultAgentSetting, // 사용할 Agent
            buildSources,
            bakeBound, // Bake 범위, 빈 Bounds를 사용하면 전체 범위라는데 작동을 안함...
            Vector3.zero, // NavMesh 시작 지점
            Quaternion.identity // up 방향을 정해주는 회전
        );

        // 기존 Bake 데이터 청소
        NavMesh.RemoveAllNavMeshData();

        // 생성한 데이터 넣기
        NavMesh.AddNavMeshData(navMeshData);
    }

    public void AddBuildSource(NavMeshSourceTag source) => buildSources.Add(source.GetBuildSource());

    public void BakeNavMesh()
    {
        NavMeshBuilder.UpdateNavMeshData(navMeshData, defaultAgentSetting, buildSources, bakeBound);
    }

    // 씬 전환시 리셋에 사용
    public void ClearBuildSource() => buildSources.Clear();
}
