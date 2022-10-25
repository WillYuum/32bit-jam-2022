using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class GameloopManager : MonoBehaviourSingleton<GameloopManager>
{
    public TurretPlatfromTracker TurretPlatfromTracker { get; private set; }
    private Room _currentRoom;

    private void Awake()
    {
        Turret turret = FindObjectOfType<Turret>();
        TurretPlatfromTracker = new TurretPlatfromTracker(turret);

    }

    void Start()
    {
        _currentRoom = GameObject.Find("Room_1").GetComponent<Room>();

        Vector3 newCameraPos = _currentRoom.CameraPoint.position;
        newCameraPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = newCameraPos;
    }

    public void StartGameLoop()
    {

    }

    private void Update()
    {
        TurretPlatfromTracker.TrackTurretOnPlatform();
    }


    public void OnKillEnemy()
    {

    }

}
