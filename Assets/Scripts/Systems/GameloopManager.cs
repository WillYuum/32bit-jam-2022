using UnityEngine;
using Utils.GenericSingletons;
using System;
using GameLogic.Spawner;

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
    public event Action OnGameLoopStart;
    public event Action<int> OnFishTakeHit;
    public event Action OnKillEnemy;
    public event Action OnRestartGame;
    public event Action<float> OnMomentumChange;

    private InBattleTimer _inBattleTimer;

    public TypeOfShots SelectedShootType { get; private set; }

    public InvisiblityWindowTracker TurretInvisiblityWindowTracker { get; private set; }

    public ExplosionBarTracker ExplosionBarTracker { get; private set; }

    private HitPoint _fishHitPoints;
    public int FishHP { get { return _fishHitPoints.CurrenthitPoint; } }

    public int CollectedHightScore { get; private set; }
    public bool LoopIsActive { get; private set; }


    public EnemyHpCalculator EnemyHpCalculator { get; private set; }

    public KillMomentum KillMomentunTracker { get; private set; }
    public ShootBehavior CurrentShootBehavior { get; private set; }


    private Spawner _spawner;

    public MaxAmountEnemySpawned MaxAmountEnemySpawned;

    public EnemyTracker EnemyTracker { get; private set; }

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


        _inBattleTimer.IncrementTime(Time.deltaTime);


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            ExplosionBarTracker.IncreaseValue(999);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            InvokeLoseGame();
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            ExplosionBarTracker.IncreaseValue(ExplosionBarTracker.MaxExplosionBarValue);
        }
#endif
    }



    public GameDiffVariables GameDiffVariables { get; private set; }
    public void StartGameLoop(StartGameLoopStruct startGameLoopStruct)
    {
        ExplosionBarTracker = new ExplosionBarTracker(GameVariables.instance.ExplosionBarData.MaxExplosionBarValue);
        TurretInvisiblityWindowTracker = new InvisiblityWindowTracker();

        GameloopManager.instance.LoopIsActive = true;

        _inBattleTimer = new InBattleTimer();


        EnemyTracker = new EnemyTracker();


        var gameVariableFromGameLoop = new BoxGameVariable<SpawnDelayCalculator.RequiredVariables>
        {
            GetDataFromGameLoop = () => new SpawnDelayCalculator.RequiredVariables
            {
                TimePlayed = _inBattleTimer.CurrentTime,
                EnemiesKilled = EnemyTracker.EnemiesKilled,
            }
        };


        GameDiffVariables = new GameDiffVariables(gameVariableFromGameLoop);


        EnemyHpCalculator = new EnemyHpCalculator(new BoxGameVariable<EnemyHpCalculator.RequiredVariables>
        {
            GetDataFromGameLoop = () => new EnemyHpCalculator.RequiredVariables
            {
                EnemiesKilled = EnemyTracker.EnemiesKilled,
            }
        });


        MaxAmountEnemySpawned = new MaxAmountEnemySpawned(new BoxGameVariable<MaxAmountEnemySpawned.RequiredVariables>
        {
            GetDataFromGameLoop = () => new MaxAmountEnemySpawned.RequiredVariables
            {
                EnemiesKilled = EnemyTracker.EnemiesKilled,
            }
        });

        CollectedHightScore = 0;

        AudioManager.instance.PlayBGM("Main");

        int startingHP = 5;
        _fishHitPoints = new HitPoint(startingHP);

        SetShootTypeOnTurret(startGameLoopStruct.SelectTypeShot);

        float increaseRatio = 0.05f;
        float decreaseRatio = 0.03f;
        KillMomentunTracker = new KillMomentum(increaseRatio, decreaseRatio);

        enabled = true;
        LoopIsActive = true;

        if (_spawner != null)
        {
            Destroy(_spawner.gameObject);
        }

        _spawner = new GameObject("Spawner").AddComponent<Spawner>();
        // _spawner.StartSpawner(GameObject.FindObjectOfType<Room>());

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
            InvokeLoseGame();
        }
    }


    private void InvokeLoseGame()
    {
        if (LoopIsActive == false) return;

        GameloopManager.instance.LoopIsActive = false;

        _spawner.StopSpawner();

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
        // Invoke(nameof(StartGameLoop), 0.1f);
        // LoadGameFromMainGame();
        if (OnRestartGame != null)
        {
            OnRestartGame.Invoke();
        }

        OnGameLoopStart = null;
        OnKillEnemy = null;
        OnRestartGame = null;
    }

    public void InvokeKillEnemy(EnemyType enemyType)
    {
        KillMomentunTracker.IncreaseMomentum();
        GameloopManager.instance.OnMomentumChange.Invoke(KillMomentunTracker.GetMomentumRatio());

        EnemyTracker.IncreaseKilled();

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


    private Tuple<float, int>[] _peaShooterLevelConfig = new Tuple<float, int>[]
    {
        new Tuple< float, int>(0.0f, 1),
        new Tuple< float, int>(0.55f, 2),
        new Tuple< float, int>(0.75f, 3),
    };

    private Tuple<float, int>[] _laserShooterLevelConfig = new Tuple<float, int>[]
    {
        new Tuple< float, int>(0.5f, 1),
        new Tuple< float, int>(0.75f, 2),
        new Tuple< float, int>(0.9f, 3),
    };

    private void DetermineShootLevel()
    {

        int GetSelectedLevel(float momentumRatio, Tuple<float, int>[] levelConfig)
        {
            int index = (int)(levelConfig.Length * momentumRatio);
            index = Math.Min(index, levelConfig.Length - 1);

            if (momentumRatio >= levelConfig[index].Item1)
            {
                return levelConfig[index].Item2;
            }

            return CurrentShootBehavior.CurrentLevel;
        }

        int currentShootLevel = CurrentShootBehavior.CurrentLevel;
        float momentumRatio = KillMomentunTracker.GetMomentumRatio();

        int levelToUse = 0;

        switch (SelectedShootType)
        {
            case TypeOfShots.PeaShots:
                levelToUse = GetSelectedLevel(momentumRatio, _peaShooterLevelConfig);
                break;

            case TypeOfShots.Laser:
                levelToUse = GetSelectedLevel(momentumRatio, _laserShooterLevelConfig);
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

    public bool CanUseBigBoom()
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

public class InBattleTimer
{
    public float CurrentTime;

    public InBattleTimer()
    {
        CurrentTime = 0;
    }

    public void IncrementTime(float deltaTime)
    {
        CurrentTime += deltaTime;
    }
}



public class BoxGameVariable<T> where T : struct
{
    public Func<T> GetDataFromGameLoop;
}



public class GameDiffVariables
{

    private SpawnDelayCalculator _spawnDelayCalculator;


    public GameDiffVariables(BoxGameVariable<SpawnDelayCalculator.RequiredVariables> gameVariableFromGameLoop)
    {
        _spawnDelayCalculator = new(gameVariableFromGameLoop);
    }


    public float GetSpawnDelay()
    {
        float spawnDelay = _spawnDelayCalculator.SpawnDelayValue;
        Debug.Log("Spawn Delay: " + spawnDelay);
        return spawnDelay;
    }
}

public class EnemyHpCalculator
{
    private BoxGameVariable<RequiredVariables> _gameDiffVariables;
    private const int _maxDasherKills = 164;
    private const int _maxBomberKills = 190;
    private const int _maxEliteKills = 150;

    private int[] _dasherHp = new int[3] { 1, 1, 2 };
    private int[] _bomberHp = new int[3] { 5, 6, 7 };
    private int[] _eliteHp = new int[3] { 10, 11, 12 };

    public struct RequiredVariables
    {
        public int EnemiesKilled;
    }


    public EnemyHpCalculator(BoxGameVariable<RequiredVariables> gameDiffVariables)
    {
        _gameDiffVariables = gameDiffVariables;

    }

    public int GetEnemyHP(EnemyType enemyType)
    {
        //I want to get the HP from the array variable and the calculation should be percentage of the max kills
        int enemiesKilled = _gameDiffVariables.GetDataFromGameLoop().EnemiesKilled;
        int enemyLevel = GetEnemyLevel(enemiesKilled, _maxDasherKills);

        switch (enemyType)
        {
            case EnemyType.Dasher:
                return _dasherHp[enemyLevel];
            case EnemyType.Bomber:
                return _bomberHp[enemyLevel];
            case EnemyType.Elite:
                return _eliteHp[enemyLevel];
            default:
                return 0;
        }
    }

    private int GetEnemyLevel(int enemiesKilled, int maxKills)
    {
        float percentage = (float)enemiesKilled / maxKills;
        int level = (int)(percentage * 3);
        return Mathf.Min(level, 2);
    }
}

public class SpawnDelayCalculator
{
    private BoxGameVariable<RequiredVariables> _gameDiffVariables;


    public struct RequiredVariables
    {
        public float TimePlayed;
        public int EnemiesKilled;
    }

    private const float _minDelay = 1.5f;    // Minimum SpawnDelay in seconds
    private const float _maxDelay = 5.5f;    // Maximum SpawnDelay in seconds
    private const float _timePlayedFactor = 50.0f;   // The factor for time played
    private const float _enemiesKilledFactor = 5.0f; // The factor for enemies killed

    public float SpawnDelayValue
    {
        get
        {
            return CalculateSpawnDelay();
        }
    }

    public SpawnDelayCalculator(BoxGameVariable<RequiredVariables> gameDiffVariables)
    {
        _gameDiffVariables = gameDiffVariables;
    }



    // Calculate the SpawnDelay with modifiers
    private float CalculateSpawnDelay()
    {
        RequiredVariables requiredVariables = _gameDiffVariables.GetDataFromGameLoop();

        float timePlayed = requiredVariables.TimePlayed;
        int enemiesKilled = requiredVariables.EnemiesKilled;

        Debug.Log("Time played: " + timePlayed);
        Debug.Log("Enemies killed: " + enemiesKilled);

        // Time played & enemies killed are the modifiers for the SpawnDelay variable and
        // used timePlayedFactor & enemiesKilledFactor to add scaling to the value calculated.
        float spawnDelay = Mathf.Max(_minDelay, Mathf.Min(_maxDelay, _maxDelay - (timePlayed / _timePlayedFactor + enemiesKilled / _enemiesKilledFactor)));

        return spawnDelay;
    }
}


public class MaxAmountEnemySpawned
{
    public struct RequiredVariables
    {
        public int EnemiesKilled;
    }


    private BoxGameVariable<RequiredVariables> _gameDiffVariables;


    public MaxAmountEnemySpawned(BoxGameVariable<RequiredVariables> gameDiffVariables)
    {
        _gameDiffVariables = gameDiffVariables;
    }


    public int GetMaxToSpawn()
    {
        RequiredVariables requiredVariables = _gameDiffVariables.GetDataFromGameLoop();
        int maxEnemyKilled = 185;

        switch (requiredVariables.EnemiesKilled)
        {
            case int n when n >= 185:
                return 13;
            case int n when n >= (int)(maxEnemyKilled * 0.5):
                return 9;
            default:
                return 7;
        }
    }
}


public class EnemyTracker
{
    public int EnemiesKilled { get; private set; }
    public int EnemiesActive { get; private set; }

    public EnemyTracker()
    {
        EnemiesKilled = 0;
        EnemiesActive = 0;
    }


    public void IncreaseKilled()
    {
        EnemiesKilled += 1;
        DecreaseActive();
    }

    public void IncreaseActive()
    {
        EnemiesActive += 1;
    }

    private void DecreaseActive()
    {
        EnemiesActive -= 1;
        if (EnemiesActive < 0)
        {
            EnemiesActive = 0;
        }
    }
}

