using UnityEngine;

public static class DebugDraw
{
    public static void DrawBox(Vector3 center, Vector3 size, Color color, Quaternion rotation, float duration = 0, bool depthTest = true)
    {
        Vector3 halfSize = size * 0.5f;
        Vector3 topLeft = center + (rotation * new Vector2(-halfSize.x, halfSize.y));
        Vector3 topRight = center + (rotation * new Vector2(halfSize.x, halfSize.y));
        Vector3 bottomLeft = center + (rotation * new Vector2(-halfSize.x, -halfSize.y));
        Vector3 bottomRight = center + (rotation * new Vector2(halfSize.x, -halfSize.y));

        Debug.DrawLine(topLeft, topRight, color, duration, depthTest);
        Debug.DrawLine(topRight, bottomRight, color, duration, depthTest);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration, depthTest);
        Debug.DrawLine(bottomLeft, topLeft, color, duration, depthTest);
    }
}