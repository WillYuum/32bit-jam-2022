using UnityEngine;

public enum EnemyType
{
    Bomber,
    Dasher,
    Elite,
}

interface Damageable
{
    void TakeDamage(int damage);
}

public class EnemyCore<T> : MonoBehaviour, Damageable
where T : MonoBehaviour
{
    [field: SerializeField] public EnemyType EnemyType { get; private set; }
    [SerializeField] private int _startingHP = 3;

    protected HitPoint _hitPoint;
    private EnemyStateCore<T> _currentState;

    private void Awake()
    {
        _hitPoint = new HitPoint(_startingHP);
        OnAwake();
    }

    protected virtual void OnAwake()
    {

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
        _hitPoint.TakeDamage(damage);
        if (_hitPoint.IsOutOfHP())
        {
            GameloopManager.instance.InvokeKillEnemy(EnemyType);
            Die();
        }
    }

    protected void Die()
    {
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

