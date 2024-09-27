using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IUnit
{
    public GameObject gameObject {  get; }
    public Transform transform { get; }

    public event UnityAction OnDie;
    public float Hp { get; }
}
