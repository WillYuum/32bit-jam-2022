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

    public void TakeDamage()
    {
        GameloopManager.instance.InvokeFishTakeDamage();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
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
    void HandleShoot();
    void SetTurretMoveDirection(TurretMoveDirection direction);
    void UpdatePosition(Vector2 position);
}
