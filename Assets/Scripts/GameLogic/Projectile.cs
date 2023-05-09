using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private int _damage = 1;

    private Vector3 _moveDirectionWithSpeed;

    private void Awake()
    {
        _moveDirectionWithSpeed = Vector3.zero;
    }

    private void Update()
    {
        if (_moveDirectionWithSpeed != Vector3.zero)
        {
            transform.Translate(_moveDirectionWithSpeed * Time.deltaTime);
        }
    }

    public void SetShootDirection(Vector3 direction)
    {
        _moveDirectionWithSpeed = direction.normalized * _speed;
        Invoke(nameof(DisposeButlletForLongLife), 5.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.activeInHierarchy == false) return;

        // print("Projectile hit " + other.name + "by " + this.name);
        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }


    private void DisposeButlletForLongLife()
    {
        if (gameObject.activeInHierarchy == false) return;

        Destroy(gameObject);
    }
}
