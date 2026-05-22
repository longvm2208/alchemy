using UnityEngine;

public class PathGenerator
{
    #region CIRCLE
    public static Vector2[] Circle(Vector2 center, float radius, int amount)
    {
        Vector2[] path = new Vector2[amount];
        float step = Mathf.PI * 2f / amount;
        for (int i = 0; i < amount; i++)
        {
            float angle = i * step;
            path[i] = center + radius * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        return path;
    }

    public static Vector3[] CircleYZ(Vector3 center, float radius, int amount)
    {
        Vector3[] path = new Vector3[amount];
        float step = Mathf.PI * 2f / amount;
        for (int i = 0; i < amount; i++)
        {
            float angle = i * step;
            path[i] = center + radius * new Vector3(0f, Mathf.Cos(angle), Mathf.Sin(angle));
        }
        return path;
    }

    public static Vector3[] CircleZX(Vector3 center, float radius, int amount)
    {
        Vector3[] path = new Vector3[amount];
        float step = Mathf.PI * 2f / amount;
        for (int i = 0; i < amount; i++)
        {
            float angle = i * step;
            path[i] = center + radius * new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
        }
        return path;
    }

    public static Vector3[] CircleXY(Vector3 center, float radius, int amount)
    {
        Vector3[] path = new Vector3[amount];
        float step = Mathf.PI * 2f / amount;
        for (int i = 0; i < amount; i++)
        {
            float angle = i * step;
            path[i] = center + radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
        }
        return path;
    }
    #endregion

    #region ELLIPSE
    public static Vector2[] Ellipse(Vector2 center, float width, float height, int amount)
    {
        Vector2[] path = new Vector2[amount];
        float step = Mathf.PI * 2f / amount;

        for (int i = 0; i < amount; i++)
        {
            float angle = i * step;
            float x = Mathf.Cos(angle) * width;
            float y = Mathf.Sin(angle) * height;
            path[i] = center + new Vector2(x, y);
        }

        return path;
    }
    #endregion

    #region BEZIER
    public static Vector3[] QuadraticBezier(int amount, params Vector3[] points)
    {
        Vector3[] path = new Vector3[amount];
        int a = amount - 1;
        for (int i = 0; i < amount; i++)
        {
            float time = (float)i / a;
            float t = 1 - time;
            path[i] = t * t * points[0] + 2 * t * time * points[1] + time * time * points[2];
        }
        return path;
    }

    public static Vector3[] CubicBezier(int amount, params Vector3[] points)
    {
        Vector3[] path = new Vector3[amount];
        int a = amount - 1;
        for (int i = 0; i < amount; i++)
        {
            float time = (float)i / a;
            float t = 1 - time;
            path[i] = t * t * t * points[0] + 3 * t * t * time * points[1] +
                3 * t * time * time * points[2] + time * time * time * points[3];
        }
        return path;
    }
    #endregion

    #region CURVE
    /// <summary>
    /// Generates a quadratic Bezier curve with specified parameters.
    /// </summary>
    /// <param name="start">The starting point of the curve.</param>
    /// <param name="end">The ending point of the curve.</param>
    /// <param name="amount">The number of points to generate on the curve.</param>
    /// <param name="side">
    /// The orientation of the curve in relation to the line formed by the 'start' and 'end' points.
    /// Use a positive value (1) for a curve to the right and a negative value (-1) for a curve to the left.
    /// </param>
    /// <param name="sideWeight">The weight controlling the influence of the perpendicular direction on the curve.</param>
    /// <param name="centerWeight">The weight controlling the position of the control point along the line.</param>
    /// <returns>An array of points representing the quadratic Bezier curve.</returns>
    public static Vector3[] SideCurveXY(Vector3 start, Vector3 end,
        int amount, int side, float sideWeight, float centerWeight = 0.5f)
    {
        if (amount < 3)
        {
            amount = 3;
            Debug.LogWarning("The minimum value of amount must be 3");
        }
        side = side >= 0 ? 1 : -1;
        sideWeight *= Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = new Vector3(side * direction.y, -side * direction.x, direction.z);
        Vector3 center = (1 - centerWeight) * start + centerWeight * end;
        Vector3 control = center + sideWeight * perpendicular;
        return QuadraticBezier(amount, start, control, end);
    }

    public static Vector3[] SankCurveXY(Vector3 start, Vector3 end,
        int amount, float downWeight, float centerWeight = 0.65f)
    {
        if (amount < 3)
        {
            amount = 3;
            Debug.LogWarning("The minimum value of amount must be 3");
        }
        downWeight *= Vector3.Distance(start, end);
        Vector3 corner = new Vector3(end.x, start.y, start.z);
        Vector3 direction = (corner - start).normalized;
        Vector3 perpendicular = new Vector3(direction.y, -direction.x, direction.z);
        Vector3 center = (1 - centerWeight) * start + centerWeight * end;
        Vector3 control = center + downWeight * perpendicular;
        return QuadraticBezier(amount, start, control, end);
    }
    #endregion
}
