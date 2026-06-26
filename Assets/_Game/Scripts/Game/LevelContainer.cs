using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelContainer", menuName = "SO/Game/Level Container")]
public class LevelContainer : ScriptableObject
{
    const int maxValue = 1000000;

    public LevelData[] Levels;

#if UNITY_EDITOR
    class ItemMergeDepth
    {
        public ItemId Item;
        public int Depth;
    }

    [Header("Editor")]
    [SerializeField] ItemDatabaseConfig itemDatabaseConfig;
    [SerializeField] int maxDepth;
    [SerializeField] int level;

    Dictionary<ItemId, int> itemMergeDepthDict;

    [Button]
    void UpdateLevels()
    {
        DatabaseManager.Ins.Init(itemDatabaseConfig);

        itemMergeDepthDict = new Dictionary<ItemId, int>()
        {
            { ItemId.Air, 1 },
            { ItemId.Earth, 1 },
            { ItemId.Fire, 1 },
            { ItemId.Water, 1 },
            { ItemId.Time, 1 },
        };

        int loop = 0;

        while (itemMergeDepthDict.Count < itemDatabaseConfig.Items.Length && loop < 100000)
        {
            for (int i = 0; i < itemDatabaseConfig.Recipes.Length; i++)
            {
                MergeRecipe recipe = itemDatabaseConfig.Recipes[i];

                ItemId itemA = recipe.ItemAId;
                ItemId itemB = recipe.ItemBId;
                ItemId result = recipe.ResultItemId;

                if (itemMergeDepthDict.ContainsKey(itemA) && itemMergeDepthDict.ContainsKey(itemB))
                {
                    if (!itemMergeDepthDict.ContainsKey(result))
                    {
                        int depth = itemMergeDepthDict[itemA] + itemMergeDepthDict[itemB];
                        itemMergeDepthDict[result] = depth;
                    }
                }
            }

            loop++;
        }

        Debug.Log(loop);

        for (int i = 0; i < Levels.Length; i++)
        {
            LevelData level = Levels[i];
            Debug.Log($"========== Level {i + 1} ==========");
            UpdateLevel(level);
        }
    }

    void UpdateLevel(LevelData level)
    {
        foreach (var target in level.Targets)
        {
            CategoryId categoryId = target.CategoryId;
            int required = target.RequiredAmount;

            CategoryDefinition category = DatabaseManager.Ins.GetCategoryDefinition(categoryId);
            List<ItemMergeDepth> itemMergeDepths = new();

            for (int i = 0; i < category.Items.Length; i++)
            {
                ItemId itemId = category.Items[i];

                itemMergeDepths.Add(new ItemMergeDepth
                {
                    Item = itemId,
                    Depth = itemMergeDepthDict[itemId]
                });
            }

            itemMergeDepths.Sort((a, b) => a.Depth.CompareTo(b.Depth));

            if (itemMergeDepths.Count > required)
            {
                itemMergeDepths.RemoveRange(required, itemMergeDepths.Count - required);
            }

            List<ItemId> startItems = new(level.StartItems);
            List<ItemId> targetItems = new();
            List<ItemId> nextTargetItems = new();

            for (int i = 0; i < itemMergeDepths.Count; i++)
            {
                targetItems.Add(itemMergeDepths[i].Item);
            }

            int trackDepth = 0;

            while (targetItems.Count > 0 && trackDepth < 3)
            {
                foreach (ItemId itemId in targetItems)
                {
                    Debug.Log($"----- Target: {itemId} -----");

                    if (!DatabaseManager.Ins.TryGetRecipesFor(itemId, out List<MergeRecipe> recipes))
                    {
                        Debug.LogError("Error");
                        continue;
                    }

                    int recipeIndex = -1;
                    int minDepth = maxValue;
                    ItemId itemA, itemB;

                    for (int i = 0; i < recipes.Count; i++)
                    {
                        MergeRecipe recipe = recipes[i];
                        itemA = recipe.ItemAId;
                        itemB = recipe.ItemBId;
                        int mergeDepth = 1 + Mathf.Max(
                            GetMergeMinDepth(itemA, startItems, new HashSet<ItemId>()),
                            GetMergeMinDepth(itemB, startItems, new HashSet<ItemId>()));

                        Debug.Log($"{itemA} + {itemB} = {mergeDepth}");

                        if (minDepth > mergeDepth)
                        {
                            minDepth = mergeDepth;
                            recipeIndex = i;
                        }
                    }

                    if (recipeIndex < 0)
                    {
                        Debug.LogError("Error");
                        return;
                    }

                    MergeRecipe minDepthRecipe = recipes[recipeIndex];
                    itemA = minDepthRecipe.ItemAId;
                    itemB = minDepthRecipe.ItemBId;

                    if (!startItems.Contains(itemA) && !nextTargetItems.Contains(itemA))
                    {
                        nextTargetItems.Add(itemA);
                    }

                    if (!startItems.Contains(itemB) && !nextTargetItems.Contains(itemB))
                    {
                        nextTargetItems.Add(itemB);
                    }
                }

                trackDepth++;

                if (trackDepth == 3)
                {
                    startItems.AddRange(nextTargetItems);
                }

                targetItems = new List<ItemId>(nextTargetItems);
                nextTargetItems = new List<ItemId>();
            }

            level.StartItems = startItems.ToArray();

            EditorUtility.SetDirty(level);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    int GetMergeMinDepth(ItemId itemId, List<ItemId> startItems, HashSet<ItemId> visited)
    {
        if (startItems.Contains(itemId))
        {
            return 0;
        }

        if (!visited.Add(itemId))
        {
            return maxValue;
        }

        if (!DatabaseManager.Ins.TryGetRecipesFor(itemId, out List<MergeRecipe> recipes))
        {
            return maxValue;
        }

        int min = maxValue;

        foreach (var recipe in recipes)
        {
            ItemId itemA = recipe.ItemAId;
            ItemId itemB = recipe.ItemBId;

            int depth = 1 + Mathf.Max(
                GetMergeMinDepth(itemA, startItems, visited),
                GetMergeMinDepth(itemB, startItems, visited));

            min = Mathf.Min(min, depth);
        }

        return min;
    }

    [Button]
    void FindCategory()
    {
        DatabaseManager.Ins.Init(itemDatabaseConfig);

        Dictionary<ItemId, int> resultItems = new();
        Dictionary<ItemId, int> discoveredDepth = new();
        Queue<ItemId> queue = new();

        ItemId[] startItems = Levels[level - 1].StartItems;

        foreach (var item in startItems)
        {
            queue.Enqueue(item);
            discoveredDepth[item] = 0;
            resultItems[item] = 0;
        }

        while (queue.Count > 0)
        {
            ItemId current = queue.Dequeue();
            int currentDepth = discoveredDepth[current];

            if (currentDepth >= maxDepth) continue;

            if (!DatabaseManager.Ins.TryGetRecipesWithIngredient(current, out List<MergeRecipe> recipes)) continue;

            foreach (var recipe in recipes)
            {
                ItemId other;

                if (recipe.ItemAId == current)
                {
                    other = recipe.ItemBId;
                }
                else
                {
                    other = recipe.ItemAId;
                }

                if (!discoveredDepth.ContainsKey(other)) continue;

                ItemId resultId = recipe.ResultItemId;

                int newDepth = currentDepth + 1;

                if (!resultItems.ContainsKey(resultId))
                {
                    resultItems[resultId] = newDepth;
                    queue.Enqueue(resultId);
                    discoveredDepth[resultId] = newDepth;
                }
            }
        }

        Dictionary<CategoryId, int> resultCategory = new();

        foreach (var pair in resultItems)
        {
            ItemId itemId = pair.Key;
            ItemDefinition item = DatabaseManager.Ins.GetItemDefinition(itemId);

            foreach (var categoryId in item.Categories)
            {
                if (resultCategory.ContainsKey(categoryId))
                {
                    resultCategory[categoryId]++;
                }
                else
                {
                    resultCategory.Add(categoryId, 1);
                }
            }
        }

        var sorted = resultCategory.OrderBy(pair => pair.Value).ToList();

        foreach (var pair in sorted)
        {
            Debug.Log($"{pair.Key} - {pair.Value}");
        }
    }
#endif
}