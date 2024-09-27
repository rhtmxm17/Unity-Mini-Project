using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillEffectBase
{
    public IUnit source;

    public abstract void Effect(IDamageable target);
}
