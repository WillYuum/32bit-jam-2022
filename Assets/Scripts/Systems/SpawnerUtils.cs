using UnityEngine;

public static class SpawnerUtils
{

    public static Vector2[] GetPositionsAroundObject(Vector2 objectPosition, float radius, int numberOfPoints, float startAngle = 0)
    {
        Vector2[] positions = new Vector2[numberOfPoints];
        float angle = 0;
        float angleStep = 360f / numberOfPoints;
        for (int i = 0; i < numberOfPoints; i++)
        {
            positions[i] = objectPosition + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            angle += angleStep;
        }
        return positions;
    }

    public static void PositionAroundObject(Vector2 objectToSpawnAround, Transform[] transforms, float radius)
    {
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i] != null)
            {
                Transform transform = transforms[i];
                Vector2 randomPoint = Random.insideUnitCircle * radius;
                transform.position = objectToSpawnAround + randomPoint;
            }
        }
    }

    public static void SpawnInCirclePattern(Transform[] objects, Vector3 spawnPoint)
    {
        int amountToSpawned = objects.Length;

        float angle = 0;
        float angleStep = 360 / amountToSpawned;

        for (int i = 0; i < amountToSpawned; i++)
        {
            if (objects[i] != null)
            {
                float x = Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = Mathf.Sin(angle * Mathf.Deg2Rad);

                Vector3 dir = spawnPoint + new Vector3(x, y, 0);
                objects[i].position = dir;

                angle += angleStep;
            }
        }
    }


    public static Vector3 GetCenterOfObjects(Transform[] objects, Vector3 center)
    {
        int amountOfObjects = objects.Length;
        Vector3 centerOfSwarm = Vector3.zero;

        for (int i = 0; i < amountOfObjects; i++)
        {
            if (objects[i] != null)
            {
                centerOfSwarm += objects[i].position;
            }
        }

        return (centerOfSwarm + center) / amountOfObjects;
    }
}