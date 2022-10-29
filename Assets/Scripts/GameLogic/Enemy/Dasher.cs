using UnityEngine;
using DG.Tweening;

public class Dasher : EnemyCore<Dasher>
{
    private float _rotationSpeed = 5.0f;
    private float _moveSpeed = 5.0f;
    [HideInInspector] public Transform _target;
    private float _angleToAttackTarget = 0.4f;
    private float _delayToAttack = 2.0f;


    [SerializeField] private GameObject _visual;

    protected override void OnAwake()
    {
        base.OnAwake();

        _target = GameObject.Find("Turret").transform;
        if (_target == null) Debug.LogError("Turret not found");
        SetState(new DasherRotateTowardsTurret());
    }

    public void RotateTowardsTargetAndAttack()
    {
        Vector3 direction = _target.position - transform.position;
        transform.up = direction * _rotationSpeed * Time.deltaTime;
        float angleBetween = Vector3.Dot(transform.up, direction.normalized);
        if (angleBetween > _angleToAttackTarget)
        {
            SetState(new DasherReadyToAttackState());
        }
    }

    public void ScaleAndAttack()
    {
        transform.DOScale(1.8f, 1.65f)
        .SetEase(Ease.InOutExpo)
        .OnComplete(() =>
        {
            transform.transform.localScale = Vector3.one;
            SetState(new DasherAttackTurretState());
        });
    }

    public void KeepLookingAtTurret()
    {
        Vector3 direction = _target.position - transform.position;
        transform.up = direction * _rotationSpeed * Time.deltaTime;
    }



    public void MoveTowardsPosition(Vector2 finalPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, finalPosition, _moveSpeed * Time.deltaTime);
        _moveSpeed += 0.05f;


        if (transform.position == (Vector3)finalPosition)
        {
            Explode();
        }
    }


    public void Explode()
    {
        Destroy(gameObject);
    }


    public class DasherReadyToAttackState : EnemyStateCore<Dasher>
    {
        public override void EnterState(Dasher enemy)
        {
            base.EnterState(enemy);
            _owner.ScaleAndAttack();
        }

        public override void Act()
        {
            base.Act();
            _owner.KeepLookingAtTurret();
        }
    }

    public class DasherRotateTowardsTurret : EnemyStateCore<Dasher>
    {
        public override void EnterState(Dasher enemy)
        {
            base.EnterState(enemy);
        }

        public override void Act()
        {
            base.Act();
            _owner.RotateTowardsTargetAndAttack();
        }
    }


    public class DasherAttackTurretState : EnemyStateCore<Dasher>
    {
        private Vector2 _finalPosition;
        public override void EnterState(Dasher enemy)
        {
            base.EnterState(enemy);
            _finalPosition = _owner._target.position;
        }

        public override void Act()
        {
            base.Act();
            _owner.MoveTowardsPosition(_finalPosition);
        }
    }

}

