using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Skill Effect/Default")]
public class DefaultSkillEffect : SkillEffectBase
{
    public override SkillEffectBase Clone()
    {
        return new DefaultSkillEffect();
    }

    public override void Effect(IDamageable target)
    {
        Debug.Log($"Skill Hitted: {target}");
        target.TakeDamage(5f, source);
    }
}
