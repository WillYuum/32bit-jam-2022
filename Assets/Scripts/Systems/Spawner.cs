using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;
using Utils.ArrayUtils;
using SpawnManagerMod;

public class Spawner : MonoBehaviourSingleton<Spawner>
{
    AttackWave[] _waves;

    public void StartWithScriptedWaves()
    {
        _waves = new AttackWave[]{
            new DashSwarmTutorial(),
            new DashSwarmTutorial(),
        };

        _waves[0].InvokNextSpawnAction();
    }

}



public class DashSwarmTutorial : AttackWave
{
    public DashSwarmTutorial()
    {
        AddSpawnAction(SpawnSwarms);
    }

    private void SpawnSwarms()
    {
        var dasherPrefab = SpawnManager.instance.DasherPrefab;
        int amountToSpawned = 3;
        Transform[] dashersSpawned = new Transform[amountToSpawned];

        PseudoRandArray<float> randArray = new PseudoRandArray<float>(new float[] { 1.0f, 0.5f, 0.85f });

        for (int i = 0; i < amountToSpawned; i++)
        {
            dashersSpawned[i] = dasherPrefab.CreateGameObject(new Vector3(0, 0, 0), Quaternion.identity).transform;
            dashersSpawned[i].GetComponent<Dasher>().StartSpawn();
        }

        SpawnerUtils.SpawnInCirclePattern(dashersSpawned, Vector3.zero);

        Vector3 centerOfSwarm = SpawnerUtils.GetCenterOfObjects(dashersSpawned, Vector3.zero);

        float rotationAroundCenterSpeed = 150.0f;
        BehavioralController.instance.AddBehavioral(new BehavioralData()
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
                    dashersSpawned[i].GetComponent<Dasher>().ReadyToGo(randArray.PickNext());
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
        _spawnActions[_currentActionIndex].Invoke();
        _currentActionIndex++;
    }

    public bool IsFinished()
    {
        return _currentActionIndex >= _spawnActions.Count;
    }
}
