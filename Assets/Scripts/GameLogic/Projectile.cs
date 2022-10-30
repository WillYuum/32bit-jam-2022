using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private int _damage = 1;

    private Vector3 _shootDirection;

    private void Awake()
    {
        _shootDirection = Vector3.zero;
    }

    private void Update()
    {
        if (_shootDirection != Vector3.zero)
        {
            transform.Translate(_shootDirection * _speed * Time.deltaTime);
        }
    }

    public void SetShootDirection(Vector3 direction)
    {
        _shootDirection = direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Projectile hit " + other.name);
        if (other.gameObject.TryGetComponent(out Damageable damageable))
        {
            damageable.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }
}
