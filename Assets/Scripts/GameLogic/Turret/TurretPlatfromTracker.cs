using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlatfromTracker
{
    private Platform _currentPlatform;
    private Turret _turret;

    private Transform _turretIndicatorPosition;
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
        Vector2 turretPosition = _platfromTrackData.TurretIndicatorPosition.position;
        Vector2 maxLeftPoint = _platfromTrackData.MaxLeftPoint.position;
        Vector2 maxRightPoint = _platfromTrackData.MaxLeftPoint.position;

        turretPosition.y = maxLeftPoint.y;

        // Debug.Log("Turret position " + Vector2.Distance(maxRightPoint, turretPosition));
        Debug.Log("turretPosition " + turretPosition.x);
        if (Mathf.Abs(maxLeftPoint.x - turretPosition.x) < 0.1f)
        {
            // _currentPlatform = _currentPlatform.GetConnectedPlatform(0);
            SwitchToPlatform(_currentPlatform.GetConnectingPlatform(TurretMovement.ClockWise));
        }
        else if (Mathf.Abs(maxRightPoint.x - turretPosition.x) < 0.1f)
        {
            SwitchToPlatform(_currentPlatform.GetConnectingPlatform(TurretMovement.AntiClockWise));
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

    public Vector2 MoveIndicator(TurretMovement direction)
    {
        Transform turretIndicatorPosition = _platfromTrackData.TurretIndicatorPosition;
        Vector2 newPos = turretIndicatorPosition.localPosition;
        switch (direction)
        {
            case TurretMovement.ClockWise:
                newPos.x -= moveSpeed * Time.deltaTime;
                break;
            case TurretMovement.AntiClockWise:
                newPos.x += moveSpeed * Time.deltaTime;
                break;
        }

        newPos.x = Mathf.Clamp(newPos.x, _platfromTrackData.MaxLeftPoint.localPosition.x, _platfromTrackData.MaxRightPoint.localPosition.x);
        turretIndicatorPosition.localPosition = newPos;

        return turretIndicatorPosition.position;
    }
}

public class PlatfromTrackData
{
    public Transform MaxLeftPoint { get; private set; }
    public Transform MaxRightPoint { get; private set; }
    public Transform TurretIndicatorPosition { get; private set; }


    public void SetPoints(Transform maxLeftPoint, Transform maxRightPoint, Transform turretIndicatorPosition)
    {
        MaxLeftPoint = maxLeftPoint;
        MaxRightPoint = maxRightPoint;
        TurretIndicatorPosition = turretIndicatorPosition;
    }
}
