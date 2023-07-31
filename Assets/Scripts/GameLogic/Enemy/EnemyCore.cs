using System;
using UnityEngine;
using DG.Tweening;

public enum EnemyType
{
    Bomber,
    Dasher,
    Elite,
}

interface IDamageable
{
    Transform transform { get; }
    void TakeDamage(int damage);
}

public class EnemyCore<T> : MonoBehaviour, IDamageable
where T : MonoBehaviour
{
    [field: SerializeField] public EnemyType EnemyType { get; private set; }
    [SerializeField] private SimpleFlash _simpleFlash;

    protected HitPoint _hitPoint;

    private void Awake()
    {
        switch (EnemyType)
        {
            case EnemyType.Elite:
                _hitPoint = new HitPoint(GameVariables.instance.EnemyHPData.Elite);
                break;
            case EnemyType.Dasher:
                _hitPoint = new HitPoint(GameVariables.instance.EnemyHPData.Dasher);
                break;
            case EnemyType.Bomber:
                _hitPoint = new HitPoint(GameVariables.instance.EnemyHPData.Bomber);
                break;
        }

        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }




    protected void SetOnSpawnBehavior()
    {
        transform.localScale = Vector3.zero;
    }

    public void TakeDamage(int damage)
    {
        AudioManager.instance.PlaySFX("enemyHurt");

        print("damage" + damage);

        _simpleFlash.Flash();

        _hitPoint.TakeDamage(damage);
        if (_hitPoint.IsOutOfHP())
        {
            GameloopManager.instance.InvokeKillEnemy(EnemyType);
            Die();
        }
    }

    protected void Die()
    {
        DOTween.Complete(transform);
        Destroy(gameObject);
    }
}