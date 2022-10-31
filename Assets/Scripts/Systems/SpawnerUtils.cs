using UnityEngine;

public static class SpawnerUtils
{
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