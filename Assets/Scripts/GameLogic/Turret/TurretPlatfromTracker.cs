using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TurretPlatfromTracker
{
    private PathCreator _pathCreator;
    private Turret _turret;
    private float _moveSpeed = 7.0f;

    private Transform _turretIndicatorPosition;

    private float _distanceTravelled;


    public TurretPlatfromTracker(Turret turret)
    {
        _moveSpeed = GameVariables.instance.PlayerSpeed;

        _pathCreator = GameObject.FindObjectOfType<PathCreator>();

        _turret = turret;

        _turretIndicatorPosition = _turret.transform;
    }


    public Transform MoveIndicator(RotationDirection direction)
    {
        Vector2 newPos = _turretIndicatorPosition.localPosition;
        switch (direction)
        {
            case RotationDirection.ClockWise:
                _distanceTravelled -= _moveSpeed * Time.deltaTime;
                break;
            case RotationDirection.AntiClockWise:
                _distanceTravelled += _moveSpeed * Time.deltaTime;
                break;
        }

        _turretIndicatorPosition.position = _pathCreator.path.GetPointAtDistance(_distanceTravelled);
        _turretIndicatorPosition.up = _pathCreator.path.GetNormalAtDistance(_distanceTravelled);

        return _turretIndicatorPosition;
    }
}
