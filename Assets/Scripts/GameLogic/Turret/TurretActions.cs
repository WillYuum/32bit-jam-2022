using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpawnManagerMod;

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

        float turretShootInterval = GameVariables.instance.PlayerShootInterval;
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            UseExplosionAbility();
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

    private void UseExplosionAbility()
    {
        ExplosionBarTracker explosionBarTracker = GameloopManager.instance.ExplosionBarTracker;
        if (explosionBarTracker.IsExplosionBarFull())
        {
            explosionBarTracker.ResetExplosionBar();

            List<IDamageable> damageables = new List<IDamageable>();

            RaycastHit2D[] enemies = Physics2D.CircleCastAll(transform.position, 25, Vector2.zero, 0, LayerMask.GetMask("Enemy"));

            foreach (RaycastHit2D enemy in enemies)
            {
                IDamageable damageable = enemy.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageables.Add(damageable);
                }
            }

            var explosion = SpawnManager.instance.ExplosionPrefab.CreateGameObject(Vector3.zero, Quaternion.identity);

            float currentRadius = 0;
            float speedIncreaseRadius = 50;

            explosion.transform.localScale += Vector3.one * currentRadius;

            BehavioralData behavioralData = new BehavioralData()
            {
                UpdateBehavior = () =>
                {
                    currentRadius += speedIncreaseRadius * Time.deltaTime;
                    foreach (IDamageable damageable in damageables)
                    {
                        if (damageable != null)
                        {
                            bool enemyInRadius = Vector2.Distance(Vector2.zero, damageable.transform.position) <= currentRadius;

                            if (enemyInRadius)
                            {
                                int damageAmount;
                                if (damageable.transform.TryGetComponent(out EnemyCore<Elite> enemyCore))
                                {
                                    damageAmount = (int)(GameVariables.instance.EnemyHPData.Elite * 0.15f);
                                }
                                else
                                {
                                    damageAmount = 999;
                                }

                                damageable.TakeDamage(damageAmount);
                                damageables.Remove(damageable);
                            }
                        }
                        else
                        {
                            damageables.Remove(damageable);
                        }
                    }
                },
                OnBehaviorEnd = () =>
                {
                }
            };
            // BehavioralController.instance.AddBehavioral()
            // _turretActions.UseExplosionAbility();
        }
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


