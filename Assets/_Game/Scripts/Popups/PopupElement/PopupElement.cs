using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupElement : Popup
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text description;
    [SerializeField] Button button;
    [SerializeField] CombinationTable combinationTable;
    [SerializeField] MakeTable makeTable;

    private void Awake()
    {
        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    public void Init(ItemDefinition item)
    {
        icon.sprite = item.Icon;
        nameText.text = item.Name;
        description.text = item.Description;

        List<MergeRecipe> recipes;

        if (!DatabaseManager.Ins.TryGetRecipesFor(item.Id, out recipes)) return;

        combinationTable.Init(recipes);

        if (!DatabaseManager.Ins.TryGetRecipesWithIngredient(item.Id, out recipes)) return;

        List<ItemId> resultIds = new();

        for (int i = 0; i < recipes.Count; i++)
        {
            if (resultIds.Contains(recipes[i].ResultItemId)) continue;
            resultIds.Add(recipes[i].ResultItemId);
        }

        makeTable.Init(resultIds);
    }

    void OnClick()
    {
        Close();
    }
}
