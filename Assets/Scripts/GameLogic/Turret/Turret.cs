using UnityEngine;
using SpawnManagerMod;
using DG.Tweening;

public class Turret : MonoBehaviour, ITurretActions, IDamageable
{
    public TurretMoveDirection MoveDirection { get; private set; }

    [SerializeField] private Animator _animator;

    [SerializeField] private Transform _pointOfShot;

    [SerializeField] private SpriteRenderer _spriteRenderer;



    public void SetTurretMoveDirection(TurretMoveDirection direction)
    {
        MoveDirection = direction;
    }



    public void UpdatePosition(Vector2 position)
    {
        transform.position = position;
    }


    //NOTE: Anim will have an event that will call the shoot bullet function
    public void PlayAnim()
    {
        _animator.Play("Shoot");
        // switch (GameloopManager.instance.CurrentTypeShot)
        // {
        //     case TypeOfShots.SingleShot:
        //         break;
        // }
    }


    public void shoot()
    {
        AudioManager.instance.PlaySFX("playerFire");


        switch (GameloopManager.instance.CurrentTypeShot)
        {
            case TypeOfShots.SingleShot:
                GameObject spawnedBullet = SpawnManager.instance.TurretBulletPrefab.CreateGameObject(_pointOfShot.position, Quaternion.identity);
                spawnedBullet.GetComponent<Projectile>().SetShootDirection(transform.up);
                break;

            case TypeOfShots.TripleShot:
                Vector3 direction = _pointOfShot.up;
                Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

                float bulletDistance = 1.0f;
                int bulletCount = 3;
                float bulletSpacing = 0.2f;


                for (int i = 0; i < bulletCount; i++)
                {
                    // Spawn bullet
                    Vector3 spawnPosition = _pointOfShot.position * bulletDistance;
                    GameObject bullet = SpawnManager.instance.TurretBulletPrefab.CreateGameObject(spawnPosition, Quaternion.identity);

                    // Set bullet direction
                    float directionModifier = (i == 0) ? 1f : ((i == bulletCount - 1) ? -1f : 0f);
                    Vector3 bulletDirection = direction + transform.right * bulletSpacing * directionModifier;
                    bullet.GetComponent<Projectile>().SetShootDirection(bulletDirection);
                }
                break;
        }
    }

    public void TakeDamage(int damageCount = 1)
    {
        if (GameloopManager.instance.LoopIsActive == false) return;

        var invinsibility = GameloopManager.instance.TurretInvisiblityWindowTracker;
        if (invinsibility.IsInvisiblityWindowActive) return;

        invinsibility.TakeHit();
        AudioManager.instance.PlaySFX("playerHurt");

        var fadeColor = Color.white;
        fadeColor.a = 0.5f;

        int loop = 6;
        float durationOfInvinsibility = invinsibility.InvisiblityWindowDuration;

        //Tween fade spirete color like ping yoyo
        _spriteRenderer.DOColor(fadeColor, durationOfInvinsibility / (float)loop).SetLoops(loop, LoopType.Yoyo).OnComplete(() =>
        {
            _spriteRenderer.color = Color.white;
            invinsibility.SetIsVunrable();
        });

        GameloopManager.instance.InvokeFishTakeDamage(damageCount);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
        //Check if hit by enemy bullet

    }
}

public class HitPoint
{
    public int CurrenthitPoint { get; private set; }
    public HitPoint(int maxHitPoint)
    {
        CurrenthitPoint = maxHitPoint;
    }

    public void TakeDamage(int damage)
    {
        CurrenthitPoint -= damage;
    }

    public void Heal(int heal)
    {
        CurrenthitPoint += heal;
    }

    public bool IsOutOfHP()
    {
        return CurrenthitPoint <= 0;
    }
}


public interface ITurretActions
{
    void PlayAnim();
    void SetTurretMoveDirection(TurretMoveDirection direction);
    void UpdatePosition(Vector2 position);
    void shoot();

}
