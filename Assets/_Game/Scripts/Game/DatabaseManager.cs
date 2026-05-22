using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : SingletonMonoBehaviour<DatabaseManager>
{
    [SerializeField] ItemDatabaseConfig config;

    bool isInitialized = false;
    Dictionary<ItemId, ItemDefinition> items;
    Dictionary<MergeKey, MergeRecipe> mergeMap;
    Dictionary<ItemId, List<MergeRecipe>> resultMap;
    Dictionary<ItemId, List<MergeRecipe>> ingredientMap;
    bool needUpdateUnlockedItems = true;
    List<ItemDefinition> unlockedItems;

    public void Init()
    {
        if (isInitialized) return;

        isInitialized = true;

        items = new Dictionary<ItemId, ItemDefinition>();

        for (int i = 0; i < config.Items.Length; i++)
        {
            items.Add(config.Items[i].Id, config.Items[i]);
        }

        mergeMap = new Dictionary<MergeKey, MergeRecipe>();

        for (int i = 0; i < config.Recipes.Length; i++)
        {
            MergeKey key = new MergeKey(config.Recipes[i].ItemAId, config.Recipes[i].ItemBId);
            mergeMap.Add(key, config.Recipes[i]);
        }

        resultMap = new Dictionary<ItemId, List<MergeRecipe>>();

        for (int i = 0; i < config.Recipes.Length; i++)
        {
            ItemId resultId = config.Recipes[i].ResultItemId;
            if (resultMap.ContainsKey(resultId))
            {
                resultMap[resultId].Add(config.Recipes[i]);
            }
            else
            {
                resultMap[resultId] = new List<MergeRecipe> { config.Recipes[i] };
            }
        }

        ingredientMap = new Dictionary<ItemId, List<MergeRecipe>>();

        for (int i = 0; i < config.Recipes.Length; i++)
        {
            AddIngredient(config.Recipes[i].ItemAId, config.Recipes[i]);
            AddIngredient(config.Recipes[i].ItemBId, config.Recipes[i]);
        }

        needUpdateUnlockedItems = true;
    }

    void AddIngredient(ItemId ingredientId, MergeRecipe recipe)
    {
        if (ingredientMap.ContainsKey(ingredientId))
        {
            ingredientMap[ingredientId].Add(recipe);
        }
        else
        {
            ingredientMap[ingredientId] = new List<MergeRecipe> { recipe };
        }
    }

    public ItemDefinition GetItemDefinition(ItemId id)
    {
        if (items.TryGetValue(id, out ItemDefinition item))
        {
            return item;
        }
        else
        {
            Debug.LogError($"Item with ID {id} not found in database.");
            return null;
        }
    }

    public bool TryGetMergeResult(ItemId itemAId, ItemId itemBId, out MergeRecipe recipe)
    {
        MergeKey key = new MergeKey(itemAId, itemBId);
        return mergeMap.TryGetValue(key, out recipe);
    }

    public bool TryGetRecipesFor(ItemId resultId, out List<MergeRecipe> recipes)
    {
        return resultMap.TryGetValue(resultId, out recipes);
    }

    public bool TryGetRecipesWithIngredient(ItemId ingredientId, out List<MergeRecipe> recipes)
    {
        return ingredientMap.TryGetValue(ingredientId, out recipes);
    }

    public void NeedUpdateUnlockedItems()
    {
        needUpdateUnlockedItems = true;
    }

    public List<ItemDefinition> GetUnlockedItems()
    {
        if (needUpdateUnlockedItems)
        {
            unlockedItems ??= new List<ItemDefinition>();
            unlockedItems.Clear();

            for (int i = 0; i < config.Items.Length; i++)
            {
                if (GamePref.Ins.DiscoveredItems.Contains(config.Items[i].Id))
                {
                    unlockedItems.Add(config.Items[i]);
                }
            }
        }

        return unlockedItems;
    }
}