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


    public Vector3 GetRandomSpawnPositionWithinRoomRange(float rangeScale = 1.0f)
    {
        List<Vector3> polygonRoom = new List<Vector3>();

        foreach (var platform in _platforms)
        {
            polygonRoom.Add(platform.transform.position);
        }

        return GeneratePointInsidePolygon(polygonRoom, rangeScale);
    }

    private Vector3 GeneratePointInsidePolygon(List<Vector3> polygon, float rangeScale)
    {
        Vector3 MinVec = MinPointOnThePolygon(polygon);
        Vector3 MaxVec = MaxPointOnThePolygon(polygon);
        Vector3 GenVector;

        float x = ((Random.value) * (MaxVec.x - MinVec.x)) + MinVec.x;
        float y = ((Random.value) * (MaxVec.y - MinVec.y)) + MinVec.y;
        GenVector = new Vector3(x, y, transform.position.z);

        while (!IsPointInPolygon(polygon, GenVector))
        {
            x = ((Random.value) * (MaxVec.x - MinVec.x)) + MinVec.x;
            y = ((Random.value) * (MaxVec.y - MinVec.y)) + MinVec.y;
            GenVector.x = x;
            GenVector.y = y;
        }

        if (rangeScale > 1 || rangeScale <= 0)
        {
            return GenVector;
        }
        else
        {
            return GenVector * rangeScale;
        }

    }

    private Vector3 MinPointOnThePolygon(List<Vector3> polygon)
    {
        float minX = polygon[0].x;
        float minY = polygon[0].y;
        for (int i = 1; i < polygon.Count; i++)
        {
            if (minX > polygon[i].x)
            {
                minX = polygon[i].x;
            }
            if (minY > polygon[i].y)
            {
                minY = polygon[i].y;
            }
        }
        return new Vector3(minX, minY, transform.position.z);
    }

    private Vector3 MaxPointOnThePolygon(List<Vector3> polygon)
    {
        float maxX = polygon[0].x;
        float maxY = polygon[0].y;
        for (int i = 1; i < polygon.Count; i++)
        {
            if (maxX < polygon[i].x)
            {
                maxX = polygon[i].x;
            }
            if (maxY < polygon[i].y)
            {
                maxY = polygon[i].y;
            }
        }
        return new Vector3(maxX, maxY, transform.position.z);
    }

    private bool IsPointInPolygon(List<Vector3> polygon, Vector3 point)
    {
        bool isInside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            if (((polygon[i].x > point.x) != (polygon[j].x > point.x)) &&
            (point.y < (polygon[j].y - polygon[i].y) * (point.x - polygon[i].x) / (polygon[j].x - polygon[i].x) + polygon[i].y))
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }

}
