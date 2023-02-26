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
    private EnemyStateCore<T> _currentState;

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


    public virtual Tweener SpawnEnemy()
    {
        //tween scale up then enemy then invoke onSpawnComplete
        transform.localScale = Vector3.zero;
        return transform.DOScale(Vector3.one, 3.5f);
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.Act();
        }
    }

    public void SetState(EnemyStateCore<T> newState)
    {
        if (_currentState != null)
        {
            _currentState.ExitState();
        }

        _currentState = newState;
        _currentState.EnterState(this as T);
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


public class EnemyStateCore<T>
{
    protected T _owner;
    public virtual void EnterState(T enemy)
    {
        _owner = enemy;
    }

    public virtual void ExitState() { }

    public virtual void Act() { }
}

