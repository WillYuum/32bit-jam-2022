using UnityEngine;
using DG.Tweening;

public class Bomber : EnemyCore<Bomber>
{

    private Transform _target;
    private float _moveSpeed = 5.0f;
    [SerializeField] private Transform _bombRangeIndicator;
    private float _bombRange = 2.65f;


    protected override void OnAwake()
    {
        base.OnAwake();
        _bombRangeIndicator.gameObject.SetActive(false);
        _target = GameObject.Find("Turret").transform;
    }

    public void MoveToTurret()
    {
        Vector2 direction = _target.position - transform.position;
        transform.up = direction * _moveSpeed * Time.deltaTime;
    }

    public void InvokeExplode()
    {
        _bombRangeIndicator.gameObject.SetActive(true);
        _bombRangeIndicator.DOScale(_bombRange, 0.5f).SetEase(Ease.InOutExpo);

        float delayTillExplode = 1.35f;
        Invoke(nameof(Explode), delayTillExplode);
    }

    private void Explode()
    {
        var hit = Physics2D.CircleCast(transform.position, _bombRange, Vector2.zero, 0.0f, LayerMask.GetMask("Player"));

        if (hit.collider != null)
        {
            Debug.Break();
            hit.collider.GetComponent<Turret>().TakeDamage(1);
        }

        Destroy(gameObject);
    }




    class MoveToTurretState : EnemyStateCore<Bomber>
    {
        public override void EnterState(Bomber enemy)
        {
            base.EnterState(enemy);
        }

        public override void Act()
        {
            base.Act();
            _owner.MoveToTurret();
        }
    }


    class EnterExplodeState : EnemyStateCore<Bomber>
    {
        public override void EnterState(Bomber enemy)
        {
            base.EnterState(enemy);
            _owner.InvokeExplode();
        }

        public override void Act()
        {
            base.Act();
        }
    }
}
