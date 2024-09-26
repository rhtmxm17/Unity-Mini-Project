using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SampleSkill : MonoBehaviour
{
    public IDamageable.Flag hitMask;

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Init()
    {
        body.velocity = transform.forward * 5f;
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable target) && hitMask.HasFlag(target.HitFlag))
        {
            Debug.Log($"OnTriggerEnter: {other.name}");
            target.TakeDamage(5f);
            Destroy(gameObject);
        }
    }

}
