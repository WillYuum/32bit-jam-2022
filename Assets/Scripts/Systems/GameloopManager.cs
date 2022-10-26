using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;
using SpawnManagerMod;

public class GameloopManager : MonoBehaviourSingleton<GameloopManager>
{
    public TurretPlatfromTracker TurretPlatfromTracker { get; private set; }
    private Room _currentRoom;


    private void Awake()
    {

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


    public void StartGameLoop()
    {
        enabled = true;

        Turret turret = FindObjectOfType<Turret>();
        TurretPlatfromTracker = new TurretPlatfromTracker(turret);


        _currentRoom = GameObject.Find("Room_1").GetComponent<Room>();
        _currentRoom.UpdatePlatformNeighbors();

        Vector3 newCameraPos = _currentRoom.CameraPoint.position;
        newCameraPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = newCameraPos;


        InvokeRepeating(nameof(SpawnEnemyRandomly), 0, 0.5f);
    }

    private void SpawnEnemyRandomly()
    {
        Vector3 spawnPoint = _currentRoom.GetRandomSpawnPositionWithinRoomRange(0.25f);
        SpawnManager.instance.SpawnBerserker(spawnPoint, Quaternion.identity);
    }

    private void Update()
    {
        TurretPlatfromTracker.TrackTurretOnPlatform();
    }


    public void OnKillEnemy()
    {

    }

}
