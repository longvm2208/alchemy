using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TargetState
{
    public int Count;
    public HashSet<ItemId> Collected;
}

public class BoardManager : SingletonMonoBehaviour<BoardManager>
{
    [SerializeField] float mergeRadiusSqr;
    [SerializeField] TargetItemMenu targetItemMenu;
    [SerializeField] StartItemMenu startItemMenu;
    [SerializeField] ItemView itemViewPrefab;
    [SerializeField] RectTransform mergeBoard;
    [SerializeField] RectTransform bottomAnchor;
    [SerializeField] RectTransform bottomFollower;
    [SerializeField] Canvas canvas;
    public float ScaleFactor => canvas.scaleFactor;

    List<ItemView> itemViews;
    public List<ItemView> ItemViews => itemViews;
    //Dictionary<ItemId, int> targetDict;
    //public Dictionary<ItemId, int> TargetDict => targetDict;
    //Dictionary<CategoryKey, TargetState> targetDict;
    //public Dictionary<CategoryKey, TargetState> TargetDict => targetDict;
    HashSet<ItemId> newItemIds;

    public void UpdateBoardPosAndSize()
    {
        bottomFollower.position = bottomAnchor.position;
        float height = mergeBoard.anchoredPosition.y - bottomFollower.anchoredPosition.y;
        mergeBoard.SetSizeDeltaY(height);
    }

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

        //targetDict ??= new();
        //targetDict.Clear();

        //LevelTarget[] targets = LevelManager.Ins.CurrentLevel.Targets;
        LevelTarget[] targets = LevelManager.Ins.CurrentLevel.Targets;
        for (int i = 0; i < targets.Length; i++)
        {
            //CategoryKey key = new CategoryKey(targets[i].GroupId, targets[i].BranchId);
            //targetDict[key] = new TargetState
            //{
            //    Count = targets[i].RequiredAmount,
            //    Collected = new HashSet<ItemId>()
            //};
        }

        newItemIds ??= new HashSet<ItemId>();
        newItemIds.Clear();

        targetItemMenu.Init();
        startItemMenu.Init();
    }

    public ItemView SpawnItem(ItemId id)
    {
        ItemView itemView = itemViewPrefab.GetInstance(mergeBoard);
        itemView.Init(id);

        itemViews ??= new List<ItemView>();
        itemViews.Add(itemView);

        return itemView;
    }

    public void RemoveItem(ItemView itemView)
    {
        itemViews.Remove(itemView);
    }

    public bool IsFullyInside(RectTransform target)
    {
        return target.IsFullyInside(mergeBoard);
    }

    public void UpdateMergeCandidate(ItemView itemView)
    {
        ItemView candidate = null;
        float minDistanceSqr = mergeRadiusSqr;
        Vector2 draggingPos = itemView.rect.anchoredPosition;

        for (int i = 0; i < itemViews.Count; i++)
        {
            if (itemViews[i] == itemView) continue;
            float distanceSqr = (itemViews[i].rect.anchoredPosition - draggingPos).sqrMagnitude;
            if (distanceSqr < minDistanceSqr)
            {
                candidate = itemViews[i];
                minDistanceSqr = distanceSqr;
            }
        }

        itemView.SetMergeCandidate(candidate);
    }

    public bool TryMerge(ItemView itemViewA, ItemView itemViewB)
    {
        if (DatabaseManager.Ins.TryGetMergeResult(itemViewA.Id, itemViewB.Id, out MergeRecipe recipe))
        {
            RemoveItem(itemViewA);
            RemoveItem(itemViewB);

            bool collectTarget = false;
            ItemDefinition item = DatabaseManager.Ins.GetItemDefinition(recipe.ResultItemId);
            //CategoryKey key = new CategoryKey(item.GroupId, item.BranchId);
            //if (targetDict.ContainsKey(key) && targetDict[key].Collected.Add(item.Id))
            //{
            //    collectTarget = true;
            //    targetDict[key].Count--;

            //    if (targetDict[key].Count == 0)
            //    {
            //        targetDict.Remove(key);
            //    }
            //}

            //BoosterManager.Ins.UpdateHint();

            //if (targetDict.Count == 0)
            //{
            //    GameCanvas.Ins.Timer.Stop();
            //}

            Merge(itemViewA, itemViewB, recipe, collectTarget);

            return true;
        }

        return false;
    }

    void Merge(ItemView itemViewA, ItemView itemViewB, MergeRecipe recipe, bool collectTarget)
    {
        Vector2 posB = itemViewB.rect.anchoredPosition;

        ItemView resultItem = SpawnItem(recipe.ResultItemId);
        resultItem.rect.anchoredPosition = posB;
        resultItem.rect.localScale = Vector3.zero;
        resultItem.rect.SetAsLastSibling();

        Sequence seq = DOTween.Sequence();

        seq.Append(itemViewA.rect.DOAnchorPos(posB, 0.08f));
        seq.Join(itemViewB.rect.DOScale(1.08f, 0.08f));

        seq.Append(itemViewA.rect.DOScale(0f, 0.12f));
        seq.Join(itemViewB.rect.DOScale(0f, 0.12f));

        seq.AppendCallback(() =>
        {
            itemViewA.Destroy();
            itemViewB.Destroy();
        });

        seq.Append(resultItem.rect.DOScale(1.15f, 0.15f));
        seq.Append(resultItem.rect.DOScale(1f, 0.08f));

        seq.OnComplete(() =>
        {
            OnMergeComplete(recipe, collectTarget, resultItem, itemViewB.rect.position);
        });
    }

    void OnMergeComplete(MergeRecipe recipe, bool collectTarget, ItemView resultItem, Vector3 collectPos)
    {
        if (!newItemIds.Contains(recipe.ResultItemId))
        {
            newItemIds.Add(recipe.ResultItemId);
            startItemMenu.AddItem(recipe.ResultItemId);
        }

        if (!GamePref.Ins.DiscoveredRecipes.Contains(recipe.Id))
        {
            GamePref.Ins.DiscoveredRecipes.Add(recipe.Id);
        }

        if (GamePref.Ins.DiscoveredItems.Contains(recipe.ResultItemId))
        {
            CheckCollectTarget();
            CheckWin(1);
        }
        else
        {
            GamePref.Ins.DiscoveredItems.Add(recipe.ResultItemId);
            DatabaseManager.Ins.NeedUpdateUnlockedItems();

            UIManager.Ins.Open<PopupNewItem>().Init(recipe.ResultItemId).OnClose(() =>
            {
                CheckCollectTarget();
                CheckWin(0.25f);
            });
        }

        void CheckCollectTarget()
        {
            if (collectTarget)
            {
                RemoveItem(resultItem);
                resultItem.Remove(false);
                targetItemMenu.CollectTarget(recipe.ResultItemId, collectPos);
            }
        }
    }

    void CheckWin(float delay = 0)
    {
        //if (targetDict.Count == 0)
        //{
        //    LevelManager.Ins.WinLevel(delay);
        //}
    }

    public void ClearBoard()
    {
        if (!itemViews.IsNullOrEmpty())
        {
            for (int i = 0; i < itemViews.Count; i++)
            {
                itemViews[i].Remove();
            }
            itemViews.Clear();
        }

        BoosterManager.Ins.UpdateHint();
    }
}
