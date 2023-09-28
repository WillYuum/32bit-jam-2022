using UnityEngine;
using Utils.ArrayUtils;
using static GameLogic.Spawner.EnemySpawner.EnemySpawnerHandler;

namespace GameLogic.Spawner
{
    public class Spawner : MonoBehaviour
    {
        private ArrayTools.PseudoRandArray<SpawnAction> _waves;
        private float _currentTimer;

        private void Awake()
        {
            GameloopManager.instance.OnRestartGame += () =>
            {
                _currentTimer = GameloopManager.instance.GameDiffVariables.GetSpawnDelay();

                SpawnAction[] waves = new SpawnAction[]{
            new DasherSwarmSpawn(),
            new BombrSpawn(),
            new DasherSwarmSpawn(),
            new EliteSpawn(),
            new BombrSpawn(),
            new BombrSpawn(),
            new EliteSpawn(),
            new DasherSwarmSpawn(),
            new BombrSpawn(),
            new DasherSwarmSpawn(),
            new DasherSwarmSpawn(),
            new BombrSpawn(),
            new BombrSpawn(),
            new DasherSwarmSpawn(),
            new EliteSpawn(),
            new BombrSpawn(),
            new EliteSpawn(),
        };


                //NOTE FOR FUTURE LOGIC:
                //I want to instead of create an array of attack waves, it should create automatically
                //base on the variable(TimePassed since playing || Score || etc...)
                //When all attack waves are done, create more of the them but with more difficulty

                //IDEA: If x seconds passed in game, check how much enemies the player is killing,
                //  if player is killing too much, spawn more enemies(Decrease start next wave)

                _waves = new ArrayTools.PseudoRandArray<SpawnAction>(waves);
            };

            _currentTimer = GameloopManager.instance.GameDiffVariables.GetSpawnDelay();
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
                _currentTimer = GameloopManager.instance.GameDiffVariables.GetSpawnDelay();

                _waves.PickNext().InvokSpawnAction();
            }
        }


        public void ResetSpawnerProps()
        {

        }

        public void StartSpawner(Room roomToSpawn)
        {
            enabled = true;

            SpawnAction[] waves = new SpawnAction[]{
            new DasherSwarmSpawn(),
            new DasherSwarmSpawn(),
            new DasherSwarmSpawn(),
            new BombrSpawn(),
            new DasherSwarmSpawn(),
            new EliteSpawn(),
            new BombrSpawn(),
            new BombrSpawn(),
            new EliteSpawn(),
            new DasherSwarmSpawn(),
            new BombrSpawn(),
            new DasherSwarmSpawn(),
            new DasherSwarmSpawn(),
            new BombrSpawn(),
            new BombrSpawn(),
            new DasherSwarmSpawn(),
            new EliteSpawn(),
            new BombrSpawn(),
            new EliteSpawn(),
    };


            foreach (SpawnAction item in waves)
            {
                item.SetRoomToSpawn(roomToSpawn);
            }


            _waves = new ArrayTools.PseudoRandArray<SpawnAction>(waves);

            _waves.PickNext().InvokSpawnAction();
        }

        public void StopSpawner()
        {
            _waves = null;
            enabled = false;
        }
    }
}