using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BigBoomBehavior : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _bigBoomVisual;

    public void Explode()
    {
        ExplosionBarTracker explosionBarTracker = GameloopManager.instance.ExplosionBarTracker;

        explosionBarTracker.ResetExplosionBar();

        List<IDamageable> damageables = new List<IDamageable>();

        RaycastHit2D[] enemies = Physics2D.CircleCastAll(transform.position, 25, Vector2.zero, 0, LayerMask.GetMask("Enemy"));

        foreach (RaycastHit2D enemy in enemies)
        {
            IDamageable damageable = enemy.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageables.Add(damageable);
            }
        }

        float currentRadius = 0;
        float speedIncreaseRadius = 0.5f;

        int damageAmount = 999;
        for (int i = 0; i < damageables.Count; i++)
        {
            IDamageable damageable = damageables[i];

            damageable.TakeDamage(new TakeDamageData
            {
                DamageAmount = damageAmount,
                TakeDamageType = EnemyTakeDamageData.Explosion,
            });
        }



        transform.localScale += Vector3.one * currentRadius;

        BehavioralData behavioralData = new BehavioralData();

        void ScaleUpExplosion()
        {
            currentRadius += speedIncreaseRadius * Time.deltaTime;
            transform.localScale += Vector3.one * currentRadius;


            bool bigBoomEnded = transform.localScale.x > 10;
            if (bigBoomEnded)
            {
                BehavioralController.instance.RemoveBehavioral(behavioralData, true);
            }
        }



        void FadeOutExplosion()
        {
            _bigBoomVisual.DOColor(Color.clear, 0.5f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }


        behavioralData.UpdateBehavior = ScaleUpExplosion;
        behavioralData.OnBehaviorEnd = FadeOutExplosion;

        BehavioralController.instance.AddBehavioral(behavioralData);
    }
}
