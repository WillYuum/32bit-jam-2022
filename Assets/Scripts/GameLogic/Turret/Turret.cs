using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpawnManagerMod;

public enum TypeOfShots
{
    SingleShot,
}

public class Turret : MonoBehaviour, ITurretActions
{
    public HitPoint _hitPoints;
    public TurretMoveDirection MoveDirection { get; private set; }
    private TypeOfShots _currentTypeShot;

    [SerializeField] private Transform _pointOfShot;

    public void SetTurretMoveDirection(TurretMoveDirection direction)
    {
        MoveDirection = direction;
    }

    public void UpdatePosition(Vector2 position)
    {
        transform.position = position;
    }

    public void HandleShoot()
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
        Debug.Log("SingleShot");

        SpawnManager.instance.SpawnBullet(_pointOfShot.position, _pointOfShot.rotation);


    }

    public void TakeDamage(int amount)
    {
        _hitPoints.TakeDamage(amount);

        if (_hitPoints.IsOutOfHP())
        {
            Destroy(gameObject);
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if hit by enemy bullet

    }
}

public class HitPoint
{
    public int hitPoint { get; private set; }
    public int maxHitPoint { get; private set; }
    public HitPoint(int hitPoint, int maxHitPoint)
    {
        this.hitPoint = hitPoint;
        this.maxHitPoint = maxHitPoint;
    }

    public void TakeDamage(int damage)
    {
        hitPoint -= damage;
    }

    public void Heal(int heal)
    {
        hitPoint += heal;
    }

    public bool IsOutOfHP()
    {
        return hitPoint <= 0;
    }
}


public interface ITurretActions
{
    void HandleShoot();
    void SetTurretMoveDirection(TurretMoveDirection direction);
    void UpdatePosition(Vector2 position);
}
