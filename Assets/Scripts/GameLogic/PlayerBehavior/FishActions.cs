using System;
using SpawnManagerMod;
using UnityEngine;

public class FishMoveLogic
{
    private RotationDirection _currentRotationDirection;
    public TurretPlatfromTracker TurretPlatfromTracker { get; private set; }

    private ITurretActions _turretActions;


    private Action UpdateAction;

    public FishMoveLogic(Turret turret, ITurretActions turretActions)
    {
        _currentRotationDirection = RotationDirection.ClockWise;
        TurretPlatfromTracker = new TurretPlatfromTracker(turret);

        _turretActions = turretActions;

        bool isMobile = Application.isMobilePlatform;

        if (isMobile)
        {
            Vector2 leftSideScreen = new Vector2(Screen.width / 2, 0);
            UpdateAction = () =>
            {
                //if click left side switch to anti clockwise
                //if click right side switch to clockwise
                if (Input.GetTouch(0).position.x > leftSideScreen.x)
                {
                    SetMoveDirection(TurretMoveDirection.AntiClockWise);
                }
                else
                {
                    SetMoveDirection(TurretMoveDirection.ClockWise);
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    _turretActions.SetTurretMoveDirection(TurretMoveDirection.None);
                }
            };
        }
        else
        {
            UpdateAction = () =>
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    SetMoveDirection(TurretMoveDirection.ClockWise);


                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    SetMoveDirection(TurretMoveDirection.AntiClockWise);

                }

                if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                {
                    _turretActions.SetTurretMoveDirection(TurretMoveDirection.None);
                }

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    Move();
                }
            };
        }
    }


    public void Update()
    {
        UpdateAction();
    }

    private void SetMoveDirection(TurretMoveDirection turretMoveDirection)
    {
        _currentRotationDirection = turretMoveDirection == TurretMoveDirection.ClockWise ? RotationDirection.ClockWise : RotationDirection.AntiClockWise;
        _turretActions.SetTurretMoveDirection(turretMoveDirection);
    }


    private void Move()
    {
        Transform turretIndicatorTransform = TurretPlatfromTracker.MoveIndicator(_currentRotationDirection);
        _turretActions.UpdateTransformProps(turretIndicatorTransform.position, turretIndicatorTransform.up);
    }
}

public class FishShootLogic
{
    private bool _shootToggle = false;
    public ShootController ShootController { get; private set; }
    private ITurretActions _turretActions;

    private Action UpdateAction;

    public FishShootLogic(ITurretActions turretActions)
    {
        ShootController = new ShootController(GameVariables.instance.PlayerShootInterval);

        _turretActions = turretActions;
        if (_turretActions == null) Debug.LogError("Turret actions is null");

        bool isMobile = Application.isMobilePlatform;

        if (isMobile)
        {
            _shootToggle = true;
            UpdateAction = () =>
            {
                if (ShootController.CanShootProjectile)
                {
                    Shoot();
                }
            };
        }
        else
        {
            UpdateAction = () =>
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _shootToggle = !_shootToggle;
                    Debug.Log("Switching shoot toggle to: " + _shootToggle);
                }

                if (_shootToggle)
                {
                    Shoot();
                }

                if (Input.GetKeyDown(KeyCode.R) && ShootController.CanShootProjectile)
                {
                    UseExplosionAbility();
                }
            };
        }
    }

    public void Update()
    {
        ShootController.UpdateTimer();


        UpdateAction();
    }


    private void Shoot()
    {
        _turretActions.PlayAnim();
        ShootController.SetWaitTillShootAnimation(true);
    }


    private void UseExplosionAbility()
    {
        ExplosionBarTracker explosionBarTracker = GameloopManager.instance.ExplosionBarTracker;

        if (explosionBarTracker.CanUseBigBoom())
        {
            AudioManager.instance.PlaySFX("playerSpecial");

            var explosion = SpawnManager.instance.ExplosionPrefab.CreateGameObject(Vector3.zero, Quaternion.identity);
            explosion.GetComponent<BigBoomBehavior>().Explode();
        }
    }
}


public class ShootController
{
    private float _shootInterval;
    private float _shootTimer;
    public bool CanShootProjectile { get; private set; }
    private bool _waiting = false;

    public ShootController(float shootInterval)
    {
        _shootInterval = shootInterval;
        _shootTimer = 0;
        CanShootProjectile = false;
    }


    public void UpdateTimer()
    {
        if (_waiting) return;

        if (_shootTimer < _shootInterval)
        {
            _shootTimer += Time.deltaTime;
        }
        else
        {
            CanShootProjectile = true;
        }
    }

    public void SetWaitTillShootAnimation(bool wait)
    {
        if (wait)
        {
            CanShootProjectile = false;
            _waiting = true;
        }
        else
        {
            _waiting = false;
            _shootTimer = 0.0f;
            CanShootProjectile = false;
        }
    }
}

