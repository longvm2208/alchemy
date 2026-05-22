using System.Collections.Generic;
using UnityEngine;

public enum LineMode
{
    Line,
    Ray,
    Segment
}

public class Geometry
{
    public static bool TryGetIntersection2D(
        Vector3 start1, Vector3 end1, LineMode mode1,
        Vector3 start2, Vector3 end2, LineMode mode2,
        out Vector3 intersection)
    {
        intersection = Vector3.zero;

        Vector3 dir1 = end1 - start1;
        Vector3 dir2 = end2 - start2;

        float denominator = (-dir1.x * dir2.y + dir1.y * dir2.x);

        if (Mathf.Abs(denominator) < 0.0001f) return false; // Lines are parallel

        Vector3 delta = start2 - start1;
        float t = (-dir2.y * delta.x + dir2.x * delta.y) / denominator;
        float u = (-dir1.y * delta.x + dir1.x * delta.y) / denominator;

        if (mode1 == LineMode.Ray && t < 0) return false;
        if (mode1 == LineMode.Segment && (t < 0 || t > 1)) return false;

        if (mode2 == LineMode.Ray && u < 0) return false;
        if (mode2 == LineMode.Segment && (u < 0 || u > 1)) return false;

        intersection = start1 + t * dir1;
        return true;
    }

    public static bool TryGetIntersection2D(
        Vector3 lineStart, Vector3 lineEnd, LineMode lineMode,
        Vector3 rectCenter, Vector3 rectSize, float rectRotation,
        out List<Vector3> intersections)
    {
        Vector3[] corners = GetRectCorners(rectCenter, rectSize, rectRotation);
        return TryGetIntersection2D(lineStart, lineEnd, lineMode, corners, out intersections);
    }

    public static bool TryGetIntersection2D(
        Vector3 lineStart, Vector3 lineEnd, LineMode lineMode,
        Vector3[] corners, out List<Vector3> intersections)
    {
        intersections = new List<Vector3>();

        for (int i = 0; i < 4; i++)
        {
            Vector3 edgeStart = corners[i];
            Vector3 edgeEnd = corners[(i + 1) % 4];

            if (TryGetIntersection2D(
                lineStart, lineEnd, lineMode,
                edgeStart, edgeEnd, LineMode.Segment,
                out Vector3 intersection))
            {
                if (ContainsPoint(intersections, intersection)) continue;
                intersections.Add(intersection);
            }
        }

        return intersections.Count > 0;
    }

    static bool ContainsPoint(List<Vector3> points, Vector3 point)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if ((points[i] - point).sqrMagnitude < 0.0001f)
            {
                return true;
            }
        }
        return false;
    }

    public static Vector3[] GetRectCorners(Vector3 center, Vector3 size, float rotation)
    {
        Vector3 halfSize = 0.5f * size;
        Quaternion rot = Quaternion.Euler(0, 0, rotation);
        return new Vector3[]
        {
            center + rot * new Vector3(-halfSize.x, halfSize.y, 0),
            center + rot * new Vector3(-halfSize.x, -halfSize.y, 0),
            center + rot * new Vector3(halfSize.x, -halfSize.y, 0),
            center + rot * new Vector3(halfSize.x, halfSize.y, 0)
        };
    }

    public static Vector3[] GetBoxCollider2DCorners(BoxCollider2D boxCollider2D, Transform transform)
    {
        Vector3 scale = transform.lossyScale;
        Vector3 size = boxCollider2D.size;
        Vector3 offset = boxCollider2D.offset;

        size = new Vector3(size.x * scale.x, size.y * scale.y, 0);
        offset = new Vector3(offset.x * scale.x, offset.y * scale.y, 0);

        // Get the transform's position and rotation
        Vector3 position = transform.TransformPoint(offset); // Account for the collider's offset
        Quaternion rotation = transform.rotation;

        // Calculate local corners
        Vector3 halfSize = size * 0.5f;
        Vector3[] corners = new Vector3[4]
        {
            new Vector3(-halfSize.x, halfSize.y),  // Top-left
            new Vector3(halfSize.x, halfSize.y),    // Top-right
            new Vector3(halfSize.x, -halfSize.y),  // Bottom-right
            new Vector3(-halfSize.x, -halfSize.y), // Bottom-left
        };

        // Transform local corners to world space
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = position + rotation * corners[i];
        }

        return corners;
    }

    public static bool TryProjectPointOnLine2D(
        Vector3 point,
        Vector3 lineStart, Vector3 lineEnd, LineMode lineMode,
        out Vector3 projected)
    {
        projected = Vector3.zero;

        Vector3 direction = lineEnd - lineStart;
        float sqrLength = direction.sqrMagnitude;

        if (Mathf.Approximately(sqrLength, 0f)) return false; // Line is a point

        Vector3 ap = point - lineStart;
        float t = Vector3.Dot(ap, direction) / sqrLength;

        if (lineMode == LineMode.Ray && t < 0) return false;
        if (lineMode == LineMode.Segment && (t < 0 || t > 1)) return false;

        projected = lineStart + t * direction;
        return true;
    }

    public static bool TryReflectPointOverLine2D(
        Vector3 point,
        Vector3 lineStart, Vector3 lineEnd, LineMode lineMode,
        out Vector3 reflected)
    {
        reflected = Vector3.zero;

        if (!TryProjectPointOnLine2D(point, lineStart, lineEnd, lineMode, out Vector3 projected))
        {
            return false;
        }

        reflected = 2 * projected - point;
        return true;
    }

    public static void SortClockwise(List<Vector2> points)
    {
        if (points == null || points.Count <= 2) return;

        Vector2 center = Vector2.zero;
        foreach (Vector2 point in points)
        {
            center += point;
        }
        center /= points.Count;

        int ComparePoints(Vector2 a, Vector2 b, Vector2 center)
        {
            float angleA = Mathf.Atan2(a.y - center.y, a.x - center.x);
            float angleB = Mathf.Atan2(b.y - center.y, b.x - center.x);

            return angleA.CompareTo(angleB);
        }

        points.Sort((a, b) => ComparePoints(a, b, center));
    }

    public static void SortCounterClockwise(List<Vector2> points)
    {
        if (points == null || points.Count <= 2) return;

        Vector2 center = Vector2.zero;
        foreach (Vector2 point in points)
        {
            center += point;
        }
        center /= points.Count;

        int ComparePoints(Vector2 a, Vector2 b, Vector2 center)
        {
            float angleA = Mathf.Atan2(a.y - center.y, a.x - center.x);
            float angleB = Mathf.Atan2(b.y - center.y, b.x - center.x);

            return angleB.CompareTo(angleA);
        }

        points.Sort((a, b) => ComparePoints(a, b, center));
    }
}
