using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlatfromTracker
{
    private Platform _currentPlatform;
    private Turret _turret;
    private float _moveSpeed = 7.0f;

    private PlatfromTrackData _platfromTrackData;

    public TurretPlatfromTracker(Turret turret, Platform startingPlatform)
    {
        _moveSpeed = GameVariables.instance.PlayerSpeed;

        _turret = turret;
        _currentPlatform = startingPlatform;

        _platfromTrackData = new PlatfromTrackData();

        SwitchToPlatform(_currentPlatform);
        TrackTurretOnPlatform();
    }



    public void TrackTurretOnPlatform()
    {
        Vector2 turretPositionIndicator = _platfromTrackData.TurretIndicatorPosition.localPosition;
        Vector2 maxLeftPoint = _platfromTrackData.MaxClockwisePoint.localPosition;
        Vector2 maxRightPoint = _platfromTrackData.MaxAntiClockwisePoint.localPosition;

        turretPositionIndicator.y = maxLeftPoint.y;

        switch (_turret.MoveDirection)
        {
            case TurretMoveDirection.ClockWise:
                if (Mathf.Abs(maxLeftPoint.x - turretPositionIndicator.x) < 0.1f)
                {
                    var platform = _currentPlatform.GetConnectingPlatform(RotationDirection.ClockWise);
                    if (platform != null)
                    {
                        SwitchToPlatform(platform);
                        Transform newMaxRightPoint = platform.GetAntiClockWisePoint();
                        _platfromTrackData.TurretIndicatorPosition.position = newMaxRightPoint.position;
                    }
                }
                break;

            case TurretMoveDirection.AntiClockWise:
                if (Mathf.Abs(maxRightPoint.x - turretPositionIndicator.x) < 0.1f)
                {
                    var platform = _currentPlatform.GetConnectingPlatform(RotationDirection.AntiClockWise);
                    if (platform != null)
                    {
                        SwitchToPlatform(platform);
                        Transform newMaxLeftPoint = platform.GetClickWisePoint();
                        _platfromTrackData.TurretIndicatorPosition.position = newMaxLeftPoint.position;

                    }
                }
                break;

            default:
                break;
        }


    }

    private void SwitchToPlatform(Platform platform)
    {
        if (platform == null) return;

        // Debug.Log("SWITCHING TO PLATFORM " + platform.name);
        _currentPlatform = platform;


        Transform maxLeftPoint = platform.GetClickWisePoint();
        Transform maxRightPoint = platform.GetAntiClockWisePoint();
        Transform turretIndicatorPosition = platform.TurretIndicatorPosition;


        _platfromTrackData.SetPoints(maxLeftPoint, maxRightPoint, turretIndicatorPosition);

        //Make turrent face the platform
        platform.KeepTurruetPerpendicularyAligned(_turret.transform);
    }

    public Vector2 MoveIndicator(RotationDirection direction)
    {
        Transform turretIndicatorPosition = _platfromTrackData.TurretIndicatorPosition;
        Vector2 newPos = turretIndicatorPosition.localPosition;
        switch (direction)
        {
            case RotationDirection.ClockWise:
                newPos.x -= _moveSpeed * Time.deltaTime;
                break;
            case RotationDirection.AntiClockWise:
                newPos.x += _moveSpeed * Time.deltaTime;
                break;
        }

        newPos.x = Mathf.Clamp(newPos.x, _platfromTrackData.MaxClockwisePoint.localPosition.x, _platfromTrackData.MaxAntiClockwisePoint.localPosition.x);
        turretIndicatorPosition.localPosition = newPos;

        return turretIndicatorPosition.position;
    }
}

public class PlatfromTrackData
{
    public Transform MaxClockwisePoint { get; private set; }
    public Transform MaxAntiClockwisePoint { get; private set; }
    public Transform TurretIndicatorPosition { get; private set; }


    public void SetPoints(Transform maxLeftPoint, Transform maxRightPoint, Transform turretIndicatorPosition)
    {
        MaxClockwisePoint = maxLeftPoint;
        MaxAntiClockwisePoint = maxRightPoint;
        TurretIndicatorPosition = turretIndicatorPosition;
    }
}
