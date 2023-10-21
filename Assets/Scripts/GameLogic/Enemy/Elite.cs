using UnityEngine;
using DG.Tweening;

public class Elite : EnemyCore<Elite>
{

    [field: SerializeField] public ShootController ShootController { get; private set; }
    [HideInInspector] public Transform _target;

    private float _delayToAttack = 1.85f;

    protected override void OnAwake()
    {
        ShootController = new ShootController(1.0f);
        _target = GameObject.Find("Turret").transform;
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
}
