using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : Singleton<DatabaseManager>
{
    ItemDatabaseConfig config;
    bool isInitialized = false;
    Dictionary<ItemId, ItemDefinition> items;
    Dictionary<MergeKey, List<MergeRecipe>> mergeMap;
    Dictionary<ItemId, List<MergeRecipe>> resultMap;
    Dictionary<ItemId, List<MergeRecipe>> ingredientMap;
    bool needUpdateUnlockedItems = true;
    List<ItemDefinition> unlockedItems;
    Dictionary<CategoryId, CategoryDefinition> categories;

    public CategoryDefinition[] Categories => config.Categories;

    public void Init(ItemDatabaseConfig config)
    {
        if (isInitialized) return;

        this.config = config;

        isInitialized = true;

        items = new Dictionary<ItemId, ItemDefinition>();

        for (int i = 0; i < config.Items.Length; i++)
        {
            ItemDefinition item = config.Items[i];

            // Items
            items.Add(item.Id, item);
        }

        mergeMap = new Dictionary<MergeKey, List<MergeRecipe>>();
        resultMap = new Dictionary<ItemId, List<MergeRecipe>>();
        ingredientMap = new Dictionary<ItemId, List<MergeRecipe>>();

        for (int i = 0; i < config.Recipes.Length; i++)
        {
            MergeRecipe recipe = config.Recipes[i];

            // Merge Map
            MergeKey key = new MergeKey(recipe.ItemAId, recipe.ItemBId);
            if (mergeMap.ContainsKey(key))
            {
                if (mergeMap[key].Contains(recipe))
                {
                    Debug.LogError("Duplicated recipe");
                }
                else
                {
                    mergeMap[key].Add(recipe);
                }
            }
            else
            {
                mergeMap[key] = new List<MergeRecipe>() { recipe };
            }

            // Result Map
            ItemId resultId = recipe.ResultItemId;
            if (resultMap.ContainsKey(resultId))
            {
                resultMap[resultId].Add(recipe);
            }
            else
            {
                resultMap[resultId] = new List<MergeRecipe> { recipe };
            }

            // Ingredient Map
            AddIngredient(recipe.ItemAId, recipe);
            AddIngredient(recipe.ItemBId, recipe);
        }

        needUpdateUnlockedItems = true;

        categories = new Dictionary<CategoryId, CategoryDefinition>();

        for (int i = 0; i < config.Categories.Length; i++)
        {
            categories.Add(config.Categories[i].Id, config.Categories[i]);
        }
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

    public CategoryDefinition GetCategoryDefinition(CategoryId categoryId)
    {
        return categories[categoryId];
    }

    public MergeRecipe[] GetMergeRecipes()
    {
        return config.Recipes;
    }

    public bool TryGetRecipes(ItemId itemAId, ItemId itemBId, out List<MergeRecipe> recipes)
    {
        MergeKey key = new MergeKey(itemAId, itemBId);
        return mergeMap.TryGetValue(key, out recipes);
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