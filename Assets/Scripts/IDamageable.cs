using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 타격 가능(또는 투사체가 가로막히는)
public interface IDamageable
{
    [System.Flags]
    public enum Flag
    {
        Disable = 0,
        Wall    = 1 << 0,
        Player  = 1 << 1,
        Monster = 1 << 2,
    }

    public Flag HitFlag { get; }

    public void TakeDamage(float damage, IUnit source = null);
}
