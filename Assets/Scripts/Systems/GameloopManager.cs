using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class GameloopManager : MonoBehaviourSingleton<GameloopManager>
{
    public TurretPlatfromTracker TurretPlatfromTracker { get; private set; }

    private void Awake()
    {
        Turret turret = FindObjectOfType<Turret>();
        TurretPlatfromTracker = new TurretPlatfromTracker(turret);
    }

    public void StartGameLoop()
    {

    }

    private void Update()
    {
        TurretPlatfromTracker.TrackTurretOnPlatform();
    }
}
