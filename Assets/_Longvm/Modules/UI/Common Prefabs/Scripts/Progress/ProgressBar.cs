using UnityEngine;

public class ProgressBar : ExtendedMonoBehaviour
{
    enum Direction
    {
        Horizontal,
        Vertical
    }

    [SerializeField] Direction dir;
    [SerializeField] float size;
    public float Size => size;
    [SerializeField] RectTransform mask;
    [SerializeField] RectTransform fill;

    public void SetSize(float size)
    {
        this.size = size;

        switch (dir)
        {
            case Direction.Horizontal:
                rectTransform.SetSizeDeltaX(size);
                fill.SetSizeDeltaX(size);
                break;
            case Direction.Vertical:
                rectTransform.SetSizeDeltaY(size);
                fill.SetSizeDeltaY(size);
                break;
        }
    }

    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);

        switch (dir)
        {
            case Direction.Horizontal:
                mask.SetSizeDeltaX(progress * size);
                break;
            case Direction.Vertical:
                mask.SetSizeDeltaY(progress * size);
                break;
        }
    }
}
