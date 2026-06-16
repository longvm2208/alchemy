using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] RectTransform combinationsContent;
    [SerializeField] CombinationView combinationViewPrefab;
    [SerializeField] TMP_Text undiscoveredCombText;
    [SerializeField] RectTransform undiscoveredCombRect;
    [SerializeField] VerticalLayout combinationsLayout;
    [SerializeField] RectTransform makesGridContent;
    [SerializeField] ItemMakeView itemMakeViewPrefab;
    [SerializeField] VerticalGridLayout makesGridLayout;
    [SerializeField] VerticalLayout makesVerticalLayout;
    [SerializeField] TMP_Text undiscoveredItemText;
    [SerializeField] RectTransform undiscoveredItemRect;
    [SerializeField] VerticalLayout infoLayout;

    List<CombinationView> combinationViews;
    List<ItemMakeView> itemMakeViews;

    public void Init(ItemDefinition itemDefinition)
    {
        CleanUp();

        iconImage.sprite = itemDefinition.Icon;
        nameText.text = itemDefinition.Name;
        descriptionText.text = itemDefinition.Description;

        SetupCombinations(itemDefinition.Id);
        SetupMakes(itemDefinition.Id);

        this.InvokeNextFrame(() =>
        {
            combinationsLayout.UpdateLayout();
            makesGridLayout.UpdateLayout();
            makesVerticalLayout.UpdateLayout();
            infoLayout.UpdateLayout();
        });
    }

    void CleanUp()
    {
        if (!combinationViews.IsNullOrEmpty())
        {
            for (int i = 0; i < combinationViews.Count; i++)
            {
                combinationViews[i].Destroy();
            }
            combinationViews.Clear();
        }
        combinationViews ??= new List<CombinationView>();

        if (!itemMakeViews.IsNullOrEmpty())
        {
            for (int i = 0; i < itemMakeViews.Count; i++)
            {
                itemMakeViews[i].Destroy();
            }
            itemMakeViews.Clear();
        }
        itemMakeViews ??= new List<ItemMakeView>();
    }

    void SetupCombinations(ItemId id)
    {
        if (DatabaseManager.Ins.TryGetRecipesFor(id, out List<MergeRecipe> recipes))
        {
            combinationsContent.gameObject.SetActive(true);

            int undiscoveredCount = 0;

            for (int i = 0; i < recipes.Count; i++)
            {
                if (GamePref.Ins.DiscoveredRecipes.Contains(recipes[i].Id))
                {
                    CombinationView view = Instantiate(combinationViewPrefab, combinationsContent);
                    view.Init(recipes[i]);
                    combinationViews.Add(view);
                }
                else
                {
                    undiscoveredCount++;
                }
            }

            if (undiscoveredCount > 0)
            {
                undiscoveredCombRect.gameObject.SetActive(true);
                undiscoveredCombRect.SetAsLastSibling();

                if (undiscoveredCount == 1)
                {
                    undiscoveredCombText.text = "and 1 undiscovered combination";
                }
                else
                {
                    undiscoveredCombText.text = $"and {undiscoveredCount} undiscovered combinations";
                }
            }
            else
            {
                undiscoveredCombRect.gameObject.SetActive(false);
            }
        }
        else
        {
            combinationsContent.gameObject.SetActive(false);
        }
    }

    void SetupMakes(ItemId id)
    {
        if (DatabaseManager.Ins.TryGetRecipesWithIngredient(id, out List<MergeRecipe> recipes))
        {
            makesVerticalLayout.gameObject.SetActive(true);

            int undiscoveredCount = 0;

            for (int i = 0; i < recipes.Count; i++)
            {
                if (GamePref.Ins.DiscoveredItems.Contains(recipes[i].ResultItemId))
                {
                    ItemMakeView view = Instantiate(itemMakeViewPrefab, makesGridContent);
                    view.Init(recipes[i]);
                    itemMakeViews.Add(view);
                }
                else
                {
                    undiscoveredCount++;
                }
            }

            if (undiscoveredCount > 0)
            {
                undiscoveredItemRect.gameObject.SetActive(true);

                if (undiscoveredCount == 1)
                {
                    undiscoveredItemText.text = "and 1 undiscovered item";
                }
                else
                {
                    undiscoveredItemText.text = $"and {undiscoveredCount} undiscovered items";
                }
            }
            else
            {
                undiscoveredItemRect.gameObject.SetActive(false);
            }
        }
        else
        {
            makesVerticalLayout.gameObject.SetActive(false);
        }
    }
}
