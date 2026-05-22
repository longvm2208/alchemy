using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetItemMenu : MonoBehaviour
{
    [SerializeField] float downWeight;
    [SerializeField] ScrollRect scroll;
    [SerializeField] HorizontalLayout horizontalLayout;
    [SerializeField] TargetItemView itemViewPrefab;
    [SerializeField] TargetItemFly itemFlyPrefab;
    [SerializeField] RectTransform itemFlyHolder;

    List<TargetItemView> itemViews = new();

    LevelTarget[] targets => LevelManager.Ins.CurrentLevel.Targets;

    public void Init()
    {
        if (!itemViews.IsNullOrEmpty())
        {
            for (int i = 0; i < itemViews.Count; i++)
            {
                itemViews[i].Destroy();
            }
            itemViews.Clear();
        }

        itemViews ??= new List<TargetItemView>();

        for (int i = 0; i < targets.Length; i++)
        {
            var itemView = itemViewPrefab.GetInstance(scroll.content);
            itemView.Init(targets[i]);
            itemViews.Add(itemView);
        }

        horizontalLayout.UpdateLayout();
    }

    public void CollectTarget(ItemId id, Vector3 pos)
    {
        int index = -1;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].Id == id)
            {
                index = i;
            }
        }

        if (index == -1) return;

        TargetItemFly itemFly = itemFlyPrefab.GetInstance(itemFlyHolder);
        itemFly.Init(id, pos);
        itemFly.Move(itemViews[index].transform.position, () =>
        {
            itemFly.Destroy();
            itemViews[index].UpdateProgress();
        });
    }
}
