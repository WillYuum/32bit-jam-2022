using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    [SerializeField] private Transform[] _connectingPoints;

    private Platform[] _connectedPlatforms;

    [SerializeField] public Transform TurretIndicatorPosition;

    private void Awake()
    {
        _connectedPlatforms = new Platform[2];

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
        // Vector3 turretPosition = turret.position;
        // Vector3 platformPosition = transform.position;
        // Vector3 direction = turretPosition - platformPosition;
        // direction.y = 0;
        // turret.rotation = Quaternion.LookRotation(direction);
    }

    // private void OnEndPointTriggerEnter(Collider2D other)
    // {
    //     //enable switching to another platform
    // }

    // private void OnEndPointTriggerExit(Collider2D other)
    // {
    //     //disable to switch to another platform
    // }


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
                    bool isOnRigthSide = connectingPoint.position.x > transform.position.x;
                    if (isOnRigthSide)
                    {
                        Debug.Log("isRight? " + connectingPoint.name);
                        _connectedPlatforms[0] = platform;
                    }
                    else
                    {
                        _connectedPlatforms[1] = platform;
                    }

                    // if (_connectedPlatforms[0] == null)
                    // {
                    //     Debug.Log(gameObject.name + " Connected to platform 0 with" + platform.name);
                    //     _connectedPlatforms[0] = platform;
                    // }
                    // else if (_connectedPlatforms[1] == null)
                    // {
                    //     Debug.Log(gameObject.name + " Connected to platform 1 with" + platform.name);
                    //     _connectedPlatforms[1] = platform;
                    // }
                }
            }
        }
    }


    public Platform GetConnectingPlatform(TurretMovement side)
    {
        if (side == TurretMovement.ClockWise)
        {
            return _connectedPlatforms[0];
        }
        else
        {
            return _connectedPlatforms[1];
        }
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
