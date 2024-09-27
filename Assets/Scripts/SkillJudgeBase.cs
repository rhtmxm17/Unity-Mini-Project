using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SkillJudgeBase
{
    public UnityAction<IDamageable> OnSkillHited;

    public IDamageable.Flag hitMask;

    public abstract void Perform();

    public void WhenTriggered(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable target) && hitMask.HasFlag(target.HitFlag))
        {
            OnSkillHited?.Invoke(target);
        }
    }
}
