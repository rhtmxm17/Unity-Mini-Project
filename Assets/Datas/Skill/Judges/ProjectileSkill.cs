using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Skill Judge/Projectile Skill")]
public class ProjectileSkill : SkillJudgeBase
{
    [SerializeField] Projectile projectilePrefab;

    public ProjectileSkill() { }

    public ProjectileSkill(ProjectileSkill other) : base(other)
    {
        this.projectilePrefab = other.projectilePrefab;
    }

    public override SkillJudgeBase Clone()
    {
        var clone = CreateInstance<ProjectileSkill>();
        clone.projectilePrefab = this.projectilePrefab;
        clone.hitMask = this.hitMask;
        return clone;
    }

    public override void Perform(Transform transform)
    {
        var instance = GameObject.Instantiate(projectilePrefab, transform.position, transform.rotation);
        instance.OnTriggered += WhenTriggered;
    }
}
