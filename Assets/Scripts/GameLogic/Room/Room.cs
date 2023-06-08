using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform[] _roomBounds;


    public Vector3 GetRandomSpawnPositionWithinRoomRange(float rangeScale = 1.0f)
    {
        List<Vector3> polygonRoom = new List<Vector3>();

        foreach (var bound in _roomBounds)
        {
            polygonRoom.Add(bound.position);
        }

        return GeneratePointInsidePolygon(polygonRoom, rangeScale);
    }

    public Vector3 GetRandomPositionInTopsideOfOctagon(float rangeScale = 1.0f)
    {
        Vector2[][] triangles = SubdivideOctagon();

        List<Vector3> polygonOctagon = new List<Vector3>();

        Vector2[] topSide = triangles[0];

        polygonOctagon.Add(topSide[0]);
        polygonOctagon.Add(topSide[1]);
        polygonOctagon.Add(topSide[2]);

#if UNITY_EDITOR
        Debug.DrawLine(topSide[0], topSide[1], Color.red, 2f);
        Debug.DrawLine(topSide[1], topSide[2], Color.red, 2f);
        Debug.DrawLine(topSide[2], topSide[0], Color.red, 2f);
#endif

        return GeneratePointInsidePolygon(polygonOctagon, rangeScale);
    }


    private Vector2[][] SubdivideOctagon()
    {
        int trianglesount = 8;
        int octagonVerts = trianglesount * 2;
        Vector2[] vertices = new Vector2[octagonVerts];

        //Got the value from checking the edges position of the octagon in the editor
        float radius = 10.4f;
        float fullCircle = Mathf.PI * 2;
        for (int i = 0; i < octagonVerts; i++)
        {
            float angle = fullCircle / trianglesount * i;

            vertices[i] = new Vector2(radius * Mathf.Sin(angle), radius * Mathf.Cos(angle));
        }

        // Define the vertices of the triangles
        Vector2[][] triangles = new Vector2[octagonVerts][];
        for (int i = 0; i < octagonVerts; i++)
        {
            triangles[i] = new Vector2[3];

            // Triangle vertex 1 is always the center of the octagon
            triangles[i][0] = Vector3.zero;

            // Triangle vertices 2 and 3 are two adjacent vertices of the octagon
            triangles[i][1] = vertices[i];
            triangles[i][2] = vertices[(i + 1) % octagonVerts];

            // #if UNITY_EDITOR
            //             Debug.DrawLine(triangles[i][0], triangles[i][1], Color.red, 2f);
            //             Debug.DrawLine(triangles[i][1], triangles[i][2], Color.red, 2f);
            //             Debug.DrawLine(triangles[i][2], triangles[i][0], Color.red, 2f);
            // #endif
        }

        return triangles;
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
