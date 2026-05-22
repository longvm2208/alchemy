using UnityEngine;

public abstract class ExtendedMonoBehaviour : MonoBehaviour
{
    Transform _transform;
    public new Transform transform
    {
        get
        {
            if (_transform == null) _transform = base.transform;
            return _transform;
        }
    }

    RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null) _rectTransform = base.transform as RectTransform;
            return _rectTransform;
        }
    }
}
