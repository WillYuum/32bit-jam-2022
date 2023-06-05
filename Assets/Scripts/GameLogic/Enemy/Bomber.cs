using UnityEngine;
using DG.Tweening;

public class Bomber : EnemyCore<Bomber>
{

    private Transform _target;
    private float _moveSpeed = 1.75f;
    [SerializeField] private Transform _bombRangeIndicator;
    private float _bombRange = 5.0f;

    [SerializeField] private GameObject _visuals;

    private float _originalBombRangeScale;

    protected override void OnAwake()
    {
        _bombRange = GameVariables.instance.BomberExplodeRange;

        _bombRangeIndicator.gameObject.SetActive(false);
        _target = GameObject.Find("Turret").transform;

        _originalBombRangeScale = _bombRangeIndicator.localScale.x;
    }


    public void ReadyToGo()
    {
        // SetState(new MoveToTurretState());
    }

    public void MoveToTurret()
    {
        Vector2 direction = _target.position - transform.position;
        transform.Translate(direction.normalized * _moveSpeed * Time.deltaTime, Space.World);

        // if (Vector2.Distance(transform.position, _target.position) < _bombRange * 0.85f)
        // {
        //     SetState(new EnterExplodeState());
        // }

    }

    public void InvokeExplode()
    {
        SpriteRenderer rangeSpriteRenderer = _bombRangeIndicator.GetComponent<SpriteRenderer>();
        rangeSpriteRenderer.color = new Color(1, 1, 1, 0.3f);
        _bombRangeIndicator.localScale = Vector3.one * (_originalBombRangeScale * _bombRange);


        _bombRangeIndicator.gameObject.SetActive(true);
        // _bombRangeIndicator.DOScale(_originalBombRangeScale * _bombRange, 0.5f)
        // .SetEase(Ease.InOutCirc);

        float delayTillExplode = 1.35f;
        Invoke(nameof(Explode), delayTillExplode);
    }

    private void Explode()
    {
        SpriteRenderer rangeSpriteRenderer = _bombRangeIndicator.GetComponent<SpriteRenderer>();
        rangeSpriteRenderer.color = new Color(1, 1, 1, 1);

        float explosionScale = 0.5f;

        _visuals.SetActive(false);

        _bombRangeIndicator.localScale = Vector3.zero;
        _bombRangeIndicator.DOScale(_originalBombRangeScale * _bombRange, explosionScale)
       .SetEase(Ease.InOutCirc).OnComplete(() =>
       {
           rangeSpriteRenderer.DOFade(0, 0.5f);
           Destroy(gameObject);
       });

        var hit = Physics2D.CircleCast(transform.position, _bombRange, Vector2.zero, 0.0f, LayerMask.GetMask("Player"));

        AudioManager.instance.PlaySFX("bomber");

        if (hit.collider != null)
        {
            hit.collider.GetComponent<Turret>().TakeDamage(1);
        }

        gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }




    // class MoveToTurretState : EnemyStateCore<Bomber>
    // {
    //     public override void EnterState(Bomber enemy)
    //     {
    //         base.EnterState(enemy);
    //     }

    //     public override void Act()
    //     {
    //         base.Act();
    //         _owner.MoveToTurret();
    //     }
    // }


    // class EnterExplodeState : EnemyStateCore<Bomber>
    // {
    //     public override void EnterState(Bomber enemy)
    //     {
    //         base.EnterState(enemy);
    //         _owner.InvokeExplode();
    //     }

    //     public override void Act()
    //     {
    //         base.Act();
    //     }
    // }
}
