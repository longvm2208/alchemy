using UnityEngine;

public static class TransformExtensions
{
    static readonly Vector3[] cornersA = new Vector3[4];
    static readonly Vector3[] cornersB = new Vector3[4];

    #region POSITION
    public static void SetPositionX(this Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public static void SetPositionY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public static void SetPositionZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    public static void SetLocalPositionX(this Transform transform, float x)
    {
        transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
    }

    public static void SetLocalPositionY(this Transform transform, float y)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
    }

    public static void SetLocalPositionZ(this Transform transform, float z)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
    }
    #endregion

    #region ROTATION
    public static void SetRotationX(this Transform transform, float x)
    {
        transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public static void SetRotationY(this Transform transform, float y)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
    }

    public static void SetRotationZ(this Transform transform, float z)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, z);
    }

    public static void SetLocalRotationX(this Transform transform, float x)
    {
        transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    public static void SetLocalRotationY(this Transform transform, float y)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
    }

    public static void SetLocalRotationZ(this Transform transform, float z)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
    }
    #endregion

    #region SCALE
    public static void SetLocalScaleX(this Transform transform, float x)
    {
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }

    public static void SetLocalScaleY(this Transform transform, float y)
    {
        transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
    }

    public static void SetLocalScaleZ(this Transform transform, float z)
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
    }
    #endregion

    public static void DestroyChildren(this Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (Application.isPlaying)
            {
                Object.Destroy(child.gameObject);
            }
            else
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    #region ANCHOR POS
    public static void SetAnchorPosX(this RectTransform rt, float x)
    {
        rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y);
    }

    public static void SetAnchorPosY(this RectTransform rt, float y)
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
    }
    #endregion

    #region ANCHOR MIN
    public static void SetAnchorMinX(this RectTransform rt, float x)
    {
        rt.anchorMin = new Vector2(x, rt.anchorMin.y);
    }

    public static void SetAnchorMinY(this RectTransform rt, float y)
    {
        rt.anchorMin = new Vector2(rt.anchorMin.x, y);
    }
    #endregion

    #region ANCHOR MAX
    public static void SetAnchorMaxX(this RectTransform rt, float x)
    {
        rt.anchorMax = new Vector2(x, rt.anchorMax.y);
    }

    public static void SetAnchorMaxY(this RectTransform rt, float y)
    {
        rt.anchorMax = new Vector2(rt.anchorMax.x, y);
    }
    #endregion

    #region OFFSET
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }
    #endregion

    #region SIZE DELTA
    public static void SetSizeDeltaX(this RectTransform rt, float x)
    {
        rt.sizeDelta = new Vector2(x, rt.sizeDelta.y);
    }

    public static void SetSizeDeltaY(this RectTransform rt, float y)
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, y);
    }
    #endregion

    public static bool IsFullyInside(this RectTransform target, RectTransform container, Camera uiCamera = null)
    {
        target.GetWorldCorners(cornersA);

        for (int i = 0; i < 4; i++)
        {
            Vector2 localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                container,
                RectTransformUtility.WorldToScreenPoint(uiCamera, cornersA[i]),
                uiCamera,
                out localPoint
            );

            if (!container.rect.Contains(localPoint))
            {
                return false;
            }
        }

        return true;
    }

    public static bool Overlaps(this RectTransform a, RectTransform b)
    {
        a.GetWorldCorners(cornersA);
        b.GetWorldCorners(cornersB);

        Rect rectA = GetScreenRect(cornersA);
        Rect rectB = GetScreenRect(cornersB);

        return rectA.Overlaps(rectB);
    }

    static Rect GetScreenRect(Vector3[] corners)
    {
        float minX = corners[0].x;
        float minY = corners[0].y;
        float maxX = corners[0].x;
        float maxY = corners[0].y;

        for (int i = 1; i < 4; i++)
        {
            Vector3 corner = corners[i];

            minX = Mathf.Min(minX, corner.x);
            minY = Mathf.Min(minY, corner.y);
            maxX = Mathf.Max(maxX, corner.x);
            maxY = Mathf.Max(maxY, corner.y);
        }

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }

    public static Vector2 GetRandomPoint(this RectTransform rect, float padding)
    {
        return new Vector2(
            Random.Range(rect.rect.xMin + padding, rect.rect.xMax - padding),
            Random.Range(rect.rect.yMin + padding, rect.rect.yMax - padding)
        );
    }
}
