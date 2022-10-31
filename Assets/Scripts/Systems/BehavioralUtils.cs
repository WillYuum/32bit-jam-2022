using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BehavioralUtils
{
    public static void RotateAroundObject(Transform[] objects, Vector3 center, float speed)
    {
        int amountOfObjects = objects.Length;
        for (int i = 0; i < amountOfObjects; i++)
        {
            Transform obj = objects[i];
            if (obj != null)
            {
                objects[i].RotateAround(center, Vector3.forward, speed * Time.deltaTime);
            }
        }
    }


    public static void MoveOutCirclePattern(Transform[] objects, float speed)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            var obj = objects[i];
            var angle = i * Mathf.PI * 2 / objects.Length;
            var x = Mathf.Cos(angle);
            var y = Mathf.Sin(angle);
            var dir = new Vector3(x, y, 0);
            obj.position += dir * speed * Time.deltaTime;
        }
    }
}
