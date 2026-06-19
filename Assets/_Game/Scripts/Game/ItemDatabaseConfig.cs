using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "ItemDatabaseConfig", menuName = "SO/Game/Item Database Config")]
public class ItemDatabaseConfig : ScriptableObject
{
    public ItemDefinition[] Items;
    public CategoryDefinition[] Categories;
    [SerializeField, HideInInspector]
    public MergeRecipe[] Recipes;
#if UNITY_EDITOR
    [InlineButton(nameof(ShowPart))]
    [SerializeField] Vector2Int Range;
    [SerializeField, TableList] MergeRecipe[] Part;

    void ShowPart()
    {
        Part = new MergeRecipe[Range.y - Range.x];
        for (int i = Range.x; i < Range.y; i++)
        {
            Part[i] = Recipes[i + Range.x];
        }
    }
#endif

#if UNITY_EDITOR
    [Serializable]
    class ItemsSheet
    {
        public ItemsSheetRow[] Rows;
    }

    [Serializable]
    class ItemsSheetRow
    {
        public string Id;
        public string Name;
        public string Description;
    }

    [Serializable]
    class RecipesSheet
    {
        public RecipesSheetRow[] Rows;
    }

    [Serializable]
    class RecipesSheetRow
    {
        public string ResultItemId;
        public string Recipes;
    }

    [Serializable]
    class CategoriesSheet
    {
        public CategoriesSheetRow[] Rows;
    }

    [Serializable]
    class CategoriesSheetRow
    {
        public string Id;
        public string Name;
        public string Items;
    }

    [Header("Editor")]
    [SerializeField] string appsScriptUrl;
    [InlineButton(nameof(UpdateItemIds), "Update")]
    [SerializeField] MonoScript itemIdFile;
    [InlineButton(nameof(UpdateCategoryIds), "Update")]
    [SerializeField] MonoScript categoryIdFile;
    [SerializeField] DefaultAsset itemDefinitionFolder;
    [SerializeField] DefaultAsset categoryDefinitionFolder;
    [SerializeField] DefaultAsset itemSpriteFolder;

    async void UpdateItemIds()
    {
        Debug.Log("Updating...");

        string url = appsScriptUrl + "?type=Items";

        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";
            var sheet = JsonUtility.FromJson<ItemsSheet>(json);

            List<string> names = new List<string>();
            for (int i = 0; i < sheet.Rows.Length; i++)
            {
                names.Add(sheet.Rows[i].Id);
            }

            string enumFilePath = AssetDatabase.GetAssetPath(itemIdFile);
            EnumGenerator.AppendNewEnumValues(enumFilePath, names);

            Debug.Log("Load Success: " + json);
        }
        else
        {
            Debug.LogError("Load Failed: " + request.error);
        }

        Debug.Log("Update Success");
    }

    async void UpdateCategoryIds()
    {
        Debug.Log("Updating...");

        string url = appsScriptUrl + "?type=Categories";

        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";
            var sheet = JsonUtility.FromJson<CategoriesSheet>(json);

            List<string> names = new List<string>();
            for (int i = 0; i < sheet.Rows.Length; i++)
            {
                names.Add(sheet.Rows[i].Id);
            }

            string enumFilePath = AssetDatabase.GetAssetPath(categoryIdFile);
            EnumGenerator.AppendNewEnumValues(enumFilePath, names);

            Debug.Log("Load Success: " + json);
        }
        else
        {
            Debug.LogError("Load Failed: " + request.error);
        }

        Debug.Log("Update Success");
    }

    [Button]
    async void UpdateItems()
    {
        Debug.Log("Updating...");

        string url = appsScriptUrl + "?type=Items";

        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";
            var sheet = JsonUtility.FromJson<ItemsSheet>(json);

            Dictionary<ItemId, ItemDefinition> itemDict = new();

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null) continue;
                itemDict.Add(Items[i].Id, Items[i]);
            }

            List<ItemDefinition> items = new();

            for (int i = 0; i < sheet.Rows.Length; i++)
            {
                ItemsSheetRow row = sheet.Rows[i];
                ItemId id = Enum.Parse<ItemId>(row.Id);

                if (itemDict.ContainsKey(id))
                {
                    ItemDefinition item = itemDict[id];

                    bool setDirty = false;

                    if (string.Compare(item.Name, row.Name) != 0)
                    {
                        item.Name = row.Name;
                        setDirty = true;
                    }

                    if (string.Compare(item.Description, row.Description) != 0)
                    {
                        item.Description = row.Description;
                        setDirty = true;
                    }

                    if (setDirty)
                    {
                        EditorUtility.SetDirty(item);
                    }

                    items.Add(item);
                }
                else
                {
                    ItemDefinition itemDef = CreateInstance<ItemDefinition>();
                    itemDef.Id = id;
                    itemDef.Name = row.Name;
                    itemDef.Description = row.Description;

                    string folderPath = AssetDatabase.GetAssetPath(itemDefinitionFolder);
                    string assetPath = Path.Combine(folderPath, $"{row.Id}.asset");

                    AssetDatabase.CreateAsset(itemDef, assetPath);

                    items.Add(itemDef);
                }
            }

            Items = items.ToArray();
            EditorUtility.SetDirty(this);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Load Success: " + json);
        }
        else
        {
            Debug.LogError("Load Failed: " + request.error);
        }

        Debug.Log("Update Success");
    }

    [Button]
    async void UpdateCategories()
    {
        List<string> invalid = new();

        Debug.Log("Updating...");

        string url = appsScriptUrl + "?type=Categories";

        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";
            var sheet = JsonUtility.FromJson<CategoriesSheet>(json);

            Dictionary<CategoryId, CategoryDefinition> categoryDict = new();

            for (int i = 0; i < Categories.Length; i++)
            {
                if (Items[i] == null) continue;
                categoryDict.Add(Categories[i].Id, Categories[i]);
            }

            List<CategoryDefinition> categories = new();

            foreach (var row in sheet.Rows)
            {
                CategoryId id = Enum.Parse<CategoryId>(row.Id);

                if (categoryDict.ContainsKey(id))
                {
                    CategoryDefinition category = categoryDict[id];

                    bool setDirty = false;

                    if (string.Compare(category.Name, row.Name) != 0)
                    {
                        category.Name = row.Name;
                        setDirty = true;
                    }

                    string itemsString = "";

                    for (int c = 0; c < category.Items.Length; c++)
                    {
                        itemsString += $"{category.Items[c]}";
                        if (c < category.Items.Length - 1)
                        {
                            itemsString += ",";
                        }
                    }

                    if (string.Compare(itemsString, row.Items) != 0)
                    {
                        List<ItemId> itemIds = new();
                        string[] items = row.Items.Split(",");

                        foreach (var item in items)
                        {
                            if (Enum.TryParse(item, out ItemId itemId))
                            {
                                itemIds.Add(itemId);
                            }
                            else
                            {
                                Debug.LogError($"Invalid item id: {id} {item}");

                                if (!invalid.Contains(item))
                                {
                                    invalid.Add(item);
                                }
                            }
                        }

                        category.Items = itemIds.ToArray();

                        setDirty = true;
                    }

                    if (setDirty)
                    {
                        EditorUtility.SetDirty(category);
                    }

                    categories.Add(category);
                }
                else
                {
                    CategoryDefinition category = CreateInstance<CategoryDefinition>();
                    category.Id = id;
                    category.Name = row.Name;

                    List<ItemId> itemIds = new();
                    string[] items = row.Items.Split(',');

                    foreach (var item in items)
                    {
                        if (Enum.TryParse(item, out ItemId itemId))
                        {
                            itemIds.Add(itemId);
                        }
                        else
                        {
                            Debug.LogError($"Invalid item id: {id} {item}");

                            if (!invalid.Contains(item))
                            {
                                invalid.Add(item);
                            }
                        }
                    }

                    category.Items = itemIds.ToArray();

                    string folderPath = AssetDatabase.GetAssetPath(categoryDefinitionFolder);
                    string assetPath = Path.Combine(folderPath, $"{row.Id}.asset");

                    AssetDatabase.CreateAsset(category, assetPath);

                    categories.Add(category);
                }
            }

            Categories = categories.ToArray();

            Dictionary<ItemId, ItemDefinition> itemDict = new();
            for (int i = 0; i < Items.Length; i++)
            {
                itemDict.Add(Items[i].Id, Items[i]);
            }

            foreach (var category in Categories)
            {
                foreach (var itemId in category.Items)
                {
                    ItemDefinition item = itemDict[itemId];

                    List<CategoryId> categoryIds = new(item.Categories);
                    if (!categoryIds.Contains(category.Id))
                    {
                        categoryIds.Add(category.Id);
                        item.Categories = categoryIds.ToArray();
                        EditorUtility.SetDirty(item);
                    }
                }
            }

            EditorUtility.SetDirty(this);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Load Success: " + json);
        }
        else
        {
            Debug.LogError("Load Failed: " + request.error);
        }

        Debug.Log("Update Success");

        string invalidMessage = "";

        for (int i = 0; i < invalid.Count; i++)
        {
            invalidMessage += invalid[i];

            if (i < invalid.Count - 1)
            {
                invalidMessage += ", ";
            }
        }

        if (!string.IsNullOrEmpty(invalidMessage))
        {
            Debug.LogError(invalidMessage);
        }
    }

    [Button]
    async void UpdateRecipes()
    {
        Debug.Log("Updating...");

        string url = appsScriptUrl + "?type=Recipes";

        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";

            var sheet = JsonUtility.FromJson<RecipesSheet>(json);

            List<MergeRecipe> recipes = new List<MergeRecipe>(Recipes);

            foreach (var row in sheet.Rows)
            {
                if (!Enum.TryParse(row.ResultItemId, out ItemId resultItemId))
                {
                    Debug.LogError("Invalid result item id: " + row.ResultItemId);
                    continue;
                }

                if (string.IsNullOrEmpty(row.Recipes))
                {
                    Debug.LogError("Empty recipes " + row.ResultItemId);
                    continue;
                }

                string[] lines = row.Recipes.Split("\n");
                foreach (var line in lines)
                {
                    string[] itemIds = line.Split("+");

                    if (Enum.TryParse(itemIds[0], out ItemId itemAId) &&
                        Enum.TryParse(itemIds[1], out ItemId itemBId))
                    {
                        MergeRecipe recipe = new MergeRecipe()
                        {
                            Id = recipes.Count,
                            ItemAId = itemAId,
                            ItemBId = itemBId,
                            ResultItemId = resultItemId
                        };

                        recipes.Add(recipe);
                    }
                    else
                    {
                        Debug.LogError("Invalid recipe " + line);
                    }
                }
            }

            Recipes = recipes.ToArray();

            Debug.Log("Recipes length: " + Recipes.Length);

            Debug.Log("Load Success: " + json);
        }
        else
        {
            Debug.LogError("Load Failed: " + request.error);
        }

        Debug.Log("Update Success ");
    }

    [Button]
    void GetSpriteReferences()
    {
        Dictionary<ItemId, ItemDefinition> itemDict = new();
        for (int i = 0; i < Items.Length; i++)
        {
            itemDict.Add(Items[i].Id, Items[i]);
        }

        string folderPath = AssetDatabase.GetAssetPath(itemSpriteFolder);
        string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
        foreach (string guid in spriteGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null) continue;
            string spriteName = sprite.name;
            if (Enum.TryParse<ItemId>(spriteName, out ItemId itemId))
            {
                ItemDefinition item = itemDict[itemId];
                item.Icon = sprite;
                EditorUtility.SetDirty(item);
            }
            else
            {
                Debug.LogError("Invalid sprite name: " + spriteName);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button]
    void Check()
    {
        List<ItemId> itemIds = new();

        for (int i = 5; i < Items.Length; i++)
        {
            itemIds.Add(Items[i].Id);
        }

        //foreach (var cat in Categories)
        //{
        //    foreach (var item in cat.Items)
        //    {
        //        if (itemIds.Contains(item))
        //        {
        //            itemIds.Remove(item);
        //        }
        //    }
        //}

        Dictionary<int, List<ItemId>> unuseDict = new();

        foreach (var id in itemIds)
        {
            int idNumber = (int)id;

            if (idNumber.InRange3(6, 15))
            {
                // 1
                Add(1, id);
            }
            else if (idNumber.InRange3(16, 28))
            {
                // 2
                Add(2, id);
            }
            else if (idNumber.InRange3(29, 40))
            {
                // 3
                Add(3, id);
            }
            else if (idNumber.InRange3(41, 69))
            {
                // 4
                Add(4, id);
            }
            else if (idNumber.InRange3(70, 113))
            {
                // 5
                Add(5, id);
            }
            else if (idNumber.InRange3(114, 149))
            {
                // 6
                Add(6, id);
            }
            else if (idNumber.InRange3(150, 173))
            {
                // 7
                Add(7, id);
            }
            else if (idNumber.InRange3(174, 250))
            {
                // 8
                Add(8, id);
            }
            else if (idNumber.InRange3(251, 385))
            {
                // 9
                Add(9, id);
            }
            else if (idNumber.InRange3(386, 502))
            {
                // 10
                Add(10, id);
            }
            else if (idNumber.InRange3(503, 603))
            {
                // 11
                Add(11, id);
            }
            else if (idNumber.InRange3(604, 675))
            {
                // 12
                Add(12, id);
            }
            else if (idNumber.InRange3(676, 709))
            {
                // 13
                Add(13, id);
            }
            else if (idNumber.InRange3(710, 717))
            {
                // 14
                Add(14, id);
            }
            else if (idNumber.InRange3(718, 719))
            {
                // 15
                Add(15, id);
            }
        }

        string all = "";

        foreach (var pair in unuseDict)
        {
            string message = $"Tier {pair.Key}: ";

            for (int i = 0; i < pair.Value.Count; i++)
            {
                message += pair.Value[i].ToString();

                if (i < pair.Value.Count - 1)
                {
                    message += ", ";
                }
            }

            all += message;
            all += "\n";
        }

        Debug.Log(all);

        void Add(int tier, ItemId id)
        {
            if (unuseDict.ContainsKey(tier))
            {
                unuseDict[tier].Add(id);
            }
            else
            {
                unuseDict[tier] = new List<ItemId> { id };
            }
        }
    }
#endif
}
