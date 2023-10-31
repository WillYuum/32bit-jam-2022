using SpawnManagerMod;
using UnityEngine;

public class TurretMoveLogic
{
    private RotationDirection _currentRotationDirection;
    public TurretPlatfromTracker TurretPlatfromTracker { get; private set; }

    private ITurretActions _turretActions;

    private InputConnector.InputActionsHandler _inputActionsHandler;

    public TurretMoveLogic(Turret turret, ITurretActions turretActions)
    {
        _currentRotationDirection = RotationDirection.ClockWise;
        TurretPlatfromTracker = new TurretPlatfromTracker(turret);

        _turretActions = turretActions;

        _inputActionsHandler = InputConnector.Create()
    .MapInput(KeyCode.A, () => SetMoveDirection(TurretMoveDirection.ClockWise), KeyEventType.OnKeyDown)
    .MapInput(KeyCode.D, () => SetMoveDirection(TurretMoveDirection.AntiClockWise), KeyEventType.OnKeyDown)
    .MapInput(KeyCode.A, () => SetMoveDirection(TurretMoveDirection.None), KeyEventType.OnKeyUp)
    .MapInput(KeyCode.D, () => SetMoveDirection(TurretMoveDirection.None), KeyEventType.OnKeyUp)
    .MapInput(KeyCode.A, Move, KeyEventType.OnKeyHold)
    .MapInput(KeyCode.D, Move, KeyEventType.OnKeyHold)
    .Build();
    }


    public void Update()
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

public class TurretShootLogic
{
    private bool _shootToggle = false;
    public ShootController ShootController { get; private set; }
    private ITurretActions _turretActions;

    private InputConnector.InputActionsHandler _inputActionsHandler;

    public TurretShootLogic(ITurretActions turretActions)
    {
        ShootController = new ShootController(GameVariables.instance.PlayerShootInterval);

        _turretActions = turretActions;
        if (_turretActions == null) Debug.LogError("Turret actions is null");

        _inputActionsHandler = InputConnector.Create()
        .MapInput(KeyCode.Space, ToggleShooting, KeyEventType.OnKeyDown)
        .MapInput(KeyCode.R, UseExplosionAbility, KeyEventType.OnKeyDown)
        .Build();
    }

    public void Update()
    {
        ShootController.UpdateTimer();

        _inputActionsHandler.CheckForInput();

        if (_shootToggle && ShootController.CanShootProjectile)
        {
            Shoot();
        }
    }


    private void ToggleShooting()
    {
        _shootToggle = !_shootToggle;
        Debug.Log("Switching shoot toggle to: " + _shootToggle);
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

