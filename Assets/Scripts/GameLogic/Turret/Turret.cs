using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, ITurretActions
{
    public HitPoint _hitPoints;
    public TurretMoveDirection MoveDirection { get; private set; }

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
        throw new System.NotImplementedException();
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
