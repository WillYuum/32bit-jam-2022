using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] public Transform CameraPoint;
    [SerializeField] private Transform _platformHolder;
    private Platform[] _platforms;


    public void UpdatePlatformNeighbors()
    {
        if (_platforms == null)
        {
            GetAllPlatforms();
        }


        foreach (var platform in _platforms)
        {
            platform.ConnectToNeighborPlatforms();
        }
    }


    private void GetAllPlatforms()
    {
        _platforms = _platformHolder.GetComponentsInChildren<Platform>();
    }

}
