using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    [SerializeField] private Transform[] _connectingPoints;

    private ConnectedPlatforms _connectedPlatforms;

    [SerializeField] public Transform TurretIndicatorPosition;

    private void Awake()
    {
        _connectedPlatforms = new ConnectedPlatforms();

        // foreach (var endPoint in _endPoints)
        // {
        //     endPoint._OnTriggerEnter += OnEndPointTriggerEnter;
        //     endPoint._OnTriggerExit += OnEndPointTriggerExit;
        // }


        ConnectToNeighborPlatforms();
    }

    public void KeepTurruetPerpendicularyAligned(Transform turret)
    {
        turret.up = transform.up;
    }


    private void ConnectToNeighborPlatforms()
    {
        foreach (var connectingPoint in _connectingPoints)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(connectingPoint.position, 0.1f, LayerMask.GetMask("Movement_Platform"));
            foreach (var collider in colliders)
            {
                Platform platform = collider.GetComponent<Platform>();
                if (platform != null && platform != this)
                {
                    RotationDirection direction = connectingPoint.localPosition.x > 0 ? RotationDirection.AntiClockWise : RotationDirection.ClockWise;
                    Debug.Log("CONNECTING PLATFORM " + platform.name + " TO " + this.name + " IN DIRECTION " + direction);
                    _connectedPlatforms.SetPlatform(direction, platform);

                }
            }
        }
    }


    public Platform GetConnectingPlatform(RotationDirection side)
    {
        var platform = _connectedPlatforms.GetPlatform(side);
        return platform != null ? platform : null;
    }


    public Transform GetMaxLeftPoint()
    {
        return _connectingPoints[0].transform;
    }

    public Transform GetMaxRightPoint()
    {
        return _connectingPoints[1].transform;
    }

}

public class ConnectedPlatforms
{
    public Platform LeftPlatform { get; private set; }
    public Platform RightPlatform { get; private set; }


    public void SetPlatform(RotationDirection side, Platform platform)
    {
        switch (side)
        {
            case RotationDirection.ClockWise:
                LeftPlatform = platform;
                break;
            case RotationDirection.AntiClockWise:
                RightPlatform = platform;
                break;
            default:
                Debug.LogError("INVALID SIDE");
                break;
        }
    }

    public Platform GetPlatform(RotationDirection side)
    {
        switch (side)
        {
            case RotationDirection.ClockWise:
                return LeftPlatform;
            case RotationDirection.AntiClockWise:
                return RightPlatform;
            default:
                return null;
        }
    }
}
