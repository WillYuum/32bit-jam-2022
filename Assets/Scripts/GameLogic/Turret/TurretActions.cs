using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotationDirection
{
    ClockWise,
    AntiClockWise,
}

public enum TurretMoveDirection
{
    None,
    ClockWise,
    AntiClockWise,
}

public class TurretActions : MonoBehaviour
{
    private ITurretActions _turretActions;
    private ShootController _turretShootController;

    private void Awake()
    {
        _turretActions = gameObject.GetComponent<ITurretActions>();

        float turretShootInterval = 0.5f;
        _turretShootController = new ShootController(turretShootInterval);

        GameloopManager.instance.OnGameLoopStart += () =>
        {
            Move(RotationDirection.ClockWise);

        };
    }


    private void Update()
    {
        _turretShootController.UpdateShootTimer();

        if (Input.GetKey(KeyCode.A))
        {
            Move(RotationDirection.ClockWise);
            _turretActions.SetTurretMoveDirection(TurretMoveDirection.ClockWise);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(RotationDirection.AntiClockWise);
            _turretActions.SetTurretMoveDirection(TurretMoveDirection.AntiClockWise);
        }
        else
        {
            _turretActions.SetTurretMoveDirection(TurretMoveDirection.None);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (_turretShootController.CanShoot)
        {
            _turretActions.HandleShoot();
            _turretShootController.ResetShootTimer();
        }
    }

    public void Move(RotationDirection movement)
    {
        Vector2 newPos = GameloopManager.instance.TurretPlatfromTracker.MoveIndicator(movement);
        _turretActions.UpdatePosition(newPos);
    }
}


public class ShootController
{
    public float ShootInterval { get; private set; }
    public float ShootTimer { get; private set; }
    public bool CanShoot { get; private set; }

    public ShootController(float shootInterval)
    {
        ShootInterval = shootInterval;
        ShootTimer = 0;
        CanShoot = true;
    }


    public void UpdateShootTimer()
    {
        if (ShootTimer < ShootInterval)
        {
            ShootTimer += Time.deltaTime;
        }
        else
        {
            CanShoot = true;
        }
    }

    public void ResetShootTimer()
    {
        ShootTimer = 0.0f;
        CanShoot = false;
    }
}


