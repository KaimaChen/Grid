using UnityEngine;

public static class Utils
{
    public static Vector2 Lerp(Vector2Int v0, Vector2Int v1, float t)
    {
        float x = Mathf.Lerp(v0.x, v1.x, t);
        float y = Mathf.Lerp(v0.y, v1.y, t);
        return new Vector2(x, y);
    }
}