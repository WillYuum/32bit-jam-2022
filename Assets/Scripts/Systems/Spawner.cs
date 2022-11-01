using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;
using Utils.ArrayUtils;
using SpawnManagerMod;

public class Spawner : MonoBehaviourSingleton<Spawner>
{
    ArrayTools.PseudoRandArray<AttackWave> _waves;

    private float _delayToNextSpawn = 5.0f;

    private float _currentTimer;

    private void Awake()
    {
        _currentTimer = _delayToNextSpawn;
    }

    private void Update()
    {
        if (_waves == null)
        {
            return;
        }

        _currentTimer -= Time.deltaTime;

        if (_currentTimer <= 0.0f)
        {
            _currentTimer = _delayToNextSpawn;
            _delayToNextSpawn -= 0.15f;

            if (_delayToNextSpawn < 2.5f)
            {
                _delayToNextSpawn = 2.5f;
            }

            _waves.PickNext().InvokNextSpawnAction();
        }
    }

    public void StartSpawner()
    {
        AttackWave[] waves = new AttackWave[]{
            new SimpleSwarmSpawn(),
            new SimpleBombersAttack(),
            new SimpleSwarmSpawn(),
            new SimpleEliteSpawn(),
            new SimpleBombersAttack(),
            new SimpleEliteSpawn(),
            new SimpleSwarmSpawn(),
            new SimpleSwarmSpawn(),
            new SimpleEliteSpawn(),
            new SimpleBombersAttack(),
            new SimpleEliteSpawn(),
    };


        _waves = new ArrayTools.PseudoRandArray<AttackWave>(waves);

        _waves.PickNext().InvokNextSpawnAction();
    }

}


public class SimpleEliteSpawn : AttackWave
{
    public SimpleEliteSpawn()
    {
        AddSpawnAction(SpawnElite);
    }

    private void SpawnElite()
    {
        var elite = SpawnManager.instance.ElitePrefab;

        Vector3 spawnPoint = GameloopManager.instance.CurrentRoom.GetRandomSpawnPositionWithinRoomRange(0.65f);


        var spawnedElite = elite.CreateGameObject(spawnPoint, Quaternion.identity);


        var enemyProjectile = SpawnManager.instance.EliteProjectilePrefab;
        float delayToShoot = 1.7f;
        float timer = delayToShoot;

        ArrayTools.PseudoRandArray<float> angles = new ArrayTools.PseudoRandArray<float>(new float[] { 0, 45, 75 });
        var behavior = new BehavioralData();

        void UpdateBehavior()
        {
            if (spawnedElite == null)
            {
                BehavioralController.instance.RemoveBehavioral(behavior);
            }

            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
                timer = delayToShoot;

                int amountToSpawn = 6;
                Vector2[] positions = SpawnerUtils.GetPositionsAroundObject(spawnedElite.transform.position, 0.75f, amountToSpawn, angles.PickNext());

                for (int i = 0; i < 6; i++)
                {
                    var proj = enemyProjectile.CreateGameObject(positions[i], Quaternion.identity).transform;
                    proj.GetComponent<Projectile>().SetShootDirection((positions[i] - (Vector2)spawnedElite.transform.position).normalized);
                }
            }
        }


        behavior.UpdateBehavior = UpdateBehavior;
        behavior.OnBehaviorEnd = () => { };

        BehavioralController.instance.AddBehavioral(behavior);
    }
}


public class SimpleBombersAttack : AttackWave
{
    public SimpleBombersAttack()
    {
        AddSpawnAction(SpawnBombers);
    }


    public void SpawnBombers()
    {
        var bomberPrefab = SpawnManager.instance.BomberPrefab;

        for (int i = 0; i < 4; i++)
        {
            Vector3 spawnPoint = GameloopManager.instance.CurrentRoom.GetRandomSpawnPositionWithinRoomRange(0.7f);
            var bomber = bomberPrefab.CreateGameObject(new Vector3(0, 0, 0), Quaternion.identity);
        }

    }

}


public class SimpleSwarmSpawn : AttackWave
{
    public SimpleSwarmSpawn()
    {
        AddSpawnAction(SpawnSwarms);
    }

    private void SpawnSwarms()
    {
        var dasherPrefab = SpawnManager.instance.DasherPrefab;
        int amountToSpawned = 3;
        Transform[] dashersSpawned = new Transform[amountToSpawned];

        PseudoRandArray<float> randArray = new PseudoRandArray<float>(new float[] { 1.0f, 0.5f, 0.85f });

        Vector3 spawnPoint = GameloopManager.instance.CurrentRoom.GetRandomSpawnPositionWithinRoomRange(0.5f);

        for (int i = 0; i < amountToSpawned; i++)
        {
            dashersSpawned[i] = dasherPrefab.CreateGameObject(spawnPoint, Quaternion.identity).transform;
            dashersSpawned[i].GetComponent<Dasher>().StartSpawn();
        }

        SpawnerUtils.SpawnInCirclePattern(dashersSpawned, spawnPoint);

        Vector3 centerOfSwarm = SpawnerUtils.GetCenterOfObjects(dashersSpawned, spawnPoint);

        float rotationAroundCenterSpeed = 150.0f;
        BehavioralController.instance.AddBehavioral(new BehavioralDataWithTimer()
        {
            DurationOfBehavior = 1.5f,
            UpdateBehavior = () =>
            {
                BehavioralUtils.RotateAroundObject(dashersSpawned, centerOfSwarm, rotationAroundCenterSpeed);
            },
            OnBehaviorEnd = () =>
            {
                for (int i = 0; i < dashersSpawned.Length; i++)
                {
                    if (dashersSpawned[i] != null)
                    {
                        dashersSpawned[i].GetComponent<Dasher>().ReadyToGo(randArray.PickNext());
                    }
                }
            }
        });
    }
}


public abstract class AttackWave
{
    private List<Action> _spawnActions = new List<Action>();
    private int _currentActionIndex = 0;

    public void AddSpawnAction(Action spawnAction)
    {
        _spawnActions.Add(spawnAction);
    }

    public void InvokNextSpawnAction()
    {
        if (_currentActionIndex >= _spawnActions.Count)
        {
            _currentActionIndex = 0;
        }
        _spawnActions[_currentActionIndex].Invoke();
        _currentActionIndex++;
    }

    public bool IsFinished()
    {
        return _currentActionIndex >= _spawnActions.Count;
    }
}
