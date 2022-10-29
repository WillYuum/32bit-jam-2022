using UnityEngine;

public enum EnemyType
{
    Bomber,
    Dasher,
    Elite,
}

interface Damageable
{
    void TakeDamage(float damage);
}

public class EnemyCore<T> : MonoBehaviour, Damageable
where T : MonoBehaviour
{
    [field: SerializeField] public EnemyType EnemyType { get; private set; }

    [SerializeField] private float _startingHealth = 100.0f;
    [HideInInspector] public float CurrentHealth { get; private set; }

    private EnemyStateCore<T> _currentState;

    private void Awake()
    {
        CurrentHealth = _startingHealth;
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

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
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

