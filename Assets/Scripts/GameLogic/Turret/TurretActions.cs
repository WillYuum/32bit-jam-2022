using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotationDirection
{
    ClockWise,
    AntiClockWise,
}

public enum TurretMoveDirection
{
    None,
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
            Move(RotationDirection.ClockWise);
            _turretActions.SetTurretMoveDirection(TurretMoveDirection.ClockWise);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(RotationDirection.AntiClockWise);
            _turretActions.SetTurretMoveDirection(TurretMoveDirection.AntiClockWise);
        }
        else
        {
            _turretActions.SetTurretMoveDirection(TurretMoveDirection.None);
        }
    }

    public void Shoot()
    {
        _turretActions.HandleShoot();
    }

    public void Move(RotationDirection movement)
    {
        Vector2 newPos = GameloopManager.instance.TurretPlatfromTracker.MoveIndicator(movement);
        _turretActions.UpdatePosition(newPos);
    }
}


