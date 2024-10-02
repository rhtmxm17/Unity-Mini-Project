using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EventTrigger : MonoBehaviour
{
    public UnityEvent<GameObject> OnTriggered;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        OnTriggered.Invoke(other.gameObject);
    }
}
