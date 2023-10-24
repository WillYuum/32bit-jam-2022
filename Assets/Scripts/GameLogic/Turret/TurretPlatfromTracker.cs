using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TurretPlatfromTracker
{
    private VertexPath _vertexPath;
    private Turret _turret;
    private float _moveSpeed = 7.0f;

    private Transform _turretIndicatorPosition;

    private float _distanceTravelled;


    public TurretPlatfromTracker(Turret turret)
    {
        _moveSpeed = GameVariables.instance.PlayerSpeed;

        _vertexPath = GameObject.FindObjectOfType<PathCreator>().path;

        _turret = turret;

        _turretIndicatorPosition = _turret.transform;
    }


    public Transform MoveIndicator(RotationDirection direction)
    {
        switch (direction)
        {
            case RotationDirection.ClockWise:
                _distanceTravelled -= _moveSpeed * Time.deltaTime;
                break;
            case RotationDirection.AntiClockWise:
                _distanceTravelled += _moveSpeed * Time.deltaTime;
                break;
        }

        _turretIndicatorPosition.position = _vertexPath.GetPointAtDistance(_distanceTravelled);
        _turretIndicatorPosition.up = _vertexPath.GetNormalAtDistance(_distanceTravelled);

        return _turretIndicatorPosition;
    }


    public void SetToStartingPosition()
    {
        Vector3 startingPosition = new Vector3(-0.3f, -10.3f, 0f);
        _distanceTravelled = _vertexPath.GetClosestDistanceAlongPath(startingPosition);
        _turretIndicatorPosition.position = _vertexPath.GetClosestPointOnPath(startingPosition);
        _turretIndicatorPosition.up = _vertexPath.GetNormalAtDistance(_distanceTravelled);
    }

    public void SetToPosition(Vector3 position)
    {
        _distanceTravelled = _vertexPath.GetClosestDistanceAlongPath(position);
        _turretIndicatorPosition.position = _vertexPath.GetClosestPointOnPath(position);
        _turretIndicatorPosition.up = _vertexPath.GetNormalAtDistance(_distanceTravelled);
    }
}
