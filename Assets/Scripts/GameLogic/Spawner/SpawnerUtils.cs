using UnityEngine;


namespace GameLogic.Spawner.Utils
{
    public static class SpawnerUtils
    {

        public static Vector2[] GetPositionsAroundObject(Vector2 objectPosition, float radius, int numberOfPoints, float startAngle = 0)
        {
            Vector2[] positions = new Vector2[numberOfPoints];
            float angle = startAngle;
            float angleStep = 360f / numberOfPoints;
            for (int i = 0; i < numberOfPoints; i++)
            {
                positions[i] = objectPosition + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
                angle += angleStep;
            }
            return positions;
        }

        public static void PositionInCirclePattern(Transform[] objects, Vector3 spawnPoint)
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



        public static void RemoveNullsFromArray<T>(ref T[] array)
        {
            int nonNullCount = 0;
            int arrayLength = array.Length;

            // Count the non-null elements
            for (int i = 0; i < arrayLength; i++)
            {
                if (array[i] != null)
                {
                    nonNullCount++;
                }
            }

            // Create a new array with the correct size
            T[] newArray = new T[nonNullCount];

            // Copy the non-null elements to the new array
            int newIndex = 0;
            for (int i = 0; i < arrayLength; i++)
            {
                if (array[i] != null)
                {
                    newArray[newIndex] = array[i];
                    newIndex++;
                }
            }

            // Update the reference to the new array
            array = newArray;
        }
        public static T[] RemoveNullsFromArray<T>(T[] array)
        {
            int nonNullCount = 0;
            int arrayLength = array.Length;

            // Count the non-null elements
            for (int i = 0; i < arrayLength; i++)
            {
                if (array[i] != null)
                {
                    nonNullCount++;
                }
            }

            // Create a new array with the correct size
            T[] newArray = new T[nonNullCount];

            // Copy the non-null elements to the new array
            int newIndex = 0;
            for (int i = 0; i < arrayLength; i++)
            {
                if (array[i] != null)
                {
                    newArray[newIndex] = array[i];
                    newIndex++;
                }
            }

            // Update the reference to the new array
            return newArray;
        }


        public static bool IsArrayIsFullOfNulls<T>(T[] array)
        {
            int arrayLength = array.Length;
            for (int i = 0; i < arrayLength; i++)
            {
                if (array[i] != null)
                {
                    return false;
                }
            }
            return true;
        }

    }
}