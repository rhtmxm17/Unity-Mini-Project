using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Skill Effect/Default")]
public class DefaultSkillEffect : SkillEffectBase
{
    [SerializeField] float baseDamage;

    public override SkillEffectBase Clone()
    {
        var clone = CreateInstance<DefaultSkillEffect>();
        clone.source = this.source;
        clone.baseDamage = this.baseDamage;
        return clone;
    }

    public override void Effect(IDamageable target)
    {
        Debug.Log($"Skill Hitted: {target}");
        target.TakeDamage(baseDamage, source);
    }
}
