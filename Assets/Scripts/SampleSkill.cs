using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSkill : MonoBehaviour
{
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Init(PlayerModel source)
    {
        body.velocity = transform.forward * 5f;
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");

    }

}
