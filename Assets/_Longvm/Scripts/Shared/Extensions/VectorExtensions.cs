using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 Divide(this Vector2 a, Vector2 b)
    {
        return new Vector2(a.x / b.x, a.y / b.y);
    }

    public static Vector3 Divide(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vector2 GetPerpendicular(this Vector2 v)
    {
        return new Vector2(v.y, -v.x);
    }

    public static float ShortestAngleFromRightVector(this Vector2 vector2)
    {
        return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * Mathf.Sign(vector2.x));
    }

    public static float SignedAngleFromRightVector(this Vector2 vector2)
    {
        return Mathf.Repeat(Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg, 360f);
    }
}
