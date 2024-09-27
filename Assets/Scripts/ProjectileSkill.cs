using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkill : SkillJudgeBase
{
    [SerializeField] Projectile projectilePrefab;

    public override void Perform()
    {
        var instance = GameObject.Instantiate(projectilePrefab);
        instance.OnTriggerEnter += WhenTriggered;
    }
}
