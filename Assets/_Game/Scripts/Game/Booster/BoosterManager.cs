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
    Dictionary<ItemId, int> targets => BoardManager.Ins.TargetDict;

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
            hintId = targets.First().Key;

            if (!DatabaseManager.Ins.TryGetRecipesFor(hintId, out List<MergeRecipe> recipes))
            {
                Debug.LogError($"No recipe found for target item {hintId}");
                return false;
            }

            MergeRecipe recipe = recipes[0];

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
                float padding = Mathf.Max(hintItemA.rect.sizeDelta.x, hintItemA.rect.sizeDelta.y) * 0.5f;
                hintItemA.rect.anchoredPosition = mergeBoard.GetRandomPoint(padding);
                hintItemA.rect.localScale = Vector3.zero;
                hintItemA.rect.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
            }

            if (hintItemB == null)
            {
                hintItemB = BoardManager.Ins.SpawnItem(recipe.ItemBId);
                hintItemB.rect.anchoredPosition = GetRandomNonOverlappingPoint(mergeBoard, hintItemB.rect, hintItemA.rect);
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
        return targets.Count > 0 && (hintItemA == null || hintItemB == null);
    }

    Vector2 GetRandomNonOverlappingPoint(RectTransform parent, RectTransform target, RectTransform other, int maxAttempts = 100)
    {
        float padding = Mathf.Max(target.sizeDelta.x, target.sizeDelta.y) * 0.5f;
        Vector2 pos = parent.GetRandomPoint(padding);
        Vector2 size = target.sizeDelta;

        for (int i = 0; i < maxAttempts; i++)
        {
            pos = parent.GetRandomPoint(padding);
            Rect rect = new Rect(pos - size * 0.5f, size);
            if (!rect.Overlaps(other.rect))
            {
                break;
            }
        }

        return pos;
    }

    public void UpdateHint()
    {
        hintItemA = itemViews != null && itemViews.Contains(hintItemA) ? hintItemA : null;
        hintItemB = itemViews != null && itemViews.Contains(hintItemB) ? hintItemB : null;

        if (!targets.ContainsKey(hintId))
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
