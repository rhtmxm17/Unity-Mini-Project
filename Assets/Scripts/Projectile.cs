using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public event UnityAction<Collider> OnTriggerEnter;

#if UNITY_EDITOR
    private void Awake()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Projectile"))
            Debug.LogWarning("투사체 레이어 설정 누락");
    }
#endif
}
