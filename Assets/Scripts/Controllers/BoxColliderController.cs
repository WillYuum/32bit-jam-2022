using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderController : MonoBehaviour
{
    public event Action<Collider2D> _OnTriggerEnter;
    public event Action<Collider2D> _OnTriggerExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _OnTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _OnTriggerExit?.Invoke(other);
    }
}
