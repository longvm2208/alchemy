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
    [SerializeField] Transform itemFlyHolder;

    Dictionary<CategoryId, TargetItemView> categoryDict;

    LevelTarget[] targets => LevelManager.Ins.CurrentLevel.Targets;

    public void Init()
    {
        if (!categoryDict.IsNullOrEmpty())
        {
            foreach (var targetView in categoryDict.Values)
            {
                targetView.Destroy();
            }
            categoryDict.Clear();
        }

        categoryDict ??= new Dictionary<CategoryId, TargetItemView>();

        for (int i = 0; i < targets.Length; i++)
        {
            var itemView = itemViewPrefab.GetInstance(scroll.content);
            itemView.Init(targets[i]);
            categoryDict[targets[i].CategoryId] = itemView;
        }

        horizontalLayout.UpdateLayout();
    }

    public void CollectTarget(ItemId itemId, CategoryId categoryId, Vector3 pos)
    {
        this.Invoke(0.25f, () =>
        {
            TargetItemFly itemFly = itemFlyPrefab.GetInstance(itemFlyHolder);
            itemFly.Init(itemId, pos);
            itemFly.Move(categoryDict[categoryId].transform.position, () =>
            {
                itemFly.Destroy();
                categoryDict[categoryId].UpdateProgress();
            });
        });
    }
}
