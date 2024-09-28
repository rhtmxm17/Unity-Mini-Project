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
        return new ProjectileSkill(this);
    }

    public override void Perform()
    {
        var instance = GameObject.Instantiate(projectilePrefab);
        instance.OnTriggerEnter += WhenTriggered;
    }
}
