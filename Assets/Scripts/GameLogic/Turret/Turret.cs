using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpawnManagerMod;
using DG.Tweening;

public enum TypeOfShots
{
    SingleShot,
}

public class Turret : MonoBehaviour, ITurretActions, IDamageable
{
    public TurretMoveDirection MoveDirection { get; private set; }
    private TypeOfShots _currentTypeShot;

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

    public void PlayAnim()
    {
        switch (_currentTypeShot)
        {
            case TypeOfShots.SingleShot:
                SingleShot();
                break;
        }
    }

    private void SingleShot()
    {
        _animator.Play("Shoot");
    }

    public void ShootBullet()
    {
        AudioManager.instance.PlaySFX("playerFire");

        var bullet = SpawnManager.instance.TurretBulletPrefab.CreateGameObject(_pointOfShot.position, Quaternion.identity);
        bullet.GetComponent<Projectile>().SetShootDirection(transform.up);
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
    void ShootBullet();

}
