using UnityEngine;
using SpawnManagerMod.Configs;
using Utils.GenericSingletons;

namespace SpawnManagerMod
{

    public class SpawnManager : MonoBehaviourSingleton<SpawnManager>
    {
        //Example of how to use the SpawnManager
        [field: SerializeField] public PrefabConfig TurretBulletPrefab { get; private set; }
        [field: SerializeField] public PrefabConfig DasherPrefab { get; private set; }
        [field: SerializeField] public PrefabConfig BomberPrefab { get; private set; }
        [field: SerializeField] public PrefabConfig ElitePrefab { get; private set; }
        [field: SerializeField] public PrefabConfig EliteProjectilePrefab { get; private set; }
    }
}