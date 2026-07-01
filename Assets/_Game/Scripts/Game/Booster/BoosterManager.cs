using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoosterManager : SingletonMonoBehaviour<BoosterManager>
{
    [SerializeField] RectTransform mergeBoard;
    [SerializeField] HintButton hintButton;
    [SerializeField] ExtraTimeButton extraTimeButton;

    ItemId hintId;
    ItemView hintItemA;
    ItemView hintItemB;

    List<ItemView> itemViews => BoardManager.Ins.ItemViews;
    Dictionary<CategoryId, LevelTargetState> targetDict => BoardManager.Ins.TargetDict;
    HashSet<ItemId> availableItems => BoardManager.Ins.AvailableItems;

    public void Init()
    {
        hintButton.Init();
        extraTimeButton.Init();
        hintItemA = null;
        hintItemB = null;
    }

    public bool ExecuteHint()
    {
        if (CanHint())
        {
            MergeRecipe recipe = FindHintRecipe();

            if (recipe == null)
            {
                Debug.LogError("No hint found");
                return false;
            }

            hintId = recipe.ResultItemId;

            if (!itemViews.IsNullOrEmpty())
            {
                for (int i = 0; i < itemViews.Count; i++)
                {
                    if (itemViews[i].Id == recipe.ItemAId)
                    {
                        hintItemA = itemViews[i];
                    }
                    else if (itemViews[i].Id == recipe.ItemBId)
                    {
                        hintItemB = itemViews[i];
                    }
                    if (hintItemA != null && hintItemB != null)
                    {
                        break;
                    }
                }
            }

            if (hintItemA == null)
            {
                hintItemA = BoardManager.Ins.SpawnItem(recipe.ItemAId);
                float padding = Mathf.Max(hintItemA.rect.sizeDelta.x, hintItemA.rect.sizeDelta.y) * 0.5f + 60;
                Vector2 pos = Vector2.zero;
                if (hintItemB != null)
                {
                    pos = new Vector2
                        (
                            -hintItemB.rect.anchoredPosition.x,
                            -hintItemB.rect.anchoredPosition.y
                        );
                }
                else
                {
                    pos = new Vector2
                        (
                            Random.Range(-0.5f * mergeBoard.rect.width + padding, -padding),
                            Random.Range(-0.5f * mergeBoard.rect.height + padding, -padding)
                        );
                }
                hintItemA.rect.anchoredPosition = pos;
                hintItemA.rect.localScale = Vector3.zero;
                hintItemA.rect.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
            }

            if (hintItemB == null)
            {
                hintItemB = BoardManager.Ins.SpawnItem(recipe.ItemBId);
                hintItemB.rect.anchoredPosition = new Vector2
                    (
                        -hintItemA.rect.anchoredPosition.x,
                        -hintItemA.rect.anchoredPosition.y
                    );
                hintItemB.rect.localScale = Vector3.zero;
                hintItemB.rect.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
            }

            hintItemA.StartHint();
            hintItemB.StartHint();
            hintItemA.rect.SetAsLastSibling();
            hintItemB.rect.SetAsLastSibling();

            return true;
        }

        return false;
    }

    bool CanHint()
    {
        return !targetDict.IsNullOrEmpty() && (hintItemA == null || hintItemB == null);
    }

    MergeRecipe FindHintRecipe()
    {
        List<ItemId> targetIds = new();
        HashSet<ItemId> visited = new();

        foreach (var pair in targetDict)
        {
            CategoryId categoryId = pair.Key;
            LevelTargetState targetState = pair.Value;
            CategoryDefinition category = DatabaseManager.Ins.GetCategoryDefinition(categoryId);

            Debug.Log("Find hint");

            for (int i = 0; i < category.Items.Length; i++)
            {
                ItemId itemId = category.Items[i];
                if (targetState.Collected.Contains(itemId))
                {
                    visited.Add(itemId);
                    continue;
                }
                if (availableItems.Contains(itemId)) continue;
                targetIds.Add(itemId);
            }
        }

        List<ItemId> startIds = new(availableItems);
        Dictionary<ItemId, int> mergeCountDict = GetMergeCountDict(startIds);
        
        targetIds.Sort((a, b) => mergeCountDict[a].CompareTo(mergeCountDict[b]));

        Queue<ItemId> queue = new(targetIds);

        while (queue.Count > 0)
        {
            ItemId itemId = queue.Dequeue();

            if (!DatabaseManager.Ins.TryGetRecipesFor(itemId, out List<MergeRecipe> recipes)) continue;

            recipes.Sort((x, y) =>
            {
                int mergeCountA = mergeCountDict[x.ItemAId] + mergeCountDict[x.ItemBId];
                int mergeCountB = mergeCountDict[y.ItemAId] + mergeCountDict[y.ItemBId];
                return mergeCountA.CompareTo(mergeCountB);
            });

            for (int i = 0; i < recipes.Count; i++)
            {
                MergeRecipe recipe = recipes[i];

                ItemId itemAId = recipe.ItemAId;
                ItemId itemBId = recipe.ItemBId;

                if (availableItems.Contains(itemAId) && availableItems.Contains(itemBId))
                {
                    return recipe;
                }

                if (!availableItems.Contains(itemAId) && visited.Add(itemAId))
                {
                    queue.Enqueue(itemAId);
                }

                if (!availableItems.Contains(itemBId) && visited.Add(itemBId))
                {
                    queue.Enqueue(itemBId);
                }
            }
        }

        return null;
    }

    Dictionary<ItemId, int> GetMergeCountDict(List<ItemId> startIds)
    {
        MergeRecipe[] recipes = DatabaseManager.Ins.GetMergeRecipes();

        Dictionary<ItemId, int> mergeCountDict = new();

        for (int i = 0; i < startIds.Count; i++)
        {
            mergeCountDict.Add(startIds[i], 0);
        }

        HashSet<ItemId> visited = new(startIds);
        List<ItemId> next = new();

        int loop = 0;

        do
        {
            next.Clear();

            for (int i = 0; i < recipes.Length; i++)
            {
                MergeRecipe recipe = recipes[i];

                if (!mergeCountDict.ContainsKey(recipe.ItemAId) ||
                    !mergeCountDict.ContainsKey(recipe.ItemBId)) continue;

                if (!visited.Add(recipe.ResultItemId)) continue;

                next.Add(recipe.ResultItemId);

                mergeCountDict[recipe.ResultItemId] = 1 + mergeCountDict[recipe.ItemAId] + mergeCountDict[recipe.ItemBId];
            }

        } while (next.Count > 0 && ++loop < 1000);

        return mergeCountDict;
    }

    public void UpdateHint()
    {
        hintItemA = itemViews != null && itemViews.Contains(hintItemA) ? hintItemA : null;
        hintItemB = itemViews != null && itemViews.Contains(hintItemB) ? hintItemB : null;

        if (!availableItems.Contains(hintId))
        {
            if (hintItemA != null)
            {
                hintItemA.StopHint();
                hintItemA = null;
            }
            if (hintItemB != null)
            {
                hintItemB.StopHint();
                hintItemB = null;
            }
        }
    }

    bool CanExtraTime()
    {
        return GameCanvas.Ins.Timer.TimeRemaining > 0;
    }

    public bool ExecuteExtraTime()
    {
        if (CanExtraTime())
        {
            GameCanvas.Ins.Timer.AddTime(30);
            return true;
        }

        return false;
    }
}
