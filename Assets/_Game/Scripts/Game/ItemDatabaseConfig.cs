using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "ItemDatabaseConfig", menuName = "SO/Game/Item Database Config")]
public class ItemDatabaseConfig : ScriptableObject
{
    public ItemDefinition[] Items;
    [TableList]
    public MergeRecipe[] Recipes;
    [TableList]
    public GroupDefinition[] Groups;
    [TableList]
    public BranchDefinition[] Branches;

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
        public string Group;
        public string Branch;
        public string Name;
        public string Description;
    }

    [Serializable]
    class MergeMapSheet
    {
        public MergeMapSheetRow[] Rows;
    }

    [Serializable]
    class MergeMapSheetRow
    {
        public string Id;
        public string ItemAId;
        public string ItemBId;
        public string ResultItemId;
    }

    [Header("Editor")]
    [SerializeField] string appsScriptUrl;
    [SerializeField, Sirenix.OdinInspector.FilePath] string enumFilePath;
    [SerializeField, FolderPath] string itemFolderPath;

    [Button]
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

            List<string> itemNames = new List<string>();
            for (int i = 0; i < sheet.Rows.Length; i++)
            {
                itemNames.Add(sheet.Rows[i].Id);
            }

            EnumGenerator.AppendNewEnumValues(enumFilePath, itemNames);

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
                    ItemDefinition itemDef = itemDict[id];

                    bool setDirty = false;

                    if (string.Compare(itemDef.Description, row.Description) != 0)
                    {
                        itemDef.Description = row.Description;
                        setDirty = true;
                    }

                    if (string.Compare(itemDef.Name, row.Name) != 0)
                    {
                        itemDef.Name = row.Name;
                        setDirty = true;
                    }

                    GroupId groupId = Enum.Parse<GroupId>(row.Group);

                    if (itemDef.GroupId != groupId)
                    {
                        itemDef.GroupId = groupId;
                        setDirty = true;
                    }

                    BranchId branchId = Enum.Parse<BranchId>(row.Branch);

                    if (itemDef.BranchId != branchId)
                    {
                        itemDef.BranchId = branchId;
                        setDirty = true;
                    }

                    if (setDirty)
                    {
                        EditorUtility.SetDirty(itemDef);
                    }

                    items.Add(itemDef);
                }
                else
                {
                    ItemDefinition itemDef = CreateInstance<ItemDefinition>();
                    itemDef.Id = id;
                    itemDef.Name = row.Id;
                    itemDef.Description = row.Description;
                    itemDef.GroupId = Enum.Parse<GroupId>(row.Group);
                    itemDef.BranchId = Enum.Parse<BranchId>(row.Branch);

                    string assetPath = Path.Combine(itemFolderPath, $"{row.Id}.asset");

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
    async void UpdateRecipes()
    {
        Debug.Log("Updating...");

        string url = appsScriptUrl + "?type=MergeMap";

        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";

            var sheet = JsonUtility.FromJson<MergeMapSheet>(json);

            Recipes = new MergeRecipe[sheet.Rows.Length];

            for (int i = 0; i < sheet.Rows.Length; i++)
            {
                Recipes[i] = new MergeRecipe
                {
                    Id = int.Parse(sheet.Rows[i].Id),
                    ItemAId = Enum.Parse<ItemId>(sheet.Rows[i].ItemAId),
                    ItemBId = Enum.Parse<ItemId>(sheet.Rows[i].ItemBId),
                    ResultItemId = Enum.Parse<ItemId>(sheet.Rows[i].ResultItemId)
                };
            }

            Debug.Log("Load Success: " + json);
        }
        else
        {
            Debug.LogError("Load Failed: " + request.error);
        }

        Debug.Log("Update Success");
    }

    [Button]
    void CheckItemsMissingRecipes()
    {
        HashSet<ItemId> basicItems = new HashSet<ItemId>()
        {
            ItemId.Fire,
            ItemId.Water,
            ItemId.Earth,
            ItemId.Air,
            ItemId.Wood,
            ItemId.Time,
        };

        List<ItemId> itemIds = new();

        for (int i = 0; i < Items.Length; i++)
        {
            if (basicItems.Contains(Items[i].Id)) continue;

            itemIds.Add(Items[i].Id);
        }

        for (int i = 0; i < Recipes.Length; i++)
        {
            if (itemIds.Contains(Recipes[i].ResultItemId))
            {
                itemIds.Remove(Recipes[i].ResultItemId);
            }
        }

        string message = "Items missing recipes: ";

        for (int i = 0; i < itemIds.Count; i++)
        {
            message += itemIds[i].ToString();

            if (i < itemIds.Count - 1)
            {
                message += ", ";
            }
        }

        Debug.Log(message);
    }

    [Button]
    void CheckDuplicatedRecipes()
    {
        List<MergeKey> keys = new List<MergeKey>();

        for (int i = 0; i < Recipes.Length; i++)
        {
            keys.Add(new MergeKey(Recipes[i].ItemAId, Recipes[i].ItemBId));
        }

        HashSet<MergeKey> seen = new HashSet<MergeKey>();
        List<MergeKey> duplicates = new();

        for (int i = 0; i < keys.Count; i++)
        {
            MergeKey key = keys[i];

            if (!seen.Add(key))
            {
                if (!duplicates.Contains(key))
                {
                    duplicates.Add(key);
                }
            }
        }

        Dictionary<MergeKey, List<ItemId>> dict = new();

        foreach (var dup in duplicates)
        {
            dict[dup] = new List<ItemId>();

            for (int i = 0; i < keys.Count; i++)
            {
                MergeKey key = keys[i];

                if (dup.Equals(key))
                {
                    MergeRecipe recipe = Recipes[i];

                    dict[dup].Add(recipe.ResultItemId);
                }
            }
        }

        string message = "";

        foreach (var pair in dict)
        {
            message += $"{pair.Key.A} + {pair.Key.B} = ";

            for (int i = 0; i < pair.Value.Count; i++)
            {
                message += pair.Value[i].ToString();
                message += i < pair.Value.Count - 1 ? ", " : "\n";
            }
        }

        if (string.IsNullOrEmpty(message))
        {
            message = "No duplicates found";
        }

        Debug.Log(message);
    }

    [Button]
    void UpdateTier()
    {
        List<ItemDefinition> remainingItems = new();
        Dictionary<ItemId, ItemDefinition> itemDict = new();
        Dictionary<ItemId, MergeRecipe> resultMap = new();

        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].Tier <= 0)
            {
                remainingItems.Add(Items[i]);
            }

            itemDict.Add(Items[i].Id, Items[i]);
        }

        for (int i = 0; i < Recipes.Length; i++)
        {
            resultMap.Add(Recipes[i].ResultItemId, Recipes[i]);
        }

        int loop = 0;

        while (remainingItems.Count > 0 && loop < 1000000)
        {
            for (int i = remainingItems.Count - 1; i >= 0; i--)
            {
                ItemDefinition item = remainingItems[i];
                MergeRecipe recipe = resultMap[item.Id];

                ItemDefinition itemA = itemDict[recipe.ItemAId];
                ItemDefinition itemB = itemDict[recipe.ItemBId];

                if (itemA.Tier <= 0 || itemB.Tier <= 0) continue;

                item.Tier = itemA.Tier + itemB.Tier;
                EditorUtility.SetDirty(item);
                remainingItems.RemoveAt(i);
            }

            loop++;
        }

        Debug.Log(loop);

        for (int i = 0; i < remainingItems.Count; i++)
        {
            Debug.Log(remainingItems[i].Id);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button]
    void ListItems()
    {
        string str = "Items: ";

        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].GroupId != GroupId.G1) continue;
            
            str += Items[i].Id.ToString();
            str += ", ";
        }

        Debug.Log(str);
    }

    [Button]
    void ListRecipes()
    {
        Dictionary<ItemId, ItemDefinition> itemDict = new();

        for (int i = 0; i < Items.Length; i++)
        {
            itemDict.Add(Items[i].Id, Items[i]);
        }

        string str = "Recipes: \n";

        for (int i = 0; i < Recipes.Length; i++)
        {
            if (itemDict[Recipes[i].ResultItemId].GroupId != GroupId.G1) continue;

            str += $"{Recipes[i].ItemAId} + {Recipes[i].ItemBId} = {Recipes[i].ResultItemId}";
            str += "\n";
        }

        Debug.Log(str);
    }
#endif
}
