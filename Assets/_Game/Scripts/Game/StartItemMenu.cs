using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartItemMenu : MonoBehaviour
{
    [SerializeField] ScrollRect scroll;
    [SerializeField] HorizontalGridLayout horizontalGridLayout;
    [SerializeField] StartItemView itemViewPrefab;

    List<StartItemView> itemViews = new();

    public void Init()
    {
        ItemId[] itemIds = LevelManager.Ins.CurrentLevel.StartItems;

        if (!itemViews.IsNullOrEmpty())
        {
            for (int i = 0; i < itemViews.Count; i++)
            {
                itemViews[i].Destroy();
            }
            itemViews.Clear();
        }

        itemViews ??= new List<StartItemView>();

        for (int i = 0; i < itemIds.Length; i++)
        {
            StartItemView itemView = itemViewPrefab.GetInstance(scroll.content);
            itemView.Init(itemIds[i], this);
            itemViews.Add(itemView);
        }

        horizontalGridLayout.UpdateLayout();
    }

    public void AddItem(ItemId id)
    {
        StartItemView itemView = itemViewPrefab.GetInstance(scroll.content);
        itemView.Init(id, this);
        itemView.TweenScale();
        itemViews.Add(itemView);

        horizontalGridLayout.UpdateLayout();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        scroll.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        scroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scroll.OnEndDrag(eventData);
    }
}
