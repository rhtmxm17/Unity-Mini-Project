using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillEffectBase : ScriptableObject
{
    public IUnit source;

    public abstract SkillEffectBase Clone();

    public abstract void Effect(IDamageable target);
}
