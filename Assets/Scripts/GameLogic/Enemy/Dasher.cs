using UnityEngine;
using DG.Tweening;
using System;

public class Dasher : EnemyCore<Dasher>
{
    private float _rotationSpeed = 5.0f;
    private float _moveSpeed = 5.0f;
    [HideInInspector] public Transform _target;
    private float _angleToAttackTarget = 0.4f;
    private float _delayToAttack = 1.45f;

    private Vector3 _dashToPosition;

    private float _explodeRange = 1.5f;


    [SerializeField] private GameObject _visual;


    void Update()
    {
        if (_dashToPosition != Vector3.zero)
        {
            RotateTowardsTarget(_dashToPosition);
            MoveTowardsTarget(_dashToPosition);
        }
    }

    public void SetDashTarget(Vector2 dashPosition)
    {
        _dashToPosition = dashPosition;
    }

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
            if (gameObject != null)
            {
                transform.transform.localScale = Vector3.one;
            }

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
            if (gameObject != null)
            {
                transform.transform.localScale = Vector3.one;
            }

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
        transform.position = Vector2.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);
        _moveSpeed += 0.05f;

        bool isNearFish = Vector2.Distance(transform.position, target) < Mathf.Epsilon;
        if (isNearFish)
        {
            Explode();
        }
    }


    public void Explode()
    {
        var hit = Physics2D.CircleCast(transform.position, _explodeRange, Vector2.zero, 1.0f, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            int damageAmount = 1;
            hit.collider.GetComponent<Turret>().TakeDamage(damageAmount);
        }

        Destroy(gameObject);
    }
}

