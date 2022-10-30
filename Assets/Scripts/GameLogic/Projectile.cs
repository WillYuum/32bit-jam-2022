using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private int _damage = 1;

    private void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
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
