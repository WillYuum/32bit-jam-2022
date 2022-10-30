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
    [SerializeField] private TurretEvents _turretAnimationEvents;


    private void Start()
    {
        _turretAnimationEvents.OnTurretShoot += ShootBullet;
    }

    private void Awake()
    {
        _turretActions = gameObject.GetComponent<ITurretActions>();

        float turretShootInterval = 2.5f;
        _turretShootController = new ShootController(turretShootInterval);


        //This is to set the fish on the plaform as soon as the game starts
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
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                _turretActions.SetTurretMoveDirection(TurretMoveDirection.None);
            }
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
            _turretShootController.SetPaused();
            _turretActions.PlayAnim();
        }
    }

    private void ShootBullet()
    {
        _turretShootController.ResetShootTimer();
        _turretActions.ShootBullet();
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
    private bool _paused = false;

    public ShootController(float shootInterval)
    {
        ShootInterval = shootInterval;
        ShootTimer = 0;
        CanShoot = false;
    }


    public void UpdateShootTimer()
    {
        if (_paused) return;

        if (ShootTimer < ShootInterval)
        {
            ShootTimer += Time.deltaTime;
        }
        else
        {
            CanShoot = true;
        }
    }

    public void SetPaused()
    {
        CanShoot = false;
        _paused = true;
    }

    public void ResetShootTimer()
    {
        _paused = false;
        ShootTimer = 0.0f;
        CanShoot = false;
    }
}


