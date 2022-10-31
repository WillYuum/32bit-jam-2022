using UnityEngine;
using DG.Tweening;

public class Bomber : EnemyCore<Bomber>
{

    private Transform _target;
    private float _moveSpeed = 1.75f;
    [SerializeField] private Transform _bombRangeIndicator;
    private float _bombRange = 5.0f;

    private float _originalBombRangeScale;

    protected override void OnAwake()
    {
        base.OnAwake();

        _bombRange = GameVariables.instance.BomberExplodeRange;

        _bombRangeIndicator.gameObject.SetActive(false);
        _target = GameObject.Find("Turret").transform;
        SetState(new MoveToTurretState());

        _originalBombRangeScale = _bombRangeIndicator.localScale.x;
    }

    public void MoveToTurret()
    {
        Vector2 direction = _target.position - transform.position;
        transform.Translate(direction.normalized * _moveSpeed * Time.deltaTime, Space.World);

        if (Vector2.Distance(transform.position, _target.position) < _bombRange * 0.85f)
        {
            SetState(new EnterExplodeState());
        }

    }

    public void InvokeExplode()
    {
        _bombRangeIndicator.localScale = Vector3.one * _originalBombRangeScale;

        _bombRangeIndicator.gameObject.SetActive(true);
        _bombRangeIndicator.DOScale(_originalBombRangeScale * _bombRange, 0.5f)
        .SetEase(Ease.InOutCirc);

        float delayTillExplode = 1.35f;
        Invoke(nameof(Explode), delayTillExplode);
    }

    private void Explode()
    {
        var hit = Physics2D.CircleCast(transform.position, _bombRange, Vector2.zero, 0.0f, LayerMask.GetMask("Player"));

        AudioManager.instance.PlaySFX("bomber");

        if (hit.collider != null)
        {
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
