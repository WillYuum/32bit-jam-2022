using UnityEngine;
using SpawnManagerMod.Configs;
using Utils.GenericSingletons;

namespace SpawnManagerMod
{

    public class SpawnManager : MonoBehaviourSingleton<SpawnManager>
    {
        //Example of how to use the SpawnManager
        [field: SerializeField] public PrefabConfig EnemyPrefab;
        [field: SerializeField] public PrefabConfig TurretBulletPrefab;
        [field: SerializeField] public PrefabConfig BerserkerPrefab;
    }
}