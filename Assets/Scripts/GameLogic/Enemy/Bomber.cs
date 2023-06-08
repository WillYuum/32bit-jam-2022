using UnityEngine;
using DG.Tweening;

public class Bomber : EnemyCore<Bomber>
{

    private float _moveSpeed = 1.75f;
    [SerializeField] private Transform _bombRangeIndicator;
    private float _bombRange = 5.0f;
    private float _delayToAttack = 1.15f;

    [SerializeField] private GameObject _visuals;

    private float _originalBombRangeScale;

    protected override void OnAwake()
    {
        _bombRange = GameVariables.instance.BomberExplodeRange;

        _bombRangeIndicator.gameObject.SetActive(false);

        _originalBombRangeScale = _bombRangeIndicator.localScale.x;
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

    public void HandleAttackingTarget(Vector3 target)
    {
        if (IsGoingToExplode()) return;


        Vector2 direction = target - transform.position;
        transform.Translate(direction.normalized * _moveSpeed * Time.deltaTime, Space.World);

        if (Vector2.Distance(transform.position, target) < _bombRange * 0.85f)
        {
            InvokeExplode();
        }
    }


    //Temp solution, should have another way to check if the enemy is going to explode
    private bool IsGoingToExplode()
    {
        return _bombRangeIndicator.gameObject.activeSelf;
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
}
