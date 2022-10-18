using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretMovement
{
    ClockWise,
    AntiClockWise,
}

public class TurretActions : MonoBehaviour
{
    private ITurretActions _turretActions;

    private void Awake()
    {
        _turretActions = gameObject.GetComponent<ITurretActions>();
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Move(TurretMovement.ClockWise);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(TurretMovement.AntiClockWise);
        }
    }

    public void Shoot()
    {
        _turretActions.HandleShoot();
    }

    public void Move(TurretMovement movement)
    {
        Vector2 newPos = GameloopManager.instance.TurretPlatfromTracker.MoveIndicator(movement);
        _turretActions.UpdatePosition(newPos);
    }
}


