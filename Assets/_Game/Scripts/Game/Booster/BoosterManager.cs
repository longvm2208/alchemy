using DG.Tweening;
using System.Collections.Generic;
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
                hintItemA.rect.anchoredPosition = new Vector2
                    (
                        Random.Range(-0.5f * mergeBoard.rect.width + padding, -padding),
                        Random.Range(-0.5f * mergeBoard.rect.height + padding, -padding)
                    );
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
        Queue<ItemId> queue = new();
        HashSet<ItemId> visited = new();

        foreach (var pair in targetDict)
        {
            CategoryId categoryId = pair.Key;
            LevelTargetState targetState = pair.Value;
            CategoryDefinition category = DatabaseManager.Ins.GetCategoryDefinition(categoryId);

            for (int i = 0; i < category.Items.Length; i++)
            {
                if (targetState.Collected.Contains(category.Items[i])) continue;

                queue.Enqueue(category.Items[i]);
            }
        }

        while (queue.Count > 0)
        {
            ItemId itemId = queue.Dequeue();

            if (!DatabaseManager.Ins.TryGetRecipesFor(itemId, out List<MergeRecipe> recipes)) continue;

            for (int i = 0; i < recipes.Count; i++)
            {
                MergeRecipe recipe = recipes[i];
                ItemId itemAId = recipe.ItemAId;
                ItemId itemBId = recipe.ItemBId;

                if (availableItems.Contains(itemAId) && availableItems.Contains(itemBId))
                {
                    return recipe;
                }
                else if (availableItems.Contains(itemAId) && visited.Add(itemBId))
                {
                    queue.Enqueue(itemBId);
                }
                else if (availableItems.Contains(itemBId) && visited.Add(itemAId))
                {
                    queue.Enqueue(itemAId);
                }
                else
                {
                    if (visited.Add(itemAId))
                    {
                        queue.Enqueue(itemAId);
                    }
                    if (visited.Add(itemBId))
                    {
                        queue.Enqueue(itemBId);
                    }
                }
            }
        }

        return null;
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
