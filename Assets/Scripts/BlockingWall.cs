using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingWall : MonoBehaviour, IDamageable
{
    public IDamageable.Flag HitFlag { get => IDamageable.Flag.Wall; }

    public void TakeDamage(float damage) { }
}
