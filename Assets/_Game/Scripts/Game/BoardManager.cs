using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
public class BoardManager : SingletonMonoBehaviour<BoardManager>
{
    struct MergeResult
    {
        public MergeRecipe Recipe;
        public bool IsTarget;
        public List<CategoryId> TargetCategories;
    }

    [SerializeField] float mergeRadiusSqr;
    [SerializeField] TargetItemMenu targetItemMenu;
    [SerializeField] StartItemMenu startItemMenu;
    [SerializeField] ItemView itemViewPrefab;
    [SerializeField] RectTransform mergeBoard;
    [SerializeField] RectTransform bottomAnchor;
    [SerializeField] RectTransform bottomFollower;
    [SerializeField] PosConfig posConfig;
    [SerializeField] Canvas canvas;
    public float ScaleFactor => canvas.scaleFactor;
    [SerializeField] Transform vfxHolder;
    [SerializeField] MergeImpact mergeImpact;

    List<ItemView> itemViews;
    public List<ItemView> ItemViews => itemViews;
    Dictionary<CategoryId, LevelTargetState> targetDict;
    public Dictionary<CategoryId, LevelTargetState> TargetDict => targetDict;
    HashSet<ItemId> availableItems;
    public HashSet<ItemId> AvailableItems => availableItems;

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

        targetDict ??= new();
        targetDict.Clear();

        LevelTarget[] targets = LevelManager.Ins.CurrentLevel.Targets;
        for (int i = 0; i < targets.Length; i++)
        {
            CategoryId key = targets[i].CategoryId;
            targetDict[key] = new LevelTargetState
            {
                Count = targets[i].RequiredAmount,
                Collected = new HashSet<ItemId>()
            };
        }

        availableItems ??= new HashSet<ItemId>();
        availableItems.Clear();
        ItemId[] startItems = LevelManager.Ins.CurrentLevel.StartItems;
        for (int i = 0; i < startItems.Length; i++)
        {
            availableItems.Add(startItems[i]);
        }

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
        if (itemViews.Contains(itemView))
        {
            itemViews.Remove(itemView);
        }
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
        if (DatabaseManager.Ins.TryGetRecipes(itemViewA.Id, itemViewB.Id, out List<MergeRecipe> recipes))
        {
            RemoveItem(itemViewA);
            RemoveItem(itemViewB);

            List<MergeResult> results = new();

            for (int i = 0; i < recipes.Count; i++)
            {
                MergeRecipe recipe = recipes[i];
                ItemId resultItemId = recipes[i].ResultItemId;
                ItemDefinition item = DatabaseManager.Ins.GetItemDefinition(resultItemId);

                MergeResult result = new MergeResult()
                {
                    Recipe = recipe,
                };

                if (!item.Categories.IsNullOrEmpty())
                {
                    for (int j = 0; j < item.Categories.Length; j++)
                    {
                        CategoryId categoryId = item.Categories[j];

                        if (targetDict.ContainsKey(categoryId) && targetDict[categoryId].Collected.Add(resultItemId))
                        {
                            targetDict[categoryId].Count--;

                            if (targetDict[categoryId].Count == 0)
                            {
                                targetDict.Remove(categoryId);
                            }

                            result.IsTarget = true;

                            if (result.TargetCategories == null)
                            {
                                result.TargetCategories = new List<CategoryId>();
                            }

                            result.TargetCategories.Add(categoryId);
                        }
                    }
                }

                results.Add(result);
            }

            BoosterManager.Ins.UpdateHint();

            if (targetDict.Count == 0)
            {
                GameCanvas.Ins.Timer.Stop();
            }

            Merge(itemViewA, itemViewB, results);

            return true;
        }

        return false;
    }

    void Merge(ItemView itemViewA, ItemView itemViewB, List<MergeResult> results)
    {
        Vector2 posB = itemViewB.rect.anchoredPosition;

        List<ItemView> resultItems = new();

        for (int i = 0; i < results.Count; i++)
        {
            ItemView resultItem = SpawnItem(results[i].Recipe.ResultItemId);
            resultItem.rect.localScale = Vector3.zero;
            resultItem.rect.SetAsLastSibling();
            resultItems.Add(resultItem);
        }

        Arrange(posB, resultItems);

        Sequence seq = DOTween.Sequence();

        seq.Append(itemViewA.rect.DOAnchorPos(posB, 0.08f));
        seq.Join(itemViewB.rect.DOScale(1.08f, 0.08f));

        seq.Append(itemViewA.rect.DOScale(0f, 0.12f));
        seq.Join(itemViewB.rect.DOScale(0f, 0.12f));

        seq.AppendCallback(() =>
        {
            Vector3 impactPos = itemViewB.transform.position;
            impactPos.z = 0;
            Impact(impactPos);

            itemViewA.Destroy();
            itemViewB.Destroy();
        });

        for (int i = 0; i < resultItems.Count; i++)
        {
            if (i == 0)
            {
                seq.Append(resultItems[i].rect.DOScale(1.15f, 0.15f));
            }
            else
            {
                seq.Join(resultItems[i].rect.DOScale(1.15f, 0.15f));
            }
        }

        for (int i = 0; i < resultItems.Count; i++)
        {
            if (i == 0)
            {
                seq.Append(resultItems[i].rect.DOScale(1f, 0.08f));
            }
            else
            {
                seq.Join(resultItems[i].rect.DOScale(1f, 0.08f));
            }
        }

        seq.OnComplete(() =>
        {
            OnMergeComplete(results, resultItems);
        });
    }

    void Impact(Vector3 pos)
    {
        MergeImpact impact = mergeImpact.GetInstance(vfxHolder);
        impact.transform.position = pos;
        impact.Play();
    }

    void Arrange(Vector2 center, List<ItemView> itemViews)
    {
        Vector2[] posArray = posConfig.Value[itemViews.Count - 1].Array;

        for (int i = 0; i < itemViews.Count; i++)
        {
            itemViews[i].rect.anchoredPosition = center + posArray[i];
        }
    }

    void OnMergeComplete(List<MergeResult> results, List<ItemView> resultItems)
    {
        for (int i = 0; i < results.Count; i++)
        {
            MergeResult result = results[i];
            MergeRecipe recipe = result.Recipe;
            ItemView resultItem = resultItems[i];

            if (!availableItems.Contains(recipe.ResultItemId))
            {
                availableItems.Add(recipe.ResultItemId);
                startItemMenu.AddItem(recipe.ResultItemId);
            }

            if (!GamePref.Ins.DiscoveredRecipes.Contains(recipe.Id))
            {
                GamePref.Ins.DiscoveredRecipes.Add(recipe.Id);
            }

            if (GamePref.Ins.DiscoveredItems.Contains(recipe.ResultItemId))
            {
                CheckCollectTarget(result, resultItem);
                CheckWin(1);
            }
            else
            {
                GamePref.Ins.DiscoveredItems.Add(recipe.ResultItemId);
                DatabaseManager.Ins.NeedUpdateUnlockedItems();

                UIManager.Ins.Open<PopupNewItem>().Init(recipe.ResultItemId).OnClose(() =>
                {
                    CheckCollectTarget(result, resultItem);
                    CheckWin(0.25f);
                });
            }
        }
    }
    
    void CheckCollectTarget(MergeResult result, ItemView resultItem)
    {
        if (result.IsTarget)
        {
            RemoveItem(resultItem);
            resultItem.Remove(false);

            ItemId resultItemId = result.Recipe.ResultItemId;
            Vector3 pos = resultItem.rect.position;

            for (int i = 0; i < result.TargetCategories.Count; i++)
            {
                CategoryId categoryId = result.TargetCategories[i];
                targetItemMenu.CollectTarget(resultItemId, categoryId, pos);
            }
        }
    }

    void CheckWin(float delay = 0)
    {
        if (targetDict.Count == 0)
        {
            LevelManager.Ins.WinLevel(delay);
        }
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
