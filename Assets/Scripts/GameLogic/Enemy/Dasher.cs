using UnityEngine;
using DG.Tweening;

public class Dasher : EnemyCore<Dasher>
{
    private float _rotationSpeed = 5.0f;
    private float _moveSpeed = 5.0f;
    [HideInInspector] public Transform _target;
    private float _angleToAttackTarget = 0.4f;
    private float _delayToAttack = 1.45f;

    private float _explodeRange = 1.5f;


    [SerializeField] private GameObject _visual;

    public void RotateTowardsTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        transform.up = direction * _rotationSpeed * Time.deltaTime;
    }

    public bool RotateTowardsTargetAndAttack(Vector2 target)
    {
        Vector3 direction = (Vector3)target - transform.position;
        transform.up = direction * _rotationSpeed * Time.deltaTime;
        float angleBetween = Vector3.Dot(transform.up, direction.normalized);
        if (angleBetween > _angleToAttackTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DashTowardsTarget(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);
        _moveSpeed += 0.05f;
    }

    public Sequencer.SequenceState ScaleUpAndSpawn()
    {
        var sequenceState = Sequencer.CreateSequenceState();

        base.SetOnSpawnBehavior();

        transform.DOScale(1.0f, _delayToAttack)
        .SetEase(Ease.InOutExpo)
        .OnComplete(() =>
        {
            transform.transform.localScale = Vector3.one;
            sequenceState.FinishSequence();
        });

        return sequenceState;
    }

    public Sequencer.SequenceState ScaleUpAndAttack()
    {
        var sequenceState = Sequencer.CreateSequenceState();

        transform.DOScale(1.5f, _delayToAttack)
        .SetEase(Ease.InOutExpo)
        .OnComplete(() =>
        {
            transform.transform.localScale = Vector3.one;
            sequenceState.FinishSequence();
        });

        return sequenceState;
    }

    public void KeepLookingAtTurret()
    {
        Vector3 direction = _target.position - transform.position;
        transform.up = direction * _rotationSpeed * Time.deltaTime;
    }



    public void MoveTowardsTarget(Vector2 target)
    {
        print("Moving towards target");
        transform.position = Vector2.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);
        _moveSpeed += 0.05f;


        if (transform.position == (Vector3)target)
        {
            Explode();
        }
    }


    public void Explode()
    {
        var hit = Physics2D.CircleCast(transform.position, _explodeRange, Vector2.zero, 1.0f, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            hit.collider.GetComponent<Turret>().TakeDamage(1);
        }

        Destroy(gameObject);
    }
}

