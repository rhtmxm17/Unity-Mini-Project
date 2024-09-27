using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SkillJudgeBase : ScriptableObject
{
    public UnityAction<IDamageable> OnSkillHited;

    public IDamageable.Flag hitMask;

    public SkillJudgeBase() { }

    public SkillJudgeBase(SkillJudgeBase other)
    {
        this.hitMask = other.hitMask;
    }

    public abstract SkillJudgeBase Clone();

    public abstract void Perform();

    public void WhenTriggered(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable target) && hitMask.HasFlag(target.HitFlag))
        {
            OnSkillHited?.Invoke(target);
        }
    }
}
