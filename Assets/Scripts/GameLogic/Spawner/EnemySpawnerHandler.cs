using System;
using GameLogic.Spawner.Utils;
using SpawnManagerMod;
using UnityEngine;
using Utils.ArrayUtils;

namespace GameLogic.Spawner.EnemySpawner
{
    public static class EnemySpawnerHandler
    {

        public class EliteSpawn : SpawnAction
        {
            public EliteSpawn()
            {
                AddSpawnAction(SpawnElite);
            }

            private void SpawnElite()
            {
                Sequencer sequencer = Sequencer.CreateSequencer("Spawn Bomber");


                var elitePrefab = SpawnManager.instance.ElitePrefab;

                Elite spawnedElite = null;

                Sequencer.SequenceState SpawnElite()
                {
                    Vector3 spawnPoint = base.GetCalculatedPosition();
                    spawnedElite = elitePrefab.CreateGameObject(spawnPoint, Quaternion.identity).GetComponent<Elite>();
                    return spawnedElite.ScaleUpAndSpawn();
                }


                Sequencer.SequenceState EliteAttack()
                {
                    var enemyProjectile = SpawnManager.instance.EliteProjectilePrefab;
                    float delayToShoot = 1.7f;
                    float timer = delayToShoot;

                    ArrayTools.PseudoRandArray<float> angles = new ArrayTools.PseudoRandArray<float>(new float[] { 0, 45, 75 });

                    BehavioralData behavior = new BehavioralData();


                    void UpdateBehavior()
                    {
                        if (spawnedElite == null)
                        {
                            BehavioralController.instance.RemoveBehavioral(behavior);
                        }

                        if (spawnedElite.Stunned) return;

                        timer -= Time.deltaTime;

                        if (timer <= 0.0f)
                        {
                            timer = delayToShoot;

                            int amountToSpawnBullet = 6;
                            float radius = 0.75f;
                            Vector2[] positions = SpawnerUtils.GetPositionsAroundObject(spawnedElite.transform.position, radius, amountToSpawnBullet, angles.PickNext());

                            for (int i = 0; i < amountToSpawnBullet; i++)
                            {
                                Transform proj = enemyProjectile.CreateGameObject(positions[i], Quaternion.identity).transform;
                                Vector3 direction = (proj.position - spawnedElite.transform.position).normalized;
                                proj.GetComponent<Projectile>().SetShootDirection(direction);
                            }
                        }
                    }


                    behavior.UpdateBehavior = UpdateBehavior;
                    behavior.OnBehaviorEnd = BehavioralController.NULL_BEHAVIOR;

                    BehavioralController.instance.AddBehavioral(behavior);



                    return Sequencer.SequenceState.Finish();
                }

                sequencer.AddSequence(SpawnElite);
                sequencer.AddSequence(EliteAttack);

                sequencer.StartSequencer();
            }
        }


        public class BombrSpawn : SpawnAction
        {
            public BombrSpawn()
            {
                AddSpawnAction(SpawnBombers);
            }


            public void SpawnBombers()
            {
                var bomberPrefab = SpawnManager.instance.BomberPrefab;

                Sequencer sequencer = Sequencer.CreateSequencer("Spawn Bomber");

                Transform target = GameObject.Find("Turret").transform;

                Bomber spawnedBomber = null;

                Sequencer.SequenceState SpawnBomber()
                {
                    Vector3 spawnPoint = base.GetCalculatedPosition();
                    spawnedBomber = bomberPrefab.CreateGameObject(new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Bomber>();
                    return spawnedBomber.ScaleUpAndSpawn();
                }


                Sequencer.SequenceState AttackTarget()
                {
                    var attackBehavior = new BehavioralData();

                    void UpdateBehavior()
                    {
                        if (target == null)
                        {
                            BehavioralController.instance.RemoveBehavioral(attackBehavior);
                            return;
                        }

                        if (spawnedBomber == null)
                        {
                            BehavioralController.instance.RemoveBehavioral(attackBehavior);
                            return;
                        }

                        spawnedBomber.HandleAttackingTarget(target.position);
                    }


                    attackBehavior.UpdateBehavior = UpdateBehavior;
                    attackBehavior.OnBehaviorEnd = BehavioralController.NULL_BEHAVIOR;

                    BehavioralController.instance.AddBehavioral(attackBehavior);

                    return Sequencer.SequenceState.Finish();
                }


                sequencer.AddSequence(SpawnBomber);
                sequencer.AddSequence(AttackTarget);

                sequencer.StartSequencer();
            }
        }


        /*
            Swarm Spawn is made to handle spawning multiple of the dasher enemies with many types of difficulties
        */
        public class DasherSwarmSpawn : SpawnAction
        {
            public DasherSwarmSpawn()
            {
                AddSpawnAction(EasySwarm);
            }


            private bool CheckIfAllDashersAreDead(Transform[] dashers)
            {
                foreach (Transform dasher in dashers)
                {
                    if (dasher != null)
                    {
                        return false;
                    }
                }

                return true;
            }

            private void EasySwarm()
            {
                Transform target = GameObject.Find("Turret").transform;

                var dasherPrefab = SpawnManager.instance.DasherPrefab;
                int amountToSpawn = 3;
                Transform[] dashersSpawned = new Transform[amountToSpawn];

                PseudoRandArray<float> attackDelaysArray = new PseudoRandArray<float>(new float[] { 1.0f, 0.5f, 0.85f });

                Vector3 centerSwarmSpawnPoint = base.GetCalculatedPosition();
                // Vector3 centerSwarmSpawnPoint = CurrentRoom.GetRandomPositionInTopsideOfOctagon(1.0f);

                Sequencer sequencer = Sequencer.CreateSequencer("SmallSwarm");

                Sequencer.SequenceState SpawnObjects()
                {
                    Sequencer.SequenceState state = null;

                    for (int i = 0; i < amountToSpawn; i++)
                    {
                        dashersSpawned[i] = dasherPrefab.CreateGameObject(centerSwarmSpawnPoint, Quaternion.identity).transform;
                    }

                    SpawnerUtils.PositionInCirclePattern(dashersSpawned, centerSwarmSpawnPoint);


                    for (int i = 0; i < amountToSpawn; i++)
                    {
                        state = dashersSpawned[i].GetComponent<Dasher>().ScaleUpAndSpawn();
                    }

#if UNITY_EDITOR
                    if (state == null)
                    {
                        Debug.LogError("State is null");
                    }
#endif


                    return state;
                }

                Sequencer.SequenceState GetReadyToAttackBehavior()
                {

                    Sequencer.SequenceState state = new Sequencer.SequenceState();

                    Vector3 centerOfSwarm = SpawnerUtils.GetCenterOfObjects(dashersSpawned, centerSwarmSpawnPoint);

                    // dashersSpawned = SpawnerUtils.RemoveNullsFromArray<Transform>(dashersSpawned);
                    float rotationAroundCenterSpeed = 150.0f;

                    Debug.Log("DashSpawned: " + dashersSpawned.Length);

                    Dasher[] dasherScripts = new Dasher[dashersSpawned.Length];
                    for (int i = 0; i < dashersSpawned.Length; i++)
                    {
                        if (dashersSpawned[i] != null)
                        {
                            dasherScripts[i] = dashersSpawned[i].GetComponent<Dasher>();
                        }
                    }

                    BehavioralDataWithTimer behaviorWithTime = new BehavioralDataWithTimer();
                    behaviorWithTime.DurationOfBehavior = 1.5f;
                    behaviorWithTime.UpdateBehavior = () =>
                    {
                        BehavioralUtils.RotateAroundObject(dashersSpawned, centerSwarmSpawnPoint, rotationAroundCenterSpeed);
                        for (int i = 0; i < dashersSpawned.Length; i++)
                        {
                            if (dashersSpawned[i] != null)
                            {
                                dasherScripts[i].RotateTowardsTarget(target.position);
                            }
                        }

                    };

                    behaviorWithTime.OnBehaviorEnd = () =>
                    {
                        state.FinishSequence();
                    };

                    BehavioralController.instance.AddBehavioral(behaviorWithTime);

                    return state;
                }


                Sequencer.SequenceState AttackTarget()
                {
                    // SpawnerUtils.RemoveNullsFromArray<Transform>(ref dashersSpawned);

                    if (CheckIfAllDashersAreDead(dashersSpawned))
                    {
                        return Sequencer.SequenceState.Finish();
                    }

                    var dasherAttackSequence = Sequencer.CreateSequencer("AttackBehavior");
                    // List<Dasher> dasherScriptAttacking = new List<Dasher>();


                    int amountOfAttacks = dashersSpawned.Length;
                    for (int i = 0; i < amountOfAttacks; i++)
                    {
                        if (dashersSpawned[i] == null) continue;

                        Dasher dasher = dashersSpawned[i].GetComponent<Dasher>();

                        if (dasher == null) continue;

                        dasherAttackSequence.AddSequence(() =>
                        {
                            if (dasher == null) return Sequencer.SequenceState.Finish();

                            var future = dasher.ScaleUpAndAttack();

                            future.Handle(() =>
                            {
                                // dasherScriptAttacking.Add(dasher);
                                dasher.SetDashTarget(target.position);
                            });

                            return future;
                        });
                    }


                    dasherAttackSequence.StartSequencer();


                    return Sequencer.SequenceState.Finish();
                }



                sequencer.AddSequence(SpawnObjects);
                sequencer.AddSequence(GetReadyToAttackBehavior);
                sequencer.AddSequence(AttackTarget);
                sequencer.StartSequencer();




            }
        }


        public abstract class SpawnAction
        {
            private Room _currentRoom;
            private Action _spawnAction;

            public void AddSpawnAction(Action spawnAction)
            {
                _spawnAction = spawnAction;
            }

            public void InvokSpawnAction()
            {

                _spawnAction?.Invoke();
            }

            public void SetRoomToSpawn(Room room)
            {
                _currentRoom = room;
            }

            protected Vector3 GetCalculatedPosition()
            {
                return _currentRoom.GetRandomSpawnPositionWithinRoomRange(0.65f);
            }
        }
    }
}