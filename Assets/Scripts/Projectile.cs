using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public event UnityAction<Collider> OnTriggered;
    public float speed;

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();

#if UNITY_EDITOR
        if (gameObject.layer != LayerMask.NameToLayer("Projectile"))
            Debug.LogWarning("투사체 레이어 설정 누락");
#endif
    }

    private void OnEnable()
    {
        body.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggered?.Invoke(other);
    }
}
