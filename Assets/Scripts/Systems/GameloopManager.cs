using UnityEngine;
using Utils.GenericSingletons;
using System;



public enum TypeOfShots
{
    PeaShots,
    Laser,
}

public struct StartGameLoopStruct
{
    public TypeOfShots SelectTypeShot;
}


public class GameloopManager : MonoBehaviourSingleton<GameloopManager>
{
    public TurretPlatfromTracker TurretPlatfromTracker { get; private set; }
    // public Room CurrentRoom;

    public event Action OnGameLoopStart;
    public event Action<int> OnFishTakeHit;
    public event Action OnKillEnemy;
    public event Action OnRestartGame;
    public event Action<float> OnMomentumChange;



    public TypeOfShots SelectedShootType { get; private set; }

    public InvisiblityWindowTracker TurretInvisiblityWindowTracker { get; private set; }

    public ExplosionBarTracker ExplosionBarTracker { get; private set; }

    private HitPoint _fishHitPoints;
    public int FishHP { get { return _fishHitPoints.CurrenthitPoint; } }

    public int CollectedHightScore { get; private set; }
    public bool LoopIsActive { get; private set; }


    public KillMomentum KillMomentunTracker { get; private set; }
    public ShootBehavior CurrentShootBehavior { get; private set; }

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


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            ExplosionBarTracker.IncreaseValue(999);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            InvokeLosegame();
        }


        if (Input.GetKeyDown(KeyCode.U))
        {
            KillMomentunTracker.IncreaseMomentum();
            DetermineShootLevel();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            KillMomentunTracker.DecreaseMomentum();
            DetermineShootLevel();
        }
#endif
    }

    public void StartGameLoop(StartGameLoopStruct startGameLoopStruct)

    {
        ExplosionBarTracker = new ExplosionBarTracker(GameVariables.instance.ExplosionBarData.MaxExplosionBarValue);
        TurretInvisiblityWindowTracker = new InvisiblityWindowTracker();

        CollectedHightScore = 0;

        AudioManager.instance.PlayBGM("Main");

        int startingHP = 5;
        _fishHitPoints = new HitPoint(startingHP);

        Turret turret = FindObjectOfType<Turret>();
        TurretPlatfromTracker = new TurretPlatfromTracker(turret);

        SetShootTypeOnTurret(startGameLoopStruct.SelectTypeShot);

        float increaseRatio = 0.08f;
        float decreaseRatio = 0.02f;
        KillMomentunTracker = new KillMomentum(increaseRatio, decreaseRatio);

        enabled = true;
        LoopIsActive = true;
        Spawner.instance.StartSpawner(GameObject.FindObjectOfType<Room>());

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

        DetermineShootLevel();

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

        DetermineShootLevel();

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


    private void DetermineShootLevel()
    {

        int currentShootLevel = CurrentShootBehavior.CurrentLevel;
        float momentumRatio = KillMomentunTracker.GetMomentumRatio();

        int levelToUse = 0;

        switch (SelectedShootType)
        {
            case TypeOfShots.PeaShots:
                if (momentumRatio < 0.5f)
                {
                    levelToUse = 1;
                }
                else if (momentumRatio >= 0.75f && momentumRatio < 0.9f)
                {
                    levelToUse = 2;
                }
                else
                {
                    levelToUse = 3;
                }
                break;

            case TypeOfShots.Laser:
                if (momentumRatio < 0.5f)
                {
                    levelToUse = 1;
                }
                else
                {
                    levelToUse = 2;
                }
                break;
        }

        bool sameLevel = currentShootLevel == levelToUse;
        if (sameLevel) return;

        CurrentShootBehavior.SetLevel(levelToUse);
    }


    private void SetShootTypeOnTurret(TypeOfShots typeOfShots)
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

        CurrentShootBehavior.SetLevel(1);
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
    private float _maxMomentum;
    public float CurrentMomentumValue { get; private set; }
    private float _increaseValue;
    private float _decreaseValue;
    public KillMomentum(float increaseValRatio, float decreaseValRatio)
    {
        _maxMomentum = 1.0f;

        _increaseValue = increaseValRatio;
        _decreaseValue = decreaseValRatio;

        CurrentMomentumValue = 0;
    }


    public void IncreaseMomentum()
    {
        CurrentMomentumValue += _increaseValue;
        if (CurrentMomentumValue > _maxMomentum)
        {
            CurrentMomentumValue = _maxMomentum;
        }
    }

    public void DecreaseMomentum()
    {
        CurrentMomentumValue -= _decreaseValue;
        if (CurrentMomentumValue < 0)
        {
            CurrentMomentumValue = 0;
        }
    }

    public float GetMomentumRatio()
    {
        return CurrentMomentumValue / _maxMomentum;
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