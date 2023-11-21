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

        bool isMobile = GameManager.instance.PlayedOnMobileBrowser();

        // #if UNITY_EDITOR
        //         isMobile = true;
        // #endif



        if (isMobile)
        {
            InputHandler mobileInputHandler = new();

            Vector2 leftSideScreen = new Vector2(Screen.width / 2, 0);
            UpdateAction = () =>
            {

                if (!mobileInputHandler.Pressed()) return;

                if (mobileInputHandler.ClickedOnLeftSide())
                {
                    SetMoveDirection(TurretMoveDirection.ClockWise);
                }
                else
                {
                    SetMoveDirection(TurretMoveDirection.AntiClockWise);
                }

                if (mobileInputHandler.Holding())
                {
                    Move();
                }

                if (mobileInputHandler.Exited())
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

        bool isMobile = GameManager.instance.PlayedOnMobileBrowser();

        if (isMobile)
        {
            GameScreen gameScreen = GameUI.instance.GetCurrentScreen<GameScreen>();
            gameScreen.EnableTapOnButton(UseExplosionAbility);

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
                }

                if (_shootToggle && ShootController.CanShootProjectile)
                {
                    Shoot();
                }

                if (Input.GetKeyDown(KeyCode.R))
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

    public void CleanUp()
    {
        GameScreen gameScreen = GameUI.instance.GetCurrentScreen<GameScreen>();
        gameScreen.DisableTapOnButton();
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



public class InputHandler
{

    private bool _isHolding = false;

    public bool ClickedOnLeftSide()
    {
#if UNITY_EDITOR
        return ClickedOnLeftSideInEditor();
#else
        return ClickedOnLeftSideOnMobile();
#endif
    }


    public bool Pressed()
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0);
#else
        _isHolding = Input.touchCount > 0;
        return Input.touchCount > 0;
#endif
    }

    public bool Holding()
    {

#if UNITY_EDITOR
        return Input.GetMouseButton(0);
#else
        return CheckIfHoldingOnMobole();
#endif
    }


    private bool CheckIfHoldingOnMobole()
    {
        return _isHolding;
        // return Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved;
    }


    public bool Exited()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
#else
_isHolding = false;
        return Input.GetTouch(0).phase == TouchPhase.Ended;
#endif
    }

    private bool ClickedOnLeftSideInEditor()
    {
        Vector2 mousePosition = Input.mousePosition;
        return PositionIsOnLeftSide(mousePosition);
    }


    private bool ClickedOnLeftSideOnMobile()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;
            return PositionIsOnLeftSide(touchPosition);
        }
        return false;
    }


    private bool PositionIsOnLeftSide(Vector2 position)
    {
        Vector2 leftSideScreen = new Vector2(Screen.width / 2, 0);
        if (position.x > leftSideScreen.x)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}