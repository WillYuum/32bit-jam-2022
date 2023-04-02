using UnityEngine;
using Utils.GenericSingletons;
using System;



public enum TypeOfShots
{
    PeaShots,
    Laser,
}


public class GameloopManager : MonoBehaviourSingleton<GameloopManager>
{
    public TurretPlatfromTracker TurretPlatfromTracker { get; private set; }
    public Room CurrentRoom;

    public event Action OnGameLoopStart;
    public event Action<int> OnFishTakeHit;
    public event Action OnKillEnemy;
    public event Action OnRestartGame;
    public event Action<float> OnMomentumChange;


    private int _currentShootLevel = 0;

    public TypeOfShots SelectedShootType { get; private set; }

    public InvisiblityWindowTracker TurretInvisiblityWindowTracker { get; private set; }

    public ExplosionBarTracker ExplosionBarTracker { get; private set; }

    private HitPoint _fishHitPoints;
    public int FishHP { get { return _fishHitPoints.CurrenthitPoint; } }

    public int CollectedHightScore { get; private set; }

    public bool LoopIsActive { get; private set; }

    private void Awake()
    {
        print("AWAKE");

        LoopIsActive = false;
#if UNITY_EDITOR
        Turret turret = FindObjectOfType<Turret>();
        if (turret != null)
        {
            enabled = false;
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
        if (GameloopManager.instance.LoopIsActive == false) return;

        TurretPlatfromTracker.TrackTurretOnPlatform();


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            ExplosionBarTracker.IncreaseValue(999);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            InvokeLosegame();
        }



        if (Input.GetKeyDown(KeyCode.M))
        {
            CurrentShootBehavior.Upgrade();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            CurrentShootBehavior.Downgrade();
        }


        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CurrentShootBehavior.Upgrade();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentShootBehavior.Downgrade();
        }
#endif
    }


    public void SetShootType(TypeOfShots shootType)
    {
        SelectShootType(shootType);
    }

    public void StartGameLoop()
    {
        ExplosionBarTracker = new ExplosionBarTracker(GameVariables.instance.ExplosionBarData.MaxExplosionBarValue);
        TurretInvisiblityWindowTracker = new InvisiblityWindowTracker();

        CollectedHightScore = 0;

        AudioManager.instance.PlayBGM("Main");

        int startingHP = 5;
        _fishHitPoints = new HitPoint(startingHP);

        // CurrentRoom = GameObject.Find("Room_1").GetComponent<Room>();
        CurrentRoom = GameObject.FindObjectOfType<Room>();
        CurrentRoom.UpdatePlatformNeighbors();

        Turret turret = FindObjectOfType<Turret>();
        TurretPlatfromTracker = new TurretPlatfromTracker(turret, CurrentRoom.GetFirstPlaform());

        // Vector3 newCameraPos = _currentRoom.CameraPoint.position;
        // newCameraPos.z = Camera.main.transform.position.z;
        // Camera.main.transform.position = newCameraPos;

        // SelectShootType(TypeOfShots.PeaShots);

        float maxMomentumValue = 5;
        float increaseRatio = maxMomentumValue * 0.15f;
        float decreaseRatio = maxMomentumValue * 0.1f;

        KillMomentunTracker = new KillMomentum(maxMomentumValue, increaseRatio, decreaseRatio);



        enabled = true;
        LoopIsActive = true;
        Spawner.instance.StartSpawner();

        if (OnGameLoopStart != null)
        {
            OnGameLoopStart.Invoke();
        }
        print("GameLoop Started");
    }

    public void InvokeFishTakeDamage(int damageAmount)
    {
        if (LoopIsActive == false) return;

        KillMomentunTracker.DecreaseMomentum();
        GameloopManager.instance.OnMomentumChange.Invoke(KillMomentunTracker.GetMomentumRatio());

        CheckForChangeShootBehavior();

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

        GameloopManager.instance.LoopIsActive = false;

        AudioManager.instance.StopAllBGM();
        AudioManager.instance.PlaySFX("gameOver");

        BehavioralController.instance.Reset();

        var loseScreen = GameUI.instance.LoadLoseScreen();
        loseScreen.OpenScreen(() =>
        {

        });

        enabled = false;
    }


    public void InvokeRestartGame()
    {
        //reset events
        OnGameLoopStart = null;
        OnKillEnemy = null;
        OnRestartGame = null;

        Invoke(nameof(StartGameLoop), 0.1f);
    }

    public void InvokeKillEnemy(EnemyType enemyType)
    {
        KillMomentunTracker.IncreaseMomentum();
        GameloopManager.instance.OnMomentumChange.Invoke(KillMomentunTracker.GetMomentumRatio());

        CheckForChangeShootBehavior();

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


    private void CheckForChangeShootBehavior()
    {
        int currentShootLevel = CurrentShootBehavior.CurrentLevel;
        float momentumRatio = KillMomentunTracker.GetMomentumRatio();
        int levelToUse = GameVariables.instance.TurretPeaShotUnlockData.GetLevelRelativeToMomentum(momentumRatio);


        bool sameLevel = currentShootLevel == levelToUse;
        if (sameLevel) return;


        bool shouldUpgrade = currentShootLevel < levelToUse;
        if (shouldUpgrade)
        {
            CurrentShootBehavior.Upgrade();
        }
        else
        {
            CurrentShootBehavior.Downgrade();
        }
    }

    public KillMomentum KillMomentunTracker { get; private set; }

    public ShootBehavior CurrentShootBehavior { get; private set; }
    private void SelectShootType(TypeOfShots typeOfShots)
    {
        Turret turret = FindObjectOfType<Turret>();

        switch (typeOfShots)
        {
            case TypeOfShots.PeaShots:
                CurrentShootBehavior = new PeaShootBehavior();
                break;
            case TypeOfShots.Laser:
                CurrentShootBehavior = new LaserShootBehavior();
                break;
            default:
#if UNITY_EDITOR
                Debug.LogError("No ShootBehavior found for " + typeOfShots);
#endif
                break;
        }

        CurrentShootBehavior.SetTurretTransform(turret.GetShootPoint());
        SelectedShootType = typeOfShots;
    }

}



public class ExplosionBarTracker
{
    private float _currentExplosionBarValue;
    private float _maxExplosionBarValue;

    public float CurrentExplosionBarValue { get { return _currentExplosionBarValue; } }
    public float MaxExplosionBarValue { get { return _maxExplosionBarValue; } }

    public event Action OnUpdate;

    public ExplosionBarTracker(float maxExplosionBarValue)
    {
        _maxExplosionBarValue = maxExplosionBarValue;
        _currentExplosionBarValue = 0;

        if (OnUpdate != null)
        {
            OnUpdate.Invoke();
        }
    }

    public void IncreaseValue(float amount)
    {
        _currentExplosionBarValue += amount;
        if (_currentExplosionBarValue > _maxExplosionBarValue)
        {
            _currentExplosionBarValue = _maxExplosionBarValue;
        }

        if (OnUpdate != null)
        {
            OnUpdate.Invoke();
        }
    }

    public float GetRatio()
    {
        return _currentExplosionBarValue / _maxExplosionBarValue;
    }

    public void ResetExplosionBar()
    {
        _currentExplosionBarValue = 0;
        OnUpdate.Invoke();
    }

    public bool IsExplosionBarFull()
    {
        return _currentExplosionBarValue >= _maxExplosionBarValue;
    }
}


public class InvisiblityWindowTracker
{
    public bool IsInvisiblityWindowActive { get; private set; }
    public float InvisiblityWindowDuration { get; private set; }

    public InvisiblityWindowTracker()
    {
        InvisiblityWindowDuration = GameVariables.instance.InvinsibilityWindowDuration;
        IsInvisiblityWindowActive = false;
    }

    public void SetIsVunrable()
    {
        IsInvisiblityWindowActive = false;
    }


    public void TakeHit()
    {
        IsInvisiblityWindowActive = true;
    }
}


public class KillMomentum
{
    private float _maxMomentumValue;
    public float CurrentMomentumValue { get; private set; }
    private float _increaseRatio;
    private float _decreaseRatio;
    public KillMomentum(float maxMomentumValue, float increaseRatio, float decreaseRatio)
    {
        _maxMomentumValue = maxMomentumValue;
        CurrentMomentumValue = 0;
        _increaseRatio = increaseRatio;
        _decreaseRatio = decreaseRatio;
    }


    public void IncreaseMomentum()
    {
        CurrentMomentumValue += _increaseRatio;
        if (CurrentMomentumValue > _maxMomentumValue)
        {
            CurrentMomentumValue = _maxMomentumValue;
        }
    }

    public void DecreaseMomentum()
    {
        CurrentMomentumValue -= _decreaseRatio;
        if (CurrentMomentumValue < 0)
        {
            CurrentMomentumValue = 0;
        }
    }

    public float GetMomentumRatio()
    {
        return CurrentMomentumValue / _maxMomentumValue;
    }

    public void ResetMomentum()
    {
        CurrentMomentumValue = 0;
    }
}


public class RatioValue
{
    public float CurrentValue { get; private set; }
    public float MaxValue { get; private set; }

    public RatioValue(float maxValue)
    {
        MaxValue = maxValue;
        CurrentValue = 0;
    }

    public void IncreaseValue(float value)
    {
        CurrentValue += value;
        if (CurrentValue > MaxValue)
        {
            CurrentValue = MaxValue;
        }
    }

    public void DecreaseValue(float value)
    {
        CurrentValue -= value;
        if (CurrentValue < 0)
        {
            CurrentValue = 0;
        }
    }

    public void ResetValue()
    {
        CurrentValue = 0;
    }

    public float GetRatio()
    {
        return CurrentValue / MaxValue;
    }
}