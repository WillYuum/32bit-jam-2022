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
            Invoke(nameof(StartGameLoop), 1.5f);
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
    }


    public void StartGameLoop()
    {
        enabled = true;
        LoopIsActive = true;

        CollectedHightScore = 0;

        GameUI.instance.SwitchToScreen(GameUI.Screens.GameUI);

        int startingHP = 5;
        _fishHitPoints = new HitPoint(startingHP);

        _currentRoom = GameObject.Find("Room_1").GetComponent<Room>();
        _currentRoom.UpdatePlatformNeighbors();

        Turret turret = FindObjectOfType<Turret>();
        TurretPlatfromTracker = new TurretPlatfromTracker(turret, _currentRoom.GetFirstPlaform());

        Vector3 newCameraPos = _currentRoom.CameraPoint.position;
        newCameraPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = newCameraPos;


        if (OnGameLoopStart != null)
        {
            OnGameLoopStart.Invoke();
        }

        for (int i = 0; i < 7; i++)
        {
            Invoke(nameof(SpawnEnemyRandomly), 0.65f * i);
        }
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
                CollectedHightScore += 1;
                break;
            case EnemyType.Dasher:
                CollectedHightScore += 2;
                break;
            case EnemyType.Bomber:
                CollectedHightScore += 3;
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
        SpawnManager.instance.BerserkerPrefab.CreateGameObject(spawnPoint, Quaternion.identity);
    }

}
