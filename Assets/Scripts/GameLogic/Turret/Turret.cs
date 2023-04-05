using UnityEngine;
using SpawnManagerMod;
using DG.Tweening;
using System;

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


    public Transform GetShootPoint()
    {
        return _pointOfShot;
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

        GameloopManager.instance.CurrentShootBehavior.Shoot();
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

public class LaserShootBehavior : ShootBehavior
{
    private float _laserShotWidth;
    private float _laserShotHeight;

    public LaserShootBehavior()
    {
        InitActions(new Action[] { LaserShotOne, LaserShotTwo, LaserShotThree });
    }

    public override void OnLevelChange()
    {

        GameObject beam = CurrentLevel switch
        {
            0 => SpawnManager.instance.BeamLevelOnePrefab.GetPrefabConfig(),
            1 => SpawnManager.instance.BeamLevelTwoPrefab.GetPrefabConfig(),
            2 => SpawnManager.instance.BeamLevelThreePrefab.GetPrefabConfig(),
            _ => SpawnManager.instance.BeamLevelOnePrefab.GetPrefabConfig()
        };

        SpriteRenderer beamSpriteRenderer = beam.GetComponentInChildren<SpriteRenderer>();

        _laserShotWidth = beamSpriteRenderer.bounds.extents.x * 2f;
        _laserShotHeight = beamSpriteRenderer.bounds.extents.y * 2f;
    }


    private void LaserShotOne()
    {
        GameObject beam = SpawnManager.instance.BeamLevelOnePrefab.CreateGameObject(ShootPointTransform.position, Quaternion.identity);
        PositionAndRotateBeam(beam);
        CheckIfHitEnemiesWithBeam(beam);
        FadeOutBeam(beam);
    }

    private void LaserShotTwo()
    {
        GameObject beam = SpawnManager.instance.BeamLevelTwoPrefab.CreateGameObject(ShootPointTransform.position, Quaternion.identity);
        PositionAndRotateBeam(beam);
        CheckIfHitEnemiesWithBeam(beam);
        FadeOutBeam(beam);
    }

    private void LaserShotThree()
    {
        GameObject beam = SpawnManager.instance.BeamLevelThreePrefab.CreateGameObject(ShootPointTransform.position, Quaternion.identity);
        PositionAndRotateBeam(beam);
        CheckIfHitEnemiesWithBeam(beam);
        FadeOutBeam(beam);
    }

    private void PositionAndRotateBeam(GameObject beam)
    {
        beam.transform.position = ShootPointTransform.position;
        beam.transform.rotation = ShootPointTransform.rotation;
    }

    private void CheckIfHitEnemiesWithBeam(GameObject beam)
    {
        Vector2 startPoint = ShootPointTransform.position;
        Vector2 endPoint = beam.transform.position + beam.transform.up * _laserShotHeight;
        Vector2 centerOfStartPoint = startPoint + ((endPoint - startPoint) / 2f);
        Vector2 size = new Vector2(_laserShotWidth, _laserShotHeight);

#if UNITY_EDITOR
        DebugDraw.DrawBox(centerOfStartPoint, size, Color.red, beam.transform.rotation, 2f);
#endif

        float angle = Vector2.SignedAngle(Vector2.up, endPoint - startPoint);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(centerOfStartPoint, size, angle, endPoint - startPoint, _laserShotHeight, LayerMask.GetMask("Enemy"));

        if (hits.Length > 0)
        {
            int damageAmount = 1;
            foreach (RaycastHit2D hit in hits)
            {
                hit.collider.GetComponent<IDamageable>().TakeDamage(damageAmount);
            }
        }
    }

    private void FadeOutBeam(GameObject beam)
    {
        beam.GetComponentInChildren<SpriteRenderer>().DOFade(0f, 0.5f).OnComplete(() =>
        {
            GameObject.Destroy(beam);
        });
    }
}


public class PeaShootBehavior : ShootBehavior
{
    public PeaShootBehavior()
    {
        InitActions(new Action[] { SinglePeaShot, DoublePeaShot, TripleBullet });
    }


    private void SinglePeaShot()
    {
        GameObject spawnedBullet = SpawnManager.instance.TurretBulletPrefab.CreateGameObject(ShootPointTransform.position, Quaternion.identity);
        spawnedBullet.GetComponent<Projectile>().SetShootDirection(ShootPointTransform.up);
    }

    private void DoublePeaShot()
    {
        Vector3 direction = ShootPointTransform.up;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

        float bulletDistance = 1.0f;
        int bulletCount = 2;
        float bulletSpacing = 0.2f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Spawn bullet
            Vector3 spawnPosition = ShootPointTransform.position * bulletDistance;
            GameObject bullet = SpawnManager.instance.TurretBulletPrefab.CreateGameObject(spawnPosition, Quaternion.identity);

            // Set bullet direction
            float directionModifier = (i == 0) ? 1f : ((i == bulletCount - 1) ? -1f : 0f);
            Vector3 bulletDirection = direction + ShootPointTransform.right * bulletSpacing * directionModifier;
            bullet.GetComponent<Projectile>().SetShootDirection(bulletDirection);
        }
    }

    private void TripleBullet()
    {
        Vector3 direction = ShootPointTransform.up;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

        float bulletDistance = 1.0f;
        int bulletCount = 3;
        float bulletSpacing = 0.2f;


        for (int i = 0; i < bulletCount; i++)
        {
            // Spawn bullet
            Vector3 spawnPosition = ShootPointTransform.position * bulletDistance;
            GameObject bullet = SpawnManager.instance.TurretBulletPrefab.CreateGameObject(spawnPosition, Quaternion.identity);

            // Set bullet direction
            float directionModifier = (i == 0) ? 1f : ((i == bulletCount - 1) ? -1f : 0f);
            Vector3 bulletDirection = direction + ShootPointTransform.right * bulletSpacing * directionModifier;
            bullet.GetComponent<Projectile>().SetShootDirection(bulletDirection);
        }
    }
}


public abstract class ShootBehavior
{
    public int CurrentLevel { get; private set; }
    private int _maxLevel;
    private Action[] shootActionsPerLevel;
    protected Transform ShootPointTransform;


    protected void InitActions(Action[] shootActionsPerLevel)
    {
        _maxLevel = shootActionsPerLevel.Length - 1;
        this.shootActionsPerLevel = shootActionsPerLevel;
        CurrentLevel = 0;
        OnLevelChange();
    }

    public void SetTurretTransform(Transform shootPntTransform)
    {
        ShootPointTransform = shootPntTransform;
    }

    public void Shoot()
    {
        shootActionsPerLevel[CurrentLevel].Invoke();
    }

    public virtual void OnLevelChange()
    {

    }

    public void Upgrade()
    {
        if (CurrentLevel >= _maxLevel) return;


        CurrentLevel++;
        OnLevelChange();

        Debug.Log("Upgrade " + CurrentLevel + " to " + (CurrentLevel));

    }

    public void Downgrade()
    {
        if (CurrentLevel <= 0) return;

        CurrentLevel--;
        OnLevelChange();
    }
}