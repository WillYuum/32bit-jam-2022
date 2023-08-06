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
    void TakeDamage(TakeDamageData damage);
}

public struct TakeDamageData
{
    public int DamageAmount;
    public EnemyTakeDamageData TakeDamageType;
}

public enum EnemyTakeDamageData
{
    Explosion,
    BulletFromPlayer,
}

public class EnemyCore<T> : MonoBehaviour, IDamageable
where T : MonoBehaviour
{
    [field: SerializeField] public EnemyType EnemyType { get; private set; }
    [SerializeField] private SimpleFlash _simpleFlash;
    public bool Stunned { get; private set; }

    protected HitPoint _hitPoint;

    private void Awake()
    {
        Stunned = false;
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

    public void TakeDamage(TakeDamageData damageData)
    {
        _hitPoint.TakeDamage(damageData.DamageAmount);
        bool isDead = _hitPoint.IsOutOfHP();

        if (isDead)
        {
            GameloopManager.instance.InvokeKillEnemy(EnemyType);
            Die();
        }
        else
        {
            if (damageData.TakeDamageType == EnemyTakeDamageData.BulletFromPlayer)
            {
                InvokeStunBehaviorOnEnemy();
            }
            else
            {
                _simpleFlash.Flash();
            }
        }


        AudioManager.instance.PlaySFX("enemyHurt");
    }

    protected virtual void InvokeStunBehaviorOnEnemy()
    {
        if (Stunned == false)
        {
            float stunDuration = 3.0f;
            _simpleFlash.FlashForSeconds(stunDuration);
            Stunned = true;
            InvokeStunBehaviorOnEnemy();
            Invoke(nameof(ResetFromStun), stunDuration);
        }
    }

    private void ResetFromStun(float stunDuration)
    {
        Stunned = false;
    }

    protected void Die()
    {
        DOTween.Complete(transform);
        Destroy(gameObject);
    }
}