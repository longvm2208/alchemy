using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ListBase : MonoBehaviour
{
    [SerializeField] float paddingTop;
    [SerializeField] float paddingBottom;
    [SerializeField] float spacing;
    [SerializeField] float cullingDistance;
    [SerializeField] ScrollRect scroll;
    [SerializeField] HolderBase holderPrefab;
    [SerializeField] EntryBase entryPrefab;

    List<HolderBase> holders;

    public virtual void Init()
    {
        if (!holders.IsNullOrEmpty())
        {
            for (int i = 0; i < holders.Count; i++)
            {
                holders[i].Destroy();
            }

            holders.Clear();
        }

        holders ??= new List<HolderBase>();

        for (int i = 0; i < GetCount(); i++)
        {
            HolderBase holder = holderPrefab.GetInstance(scroll.content);
            holder.Init(i, this);
            holders.Add(holder);
        }

        Arrange();
        OnScrolling();
    }

    public EntryBase GetEntry(Transform parent)
    {
        return entryPrefab.GetInstance(parent, true);
    }

    void OnScrolling()
    {
        if (holders.IsNullOrEmpty()) return;
        for (int i = 0; i < holders.Count; i++)
        {
            holders[i].OnScrolling(scroll.viewport, cullingDistance);
        }
    }

    void Arrange()
    {
        int count = holders.Count;

        float contentHeight = 0f;
        for (int i = 0; i < count; i++)
        {
            contentHeight += holders[i].rectTransform.rect.height;
        }

        if (count > 1)
        {
            contentHeight += spacing * (count - 1);
        }

        float totalHeight = contentHeight + paddingTop + paddingBottom;

        scroll.content.SetSizeDeltaY(totalHeight);

        float currentY = totalHeight * 0.5f - paddingTop;

        for (int i = 0; i < count; i++)
        {
            RectTransform child = holders[i].rectTransform;
            float h = child.rect.height;
            float posY = currentY - h * 0.5f;
            child.SetAnchorPosY(posY);
            currentY -= h + spacing;
        }
    }

    protected abstract int GetCount();

    #region Event Listeners
    public void OnValueChanged(Vector2 _)
    {
        OnScrolling();
    }
    #endregion
}
