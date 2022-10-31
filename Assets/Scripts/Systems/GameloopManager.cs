using UnityEngine;
using Utils.GenericSingletons;
using SpawnManagerMod;
using System;

public class GameloopManager : MonoBehaviourSingleton<GameloopManager>
{
    public TurretPlatfromTracker TurretPlatfromTracker { get; private set; }
    private Room _currentRoom;

    public event Action OnGameLoopStart;
    public event Action<int> OnFishTakeHit;
    public event Action OnKillEnemy;

    public ExplosionBarTracker ExplosionBarTracker { get; private set; }

    private HitPoint _fishHitPoints;
    public int CollectedHightScore { get; private set; }

    public bool LoopIsActive { get; private set; }

    private void Awake()
    {
        LoopIsActive = false;
#if UNITY_EDITOR
        Turret turret = FindObjectOfType<Turret>();
        if (turret != null)
        {
            enabled = false;
            Invoke(nameof(StartGameLoop), 0.1f);
        }
        else
        {
            enabled = false;
        }
#else
            enabled = false;
#endif

    }


    private void Update()
    {
        TurretPlatfromTracker.TrackTurretOnPlatform();


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            ExplosionBarTracker.IncreaseValue(999);
        }
#endif
    }


    public void StartGameLoop()
    {
        enabled = true;
        LoopIsActive = true;

        ExplosionBarTracker = new ExplosionBarTracker(GameVariables.instance.ExplosionBarData.MaxExplosionBarValue);

        CollectedHightScore = 0;

        GameUI.instance.SwitchToScreen(GameUI.Screens.GameUI);

        AudioManager.instance.PlayBGM("Main");

        int startingHP = 5;
        _fishHitPoints = new HitPoint(startingHP);

        _currentRoom = GameObject.Find("Room_1").GetComponent<Room>();
        _currentRoom.UpdatePlatformNeighbors();

        Turret turret = FindObjectOfType<Turret>();
        TurretPlatfromTracker = new TurretPlatfromTracker(turret, _currentRoom.GetFirstPlaform());

        // Vector3 newCameraPos = _currentRoom.CameraPoint.position;
        // newCameraPos.z = Camera.main.transform.position.z;
        // Camera.main.transform.position = newCameraPos;


        if (OnGameLoopStart != null)
        {
            OnGameLoopStart.Invoke();
        }

        Spawner.instance.StartWithScriptedWaves();
    }

    public void InvokeFishTakeDamage(int damageAmount)
    {
        if (LoopIsActive == false) return;

        _fishHitPoints.TakeDamage(damageAmount);
        OnFishTakeHit.Invoke(_fishHitPoints.CurrenthitPoint);

        if (_fishHitPoints.IsOutOfHP())
        {
            InvokeLosegame();
        }
    }


    private void InvokeLosegame()
    {
        if (LoopIsActive == false) return;

        CancelInvoke(nameof(SpawnEnemyRandomly));
        GameUI.instance.SwitchToScreen(GameUI.Screens.LoseScreen);
    }

    public void InvokeKillEnemy(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Elite:
                CollectedHightScore += GameVariables.instance.PointsData.Elite;
                ExplosionBarTracker.IncreaseValue(GameVariables.instance.ExplosionBarData.Elite);
                break;
            case EnemyType.Dasher:
                CollectedHightScore += GameVariables.instance.PointsData.Dasher;
                ExplosionBarTracker.IncreaseValue(GameVariables.instance.ExplosionBarData.Dasher);
                break;
            case EnemyType.Bomber:
                CollectedHightScore += GameVariables.instance.PointsData.Bomber;
                ExplosionBarTracker.IncreaseValue(GameVariables.instance.ExplosionBarData.Bomber);
                break;
        }

        if (OnKillEnemy != null)
        {
            OnKillEnemy.Invoke();
        }
    }

    private void SpawnEnemyRandomly()
    {
        Vector3 spawnPoint = _currentRoom.GetRandomSpawnPositionWithinRoomRange(0.25f);
        SpawnManager.instance.ElitePrefab.CreateGameObject(spawnPoint, Quaternion.identity);
    }

}



public class ExplosionBarTracker
{
    private float _currentExplosionBarValue;
    private float _maxExplosionBarValue;

    public float CurrentExplosionBarValue { get { return _currentExplosionBarValue; } }
    public float MaxExplosionBarValue { get { return _maxExplosionBarValue; } }

    public ExplosionBarTracker(float maxExplosionBarValue)
    {
        _maxExplosionBarValue = maxExplosionBarValue;
        _currentExplosionBarValue = 0;
    }

    public void IncreaseValue(float amount)
    {
        _currentExplosionBarValue += amount;
        if (_currentExplosionBarValue > _maxExplosionBarValue)
        {
            _currentExplosionBarValue = _maxExplosionBarValue;
        }
    }

    public float GetRatio()
    {
        return _currentExplosionBarValue / _maxExplosionBarValue;
    }

    public void ResetExplosionBar()
    {
        _currentExplosionBarValue = 0;
    }

    public bool IsExplosionBarFull()
    {
        return _currentExplosionBarValue >= _maxExplosionBarValue;
    }
}