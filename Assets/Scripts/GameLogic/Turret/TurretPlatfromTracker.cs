using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlatfromTracker
{
    private Platform _currentPlatform;
    private Turret _turret;
    private float moveSpeed = 4.0f;

    private PlatfromTrackData _platfromTrackData;

    public TurretPlatfromTracker(Turret _turret)
    {
        this._turret = _turret;
        _currentPlatform = GameObject.FindObjectOfType<Platform>();

        _platfromTrackData = new PlatfromTrackData();

        SwitchToPlatform(_currentPlatform);
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
                        Transform newMaxRightPoint = platform.GetMaxRightPoint();
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
                        Transform newMaxLeftPoint = platform.GetMaxLeftPoint();
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

        Debug.Log("SWITCHING TO PLATFORM " + platform.name);
        _currentPlatform = platform;


        Transform maxLeftPoint = platform.GetMaxLeftPoint();
        Transform maxRightPoint = platform.GetMaxRightPoint();
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
                newPos.x -= moveSpeed * Time.deltaTime;
                break;
            case RotationDirection.AntiClockWise:
                newPos.x += moveSpeed * Time.deltaTime;
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
