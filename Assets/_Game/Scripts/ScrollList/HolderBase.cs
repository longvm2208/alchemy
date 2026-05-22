using UnityEngine;

public abstract class HolderBase : PooledMonoBehaviour<HolderBase>
{
    int index;
    ListBase list;
    EntryBase entry;

    public void Init(int index, ListBase list)
    {
        this.index = index;
        this.list = list;
    }

    public void OnScrolling(RectTransform viewport, float cullingDistance)
    {
        Vector3 localPos = viewport.InverseTransformPoint(transform.position);

        if (Mathf.Abs(localPos.y) < cullingDistance)
        {
            if (entry == null)
            {
                entry = list.GetEntry(transform);
                entry.Init(index);
                entry.rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        else
        {
            if (entry != null)
            {
                entry.Destroy();
                entry = null;
            }
        }
    }

    public void Destroy()
    {
        if (entry != null)
        {
            entry.Destroy();
            entry = null;
        }

        Recycle();
    }
}
