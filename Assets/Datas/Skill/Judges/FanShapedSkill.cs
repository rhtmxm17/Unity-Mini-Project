using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Skill Judge/Fan Shaped Skill")]
public class FanShapedSkill : SkillJudgeBase
{
    public float radius; // 부채꼴 반경
    [Range(-1, 1)] public float maxCosAngle; // 코사인 부채꼴 각도

    private Collider[] results = new Collider[16];
    [SerializeField] LayerMask physicsMask;// = LayerMask.GetMask("Player", "Body");

    public override SkillJudgeBase Clone()
    {
        var clone = CreateInstance<FanShapedSkill>();
        clone.hitMask = this.hitMask;
        clone.radius = this.radius;
        clone.maxCosAngle = this.maxCosAngle;
        clone.physicsMask = this.physicsMask;
        return clone;
    }

    public override void Perform(Transform transform)
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, results, physicsMask);
        Debug.Log($"감지된 충돌체 : {count}개");

        for (int i = 0; i < count; i++)
        {
            float cosAngle = Vector3.Dot(transform.forward, (results[i].transform.position - transform.position).normalized);

            // 지정한 각도보다 작은 각도일 경우
            if (cosAngle > maxCosAngle)
                WhenTriggered(results[i]);
        }
    }
}
