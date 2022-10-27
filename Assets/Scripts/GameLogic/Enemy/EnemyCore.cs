using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    BombBarrel,
    Dasher,
    Elite,
}

public class EnemyCore : MonoBehaviour
{
    [field: SerializeField] public EnemyType EnemyType { get; private set; }

    [SerializeField] private float _startingHealth = 100.0f;
    [HideInInspector] public float CurrentHealth { get; private set; }


    private void Awake()
    {
        CurrentHealth = _startingHealth;
    }



    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        Destroy(gameObject);
    }
}
