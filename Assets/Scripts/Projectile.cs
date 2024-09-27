using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public event UnityAction<Collider> OnTriggerEnter;

}
